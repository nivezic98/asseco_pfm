using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Services
{
    public interface ITransactionService
    {
        Task<PagedSortedList<SplitTransactionList>> GetTransactions(TransactionKind kind, DateTime start, DateTime end, int page, int pageSize, string sortBy, SortOrder sortOrder);

        Task<SplitTransactionList> GetTransaction(string id);

        Task ImportTransactions(CreateTransactionList transactions);

        Task CategorizeTransaction(string id, CreateCategorizeCommand categorize);

        Task SplitTransaction(string id, CreateSplitTransactionList splitTransaction);

        Task<SpendingList> GetAnalytics( DateTime start, DateTime end, Direction direction, string? catCode);

    }
}
