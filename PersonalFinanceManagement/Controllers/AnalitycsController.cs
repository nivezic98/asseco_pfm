using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class AnalyticsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        private readonly ILogger<TransactionController> _logger;

        public AnalyticsController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

       [HttpGet]
        [Route("spending-analytics")]
        public async Task<ActionResult<SpendingList>> GetAnalytics(
            [FromQuery] string? catCode,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] Direction direction
        )
        {   
            SpendingList list = new SpendingList();
            var result = await _transactionService.GetAnalytics(start, end, direction, catCode);
            list.Group.Add(result);
            return Ok(list);
        }

    }
}