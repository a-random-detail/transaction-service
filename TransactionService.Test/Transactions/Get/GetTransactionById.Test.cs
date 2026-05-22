using System.Net;
using System.Net.Http.Json;
using TransactionService.Domain.Entities;
using TransactionService.Test.Helpers;

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
        
        //TODO fill this in with a mock service?
        Assert.That(exchangePayload.ExchangeRateDate, Is.InRange(_validTransaction.TransactionDate.AddMonths(-6), _validTransaction.TransactionDate));
        Assert.That(exchangePayload.ExchangeRate, Is.GreaterThan(0));
        Assert.That(exchangePayload.ConvertedAmount, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTransactionById_With_No_Matching_Id_Returns_NotFound()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(Guid.Empty, _ValidCountry, _ValidCurrency));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
