using System.Data;
using System.Reflection;
using DbUp;
using TransactionService.Configuration;
using Npgsql;
using Serilog;
using StackExchange.Redis;
using TransactionService.Domain.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
var appConfig = new AppConfig();
builder.Services.AddSingleton(appConfig);
builder.Services.AddSingleton<IWriteConnectionFactory>(new WriteConnectionFactory(appConfig.WriteConnectionString));
builder.Services.AddSingleton<IReadConnectionFactory>(new ReadConnectionFactory(appConfig.ReadConnectionString));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(appConfig.RedisConnectionString));
builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DeployChanges.To
    .PostgresqlDatabase(appConfig.WriteConnectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build()
    .PerformUpgrade();

app.Run();