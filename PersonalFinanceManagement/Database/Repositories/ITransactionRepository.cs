using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task ImportTransactions(CreateTransactionList transactions);

        Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind? kind, DateTime start, DateTime end, int? page, int? pageSize, string? sortBy, SortOrder? sortOrder);

        Task<TransactionEntity> GetTransaction(string id);

        Task<SpendingList> GetAnalytics(DateTime start, DateTime end, Direction direction, string? catCode);

        Task<CreateSplitTransactionList> SplitTransaction(string id, CreateSplitTransactionList splitTransaction);
        Task<CreateCategorizeCommand> CategorizeTransaction(string id, CreateCategorizeCommand categorize);

    }
}   