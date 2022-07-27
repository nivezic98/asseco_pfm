using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task ImportCategories(CreateCategoryList categories)
        {
            foreach (var cat in categories.Categories)
            {
                var categoryEntity = await _context.Category.FindAsync(cat.Code);

                if (categoryEntity != null)
                {
                    categoryEntity.ParentCode = cat.ParentCode;
                    categoryEntity.Name = cat.Name;

                    _context.Entry(categoryEntity).State = EntityState.Modified;
                }
                else
                {
                    await _context.AddAsync(_mapper.Map<CategoryEntity>(cat));
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}