using System.Diagnostics;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Helpers;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnClientService : IOpenVpnClientService
{
    private readonly ILogger<OpenVpnClientService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEasyRsaService _easyRsaService;
    private readonly string _pkiPath;
    
    private readonly OpenVpnSettings _openVpnSettings;
    private readonly int _maxAttempts = 10;

    public OpenVpnClientService(ILogger<OpenVpnClientService> logger, IConfiguration configuration,
        IServiceProvider serviceProvider, IEasyRsaService easyRsaService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _easyRsaService = easyRsaService;
        _openVpnSettings = configuration.GetSection("OpenVpn").Get<OpenVpnSettings>() 
                           ?? throw new InvalidOperationException("OpenVpn configuration section is missing.");

        if (string.IsNullOrEmpty(_openVpnSettings.EasyRsaPath) ||
            string.IsNullOrEmpty(_openVpnSettings.OutputDir) ||
            string.IsNullOrEmpty(_openVpnSettings.TlsAuthKey) ||
            string.IsNullOrEmpty(_openVpnSettings.ServerIp))
        {
            throw new InvalidOperationException("One or more OpenVpn configuration values are missing.");
        }
        _pkiPath = Path.Combine(_openVpnSettings.EasyRsaPath, "pki");//todo:fix
    }

    public async Task<GetAllFilesResult> GetAllClientConfigurations(long telegramId)
    {
        var issuedOvpnFiles = await GetIssuedOvpnFilesByTelegramIdAsync(telegramId);
        _logger.LogInformation("Found {Count} issued files in database.", issuedOvpnFiles.Count);

        List<FileInfo> fileInfos = new List<FileInfo>();

        foreach (var issuedOvpnFile in issuedOvpnFiles)
        {
            string existingOvpnFilePath = Path.Combine(_openVpnSettings.OutputDir, $"{issuedOvpnFile!.FileName}");
            _logger.LogInformation("Checking existence of file: {FilePath}", existingOvpnFilePath);

            if (File.Exists(existingOvpnFilePath))
            {
                _logger.LogInformation("File exists: {FilePath}", existingOvpnFilePath);
                fileInfos.Add(new FileInfo(existingOvpnFilePath));
            }
            else
            {
                _logger.LogCritical("File not found: {FilePath}", existingOvpnFilePath);
            }
        }

        var responseMessage = await GetResponseText(telegramId, "HereIsConfig");
        _logger.LogInformation("Generated response message for user: {TelegramId}", telegramId);

        return new GetAllFilesResult
        {
            FileInfo = fileInfos,
            Message = responseMessage
        };
    }

    public async Task<FileCreationResult> CreateClientConfiguration(long telegramId)
    {
        try
        {
            var issuedOvpnFiles = await GetIssuedOvpnFilesByTelegramIdAsync(telegramId);
            if (issuedOvpnFiles.Count >= _maxAttempts)
            {
                return new FileCreationResult { FileInfo = null, Message = await GetResponseText(telegramId, "MaxConfigError") };
            }
            
            _logger.LogInformation("Step 1: Checking if PKI directory exists...");
            _easyRsaService.InstallEasyRsa();

            _logger.LogInformation("Step 1.1: Checking if configuration already exists for this client with TelegramId: {TelegramId}.", telegramId);

            int attempt = 0;
            string baseFileName = GetBaseFileNameForCerts(telegramId.ToString(), attempt);
            _logger.LogInformation("Step 1.2: Initial base file name generated: {BaseFileName}", baseFileName);

            string baseOvpnFileName = $"{baseFileName}.ovpn";
            string ovpnFilePath = Path.Combine(_openVpnSettings.OutputDir, baseOvpnFileName);

            _logger.LogInformation("Step 1.3:Initial .ovpn file path: {OvpnFilePath}", ovpnFilePath);

            while (File.Exists(ovpnFilePath) && attempt < _maxAttempts)
            {
                _logger.LogInformation("File already exists: {OvpnFilePath}. Incrementing attempt counter.", ovpnFilePath);
                attempt++;
                baseFileName = GetBaseFileNameForCerts(telegramId.ToString(), attempt);
                ovpnFilePath = Path.Combine(_openVpnSettings.OutputDir, $"{baseFileName}.ovpn");
                _logger.LogInformation("New file path after attempt {Attempt}: {OvpnFilePath}", attempt, ovpnFilePath);
            }

            if (attempt >= _maxAttempts)
            {
                _logger.LogError(
                    "Maximum limit of {MaxAttempts} configurations reached for client '{TelegramId}'. Cannot create more files.",
                    _maxAttempts, telegramId);
                throw new InvalidOperationException(
                    $"Maximum limit of {_maxAttempts} configurations for client '{telegramId}' has been reached. Cannot create more files.");
            }

            _logger.LogInformation("Step 1.4: Final file path determined: {OvpnFilePath}. Proceeding with configuration creation.", ovpnFilePath);


            _logger.LogInformation("Step 2: Building client certificate...");
            _easyRsaService.BuildCertificate($"{baseFileName}");

            _logger.LogInformation("Step 3: Defining paths to certificates and keys...");
            string caCertContent = _easyRsaService.ReadPemContent(Path.Combine(_pkiPath, "ca.crt"));

            string crtPath = Path.Combine(_pkiPath, "issued", $"{baseFileName}.crt");
            string keyPath = Path.Combine(_pkiPath, "private", $"{baseFileName}.key");
            string reqPath = Path.Combine(_pkiPath, "reqs", $"{baseFileName}.req");
            string pemPath = Path.Combine(_pkiPath, "certs_by_serial", $"pemFile.pem");//todo: can found in /etc/openvpn/easy-rsa/pki/index.txt
            
            string clientCertContent = _easyRsaService.ReadPemContent(crtPath);
            string clientKeyContent = await File.ReadAllTextAsync(keyPath);

            _logger.LogInformation("Step 4: Generating .ovpn configuration file...");
            string ovpnContent = GenerateOvpnFile(_openVpnSettings.ServerIp, caCertContent,
                clientCertContent, clientKeyContent, _openVpnSettings.TlsAuthKey);

            _logger.LogInformation("Step 5: Ensuring output directory exists...");
            Directory.CreateDirectory(_openVpnSettings.OutputDir);

            _logger.LogInformation("Step 6: Writing .ovpn file...");
            await File.WriteAllTextAsync(ovpnFilePath, ovpnContent);

            _logger.LogInformation($"Client configuration file created: {ovpnFilePath}");
            var fileInfo = new FileInfo(ovpnFilePath);
            await SaveInfoInDataBase(telegramId, fileInfo, crtPath, keyPath, reqPath, pemPath);
            return new FileCreationResult { FileInfo = fileInfo, Message = await GetResponseText(telegramId,"HereIsConfig") };

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteAllClientConfigurations(long telegramId)
    {
        _logger.LogInformation("Starting deletion process for client with Telegram ID: {TelegramId}", telegramId);
        _logger.LogInformation("Checking if PKI directory exists...");
        _easyRsaService.InstallEasyRsa();
        
        var issuedOvpnFiles = await GetIssuedOvpnFilesByTelegramIdAsync(telegramId);
        _logger.LogInformation("Found {Count} issued files in database for deletion.", issuedOvpnFiles.Count);
    
        foreach (var issuedOvpnFile in issuedOvpnFiles)
        {
            if (issuedOvpnFile != null) await RevokeAndDeleteFile(issuedOvpnFile, telegramId);
        }
    
        _logger.LogInformation("Completed deletion process for client with Telegram ID: {TelegramId}", telegramId);
    }

    public async Task DeleteClientConfiguration(long telegramId, string filename)
    {
        _logger.LogInformation("Starting deletion process for client with Telegram ID: {TelegramId}", telegramId);
        _logger.LogInformation("Checking if PKI directory exists...");
        _easyRsaService.InstallEasyRsa();
        
        var issuedOvpnFile = await GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(telegramId, filename);
        if (issuedOvpnFile != null)
        {
            await RevokeAndDeleteFile(issuedOvpnFile, telegramId);
        }
    }

    private async Task RevokeAndDeleteFile(IssuedOvpnFile issuedOvpnFile, long telegramId)
    {
        if (!_easyRsaService.RevokeCertificate(issuedOvpnFile.CertName))
        {
            throw new Exception($"Failed to revoke certificate: {issuedOvpnFile.CertName}");
        }
        string ovpnFilePath = Path.Combine(_openVpnSettings.OutputDir, issuedOvpnFile.FileName);
        if (File.Exists(ovpnFilePath))
        {
            File.Delete(ovpnFilePath);
            _logger.LogInformation("Deleted .ovpn file: {FilePath}", ovpnFilePath);
        }
        else
        {
            _logger.LogWarning(".ovpn file not found for deletion: {FilePath}", ovpnFilePath);
        }

        await SetIsRevokeIssuedOvpnFile(issuedOvpnFile.Id, telegramId, issuedOvpnFile.CertName);
    }


    private async Task<string> GetResponseText(long telegramId, string key)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        return await localizationService.GetTextAsync(key, telegramId);
    }

    private async Task SaveInfoInDataBase(long telegramId, FileInfo fileInfo, string crtPath, string keyPath,
        string reqPath, string pemPath)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        await issuedOvpnFileService.AddIssuedOvpnFileAsync(telegramId, fileInfo, crtPath, keyPath, reqPath, pemPath);
    }
    
    private async Task<List<IssuedOvpnFile>> GetIssuedOvpnFilesByTelegramIdAsync(long telegramId)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        return await issuedOvpnFileService.GetIssuedOvpnFilesByTelegramIdAsync(telegramId);
    }
    
    private async Task<IssuedOvpnFile?> GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(long telegramId, string fileName)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        return await issuedOvpnFileService.GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(telegramId, fileName);
    }
    
    private async Task SetIsRevokeIssuedOvpnFile(int id, long telegramId, string certName)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        await issuedOvpnFileService.SetIsRevokeIssuedOvpnFileByTelegramIdAndCertNameAsync(id, telegramId, certName);
    }
    
    private string GetBaseFileNameForCerts(string fileName, int attempt)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var prefix = environment != "Production" ? $"{environment}_" : string.Empty;
        return $"{prefix}{fileName}_{attempt}";
    }

    private static string GenerateOvpnFile(string serverIp, string caCert, string clientCert, 
        string clientKey, string tlsAuthKey)
    {
        return $@"client
dev tun
proto udp
remote {serverIp} 1291
resolv-retry infinite
nobind
remote-cert-tls server
tls-version-min 1.2
verify-x509-name raspberrypi_2e39d597-c642-4f69-a6c8-149e7c9ac064 name
cipher AES-256-CBC
auth SHA256
auth-nocache
verb 3
<ca>
{caCert}
</ca>
<cert>
{clientCert}
</cert>
<key>
{clientKey}
</key>
<tls-crypt>
{File.ReadAllText(tlsAuthKey)}
</tls-crypt>
";
    }
}