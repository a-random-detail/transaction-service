namespace TransactionService.Domain.Entities;

public record CreateTransactionRequest(string Description, string TransactionDate, decimal Amount);