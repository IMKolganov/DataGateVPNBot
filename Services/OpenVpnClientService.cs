using System.Diagnostics;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Helpers;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnClientService : IOpenVpnClientService
{
    private readonly ILogger<OpenVpnClientService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _easyRsaPath;
    private readonly string _pkiPath;
    private readonly string _outputDir;
    private readonly string _tlsAuthKey;
    private readonly string _serverIp;

    public OpenVpnClientService(ILogger<OpenVpnClientService> logger,IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _easyRsaPath = configuration["OpenVpn:EasyRsaPath"] ?? throw new InvalidOperationException();
        _pkiPath = Path.Combine(_easyRsaPath, "pki");
        _outputDir = configuration["OpenVpn:OutputDir"] ?? throw new InvalidOperationException();
        _tlsAuthKey = configuration["OpenVpn:TlsAuthKey"] ?? throw new InvalidOperationException();
        _serverIp = configuration["OpenVpn:ServerIp"] ?? throw new InvalidOperationException();
    }

    public async Task<GetAllFilesResult> GetAllClientConfigurations(long telegramId)
    {
        var issuedOvpnFiles = await GetFileInfoFromDB(telegramId);
        _logger.LogInformation("Found {Count} issued files in database.", issuedOvpnFiles.Count);

        List<FileInfo> fileInfos = new List<FileInfo>();

        foreach (var issuedOvpnFile in issuedOvpnFiles)
        {
            string existingOvpnFilePath = Path.Combine(_outputDir, $"{issuedOvpnFile.FileName}");
            _logger.LogInformation("Checking existence of file: {FilePath}", existingOvpnFilePath);

            if (File.Exists(existingOvpnFilePath))
            {
                _logger.LogInformation("File exists: {FilePath}", existingOvpnFilePath);
                fileInfos.Add(new FileInfo(existingOvpnFilePath));
            }
            else
            {
                _logger.LogWarning("File not found: {FilePath}", existingOvpnFilePath);
            }
        }

        var responseMessage = await GetResponseText(telegramId);
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
            _logger.LogInformation("Step 1: Checking if PKI directory exists...");
            if (!Directory.Exists(_pkiPath))
            {
                _logger.LogInformation("PKI directory does not exist. Initializing PKI...");
                RunCommand($"cd {_easyRsaPath} && ./easyrsa init-pki");
            }
            else
            {
                _logger.LogInformation("PKI directory exists. Skipping initialization...");
            }

            _logger.LogInformation("Step 1.1: Checking if configuration already exists for this client...");
            string baseOvpnFileName = $"{telegramId.ToString()}.ovpn";
            string ovpnFilePath = Path.Combine(_outputDir, baseOvpnFileName);
            int attempt = 0;
            int maxAttempts = 9;

            while (File.Exists(ovpnFilePath) && attempt < maxAttempts)
            {
                attempt++;
                ovpnFilePath = Path.Combine(_outputDir, $"{telegramId.ToString()}_{attempt}.ovpn");
            }
            if (attempt >= maxAttempts)
            {
                throw new InvalidOperationException($"Maximum limit of {maxAttempts + 1} configurations for client '{telegramId.ToString()}' has been reached. Cannot create more files.");
            }

            _logger.LogInformation("Step 2: Building client certificate...");
            RunCommand($"cd {_easyRsaPath} && ./easyrsa build-client-full {telegramId.ToString()}_{attempt} nopass");

            _logger.LogInformation("Step 3: Defining paths to certificates and keys...");
            string caCertContent = ReadPemContent(Path.Combine(_pkiPath, "ca.crt"));
            string clientCertContent = ReadPemContent(Path.Combine(_pkiPath, "issued", $"{telegramId.ToString()}_{attempt}.crt"));
            string clientKeyContent = await File.ReadAllTextAsync(Path.Combine(_pkiPath, "private", $"{telegramId.ToString()}_{attempt}.key"));

            _logger.LogInformation("Step 4: Generating .ovpn configuration file...");
            string ovpnContent = GenerateOvpnFile(_serverIp, caCertContent, 
                clientCertContent, clientKeyContent, _tlsAuthKey);

            _logger.LogInformation("Step 5: Ensuring output directory exists...");
            Directory.CreateDirectory(_outputDir);

            _logger.LogInformation("Step 6: Writing .ovpn file...");
            await File.WriteAllTextAsync(ovpnFilePath, ovpnContent);

            _logger.LogInformation($"Client configuration file created: {ovpnFilePath}");
            var fileInfo = new FileInfo(ovpnFilePath);
            await SaveInfoInDB(telegramId, fileInfo);
            return new FileCreationResult { FileInfo = fileInfo, Message = await GetResponseText(telegramId)};
            
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Error: {ex.Message}");
            throw;
        }
    }

    private async Task<string> GetResponseText(long telegramId)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        return await localizationService.GetTextAsync("HereIsConfig", telegramId);
    }

    private async Task SaveInfoInDB(long telegramId, FileInfo fileInfo)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        await issuedOvpnFileService.AddIssuedOvpnFileAsync(telegramId, fileInfo);
    }
    
    private async Task<List<IssuedOvpnFile>> GetFileInfoFromDB(long telegramId)
    {
        using var scope = _serviceProvider.CreateScope();
        var issuedOvpnFileService = scope.ServiceProvider.GetRequiredService<IIssuedOvpnFileService>();
        return await issuedOvpnFileService.GetIssuedOvpnFilesByTelegramIdAsync(telegramId);
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

    private static void RunCommand(string command)
    {
        var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(processInfo))
        {
            if (process == null)
                throw new InvalidOperationException("Failed to start command process.");

            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Command execution failed: {error}");
        }
    }
    
    private string ReadPemContent(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return string.Join(Environment.NewLine, lines
            .SkipWhile(line => !line.StartsWith("-----BEGIN CERTIFICATE-----"))
            .TakeWhile(line => !line.StartsWith("-----END CERTIFICATE-----"))
            .Append("-----END CERTIFICATE-----"));
    }

}