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
public class TransactionsController(
    ICommandHandler<CreateTransactionCommand, Result<TransactionDto>> createTransactionHandler,
    ITransactionValidator transactionValidator,
    IQueryHandler<GetTransactionByIdQuery, Result<ConvertedTransactionDto>> getTransactionByIdHandler)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById([FromRoute] Guid id, [FromQuery] string country, [FromQuery] string currency)
    {
        var result = await getTransactionByIdHandler.HandleAsync(new GetTransactionByIdQuery(id, country, currency));
        if (!result.Success)
            return result.Type switch
            {
                ResultType.NotFound =>
                    NotFound(ApiResponse<ConvertedTransactionDto>.Fail(result.GetErrors().ToArray())),
                ResultType.ServerError =>
                    StatusCode(500, ApiResponse<ConvertedTransactionDto>.Fail(result.GetErrors().ToArray())),
                _ => UnprocessableEntity(ApiResponse<ConvertedTransactionDto>.Fail(result.GetErrors().ToArray()))
            };
        
        return Ok(ApiResponse<ConvertedTransactionDto>.OK(result.GetValue()));
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var requestIsValid = transactionValidator.IsValid(request, out List<string> errors);

        if (!requestIsValid)
        {
            return BadRequest(ApiResponse<TransactionDto>.Fail(errors.ToArray()));
        }

        var command = CreateTransactionCommand.FromRequest(request);

        var result = await createTransactionHandler.HandleAsync(command);
        
        if (!result.Success)
            return result.Type switch
            {
                ResultType.ServerError =>
                    StatusCode(500, ApiResponse<ConvertedTransactionDto>.Fail(result.GetErrors().ToArray())),
                _ => UnprocessableEntity(ApiResponse<ConvertedTransactionDto>.Fail(result.GetErrors().ToArray()))
            };

        return StatusCode(201, ApiResponse<TransactionDto>.OK(result.GetValue()));
    }
}