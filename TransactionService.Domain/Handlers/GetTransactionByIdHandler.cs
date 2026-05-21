using Microsoft.Extensions.Logging;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers.Queries;
using TransactionService.Domain.Repositories;

namespace TransactionService.Domain.Handlers;

public class GetTransactionByIdHandler(
    ITransactionReadRepository db, 
    ILogger<GetTransactionByIdHandler> logger)
    : IQueryHandler<GetTransactionByIdQuery, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> HandleAsync(GetTransactionByIdQuery query, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching transaction with id {Id}", query.Id);

        var getResult = await db.GetAsync(query);
        if (!getResult.Success)
            return Result<TransactionDto>.Fail("Unable to fetch transaction. Please try again later.");
        
        logger.LogInformation("Transaction with id {Id} fetched successfully from database", query.Id);
        return getResult; 
    }
}