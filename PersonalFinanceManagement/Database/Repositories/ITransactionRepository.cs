using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task AutoCategorize();
        Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind? kind, DateTime start, DateTime end, int? page, int? pageSize, string? sortBy, SortOrder? sortOrder);
        Task<TransactionEntity> UpdateEntity(TransactionEntity entity);
        Task<TransactionEntity> CreateTransaction(TransactionEntity entity);
        Task<TransactionEntity> GetTransaction(string id);
        Task<SpendingList> GetAnalytics(DateTime start, DateTime end, Direction direction, string? catCode);
        Task<CreateSplitCommand> SplitTransaction(SplitTransactionEntity entity);
        Task<CreateCategorizeCommand> CategorizeTransaction(string id);
        Task RemoveSplit(string id);
    }
}   