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
                
            var category_query = _context.Category.AsQueryable();
            if(!string.IsNullOrEmpty(catCode)){
                category_query.Where(x => x.Code == catCode);
            }
            var categories = await category_query.ToListAsync();
            var transactions_query = _context.Transaction.Where(x => x.Date >= start && x.Date <= end && x.Direction == direction && x.Catcode == catCode).AsQueryable();
            var transactions = await transactions_query.ToListAsync();
            
            
        
            var data = transactions.Join(categories, x => x.Id, y => y.Code, (x,y) => new { x, y } ).ToList();
            int count = 0;
            double amount = 0.0;

            foreach(var item in data)
            {
                for(int i = 0; i < data.Count; i++)
                {
                    if(data[i].y.ParentCode == item.y.ParentCode)
                    {
                        count++;
                        amount += data[i].x.Amount;
                    }
                }
                var spending = new SpendingInCategory
                {
                    Catcode = item.y.ParentCode,
                    Amount = amount,
                    Count = count
                };
                spendings.Group.Add(spending);
            }

            return spendings;
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

        public async Task AutoCategorize()
        {
            string[] allLines = System.IO.File.ReadAllLines("rules.txt");

            int n = Convert.ToInt32(allLines.Length / 4);
            for (int i = 0; i < n; i++)
            {
                string catCode = allLines[i * 4 + 2].Split(":")[1];
                string query = allLines[i * 4 + 3].Split(":")[1];
                var result = _context.Database.ExecuteSqlRaw("UPDATE public.transactions " + "SET Catcode=" + catCode + " WHERE " + query + " AND Catcode is null" + ";");
                await _context.SaveChangesAsync();
            }
        }
    }


}


