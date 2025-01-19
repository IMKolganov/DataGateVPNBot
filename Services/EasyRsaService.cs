using System.Diagnostics;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class EasyRsaService : IEasyRsaService
{
    private readonly ILogger<EasyRsaService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _easyRsaPath;
    private readonly string _pkiPath;
    
    public EasyRsaService(ILogger<EasyRsaService> logger, IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _easyRsaPath = configuration["OpenVpn:EasyRsaPath"] ?? throw new InvalidOperationException();
        _pkiPath = Path.Combine(_easyRsaPath, "pki");
    }
    
    public void InstallEasyRsa()
    {
        if (!Directory.Exists(_pkiPath))
        {
            _logger.LogInformation("PKI directory does not exist. Initializing PKI...");
            RunCommand($"cd {_easyRsaPath} && ./easyrsa init-pki");
        }
        else
        {
            _logger.LogInformation("PKI directory exists. Skipping initialization...");
        }
    }

    public void BuildCertificate(string certName = "client1")
    {
        RunCommand($"cd {_easyRsaPath} && ./easyrsa build-client-full {certName} nopass");
    }
    
    public string ReadPemContent(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return string.Join(Environment.NewLine, lines
            .SkipWhile(line => !line.StartsWith("-----BEGIN CERTIFICATE-----"))
            .TakeWhile(line => !line.StartsWith("-----END CERTIFICATE-----"))
            .Append("-----END CERTIFICATE-----"));
    }
    
    public bool RevokeCertificate(string clientName)
    {
        string certPath = Path.Combine(_pkiPath, "issued", $"{clientName}.crt");
        if (!File.Exists(certPath))
        {
            _logger.LogError($"Certificate file not found: {certPath}");
            return false;
        }

        var revokeResult = ExecuteEasyRsaCommand($"revoke {clientName}", confirm: true);
        if (!revokeResult.IsSuccess)
        {
            _logger.LogError($"Failed to revoke certificate: {revokeResult.Error}");
            return false;
        }

        var crlResult = ExecuteEasyRsaCommand("gen-crl");
        if (!crlResult.IsSuccess)
        {
            _logger.LogError($"Failed to generate CRL: {crlResult.Error}");
            return false;
        }
        
        _logger.LogInformation("Certificate successfully revoked and CRL updated.");
        return true;
    }

    private (bool IsSuccess, string Output, string Error) ExecuteEasyRsaCommand(string arguments, bool confirm = false)
    {
        try
        {
            var command = $"cd {_easyRsaPath} && ./easyrsa {arguments}";
            if (confirm) command = $"echo yes | {command}";

            RunCommand(command);
            return (true, "Command executed successfully", string.Empty);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, ex.Message);
        }
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
}