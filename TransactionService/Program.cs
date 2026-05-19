using System.Data;
using System.Reflection;
using DbUp;
using TransactionService.Configuration;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
var appConfig = new AppConfig();
builder.Services.AddSingleton(appConfig);
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(appConfig.GetConnectionString()));

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
    .PostgresqlDatabase(appConfig.GetConnectionString())
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build()
    .PerformUpgrade();

app.Run();