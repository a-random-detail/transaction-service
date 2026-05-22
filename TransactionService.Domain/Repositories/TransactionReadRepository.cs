using Dapper;
using Microsoft.Extensions.Logging;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers.Queries;
using TransactionService.Domain.Infrastructure;

namespace TransactionService.Domain.Repositories;

public interface ITransactionReadRepository
{
    Task<Result<TransactionDto>> GetAsync(GetTransactionByIdQuery query);
}

public class TransactionReadRepository (
    IReadConnectionFactory connectionFactory,
    ILogger<TransactionReadRepository> logger
    )
    : ITransactionReadRepository 
{
    public async Task<Result<TransactionDto>> GetAsync(GetTransactionByIdQuery query)
    {
        logger.LogDebug("Fetching transaction with id: {Id}", query.Id);

        using var connection = connectionFactory.Create();

        try
        {
            var dto = await connection.QuerySingleOrDefaultAsync<TransactionDto>(
                """
                Select * from transactions where id = @Id
                """,
                new {query.Id }
            );

            return Result<TransactionDto>.OK(dto);
        }
        catch (Exception ex)
        {
            logger.LogError("Database error fetching transaction with id: {Id} - Message: {Message}", query.Id, ex.Message);
            return Result<TransactionDto>.Fail(ResultType.ServerError, "Unable to fetch transaction.");
        }
    }
}