using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Services
{
    public interface ICategoryService
    {
        Task ImportCategories(CreateCategoryList categories);
        Task<CategoryList> GetCategories(string parentCode);

    }
}