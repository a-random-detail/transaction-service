using Microsoft.Extensions.Logging;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers.Queries;
using TransactionService.Domain.Infrastructure;
using TransactionService.Domain.Repositories;

namespace TransactionService.Domain.Handlers;

public class GetTransactionByIdHandler(
    ITransactionReadRepository db,
    ITreasuryExchangeRateService treasuryExchangeRateService,
    ILogger<GetTransactionByIdHandler> logger)
    : IQueryHandler<GetTransactionByIdQuery, Result<ConvertedTransactionDto>>
{
    public async Task<Result<ConvertedTransactionDto>> HandleAsync(GetTransactionByIdQuery query, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching transaction with id {Id}", query.Id);

        var transactionResult = await db.GetAsync(query);
        if (!transactionResult.Success || transactionResult.GetValue() is null)
        {
            logger.LogWarning("Transaction with id {Id} not found", query.Id);
            return Result<ConvertedTransactionDto>.Fail("Transaction not found.");
        }

        var transaction = transactionResult.GetValue()!;
        logger.LogInformation("Transaction {Id} fetched successfully", query.Id);

        var rateResult = await treasuryExchangeRateService.GetExchangeRateAsync(
            query.Country, query.Currency, transaction.TransactionDate);

        if (!rateResult.Success)
            return Result<ConvertedTransactionDto>.Fail(rateResult.GetErrors().ToArray());

        var rate = decimal.Parse(rateResult.GetValue()!.ExchangeRate);
        var converted = Math.Round(transaction.Amount * rate, 2, MidpointRounding.AwayFromZero);

        return Result<ConvertedTransactionDto>.OK(new ConvertedTransactionDto(
            transaction.Id,
            transaction.Description,
            transaction.TransactionDate,
            transaction.Amount,
            rate,
            converted,
            query.Country,
            query.Currency,
            DateOnly.Parse(rateResult.GetValue()!.RecordDate)));
    }
}