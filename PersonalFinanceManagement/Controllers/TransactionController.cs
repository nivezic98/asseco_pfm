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
            List<string> result = new List<string>();
            int i = 0;
            string line = "";
            List<String> greske = new List<string>();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                i += 1;
                result.Add(line);
            }
            var j = 0;
            foreach (string elem in result)
            {
                j += 1;
                if (j < 6)
                {
                    continue;
                }
                CreateTransactionCommand command = new CreateTransactionCommand();
                string[] lista = elem.Split(",");
                try
                {
                    command.Id = lista[0];
                    command.BeneficiaryName = lista[1];
                    command.Date = DateTime.Parse(lista[2]);
                    command.Direction = Enum.Parse<Direction>(lista[3]);
                    var k = 4;
                    try
                    {
                        command.Amount = Double.Parse(lista[4]);
                    }
                    catch (Exception e)
                    {
                        k += 1;
                        var n = lista[5].Length;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(lista[4][1]);
                        for (var iter = 0; iter < n - 1; iter++)
                        {
                            sb.Append(lista[5][iter]);
                        }
                        command.Amount = Double.Parse(sb.ToString());
                    }
                    k++;
                    command.Description = lista[k];
                    k++;
                    command.Currency = lista[k];
                    k++;
                    command.Mcc = lista[k];
                    k++;
                    command.Kind = Enum.Parse<TransactionKind>(lista[k]);

                    await _transactionService.CreateTransactions(command);
                }
                catch(Exception e){
                    
                }
    }   return Ok(result);
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