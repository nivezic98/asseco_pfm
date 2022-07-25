using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> Get(string id);
        Task<TransactionEntity> Update(TransactionEntity entity);
        Task<TransactionEntity> Create (TransactionEntity entity);
        Task<PagedSortedList<TransactionEntity>> List(Kind kind, DateTime start, DateTime end, int page = 1, int pageSize = 5, string sortBy = null, SortOrder sortOrder = SortOrder.Asc);
    }
}