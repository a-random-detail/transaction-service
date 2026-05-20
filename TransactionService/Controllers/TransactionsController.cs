using Microsoft.AspNetCore.Mvc;
using TransactionService.Contracts;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers;
using TransactionService.Domain.Handlers.Commands;

namespace TransactionService.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ICommandHandler<CreateTransactionCommand, Result<TransactionDto>> _createTransactionHandler;
    public TransactionsController(ICommandHandler<CreateTransactionCommand, Result<TransactionDto>> createTransactionHandler)
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

        var command = CreateTransactionCommand.FromRequest(request);

        var result = await _createTransactionHandler.HandleAsync(command);

        return StatusCode(201, ApiResponse<TransactionDto>.OK(result.GetValue()));
    }
}
