using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Services
{
    public interface ITransactionService
    {
        Task<PagedSortedList<Models.Transaction>> GetProducts(Kind kind, DateTime start, DateTime end, int page = 1, int pageSize = 10, string sortBy = null, SortOrder sortOrder = SortOrder.Asc);
        Task<Models.Transaction> CreateTransaction(CreateTransactionCommand command);
        Task<TransactionEntity> GetTransaction(string id);
        Task<TransactionEntity> Update(TransactionEntity entity);
    }



}
