namespace TransactionService.Domain.Entities;

public record CreateTransactionRequest(string Description, DateTimeOffset TransactionDate, Decimal Amount);