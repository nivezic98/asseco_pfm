using AutoMapper;
using PersonalFinanceManagement.API.Commands;
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

        public async Task<Category> CreateCategory(CreateCategoryCommand category)
        {
            var entity = _mapper.Map<CategoryEntity>(category);

            var existingProduct = await _categoryRepository.GetCategory(category.Code);
            if (existingProduct != null)
            {
                return null;
            }
            var result = await _categoryRepository.CreateCategory(entity);

            return _mapper.Map<Category>(result);
        }

        public async Task<CategoryList> GetCategories(string parentCode)
        {
            return await _categoryRepository.GetCategories(parentCode);        
        }

        public async Task<Category> GetCategory(string id)
        {
            var result=await _categoryRepository.GetCategory(id);
            return _mapper.Map<Category>(result);
        }

    }
}