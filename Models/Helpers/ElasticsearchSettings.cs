namespace DataGateVPNBotV1.Models.Helpers;

public class ElasticsearchSettings
{
    public string Uri { get; set; } = "http://localhost:9200";
    public string Username { get; set; }
    public string Password { get; set; }
    public string IndexFormat { get; set; } = "app-logs-{0:yyyy.MM.dd}";
}