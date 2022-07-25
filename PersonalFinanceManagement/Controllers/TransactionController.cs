using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
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
        public async Task<IActionResult> GetTransactions([FromQuery] string kind_str, [FromQuery] string starts, [FromQuery] string ends, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string sortBy, [FromQuery] SortOrder sortOrder)
        {
            page = page ?? 1;
            pageSize = pageSize ?? 10;
            _logger.LogInformation("Returning {page}. page of products", page);
            List<Error> lista = new List<Error>();
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            try
            {
                start = DateTime.Parse(starts);
            }
            catch (Exception e)
            {
                Error error = new Error();
                error.tag = "start";
                error.error = "Start date format is wrong.";
                lista.Add(error);
            }
            try
            {
                end = DateTime.Parse(ends);
            }
            catch (Exception e)
            {
                Error error = new Error();
                error.tag = "end";
                error.error = "End date format is wrong";
                lista.Add(error);
            }
            Kind kind = new Kind();
            try
            {
                kind = Enum.Parse<Kind>(kind_str);
            }
            catch
            {
                Error error = new Error();
                error.tag = "kind";
                error.error = "Transaction kind is not supported.";
                lista.Add(error);
            }
            if (lista.Count > 0)
            {
                return BadRequest(lista);
            }
            return Ok(await _transactionService.GetProducts(kind, start, end, page.Value, pageSize.Value, sortBy, sortOrder));
        }




        [HttpPost("import")]
        [Consumes("text/csv")]
        public async Task<IActionResult> CreateProduct()
        {
            StreamReader reader = new StreamReader(Request.Body);
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
                var errors = false;
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
                    //try
                    //{
                    //    command.Mcc = Enum.Parse<MccCode>(lista[k]);
                    //}
                    //catch (Exception e)
                    //{
                        //command.mcc=null;
                    //}
                    k++;
                    command.Kind = Enum.Parse<Kind>(lista[k]);
                    //return Ok(command.TransactionKind);

                    var result1 = await _transactionService.CreateTransaction(command);
                }
                catch (Exception e)
                {
                    errors = true;
                    greske.Add(elem);
                }

            }
            return Ok(greske);
        }
        [HttpPost("{id}/categorize")]
        //[Consumes("application/json")]
        public async Task<IActionResult> Categorize([FromRoute] string id, [FromQuery] string catcode)
        {
            var transaction_entity = await _transactionService.GetTransaction(id);
            if (transaction_entity == null)
            {
                return BadRequest(id);
            }
            //HttpClient client=new HttpClient();
            //string URL="https://localhost:7087/categories";
            //client.BaseAddress=new Uri(URL);
            //string parameters="?parent_id="+catcode.ToString();
            //client.DefaultRequestHeaders.Accept.Add(
            //   new MediaTypeWithQualityHeaderValue("application/json"));
            //var response = client.GetAsync(parameters).Result; 
            transaction_entity.Catcode = catcode;
            try
            {
                await _transactionService.Update(transaction_entity);
                return Ok(transaction_entity);
            }
            catch (Exception e)
            {
                return BadRequest("There is no category with given catcode.");
            }
        }
    }
}