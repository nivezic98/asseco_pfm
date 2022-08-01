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

        public async Task<CreateCategorizeCommand> CategorizeTransaction(string id, CreateCategorizeCommand command)
        {
            CreateCategorizeCommand result = new CreateCategorizeCommand();
            var entity = await GetTransaction(id);
            var code = entity.Catcode;
            result.Catcode = code;
            if (code == null)
            {
                entity.Catcode = command.Catcode;
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                result.Catcode = command.Catcode;
            }

            return result;
        }

        public async Task<TransactionEntity> CreateTransaction(TransactionEntity entity)
        {
            _context.Transaction.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<SpendingList> GetAnalytics(DateTime start, DateTime end, Direction direction, string catCode)
        {   
            SpendingList spendings = new SpendingList();

            var query = _context.Category.Include(x => x.Transactions).AsQueryable();

            if (catCode != null)
            {
                query = query.Where(x => x.Code == catCode);
            }

            var categories = await query.ToListAsync();

            foreach(var cat in categories)
            {
                var list = await _context.Category.Where(x => x.Code == cat.Code || x.ParentCode == cat.Code).Include(x => x.Transactions).ToListAsync();
                int count = 0;
                double amount = 0.0;

                foreach(var c in list)
                {
                    var transaction = c.Transactions.Where(x => x.Direction == direction && x.Date >= start && x.Date <= end);
                    amount = transaction.Select(x => x.Amount).Sum();
                    count = transaction.Count();
                }
                if(count == 0)
                {
                    continue;
                }
                spendings.Group.Add(
                    new SpendingInCategory
                    {
                        Catcode = cat.Code,
                        Amount = amount,
                        Count = count
                    }
                );
            }
            return spendings;
        }

        public async Task<TransactionEntity> GetTransaction(string id)
        {
            return await _context.Transaction.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind? kind, DateTime start, DateTime end, int? page, int? pageSize, string sortBy, SortOrder? sortOrder)
        {
            var query = _context.Transaction.Include(x => x.SplitTransaction).AsQueryable();

            if (start != DateTime.MinValue)
            {
                query = query.Where(y => y.Date >= start);
            }

            if (end != DateTime.MinValue)
            {
                query = query.Where(y => y.Date <= end);
            }
            if (kind != null)
            {
                query = query.Where(y => y.Kind.Equals(kind));
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "id":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id);
                        break;
                    case "date":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Date) : query.OrderByDescending(t => t.Date);
                        break;
                    case "kind":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Kind) : query.OrderByDescending(t => t.Kind);
                        break;
                    case "beneficiary-name":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.BeneficiaryName) : query.OrderByDescending(t => t.BeneficiaryName);
                        break;
                    case "direction":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Direction) : query.OrderByDescending(t => t.Direction);
                        break;
                    case "description":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description);
                        break;
                    case "currency":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Currency) : query.OrderByDescending(t => t.Currency);
                        break;
                    case "mcc":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Mcc) : query.OrderByDescending(t => t.Mcc);
                        break;
                    default:
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Date);
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

        public async Task<SplitTransactionEntity> SplitTransaction(String id, CreateSplitTransactionList splitTransaction)
        {
            
            SplitTransactionEntity split = new SplitTransactionEntity();
            foreach(var item in splitTransaction.Splits){
                split.Id = id;
                split.Catcode = item.Catcode;
                split.Amount = item.Amount;
                await _context.SplitTransaction.AddAsync(split);
                await _context.SaveChangesAsync();
            }
            
            return split;             
        }
        public async Task<TransactionEntity> UpdateEntity(TransactionEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task RemoveSplit(string id)
        {
            var items = _context.SplitTransaction.Where(x => x.Id == id).AsQueryable().ToList();
            foreach (var item in items)
            {
                _context.SplitTransaction.Remove(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AutoCategorizeTransactions()
        {
            string[] allLines = System.IO.File.ReadAllLines("rules.txt");
            int len = Convert.ToInt32(allLines.Length);
            int n = len/4;
            for (int k = 0; k < n; k++)
            {
                string catCode = allLines[k * 4 + 2].Split(":")[1];
                string query = allLines[k * 4 + 3].Split(":")[1];
                _context.Database.ExecuteSqlRaw("UPDATE public.transactions " + "SET Catcode=" + catCode + " WHERE " + query + " AND Catcode is null" + ";");
                await _context.SaveChangesAsync();
            }
        }
    }


}


