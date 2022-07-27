using AutoMapper;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Database.Repositories;

namespace PersonalFinanceManagement.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task CategorizeTransaction(string id, CreateCategorizeCommand categorize)
        {
            await _transactionRepository.CategorizeTransaction(id, categorize);
        }

        public async Task<SpendingList> GetAnalytics(DateTime start, DateTime end, Direction direction, string catCode)
        {
            return await _transactionRepository.GetAnalytics(start, end, direction, catCode);        
        }


        public async Task<SplitTransactionList> GetTransaction(string id)
        {
            var result = await _transactionRepository.GetTransaction(id);

            if (result == null)
            {
                return null;
            }
            return _mapper.Map<SplitTransactionList>(result);
        }

        public async Task<PagedSortedList<SplitTransactionList>> GetTransactions(TransactionKind kind, DateTime start, DateTime end, int page, int pageSize, string sortBy, SortOrder sortOrder)
        {
             var result = await _transactionRepository.GetTransactions(kind, start, end, page, pageSize, sortBy, sortOrder);

            return new PagedSortedList<SplitTransactionList>
            {
                PageSize = result.PageSize,
                Page = result.Page,
                TotalCount = result.TotalCount,
                SortBy = result.SortBy,
                SortOrder = result.SortOrder,
                Items = _mapper.Map<List<SplitTransactionList>>(result.Items)
        };
        }

        public async Task ImportTransactions(CreateTransactionList transactions)
        {
            await _transactionRepository.ImportTransactions(transactions);
        }

        public async Task SplitTransaction(string id, CreateSplitTransactionList splitTransaction)
        {
            await _transactionRepository.SplitTransaction(id, splitTransaction);
        }
    }
}