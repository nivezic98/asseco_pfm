using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDbContext _context;
        public TransactionRepository(TransactionDbContext context)
        {
            _context = context;
        }
        public async Task<TransactionEntity> Create(TransactionEntity product)
        {
           _context.Transaction.Add(product);

            await _context.SaveChangesAsync();

            return product;        
        }
        public async Task<TransactionEntity> Update(TransactionEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TransactionEntity> Get(string id)
        {
            return await _context.Transaction.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedSortedList<TransactionEntity>> List(Kind kind,DateTime start, DateTime end, int page = 1, int pageSize = 5, string sortBy = null, SortOrder sortOrder = SortOrder.Asc)
        {
            var query = _context.Transaction.Where(p => p.Kind == kind && start<=p.Date && p.Date<=end).AsQueryable();

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(totalCount * 1.0 / pageSize);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    default:
                    case "id":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                    
                }
            } 
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var items = await query.ToListAsync();

            return new PagedSortedList<TransactionEntity>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
        }




    }
}