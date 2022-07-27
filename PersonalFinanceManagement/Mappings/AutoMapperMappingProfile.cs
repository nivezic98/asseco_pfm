using AutoMapper;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {   
            CreateMap<Transaction,TransactionEntity>().ReverseMap();
            CreateMap<Category,CategoryEntity>().ReverseMap();

            CreateMap<CreateTransactionList,TransactionEntity>().ReverseMap();
            CreateMap<CreateCategoryList,CategoryEntity>().ReverseMap();

            CreateMap<PagedSortedList<TransactionEntity>, PagedSortedList<Transaction>>().ReverseMap();

            CreateMap<SplitTransactionEntity, SingleTransactionSplit>().ReverseMap();
            CreateMap<TransactionEntity, SplitTransactionList>().ReverseMap();

        }
    }
}