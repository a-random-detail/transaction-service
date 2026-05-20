using System.Net;
using System.Net.Http.Json;
using Npgsql;
using TransactionService.Contracts;
using TransactionService.Domain.Entities;
using TransactionService.Test.Helpers;

namespace TransactionService.Test.Transactions.Create;

[TestFixture]
public class CreateTransaction_Test
{

    private ApplicationFactory _appFactory = null!;
    private HttpClient _client = null!;
    private TransactionTestRepository _testRepository = null!;
    private string _postTransactionsEndpoint = "/transactions";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _appFactory = new ApplicationFactory();
        await _appFactory.StartAsync();
        _client = _appFactory.CreateClient();
        await _appFactory.InitializeRespawner();
        _testRepository = new TransactionTestRepository(_appFactory.ConnectionString);
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
    public async Task CreateTransaction_Returns_201_On_Success()
    {
        var payload =
            new CreateTransactionRequest("A lot of staplers", "2026-05-13", 55.55m);
        var response = await _client.PostAsJsonAsync(_postTransactionsEndpoint, payload);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
    
    [Test]
    public async Task CreateTransaction_Persists_To_Database()
    {
        var payload =
            new CreateTransactionRequest("A lot of staplers", "2026-05-01", 55.55m);
        var response = await _client.PostAsJsonAsync(_postTransactionsEndpoint, payload);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var responsePayload = await response.Content.ReadFromJsonAsync<ApiResponseDto<TransactionDto>>();
        
        
        Assert.That(responsePayload, Is.Not.Null);
        Assert.That(responsePayload.Success, Is.True);
        
        var transaction = responsePayload.Data;
        Assert.That(transaction, Is.Not.Null);
        Assert.That(transaction.Description, Is.EqualTo(payload.Description));
        Assert.That(transaction.TransactionDate, Is.EqualTo(ParseStringToDateOnly(payload.TransactionDate)));
        Assert.That(transaction.Amount, Is.EqualTo(payload.Amount));


        var persistedTransaction = await _testRepository.GetByIdAsync(transaction.Id);
        Assert.That(transaction, Is.EqualTo(persistedTransaction));
        
        Assert.That(persistedTransaction?.Description, Is.EqualTo(payload.Description));
        Assert.That(persistedTransaction?.TransactionDate, Is.EqualTo(ParseStringToDateOnly(payload.TransactionDate)));
        Assert.That(persistedTransaction?.Amount, Is.EqualTo(payload.Amount));
    }

    private DateOnly ParseStringToDateOnly(string date)
    {
        return DateOnly.Parse(date);
    }

}