using Microsoft.AspNetCore.Mvc;
namespace PesonalFinanceManagement.API.Controllers
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

    
    }
}