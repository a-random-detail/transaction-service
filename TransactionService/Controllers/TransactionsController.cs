using Microsoft.AspNetCore.Mvc;
using TransactionService.Contracts;

namespace TransactionService.Controllers;

[Route("[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    [HttpGet("/{id}")]
    public async Task<IActionResult> GetTransactionById()
    {
        return Ok(ApiResponse<string>.OK("hello, world!"));
    }


}