namespace TransactionService.Domain.Infrastructure;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task InvalidateAsync(string key);
}

public class CacheService: ICacheService
{
    public Task<T?> GetAsync<T>(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        throw new NotImplementedException();
    }

    public Task InvalidateAsync(string key)
    {
        throw new NotImplementedException();
    }
}