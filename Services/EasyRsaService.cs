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

        _logger.LogInformation($"Attempting to revoke certificate for: {clientName}");
        _logger.LogInformation($"Using EasyRsaPath: {_easyRsaPath}");
        _logger.LogInformation($"Using PKI Path: {_pkiPath}");
        _logger.LogInformation($"Certificate Path: {certPath}");

        var revokeResult = ExecuteEasyRsaCommand($"revoke {clientName}", confirm: true);
        if (!revokeResult.IsSuccess)
        {
            _logger.LogError($"Failed to revoke certificate: {revokeResult.Error}");
            _logger.LogInformation($"Command Output: {revokeResult.Output}");
            return false;
        }

        _logger.LogInformation($"Revocation successful. Attempting to generate CRL.");
        var crlResult = ExecuteEasyRsaCommand("gen-crl");
        if (!crlResult.IsSuccess)
        {
            _logger.LogError($"Failed to generate CRL: {crlResult.Error}");
            _logger.LogInformation($"Command Output: {crlResult.Output}");
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
            if (confirm)
            {
                _logger.LogInformation($"Confirming command with 'yes': {arguments}");
                command = $"cd {_easyRsaPath} && echo yes | ./easyrsa {arguments}";
            }

            _logger.LogInformation($"Executing command: {command}");

            var result = RunCommand(command);

            _logger.LogInformation($"Command Output: {result.Output}");
            _logger.LogInformation($"Command Error: {result.Error}");
            _logger.LogInformation($"Command Exit Code: {result.ExitCode}");

            return result.ExitCode == 0
                ? (true, result.Output, string.Empty)
                : (false, result.Output, result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception during command execution: {ex.Message}");
            return (false, string.Empty, ex.Message);
        }
    }

    private (string Output, string Error, int ExitCode) RunCommand(string command)
    {
        var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogInformation($"Starting process with command: {command}");

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start command process.");
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        _logger.LogInformation($"Process finished with ExitCode: {process.ExitCode}");
        return (output, error, process.ExitCode);
    }
}