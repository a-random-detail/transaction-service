namespace TransactionService.Domain.Entities;

public record TransactionDto(Guid Id, string Description, DateOnly TransactionDate, decimal Amount);