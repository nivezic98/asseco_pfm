using AutoMapper;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Database.Repositories;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Services
{
    public class CategoryService : ICategoryService
    {   
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<CategoryList> GetCategories(string parentCode)
        {
            return await _categoryRepository.GetCategories(parentCode);        
        }

        public async Task ImportCategories(CreateCategoryList categories)
        {
            await _categoryRepository.ImportCategories(categories);
        }

    }
}