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

    public async Task<TransactionDto?> Create(CreateTransactionRequest request)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleAsync<TransactionDto>(
        
            """
            Insert into transactions (description, transaction_date, amount)
            values (@Description, @TransactionDate, @Amount)
            returning id, description, transaction_date, amount
            """,
            new { Description = request.Description, TransactionDate = DateHelper.ParseStringToDateTime(request.TransactionDate), Amount = request.Amount });
    }
}