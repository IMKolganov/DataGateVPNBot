using System.Diagnostics;
using DataGateVPNBotV1.Models.Helpers;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class EasyRsaService : IEasyRsaService
{
    private readonly ILogger<EasyRsaService> _logger;
    private readonly string _pkiPath;
    private readonly OpenVpnSettings _openVpnSettings;

    public EasyRsaService(ILogger<EasyRsaService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var openVpnSection = configuration.GetSection("OpenVpn");
        if (!openVpnSection.Exists())
        {
            throw new InvalidOperationException("OpenVpn section is missing in the configuration.");
        }

        _openVpnSettings = openVpnSection.Get<OpenVpnSettings>()
                           ?? throw new InvalidOperationException("Failed to load OpenVpnSettings from configuration.");
        
        _logger.LogInformation("Loaded OpenVpnSettings: EasyRsaPath: {EasyRsaPath}, OutputDir: {OutputDir}, TlsAuthKey: {TlsAuthKey}, ServerIp: {ServerIp}", 
            _openVpnSettings.EasyRsaPath, 
            _openVpnSettings.OutputDir, 
            _openVpnSettings.TlsAuthKey, 
            _openVpnSettings.ServerIp);

        if (string.IsNullOrEmpty(_openVpnSettings.EasyRsaPath))
        {
            throw new InvalidOperationException("OpenVpnSettings: EasyRsaPath is missing or empty.");
        }

        if (string.IsNullOrEmpty(_openVpnSettings.OutputDir))
        {
            throw new InvalidOperationException("OpenVpnSettings: OutputDir is missing or empty.");
        }

        if (string.IsNullOrEmpty(_openVpnSettings.TlsAuthKey))
        {
            throw new InvalidOperationException("OpenVpnSettings: TlsAuthKey is missing or empty.");
        }

        if (string.IsNullOrEmpty(_openVpnSettings.ServerIp))
        {
            throw new InvalidOperationException("OpenVpnSettings: ServerIp is missing or empty.");
        }

        _pkiPath = Path.Combine(_openVpnSettings.EasyRsaPath, "pki");
        _logger.LogInformation("PKI path initialized to: {PkiPath}", _pkiPath);
    }


    public void InstallEasyRsa()
    {
        if (!Directory.Exists(_pkiPath))
        {
            _logger.LogInformation("PKI directory does not exist. Initializing PKI...");
            RunCommand($"cd {_openVpnSettings.EasyRsaPath} && ./easyrsa init-pki");
        }
        else
        {
            _logger.LogInformation("PKI directory exists. Skipping initialization...");
        }
    }

    public void BuildCertificate(string certName = "client1")
    {
        RunCommand($"cd {_openVpnSettings.EasyRsaPath} && ./easyrsa build-client-full {certName} nopass");
    }

    public string ReadPemContent(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return string.Join(Environment.NewLine, lines
            .SkipWhile(line => !line.StartsWith("-----BEGIN CERTIFICATE-----"))
            .TakeWhile(line => !line.StartsWith("-----END CERTIFICATE-----"))
            .Append("-----END CERTIFICATE-----"));
    }

    public string RevokeCertificate(string clientName)
    {
        string certPath = Path.Combine(_pkiPath, "issued", $"{clientName}.crt");
        if (!File.Exists(certPath))
        {
            _logger.LogError($"Certificate file not found: {certPath}");
            return $"Certificate file not found: {certPath}";
        }

        _logger.LogInformation($"Attempting to revoke certificate for: {clientName}");
        _logger.LogInformation($"EasyRsaPath: {_openVpnSettings.EasyRsaPath}");
        _logger.LogInformation($"PKI Path: {_pkiPath}");
        _logger.LogInformation($"Certificate Path: {certPath}");

        // Revoke the certificate
        var revokeResult = ExecuteEasyRsaCommand($"revoke {clientName}", confirm: true);
        if (!revokeResult.IsSuccess)
        {
            switch (revokeResult.ExitCode)
            {
                case 0:
                    _logger.LogInformation("Certificate revoked successfully: {ClientName}", clientName);
                    break;

                case 1:
                    if (revokeResult.Output.Contains("ERROR:Already revoked"))
                    {
                        _logger.LogWarning("Certificate is already revoked: {ClientName}", clientName);
                    }
                    else if (revokeResult.Output.Contains("ERROR: Certificate not found"))
                    {
                        _logger.LogWarning($"Certificate not found: {clientName}");
                    }
                    else
                    {
                        throw new Exception($"Failed to revoke certificate. Unknown error: {clientName}");
                    }
                    break;

                default:
                    _logger.LogError("Unexpected exit code ({ExitCode}) while revoking certificate: {ClientName}", revokeResult.ExitCode, clientName); 
                    throw new Exception($"Unexpected exit code ({revokeResult.ExitCode}) while revoking certificate: {clientName}");
            }
        }

        _logger.LogInformation("Revocation successful. Generating CRL...");

        var crlResult = ExecuteEasyRsaCommand("gen-crl");
        if (!crlResult.IsSuccess)
        {
            _logger.LogInformation($"Command Output: {crlResult.Output}");
            throw new Exception($"Failed to generate CRL: {crlResult.Error}");
        }
        
        if (!File.Exists(_openVpnSettings.CrlPkiPath))
        {
            _logger.LogInformation($"Command Output: {crlResult.Output}");
            throw new Exception($"Generated CRL not found at {_openVpnSettings.CrlPkiPath}");
        }

        try
        {
            // Copy the CRL to the OpenVPN directory
            string copyCommand = $"cp {_openVpnSettings.CrlPkiPath} {_openVpnSettings.CrlOpenvpnPath}";
            var copyResult = RunCommand(copyCommand);

            if (copyResult.ExitCode != 0)
            {
                _logger.LogInformation($"Command Output: {crlResult.Output}");
                throw new Exception($"Failed to copy CRL file: {copyResult.Error}");
            }

            _logger.LogInformation($"CRL copied to {_openVpnSettings.CrlOpenvpnPath}");

            // Update permissions for the CRL file
            string chmodCommand = $"chmod 644 {_openVpnSettings.CrlOpenvpnPath}";
            var chmodResult = RunCommand(chmodCommand);

            if (chmodResult.ExitCode != 0)
            {
                _logger.LogWarning($"Failed to set permissions on CRL file: {chmodResult.Error}");
            }
            else
            {
                _logger.LogInformation("CRL permissions updated successfully.");
            }
            
            string chownCommand = $"chown openvpn:openvpn {_openVpnSettings.CrlOpenvpnPath}";
            var chownResult = RunCommand(chownCommand);

            if (chownResult.ExitCode != 0)
            {
                _logger.LogWarning($"Failed to change owner of CRL file: {chownResult.Error}");
            }
            else
            {
                _logger.LogInformation("CRL ownership updated successfully.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during CRL update: {ex.Message}");
        }
        _logger.LogInformation("CRL successfully updated and deployed.");


        _logger.LogInformation("Certificate successfully revoked, CRL updated and deployed.");
        return "Certificate successfully revoked, CRL updated and deployed.";
    }

    private (bool IsSuccess, string Output, int ExitCode, string Error) ExecuteEasyRsaCommand(string arguments, bool confirm = false)
    {
        try
        {
            var command = $"cd {_openVpnSettings.EasyRsaPath} && ./easyrsa {arguments}";
            if (confirm)
            {
                _logger.LogInformation($"Confirming command with 'yes': {arguments}");
                command = $"cd {_openVpnSettings.EasyRsaPath} && echo yes | ./easyrsa {arguments}";
            }

            _logger.LogInformation($"Executing command: {command}");
            var result = RunCommand(command);

            _logger.LogInformation($"Command Output: {result.Output}");
            _logger.LogInformation($"Command Error: {result.Error}");
            _logger.LogInformation($"Command Exit Code: {result.ExitCode}");

            return result.ExitCode == 0
                ? (true, result.Output, result.ExitCode, string.Empty)
                : (false, result.Output, result.ExitCode, result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception during command execution: {ex.Message}");
            return (false, string.Empty, 404,ex.Message);
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

        _logger.LogInformation($"Starting process: {command}");
        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start command process.");
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        _logger.LogInformation($"Process completed with ExitCode: {process.ExitCode}");
        return (output, error, process.ExitCode);
    }

}