namespace DataGateVPNBot.Models.Helpers;

public class OpenVpnSettings
{
    public string EasyRsaPath { get; init; } = "/etc/openvpn/easy-rsa";
    public string OutputDir { get; init; } = "/etc/openvpn/clients";
    public string TlsAuthKey { get; init; } = "/etc/openvpn/easy-rsa/pki/ta.key";
    public string ServerIp { get; init; } = "213.133.91.43";
    public string CrlPkiPath { get; init; } = "/etc/openvpn/easy-rsa/pki/crl.pem";
    public string CrlOpenvpnPath { get; init; } = "/etc/openvpn/crl.pem";
    public string StatusFilePath { get; init; } = "/var/log/openvpn-status.log";
}