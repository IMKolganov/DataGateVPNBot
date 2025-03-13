using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DataGateVPNBot.Services.DashboardServices;

public class RedisCacheService
{
    private readonly IDatabase _cache;
    private readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(1);

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _cache.StringGetAsync("dashboard_openvpn_token");
    }

    public async Task SetTokenAsync(string token)
    {
        await _cache.StringSetAsync("dashboard_openvpn_token", token, _tokenExpiration);
    }
}