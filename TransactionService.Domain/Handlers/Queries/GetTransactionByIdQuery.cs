namespace TransactionService.Domain.Handlers.Queries;

public record GetTransactionByIdQuery(Guid Id, string Country, string Currency);