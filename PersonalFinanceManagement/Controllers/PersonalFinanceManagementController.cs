using Microsoft.AspNetCore.Mvc;
namespace PersonalFinanceManagement.API.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class PersonalFinanceManagementController : ControllerBase
    {

        private readonly ILogger<PersonalFinanceManagementController> _logger;

        public PersonalFinanceManagementController(ILogger<PersonalFinanceManagementController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("transactions/import")]
        public void aha(){}

        [HttpGet]
        [Route("transactions")]
        public void aha1(){}

        [HttpPost]
        [Route("categories/import")]
        public void aha2(){}

        [HttpPost]
        [Route("categories/import")]
        public void aha3(){}

        
    }
}