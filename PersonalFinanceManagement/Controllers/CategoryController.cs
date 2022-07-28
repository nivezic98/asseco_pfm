using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;
using CsvHelper;

namespace PersonalFinanceManagement.API.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;

        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
        [Route("categories/import")]
        public async Task<IActionResult> ImportCategoriesFromCSV()
        {
            StreamReader reader = new StreamReader("categories.csv");
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
                if(i<2){
                    continue;
                }
                CreateCategoryCommand categoryCommand = new CreateCategoryCommand();
                string[] elem = item.Split(",");
                if (elem.Length < 3) { continue; }
                try
                {
                    categoryCommand.Code = elem[0];
                    categoryCommand.ParentCode = elem[1];
                    categoryCommand.Name = elem[2];
                    await _categoryService.CreateCategory(categoryCommand);
                }

                catch (Exception e)
                {
                    return BadRequest();
                }
            }
            return Ok();

        }

        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<CategoryList>> GetCategories([FromQuery] string? parentId)
        {
            var result = await _categoryService.GetCategories(parentId);
            return Ok(result);
        }

    }
}