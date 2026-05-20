namespace TransactionService.Domain.Handlers.Commands;

public record CreateTransactionCommand(string Description, DateOnly TransactionDate, decimal Amount);
