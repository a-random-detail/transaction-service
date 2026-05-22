using Microsoft.AspNetCore.Mvc;
using TransactionService.Contracts;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers;
using TransactionService.Domain.Handlers.Commands;
using TransactionService.Domain.Handlers.Queries;
using TransactionService.Domain.Validators;

namespace TransactionService.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ICommandHandler<CreateTransactionCommand, Result<TransactionDto>> _createTransactionHandler;
    private readonly IQueryHandler<GetTransactionByIdQuery, Result<ConvertedTransactionDto>> _getTransactionByIdHandler;
    private readonly ITransactionValidator _transactionValidator;
    public TransactionsController(ICommandHandler<CreateTransactionCommand, Result<TransactionDto>> createTransactionHandler, ITransactionValidator transactionValidator, IQueryHandler<GetTransactionByIdQuery, Result<ConvertedTransactionDto>> getTransactionByIdHandler)
    {
        _createTransactionHandler = createTransactionHandler;
        _transactionValidator = transactionValidator;
        _getTransactionByIdHandler = getTransactionByIdHandler;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById([FromRoute] Guid id, [FromQuery] string country, [FromQuery] string currency)
    {
        var result = await _getTransactionByIdHandler.HandleAsync(new GetTransactionByIdQuery(id, country, currency));
        if (!result.Success || result.GetValue() == null)
            return NotFound();
        
        return Ok(ApiResponse<ConvertedTransactionDto>.OK(result.GetValue()));
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var requestIsValid = _transactionValidator.IsValid(request, out List<string> errors);

        if (!requestIsValid)
        {
            return BadRequest(ApiResponse<TransactionDto>.Fail(errors.ToArray()));
        }

        var command = CreateTransactionCommand.FromRequest(request);

        var result = await _createTransactionHandler.HandleAsync(command);

        return StatusCode(201, ApiResponse<TransactionDto>.OK(result.GetValue()));
    }
}
