using System.Diagnostics;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Helpers;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnClientService : IOpenVpnClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _easyRsaPath;
    private readonly string _pkiPath;
    private readonly string _outputDir;
    private readonly string _tlsAuthKey;
    private readonly string _serverIp;

    public OpenVpnClientService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
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
        List<FileInfo> fileInfos = new List<FileInfo>();
        
        foreach (var issuedOvpnFile in issuedOvpnFiles)
        {
            string existingOvpnFilePath = Path.Combine(_outputDir, $"{issuedOvpnFile.FileName}.ovpn");
            if (File.Exists(existingOvpnFilePath))
            {
                fileInfos.Add(new FileInfo(existingOvpnFilePath));
            }

        }

        return  new GetAllFilesResult()
        {
            FileInfo = fileInfos,
            Message = await GetResponseText(telegramId)
        };
    }

    public async Task<FileCreationResult> CreateClientConfiguration(long telegramId)
    {
        try
        {
            Console.WriteLine("Step 1: Checking if PKI directory exists...");
            if (!Directory.Exists(_pkiPath))
            {
                Console.WriteLine("PKI directory does not exist. Initializing PKI...");
                RunCommand($"cd {_easyRsaPath} && ./easyrsa init-pki");
            }
            else
            {
                Console.WriteLine("PKI directory exists. Skipping initialization...");
            }

            Console.WriteLine("Step 1.1: Checking if configuration already exists for this client...");
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

            Console.WriteLine("Step 2: Building client certificate...");
            RunCommand($"cd {_easyRsaPath} && ./easyrsa build-client-full {telegramId.ToString()} nopass");

            Console.WriteLine("Step 3: Defining paths to certificates and keys...");
            string caCertContent = ReadPemContent(Path.Combine(_pkiPath, "ca.crt"));
            string clientCertContent = ReadPemContent(Path.Combine(_pkiPath, "issued", $"{telegramId.ToString()}.crt"));
            string clientKeyContent = await File.ReadAllTextAsync(Path.Combine(_pkiPath, "private", $"{telegramId.ToString()}.key"));

            Console.WriteLine("Step 4: Generating .ovpn configuration file...");
            string ovpnContent = GenerateOvpnFile(_serverIp, caCertContent, 
                clientCertContent, clientKeyContent, _tlsAuthKey);

            Console.WriteLine("Step 5: Ensuring output directory exists...");
            Directory.CreateDirectory(_outputDir);

            Console.WriteLine("Step 6: Writing .ovpn file...");
            await File.WriteAllTextAsync(ovpnFilePath, ovpnContent);

            Console.WriteLine($"Client configuration file created: {ovpnFilePath}");
            var fileInfo = new FileInfo(ovpnFilePath);
            await SaveInfoInDB(telegramId, fileInfo);
            return new FileCreationResult { FileInfo = fileInfo, Message = await GetResponseText(telegramId)};
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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

            Console.WriteLine(output);
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