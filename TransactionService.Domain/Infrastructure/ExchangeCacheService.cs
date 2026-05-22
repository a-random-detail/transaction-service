using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Infrastructure;

public class ExchangeCacheService(IConnectionMultiplexer redis, TimeSpan cacheTtl, ILogger<ExchangeCacheService> logger) :
    ICacheService<TreasuryExchangeRateRecord>
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<TreasuryExchangeRateRecord?> GetOrSetAsync(string key, Func<Task<TreasuryExchangeRateRecord?>> factory)
    {
        var cached = await _db.StringGetAsync(key);
        if (!cached.IsNullOrEmpty)
        {
            logger.LogDebug("Cache hit for key {Key}", key);
            return JsonSerializer.Deserialize<TreasuryExchangeRateRecord>((string)cached!);
        }

        var value = await factory();
        if (value is not null)
            await _db.StringSetAsync(key, JsonSerializer.Serialize(value), cacheTtl);

        return value;
    }

    public async Task InvalidateAsync(string key) => await _db.KeyDeleteAsync(key);
}
