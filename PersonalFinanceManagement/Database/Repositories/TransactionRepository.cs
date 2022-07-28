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

        public async Task<CreateCategorizeCommand> CategorizeTransaction(string id)
        {   
            CreateCategorizeCommand result = new CreateCategorizeCommand();
            var entity = await GetTransaction(id);
            var code = entity.Catcode;
            result.Catcode = code;
            if(code == null){
                entity.Catcode = "string";
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                result.Catcode = "string";
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
            var queryCategories = _context.Category.Where(x => x.Code == catCode);

            var categories = await queryCategories.ToListAsync();

            var spendingByCategory = new SpendingList();

            foreach (var category in categories)
            {
                //Za svaku kategoriju dohvatam i one kategorije cija je ova kategorija roditelj
                var categoryList = await _context.Category
                    .Where(c => c.ParentCode == category.Code || c.Code == category.Code)
                    .ToListAsync();

                var amount = 0.0;
                var count = 0;

                foreach(var cat in categoryList)
                {
                    var nonSplittedTransactions = cat.Transactions.Where(t => t.Direction == direction);
                    var splittedTransactions = cat.SplitTransactions.Where(st => st.Transaction.Direction == direction);

                    if (!(start == DateTime.MinValue))
                    {
                        nonSplittedTransactions = nonSplittedTransactions.Where(t => t.Date >= start);
                        splittedTransactions = splittedTransactions.Where(st => st.Transaction.Date >= start);
                    }
                    if (!(end == DateTime.MinValue))
                    {
                        nonSplittedTransactions = nonSplittedTransactions.Where(t => t.Date <= end);
                        splittedTransactions = splittedTransactions.Where(st => st.Transaction.Date <= end);
                    }

                    amount += nonSplittedTransactions.Select(t => t.Amount).Sum() + splittedTransactions.Select(t => t.Amount).Sum();
                    count += nonSplittedTransactions.Count() + splittedTransactions.Count();
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

        public async Task<CreateSplitCommand> SplitTransaction(SplitTransactionEntity entity)
        {

            _context.SplitTransaction.Add(entity);
            await _context.SaveChangesAsync();
            CreateSplitCommand command = new CreateSplitCommand();
            command.Amount = entity.Amount;
            command.Catcode = entity.Catcode;
            return command;
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
            string[] lines = System.IO.File.ReadAllLines("rules.txt");
            
            int n=Convert.ToInt32(lines.Length/4);
            for(int i=0;i<n;i++)
            {
             string code=lines[i*4+2].Split(":")[1];
             string query=lines[i*4+3].Split(":")[1];   
             var result=_context.Database.ExecuteSqlRaw("UPDATE public.transactions \r\n"+ "SET Catcode="+code+"\r\n WHERE "+query+" AND Catcode is null"+";");
             await _context.SaveChangesAsync();
            }
        }
    }


}


