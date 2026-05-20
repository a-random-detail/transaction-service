namespace TransactionService.Domain.Handlers;

public interface ICommandHandler<in TData, TResult>
{
    Task<TResult> HandleAsync(TData data, CancellationToken ct = default);
}