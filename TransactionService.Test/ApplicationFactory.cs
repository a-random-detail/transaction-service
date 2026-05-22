using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TransactionService.Domain.Infrastructure;
using TransactionService.Test.Helpers;

namespace TransactionService.Test;

public class ApplicationFactory: WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    private Respawner _respawner = null!;
    public string ConnectionString => _postgres.GetConnectionString();
    public FakeExchangeRateService ExchangeRateService { get; } = new();

    public async Task StartAsync()
    {
        await _postgres.StartAsync();
        Environment.SetEnvironmentVariable("WRITE_DB_CONNSTRING", _postgres.GetConnectionString());
        Environment.SetEnvironmentVariable("READ_DB_CONNSTRING", _postgres.GetConnectionString());
        Environment.SetEnvironmentVariable("REDIS_CONNSTRING", "localhost:6379,abortConnect=false");
        Environment.SetEnvironmentVariable("REDIS_CONNSTRING", "localhost:6379,abortConnect=false");
        Environment.SetEnvironmentVariable("EXCHANGE_RATE_BASE_URL",
            "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange");
    }

    public async Task InitializeRespawner()
    {
        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection);
    }

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public new async Task DisposeAsync() => await _postgres.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IWriteConnectionFactory>();
            services.RemoveAll<IReadConnectionFactory>();
            services.RemoveAll<ITreasuryExchangeRateService>();

            services.AddSingleton<IWriteConnectionFactory>(new WriteConnectionFactory(_postgres.GetConnectionString()));
            services.AddSingleton<IReadConnectionFactory>(new ReadConnectionFactory(_postgres.GetConnectionString()));
            services.AddSingleton<ITreasuryExchangeRateService>(ExchangeRateService);
        });
    }
}