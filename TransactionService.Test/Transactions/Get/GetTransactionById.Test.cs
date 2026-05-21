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
    private string GetTransactionEndpoint(Guid id) =>  $"/transactions/{id}";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _appFactory = new ApplicationFactory();
        await _appFactory.StartAsync();
        _client = _appFactory.CreateClient();
        await _appFactory.InitializeRespawner();
        _testRepository = new TransactionTestRepository(_appFactory.ConnectionString);
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
        var response = await _client.GetAsync(GetTransactionEndpoint(_validTransaction!.Id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responsePayload = await response.Content.ReadFromJsonAsync<ApiResponseDto<TransactionDto>>();
        
        Assert.That(responsePayload, Is.Not.Null);
        Assert.That(responsePayload.Success, Is.True);
        
        var transaction = responsePayload.Data;
        Assert.That(transaction, Is.Not.Null);
        Assert.That(transaction.Description, Is.EqualTo(_validTransaction.Description));
        Assert.That(transaction.TransactionDate, Is.EqualTo(_validTransaction.TransactionDate));
        Assert.That(transaction.Amount, Is.EqualTo(_validTransaction.Amount));
    }

    [Test]
    public async Task GetTransactionById_With_No_Matching_Id_Returns_NotFound()
    {
        var response = await _client.GetAsync(GetTransactionEndpoint(new Guid()));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
