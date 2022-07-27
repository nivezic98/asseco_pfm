using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDbContext _context;
        private readonly IMapper _mapper;

        public TransactionRepository(TransactionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CreateCategorizeCommand> CategorizeTransaction(string id, CreateCategorizeCommand categorize)
        {
            var category = await _context.Category.FindAsync(categorize.Catcode);
            if (category == null)
            {
                return null;
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return null;
            }

            if (transaction.Catcode == "Z")
            {
                var splitsDeleted = await _context.SplitTransaction.Where(s => s.Id == id).ToListAsync();
                _context.SplitTransaction.RemoveRange(splitsDeleted);
                await _context.SaveChangesAsync();
            }

            transaction.Catcode = category.Code;

            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return categorize;
        }

        public async Task<TransactionEntity> CreateTransaction(TransactionEntity entity)
        {
            _context.Transaction.Add(entity);
            await _context.SaveChangesAsync();

            return entity;  
        }

        public async Task<SpendingList> GetAnalytics(DateTime start, DateTime end, Direction direction, string catCode)
        {
            var queryCategories = _context.Category.Include(t => t.Transactions).AsQueryable();

            if (catCode != null)
            {
                queryCategories = queryCategories.Where(x => x.Code == catCode);
            }

            var categories = await queryCategories.ToListAsync();

            var spendingByCategory = new SpendingList();

            foreach (var category in categories)
            {
    
                var categoryList = await _context.Category
                    .Where(c => c.Code == category.Code || c.ParentCode == category.Code)
                    .Include(c => c.Transactions)
                    .Include(c => c.SplitTransactions)
                    .ThenInclude(st => st.Transaction)
                    .ToListAsync();

                var amount = 0.0;
                var count = 0;

                foreach (var cat in categoryList)
                {
                    var nonSplitTransactions = cat.Transactions.Where(t => t.Direction == direction && t.Catcode != "Z");
                    var splitTransactions = cat.SplitTransactions.Where(st => st.Transaction.Direction == direction);

                    if (!(start == DateTime.MinValue))
                    {
                        nonSplitTransactions = nonSplitTransactions.Where(t => t.Date >= start);
                        splitTransactions = splitTransactions.Where(s => s.Transaction.Date >= start);
                    }
                    if (!(end == DateTime.MinValue))
                    {
                        nonSplitTransactions = nonSplitTransactions.Where(t => t.Date <= end);
                        splitTransactions = splitTransactions.Where(s => s.Transaction.Date <= end);
                    }

                    amount += nonSplitTransactions.Select(x => x.Amount).Sum() + splitTransactions.Select(x => x.Amount).Sum();
                    count += nonSplitTransactions.Count() + splitTransactions.Count();
                }

                if (count == 0)
                {
                    continue;
                }

                spendingByCategory.Group.Add(
                    new SpendingInCategory
                    {
                        Catcode = category.Code,
                        Amount = amount,
                        Count = count
                    }
                );
            }
            return spendingByCategory;
        }

        public async Task<TransactionEntity> GetTransaction(string id)
        {
            return await _context.Transaction.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind? kind, DateTime start, DateTime end, int? page, int? pageSize, string sortBy, SortOrder? sortOrder)
        {
            var query = _context.Transaction.Include(t => t.SplitTransaction).AsQueryable();


            if (kind != null)
            {
                query = query.Where(x => x.Kind.Equals(kind));
            }

            if (start != DateTime.MinValue)
            {
                query = query.Where(x => x.Date >= start);
            }

            if (end != DateTime.MinValue)
            {
                query = query.Where(x => x.Date <= end);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "id":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                    case "beneficiary-name":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName);
                        break;
                    case "date":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                        break;
                    case "direction":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Direction) : query.OrderByDescending(x => x.Direction);
                        break;
                    case "description":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                    case "currency":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Currency) : query.OrderByDescending(x => x.Currency);
                        break;
                    case "mcc":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Mcc) : query.OrderByDescending(x => x.Mcc);
                        break;
                    case "kind":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Kind) : query.OrderByDescending(x => x.Kind);
                        break;
                    default:
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Date);
            }

            var totalCount = query.Count();

            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            var items = await query.ToListAsync();

            return new PagedSortedList<TransactionEntity>
            {
                Page = page.Value,
                PageSize = pageSize.Value,
                TotalCount = totalCount,
                SortBy = sortBy ?? "date",
                SortOrder = sortOrder ?? SortOrder.Asc,
                Items = items,
            };
        }

        

        public async Task ImportTransactions(CreateTransactionList transactions)
        {
            await _context.Transaction.AddRangeAsync(_mapper.Map<IEnumerable<TransactionEntity>>(transactions.Transactions));
            await _context.SaveChangesAsync();
        }

        public async Task<CreateSplitTransactionList> SplitTransaction(string id, CreateSplitTransactionList splitTransaction)
        {
            var queryTransactions = _context.Transaction.Include(t => t.SplitTransaction).AsQueryable();

            var transaction = queryTransactions.Where(t => t.Id == id).FirstOrDefault();

            if (transaction == null)
            {
                return null;
            }

            if (splitTransaction.Splits.Select(x => (int)x.Amount).Sum() != (int)transaction.Amount)
            {
                return null;
            }

            var hasSplits = transaction.SplitTransaction.Count() > 0;

            if (hasSplits)
            {
                var splitsDeleted = await _context.SplitTransaction.Where(st => st.Id == id).ToListAsync();
                _context.SplitTransaction.RemoveRange(splitsDeleted);
                await _context.SaveChangesAsync();
            }


            transaction.Catcode = "Z";
            _context.Entry(transaction).State = EntityState.Modified;


            foreach (var spTransaction in splitTransaction.Splits)
            {
                await _context.AddAsync(new SplitTransactionEntity
                {
                    Id = id,
                    Catcode = spTransaction.Catcode,
                    Amount = spTransaction.Amount
                });
            }

            await _context.SaveChangesAsync();



            return splitTransaction;
        }

    }
}

