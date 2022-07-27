using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CreateCategoryCommand category);
        Task<Category> GetCategory(string id);

        Task<CategoryList> GetCategories(string parentCode);
    }
}