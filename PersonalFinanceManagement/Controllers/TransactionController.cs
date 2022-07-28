using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;
using System.Text;

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
        public async Task<IActionResult> ImportTransactionsFromCSV()
        {
            StreamReader reader = new StreamReader("transactions.csv");
            List<string> lines = new List<string>();
            string line = "";
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lines.Add(line);
            }
            int i = 0;
            foreach (string item in lines)
            {
                i += 1;
                if (i < 2)
                {
                    continue;
                }
                CreateTransactionCommand command = new CreateTransactionCommand();
                string[] elem = item.Split(",");
                try
                {
                    command.Id = elem[0];
                    command.BeneficiaryName = elem[1];
                    command.Date = DateTime.Parse(elem[2]);
                    command.Direction = Enum.Parse<Direction>(elem[3]);
                    int j = 4;
                    try
                    {
                        command.Amount = Double.Parse(elem[4]);
                    }
                    catch (Exception e)
                    {
                        j += 1;
                        int len = elem[5].Length;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(elem[4][1]);
                        for (var k = 0; k < len - 1; k++)
                        {
                            sb.Append(elem[5][k]);
                        }
                        command.Amount = Double.Parse(sb.ToString());
                    }
                    j++;
                    command.Description = elem[j];
                    j++;
                    command.Currency = elem[j];
                    j++;
                    command.Mcc = elem[j];
                    j++;
                    command.Kind = Enum.Parse<TransactionKind>(elem[j]);
                    await _transactionService.CreateTransactions(command);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

            }
            return Ok();
        }

        [HttpPost]
        [Route("transaction/{id}/split")]
        public async Task<IActionResult> SplitTransaction([FromRoute] string id, [FromBody] CreateSplitTransactionList splitTransaction)
        {   
            CreateSplitCommand result = new CreateSplitCommand();
            CreateSplitTransactionList list = new CreateSplitTransactionList();
            var transaction_entity = await _transactionService.GetTransaction(id);
            if (transaction_entity == null)
            {
                return BadRequest("Transaction with given id doesn't exist.");
            }
            double total = 0;
            await _transactionService.RemoveSplit(id);
            foreach (var elem in splitTransaction.Splits)
            {
                total += elem.Amount;
                SplitTransactionEntity entity = new SplitTransactionEntity();
                entity.Id = id;
                entity.Catcode = elem.Catcode;
                entity.Amount = elem.Amount;
                transaction_entity.Catcode = elem.Catcode;
                await _transactionService.UpdateEntity(transaction_entity);
                try
                {
                    result = await _transactionService.SplitTransaction(entity);
                    list.Splits.Add(result);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    return BadRequest("There is no such category.");
                }
            }
            if (total > transaction_entity.Amount)
            {
                return BadRequest("Bussines problem.");
            }
            
            return Ok(list);
        }

        [HttpPost]
        [Route("transactions/{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id)
        {
            var res = await _transactionService.CategorizeTransaction(id);

            if(res == null){
                return BadRequest();
            }

            return Ok(res);
        }
        [HttpPost("auto-categorize")] 
        public async Task<IActionResult> AutoCategorize()
    {
         await _transactionService.AutoCategorize();
         return Ok();
    }

    }
}