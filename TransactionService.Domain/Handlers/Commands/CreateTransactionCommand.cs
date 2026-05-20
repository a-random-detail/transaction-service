using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Handlers.Commands;

public record CreateTransactionCommand(string Description, DateTime TransactionDate, decimal Amount)
{
    public static CreateTransactionCommand FromRequest(CreateTransactionRequest request) =>
        new(request.Description, DateTime.Parse(request.TransactionDate).ToUniversalTime(), request.Amount);
}
