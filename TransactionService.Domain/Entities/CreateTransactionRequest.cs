namespace TransactionService.Domain.Entities;

public record CreateTransactionRequest(string Description, DateOnly TransactionDate, Decimal Amount);