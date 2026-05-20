using Microsoft.Extensions.Logging;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Infrastructure;
using TransactionService.Domain.Repositories;

namespace TransactionService.Domain.Handlers.Commands;

public class CreateTransactionHandler(
    ITransactionWriteRepository db,
    ILogger<CreateTransactionHandler> logger)
    : ICommandHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> HandleAsync(CreateTransactionCommand data, CancellationToken ct = default)
    {
        logger.LogInformation("Creating transaction {Description} on {TransactionDate}", data.Description, data.TransactionDate);

        var saveResult = await db.CreateAsync(data);

        if (!saveResult.Success)
            return Result<TransactionDto>.Fail("Unable to save transaction. Please try again later");
        
        logger.LogInformation("Transaction created {TransactionId}", saveResult.GetValue()!.Id);

        return saveResult;
    }
}