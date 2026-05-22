using System.Text.Json.Serialization;

namespace TransactionService.Domain.Entities;

public record ExchangeRateResponse(IReadOnlyList<TreasuryExchangeRateRecord> Data);

public record TreasuryExchangeRateRecord(
    [property: JsonPropertyName("record_date")] string RecordDate,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("country_currency_desc")] string CountryCurrencyDesc,
    [property: JsonPropertyName("exchange_rate")] string ExchangeRate);
    