namespace TransactionService.Configuration;
public class AppConfig
{
    private string DbHost { get; init; } = Environment.GetEnvironmentVariable("DB_HOST") ?? string.Empty;
    private string DbName { get; init; } = Environment.GetEnvironmentVariable("DB_NAME") ?? string.Empty; 
    private string DbUser { get; init; } = Environment.GetEnvironmentVariable("DB_USER") ?? string.Empty;
    private string DbPort { get; init; } = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    private string DbPassword { get; init; } = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;


    public string GetConnectionString() =>
        $"Host={DbHost};Port={DbPort};Database={DbName};Username={DbUser};Password={DbPassword}";
}
