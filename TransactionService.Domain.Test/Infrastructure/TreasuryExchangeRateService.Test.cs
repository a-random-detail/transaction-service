using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Infrastructure;

namespace TransactionService.Domain.Test.Infrastructure;

[TestFixture]
public class TreasuryExchangeRateService_Test
{
    private ICacheService<TreasuryExchangeRateRecord> _cache = null!;
    private const string Country = "Euro Zone";
    private const string Currency = "Euro";
    private readonly DateOnly _purchaseDate = new(2026, 5, 22);

    [SetUp]
    public void SetUp() => _cache = Substitute.For<ICacheService<TreasuryExchangeRateRecord>>();

    private TreasuryExchangeRateService BuildService(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(new HttpClient(new FakeHttpMessageHandler(responseJson, statusCode))
                { BaseAddress = new Uri("https://api.fiscaldata.treasury.gov") },
            _cache, NullLogger<TreasuryExchangeRateService>.Instance);

    [Test]
    public async Task GetExchangeRateAsync_Returns_Cached_Result_Without_Calling_Api()
    {
        var cached = new TreasuryExchangeRateRecord("2026-03-31", Country, Currency, "Euro Zone-Euro", "0.87");
        _cache.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns(cached);

        var service = BuildService("""{"data":[]}""");
        var result = await service.GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task GetExchangeRateAsync_Returns_Fail_When_No_Rate_Found()
    {
        _cache.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns((TreasuryExchangeRateRecord?)null);

        var service = BuildService("""{"data":[]}""");
        var result = await service.GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.Success, Is.False);
        Assert.That(result.GetErrors(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetExchangeRateAsync_Returns_Correct_Exchange_Rate()
    {
        var record = new TreasuryExchangeRateRecord("2026-03-31", Country, Currency, "Euro Zone-Euro", "0.87");
        _cache.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns(record);

        var service = BuildService("""{"data":[]}""");
        var result = await service.GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.GetValue()!.ExchangeRate, Is.EqualTo("0.87"));
    }

    [Test]
    public async Task GetExchangeRateAsync_Uses_Most_Recent_Rate_On_Or_Before_Purchase_Date()
    {
        _cache.GetOrSetAsync(
                Arg.Any<string>(),
                Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns(callInfo => callInfo.Arg<Func<Task<TreasuryExchangeRateRecord?>>>()());

        var json = """
                   {
                       "data": [
                           {
                               "record_date": "2026-03-31",
                               "country": "Euro Zone",
                               "currency": "Euro",
                               "country_currency_desc": "Euro Zone-Euro",
                               "exchange_rate": "0.87"
                           },
                           {
                               "record_date": "2025-12-31",
                               "country": "Euro Zone",
                               "currency": "Euro",
                               "country_currency_desc": "Euro Zone-Euro",
                               "exchange_rate": "0.851"
                           }
                       ]
                   }
                   """;

        var service = BuildService(json);
        var result = await service.GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.Success, Is.True);
        Assert.That(result.GetValue()!.ExchangeRate, Is.EqualTo("0.87"));
    }
      
    [Test]
    public async Task GetExchangeRateAsync_Returns_ServerError_When_Api_Returns_Non_Success_Status()
    {
        _cache.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns(callInfo => callInfo.Arg<Func<Task<TreasuryExchangeRateRecord?>>>()());

        var result = await BuildService("{}", HttpStatusCode.InternalServerError)
            .GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Type, Is.EqualTo(ResultType.ServerError));
    }

    [Test]
    public async Task GetExchangeRateAsync_Returns_ServerError_When_Api_Returns_Malformed_Json()
    {
        _cache.GetOrSetAsync(Arg.Any<string>(), Arg.Any<Func<Task<TreasuryExchangeRateRecord?>>>())
            .Returns(callInfo => callInfo.Arg<Func<Task<TreasuryExchangeRateRecord?>>>()());

        var result = await BuildService("not valid json")
            .GetExchangeRateAsync(Country, Currency, _purchaseDate);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Type, Is.EqualTo(ResultType.ServerError));
    }
}
