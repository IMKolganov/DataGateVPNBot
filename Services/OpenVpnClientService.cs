using System.Diagnostics;
using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnClientService : IOpenVpnClientService
{
    private readonly string _easyRsaPath;
    private readonly string _pkiPath;
    private readonly string _outputDir;
    private readonly string _tlsAuthKey;

    public OpenVpnClientService(IConfiguration configuration)
    {
        _easyRsaPath = configuration["OpenVpn:EasyRsaPath"];
        _pkiPath = Path.Combine(_easyRsaPath, "pki");
        _outputDir = configuration["OpenVpn:OutputDir"];
        _tlsAuthKey = configuration["OpenVpn:TlsAuthKey"];
    }

    public FileInfo CreateClientConfiguration(string clientName, string serverIp)
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
            string existingOvpnFilePath = Path.Combine(_outputDir, $"{clientName}.ovpn");
            if (File.Exists(existingOvpnFilePath))
            {
                Console.WriteLine($"Configuration for client '{clientName}' already exists at {existingOvpnFilePath}. Returning existing file.");
                return new FileInfo(existingOvpnFilePath);
            }

            Console.WriteLine("Step 2: Building client certificate...");
            RunCommand($"cd {_easyRsaPath} && ./easyrsa build-client-full {clientName} nopass");

            Console.WriteLine("Step 3: Defining paths to certificates and keys...");
            string caCertContent = ReadPemContent(Path.Combine(_pkiPath, "ca.crt"));
            string clientCertContent = ReadPemContent(Path.Combine(_pkiPath, "issued", $"{clientName}.crt"));
            string clientKeyContent = File.ReadAllText(Path.Combine(_pkiPath, "private", $"{clientName}.key"));

            Console.WriteLine("Step 4: Generating .ovpn configuration file...");
            string ovpnContent = GenerateOvpnFile(clientName, serverIp, caCertContent, clientCertContent, clientKeyContent, _tlsAuthKey);
            string ovpnFilePath = Path.Combine(_outputDir, $"{clientName}.ovpn");

            Console.WriteLine("Step 5: Ensuring output directory exists...");
            Directory.CreateDirectory(_outputDir);

            Console.WriteLine("Step 6: Writing .ovpn file...");
            File.WriteAllText(ovpnFilePath, ovpnContent);

            Console.WriteLine($"Client configuration file created: {ovpnFilePath}");
            return new FileInfo(ovpnFilePath);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private static string GenerateOvpnFile(string clientName, string serverIp, string caCert, string clientCert, string clientKey, string tlsAuthKey)
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
            .TakeWhile(line => !line.StartsWith("-----END CERTIFICATE-----")).Append("-----END CERTIFICATE-----"));
    }

}