using Dapper;
using Npgsql;
using TransactionService.Domain.Entities;

namespace TransactionService.Test.Helpers;

public class TransactionTestRepository(string ConnectionString)
{
    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<TransactionDto>("Select * from transactions where id = @id",
            new { id });
    }
}