using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;

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
        public async Task<IActionResult> ImportCategoriesFromCSV([FromBody] CreateCategoryList categories)
        {
            StreamReader reader=  new StreamReader("categories.csv");
        List<string> result= new List<string>();
        int i=0;
        string line="";
        List<String> greske=new List<string>();
        while ((line = await reader.ReadLineAsync()) != null)
                {
                    i+=1;
                    result.Add(line);
                }
        var j=0;   
        foreach(string elem in result)
        {
            j+=1;
            if(j<6)
            {
                continue;
            }
        
            CreateCategoryCommand categoryCommand = new CreateCategoryCommand();
            string[] lista=elem.Split(",");
            if(lista.Length<3){continue;}
            try{
            categoryCommand.Code=lista[0];
            categoryCommand.Name=lista[2];
            categoryCommand.ParentCode=lista[1];
            var cat = await _categoryService.CreateCategory(categoryCommand);
            }
            
            catch(Exception e)
            {

            }
        }
        return Ok(result);
            
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