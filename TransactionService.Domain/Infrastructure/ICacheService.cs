namespace TransactionService.Domain.Infrastructure;

public interface ICacheService<T>
{
    Task<T?> GetOrSetAsync(string key, Func<Task<T?>> factory);
    Task InvalidateAsync(string key);
}

