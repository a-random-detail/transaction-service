namespace TransactionService.Configuration;
public class AppConfig
{
    public string WriteConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("WRITE_DB_CONNSTRING") ?? string.Empty;

    public string ReadConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("READ_DB_CONNSTRING") ?? string.Empty;

    public String RedisConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("REDIS_CONNSTRING") ?? string.Empty;
    
    public int ExchangeRateCacheTtlHours { get; init; } =
        int.TryParse(Environment.GetEnvironmentVariable("EXCHANGE_RATE_CACHE_TTL_HOURS"), out var ttl) ? ttl : 24;
    
    public string TreasuryApiBaseUrl { get; init; } =
        Environment.GetEnvironmentVariable("EXCHANGE_RATE_BASE_URL") ?? string.Empty;
}
