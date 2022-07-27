using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public interface ICategoryRepository
    {
        Task ImportCategories(CreateCategoryList categories);

        Task<CategoryList> GetCategories(string parentCode);
    }
}