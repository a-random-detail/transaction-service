namespace TransactionService.Domain.Entities;

public record ConvertedTransactionDto(Guid Id, string Description, DateOnly TransactionDate, decimal AmountUsd, decimal ExchangeRate, decimal ConvertedAmount, string Country, string Currency, DateOnly ExchangeRateDate);