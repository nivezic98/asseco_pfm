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

        public async Task<CreateCategorizeCommand> CategorizeTransaction(string id, CreateCategorizeCommand categorize)
        {
            var res = await _transactionRepository.CategorizeTransaction(id, categorize);
            return res;
        }

        public async Task<Transaction> CreateTransactions(CreateTransactionCommand command)
        {
            var entity = _mapper.Map<TransactionEntity>(command);

            var existingProduct = await _transactionRepository.GetTransaction(command.Id);
            if (existingProduct != null)
            {
                return null;
            }
            var result = await _transactionRepository.CreateTransaction(entity);

            return _mapper.Map<Models.Transaction>(result);
        }

        public async Task<SpendingInCategory> GetAnalytics(DateTime start, DateTime end, Direction direction, string catCode)
        {
            return await _transactionRepository.GetAnalytics(start, end, direction, catCode);        
        }


        public async Task<TransactionEntity> GetTransaction(string id)
        {
            var result = await _transactionRepository.GetTransaction(id);

            if (result == null)
            {
                return null;
            }
            return _mapper.Map<TransactionEntity>(result);
        }

        public async Task<PagedSortedList<TransactionEntity>> GetTransactions(TransactionKind kind, DateTime start, DateTime end, int page, int pageSize, string sortBy, SortOrder sortOrder)
        {
             var result = await _transactionRepository.GetTransactions(kind, start, end, page, pageSize, sortBy, sortOrder);

            return new PagedSortedList<TransactionEntity>
            {
                PageSize = result.PageSize,
                Page = result.Page,
                TotalCount = result.TotalCount,
                SortBy = result.SortBy,
                SortOrder = result.SortOrder,
                Items = _mapper.Map<List<TransactionEntity>>(result.Items)
        };
        }

        public async Task<CreateSplitCommand> SplitTransaction(SplitTransactionEntity entity)
        {
            return await _transactionRepository.SplitTransaction(entity);
        }

        public async Task<TransactionEntity> UpdateEntity(TransactionEntity entity)
        {
            return await _transactionRepository.UpdateEntity(entity);
        }

        public async Task RemoveSplit(string id)
        {
            await _transactionRepository.RemoveSplit(id);
        }

        public async Task AutoCategorize()
        {
            await _transactionRepository.AutoCategorize();
        }
    }
}