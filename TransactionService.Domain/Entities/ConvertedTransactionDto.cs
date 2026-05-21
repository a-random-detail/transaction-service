namespace TransactionService.Domain.Entities;

public record ConvertedTransactionDto(Guid Id, string Description, DateOnly TransactionDate, decimal AmountUsd, string Country, string Currency, decimal ExchangeRate, decimal ConvertedAmount);