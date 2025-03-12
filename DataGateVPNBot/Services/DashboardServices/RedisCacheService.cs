using StackExchange.Redis;

namespace DataGateVPNBot.Services.DashboardServices;

public class RedisCacheService
{
    private readonly IDatabase _cache;
    private readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(1);

    public RedisCacheService(string redisConnectionString)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _cache = redis.GetDatabase();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _cache.StringGetAsync("vpn_token");
    }

    public async Task SetTokenAsync(string token)
    {
        await _cache.StringSetAsync("vpn_token", token, _tokenExpiration);
    }
}
