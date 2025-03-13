using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DataGateVPNBot.Services.DashboardServices;

public class DashBoardApiAuthService
{
    private readonly IHttpClientFactoryService _httpClientFactoryService;
    private readonly RedisCacheService _redisCacheService;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly ILogger<DashBoardApiAuthService> _logger;

    public DashBoardApiAuthService(
        IHttpClientFactoryService httpClientFactoryService,
        RedisCacheService redisCacheService,
        string clientId,
        string clientSecret,
        ILogger<DashBoardApiAuthService> logger)
    {
        _httpClientFactoryService = httpClientFactoryService;
        _redisCacheService = redisCacheService;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync()
    {
        var cachedToken = await _redisCacheService.GetTokenAsync();//todo: check expired date
        if (!string.IsNullOrEmpty(cachedToken))
        {
            _logger.LogInformation("Using cached token from Redis.");
            return cachedToken;
        }

        _logger.LogInformation("Cached token not found. Requesting new token...");

        var httpClient = _httpClientFactoryService.CreateDashboardClient();
        var requestBody = new
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, 
            "application/json");

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                _logger.LogInformation($"Attempt {attempt}: Requesting token from dashboard...");

                var response = await httpClient.PostAsync("api/token", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get token (Attempt {attempt}): " +
                                       $"{response.StatusCode} - {response.ReasonPhrase}");
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return null;
                    }

                    await Task.Delay(1000 * attempt);
                    continue;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (jsonResponse.TryGetProperty("token", out var tokenProperty))
                {
                    var newToken = tokenProperty.GetString();
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        await _redisCacheService.SetTokenAsync(newToken);
                        _logger.LogInformation("New token saved in Redis.");
                        return newToken;
                    }
                }

                _logger.LogError("Invalid response format: 'token' field not found.");
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error (Attempt {attempt}): {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"Request timeout (Attempt {attempt}): {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error (Attempt {attempt}): {ex.Message}");
            }

            await Task.Delay(1000 * attempt);
        }

        _logger.LogError("Failed to obtain token after 3 attempts.");
        return null;
    }
}