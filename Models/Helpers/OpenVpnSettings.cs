namespace DataGateVPNBotV1.Models.Helpers;

public class OpenVpnSettings
{
    public string EasyRsaPath { get; init; } = "/etc/openvpn/easy-rsa";
    public string OutputDir { get; init; } = "/etc/openvpn/clients";
    public string TlsAuthKey { get; init; } = "/etc/openvpn/easy-rsa/pki/ta.key";
    public string ServerIp { get; init; } = "213.133.91.43";
}