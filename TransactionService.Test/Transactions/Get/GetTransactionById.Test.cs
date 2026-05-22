using System.Net;
using System.Net.Http.Json;
using StackExchange.Redis;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Test.Helpers;
using ResultType = TransactionService.Domain.Core.ResultType;

namespace TransactionService.Test.Transactions.Get;

[TestFixture]
public class GetTransactionById_Test
{

    private ApplicationFactory _appFactory = null!;
    private HttpClient _client = null!;
    private TransactionTestRepository _testRepository = null!;

    private TransactionDto? _validTransaction = null;
    private string GetTransactionEndpoint(Guid id, string country, string currency) =>
        $"/transactions/{id}?country={Uri.EscapeDataString(country)}&currency={Uri.EscapeDataString(currency)}";

    private string GetParameterlessTransactionEndpoint(Guid id) => $"/transactions/{id}";
    private const string _ValidCountry = "Euro Zone";
    private const string _ValidCurrency = "Euro";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _appFactory = new ApplicationFactory();
        await _appFactory.StartAsync();
        _client = _appFactory.CreateClient();
        await _appFactory.InitializeRespawner();
        _testRepository = new TransactionTestRepository(_appFactory.ConnectionString);
    }

    [SetUp]
    public async Task SetUp()
    {
        _appFactory.ExchangeRateService.Response = Result<TreasuryExchangeRateRecord>.OK(
            new TreasuryExchangeRateRecord("2026-03-31", "Euro Zone", "Euro", "Euro Zone-Euro", "0.87"));

        var transactionRequest = new CreateTransactionRequest("valid transaction here", "2026-05-03", 49.99m);
        _validTransaction = await _testRepository.Create(transactionRequest);
    }

    [TearDown]
    public async Task TearDown() => await _appFactory.ResetAsync();

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        await _appFactory.DisposeAsync();
    }

    [Test]
    public async Task GetTransactionById_With_Valid_Id_Returns_OK()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id, _ValidCountry, _ValidCurrency));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responsePayload = await response.Content.ReadFromJsonAsync<ApiResponseDto<ConvertedTransactionDto>>();
        
        Assert.That(responsePayload, Is.Not.Null);
        Assert.That(responsePayload.Success, Is.True);
        
        var exchangePayload = responsePayload.Data;
        Assert.That(exchangePayload, Is.Not.Null);
        Assert.That(exchangePayload.Description, Is.EqualTo(_validTransaction.Description));
        Assert.That(exchangePayload.TransactionDate, Is.EqualTo(_validTransaction.TransactionDate));
        Assert.That(exchangePayload.AmountUsd, Is.EqualTo(_validTransaction.Amount));
        Assert.That(exchangePayload.Country, Is.EqualTo(_ValidCountry));
        Assert.That(exchangePayload.Currency, Is.EqualTo(_ValidCurrency));
        
        // Values taken from and derived from FakeExchangeRateService
        Assert.That(exchangePayload.ExchangeRateDate, Is.EqualTo(new DateOnly(2026, 3, 31)));
        Assert.That(exchangePayload.ConvertedAmount, Is.EqualTo(43.49m)); 
        Assert.That(exchangePayload.ExchangeRate, Is.EqualTo(0.87m));
    }

    [Test]
    public async Task GetTransactionById_With_No_Country_Returns_400()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id, "", _ValidCurrency));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task GetTransactionById_With_No_Currency_Returns_400()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id, _ValidCountry, ""));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task GetTransactionById_With_Missing_Parameters_Returns_400()
    {
        var response = await _client.GetAsync(GetParameterlessTransactionEndpoint(_validTransaction!.Id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetTransactionById_Returns_422_With_Appropriate_Error_Message_When_No_Exchange_Records()
    {
        _appFactory.ExchangeRateService.Response = Result<TreasuryExchangeRateRecord>.Fail(ResultType.NoRecordsFound,
            $"Unable to convert purchase to {_ValidCountry}-{_ValidCurrency } — no exchange rate available within 6 months of the purchase date.");
        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id, _ValidCountry, _ValidCurrency));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));

        var responsePayload = await response.Content.ReadFromJsonAsync<ApiResponseDto<ConvertedTransactionDto>>();
        Assert.That(responsePayload!.Success, Is.False);
        Assert.That(responsePayload.Errors, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetTransactionById_With_No_Matching_Id_Returns_NotFound()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(Guid.Empty, _ValidCountry, _ValidCurrency));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetTransactionById_Returns_500_When_Exchange_Rate_Service_Encounters_An_Error()
    {
        _appFactory.ExchangeRateService.Response = Result<TreasuryExchangeRateRecord>.Fail(
            ResultType.ServerError, "Exchange rate service is unavailable.");

        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id, _ValidCountry, _ValidCurrency));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

        var responsePayload = await response.Content.ReadFromJsonAsync<ApiResponseDto<ConvertedTransactionDto>>();
        Assert.That(responsePayload!.Success, Is.False);
        Assert.That(responsePayload.Errors, Has.Count.EqualTo(1));
    }
}
