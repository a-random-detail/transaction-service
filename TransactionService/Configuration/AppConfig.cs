namespace TransactionService.Configuration;
public class AppConfig
{
    public string WriteConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("WRITE_DB_CONNSTRING") ?? string.Empty;

    public string ReadConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("READ_DB_CONNSTRING") ?? string.Empty;

    public String RedisConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("REDIS_CONNSTRING") ?? string.Empty;
}
