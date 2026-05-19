using System.Data;
using Npgsql;

namespace TransactionService.Domain.Infrastructure;

public interface IReadConnectionFactory
{
    IDbConnection Create();
}

public class ReadConnectionFactory(string connectionString) : IReadConnectionFactory
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}