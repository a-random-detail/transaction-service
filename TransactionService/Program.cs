using System.Data;
using System.Reflection;
using Dapper;
using DbUp;
using TransactionService.Configuration;
using Npgsql;
using Serilog;
using StackExchange.Redis;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers;
using TransactionService.Domain.Handlers.Commands;
using TransactionService.Domain.Infrastructure;
using TransactionService.Domain.Repositories;

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
builder.Services.AddScoped<ITransactionWriteRepository, TransactionWriteRepository>();
builder.Services.AddScoped<ICommandHandler<CreateTransactionCommand, Result<TransactionDto>>, CreateTransactionHandler>();


// Handle snake case to Pascal Case field matching
DefaultTypeMap.MatchNamesWithUnderscores = true;

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