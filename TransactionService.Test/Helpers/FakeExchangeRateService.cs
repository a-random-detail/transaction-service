using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Infrastructure;

namespace TransactionService.Test.Helpers;

public class FakeExchangeRateService : ITreasuryExchangeRateService
{
    public Result<TreasuryExchangeRateRecord>? Response { get; set; } = Result<TreasuryExchangeRateRecord>.OK(
        new TreasuryExchangeRateRecord("2026-03-31", "Euro Zone", "Euro", "Euro Zone-Euro", "0.87"));

    public Task<Result<TreasuryExchangeRateRecord>> GetExchangeRateAsync(string country, string currency, DateOnly purchaseDate)
        => Task.FromResult(Response!);
}