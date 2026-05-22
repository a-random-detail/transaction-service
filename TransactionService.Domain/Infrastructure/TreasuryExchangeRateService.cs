using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Infrastructure;

public interface ITreasuryExchangeRateService
{
    Task<Result<TreasuryExchangeRateRecord>> GetExchangeRateAsync(string country, string currency, DateOnly purchaseDate);
}

public class TreasuryExchangeRateService(
    HttpClient client,
    ICacheService<TreasuryExchangeRateRecord> cacheService,
    ILogger<TreasuryExchangeRateService> logger) : ITreasuryExchangeRateService
{
    private const string TreasuryFields = "record_date,country,currency,country_currency_desc,exchange_rate";
    private string CacheKey(string country, string currency, DateOnly purchaseDate) =>
        $"exchange_rate:{country}:{currency}:{purchaseDate:yyyy-MM-dd}";
    
    public async Task<Result<TreasuryExchangeRateRecord>> GetExchangeRateAsync(string country, string currency, DateOnly purchaseDate)
    {
        logger.LogInformation("Fetching from treasury api for country: {country}, currency: {currency}, for purchase date: {purchaseDate:yyyy-MM-dd}", country, currency, purchaseDate);
        var rate = await cacheService.GetOrSetAsync(
            CacheKey(country, currency, purchaseDate),
            () => FetchRateFromTreasury(country, currency, purchaseDate));

        if (rate is not null) return Result<TreasuryExchangeRateRecord>.OK(rate);
        
        logger.LogWarning("No exchange rate found for {Country} {Currency} within 6 months of {PurchaseDate}",
            country, currency, purchaseDate);
        return Result<TreasuryExchangeRateRecord>.Fail(
            $"Unable to convert purchase to {currency} — no exchange rate available within 6 months of the purchase date.");
    }
    private async Task<TreasuryExchangeRateRecord?> FetchRateFromTreasury(string country, string currency, DateOnly purchaseDate)
    {
        var response = await client.GetFromJsonAsync<ExchangeRateResponse>(BuildRequestUrl(country, currency, purchaseDate));
        return response?.Data.Count > 0 ? response.Data[0] : null;
    }

    private string BuildRequestUrl(string country, string currency, DateOnly purchaseDate) =>
        $"?fields={TreasuryFields}" +
        $"&filter=country:eq:{country},currency:eq:{currency}" +
        $",record_date:lte:{purchaseDate:yyyy-MM-dd},record_date:gte:{purchaseDate.AddMonths(-6):yyyy-MM-dd}" +
        $"&sort=-record_date&page[size]=1";
    
}