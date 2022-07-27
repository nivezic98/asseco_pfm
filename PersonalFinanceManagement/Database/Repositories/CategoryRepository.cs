using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TransactionDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(TransactionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryEntity> CreateCategory(CategoryEntity entity)
        {
            CategoryEntity old_entity = await GetCategory(entity.Code);
            if (old_entity == null)
            {
                _context.Category.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                _context.Category.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<CategoryList> GetCategories(string parentCode)
        {
            var query = _context.Category.AsQueryable();

            if (!string.IsNullOrEmpty(parentCode))
            {
                query = query.Where(x => x.ParentCode == parentCode);
            }

            var allCat = await query.ToListAsync();
            return new CategoryList
            {
                CatList = _mapper.Map<List<Category>>(allCat)
            };
        }

        public async Task<CategoryEntity> GetCategory(string code)
        {
            return await _context.Category.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}