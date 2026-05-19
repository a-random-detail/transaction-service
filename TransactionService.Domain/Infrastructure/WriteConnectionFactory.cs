using System.Data;
using Npgsql;

namespace TransactionService.Domain.Infrastructure;

public interface IWriteConnectionFactory
{
    IDbConnection Create();
}

public class WriteConnectionFactory(string connectionString) : IWriteConnectionFactory
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}