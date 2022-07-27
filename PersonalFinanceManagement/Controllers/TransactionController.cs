using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class TransactionController : ControllerBase
    {

        private readonly ITransactionService _transactionService;

        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        [Route("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] TransactionKind? kind, 
            [FromQuery] DateTime start, 
            [FromQuery] DateTime end,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? sortBy,
            [FromQuery] SortOrder? sortOrder)
        {
            page = page ?? 1;
            pageSize = pageSize ?? 10;
            _logger.LogInformation("Returning {page}. page of transactions", page);

            var result = await _transactionService.GetTransactions(kind.Value, start, end, page.Value, pageSize.Value, sortBy, sortOrder.Value);
            return Ok(result);
        }

        [HttpPost]
        [Route("transactions/import")]
        public async Task<IActionResult> ImportTransactionsFromCSV([FromBody] CreateTransactionList transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _transactionService.ImportTransactions(transactions);
            
            return Ok();
        }

        [HttpPost]
        [Route("transaction/{id}/split")]
        public async Task<IActionResult> SplitTransaction([FromRoute] string id, [FromBody] CreateSplitTransactionList splitTransactionList)
        {
            await _transactionService.SplitTransaction(id, splitTransactionList);

            return Ok();
        }
        
        [HttpPost]
        [Route("transactions/{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromBody] CreateCategorizeCommand categorize)
        {
            await _transactionService.CategorizeTransaction(id, categorize);

            return Ok();
        }

    }
}