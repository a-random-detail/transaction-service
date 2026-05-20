using Microsoft.AspNetCore.Mvc;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers;

namespace TransactionService.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ICommandHandler<CreateTranactionCommand, Result<TransactionDto>> _createTransactionHandler;
    public TransactionsController(ICommandHandler<CreateTranactionCommand, Result<TransactionDto>> createTransactionHandler)
    {
        _createTransactionHandler = createTransactionHandler;
    }

    [HttpGet("/{id}")]
    public async Task<IActionResult> GetTransactionById([FromRoute] string id)
    {
        // return Ok(ApiResponse<string>.OK($"hello, {id}!"));
        throw new NotImplementedException();
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        
    }

}
