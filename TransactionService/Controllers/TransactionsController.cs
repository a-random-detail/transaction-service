using Microsoft.AspNetCore.Mvc;
using TransactionService.Contracts;
using TransactionService.Domain.Entities;

namespace TransactionService.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    [HttpGet("/{id}")]
    public async Task<IActionResult> GetTransactionById([FromRoute] string id)
    {
        // return Ok(ApiResponse<string>.OK($"hello, {id}!"));
        throw new NotImplementedException();
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        // return Ok(ApiResponse<CreateTransactionRequest>.OK(request));
        throw new NotImplementedException();
    }

}
