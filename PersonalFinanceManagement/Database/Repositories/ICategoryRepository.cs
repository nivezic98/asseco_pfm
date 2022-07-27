using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public interface ICategoryRepository
    {
        Task<CategoryEntity> CreateCategory(CategoryEntity entity); 
        Task<CategoryEntity> GetCategory(string code);
        Task<CategoryList> GetCategories(string parentCode);
    }
}