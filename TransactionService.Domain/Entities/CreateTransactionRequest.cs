namespace TransactionService.Domain.Entities;

public record CreateTransactionRequest(string Description, DateOnly TransactionDate, decimal Amount);