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
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _categoryService.ImportCategories(categories);

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