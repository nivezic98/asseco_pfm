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
        public async Task<Transaction> CreateTransaction(CreateTransactionCommand command)
        {
            var entity = _mapper.Map<TransactionEntity>(command);

            var existingProduct = await _transactionRepository.Get(command.Id);
            if (existingProduct != null)
            {
                return null;
            }
            var result = await _transactionRepository.Create(entity);

            return _mapper.Map<Models.Transaction>(result);
        }

        public async Task<PagedSortedList<Models.Transaction>> GetProducts(Kind kind,DateTime start, DateTime end, int page = 1, int pageSize = 10, string sortBy = null, SortOrder sortOrder = SortOrder.Asc)
        {
           var result = await _transactionRepository.List(kind,start, end,page, pageSize, sortBy, sortOrder);

            return _mapper.Map<PagedSortedList<Models.Transaction>>(result);        }
        public async Task<TransactionEntity> GetTransaction(string id)
        {
            var result =await _transactionRepository.Get(id);
            return result;
        }

        public async Task<TransactionEntity> Update(TransactionEntity entity)
        {
            return await _transactionRepository.Update(entity);
        }

        
    }
}