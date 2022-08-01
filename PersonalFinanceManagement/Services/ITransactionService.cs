using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Services
{
    public interface ITransactionService
    {   
        Task AutoCategorizeTransactions();
        Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind kind, DateTime start, DateTime end, int page, int pageSize, string sortBy, SortOrder sortOrder);

        Task<Transaction> CreateTransactions(CreateTransactionCommand command);
        Task<TransactionEntity> GetTransaction(string id);

        Task<TransactionEntity> UpdateEntity(TransactionEntity entity);

        Task<CreateCategorizeCommand> CategorizeTransaction(string id, CreateCategorizeCommand command);

        Task<SplitTransactionEntity> SplitTransaction(string id, CreateSplitTransactionList splitTransaction);

        Task<SpendingList> GetAnalytics( DateTime start, DateTime end, Direction direction, string? catCode);

        Task RemoveSplit(string id);
    }
}
