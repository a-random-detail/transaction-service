using System.Data;
using System.Reflection;
using DbUp;
using TransactionService.Configuration;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var appConfig = new AppConfig();
builder.Services.AddSingleton(appConfig);
builder.Services.AddScoped<IDbConnection>(_ =>
{
    return new NpgsqlConnection(appConfig.GetConnectionString());
});

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