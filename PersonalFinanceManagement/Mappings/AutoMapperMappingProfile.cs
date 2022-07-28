using AutoMapper;
using PersonalFinanceManagement.API.Commands;
using PersonalFinanceManagement.API.Database.Entities;
using PersonalFinanceManagement.API.Models;

namespace PersonalFinanceManagement.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {   
            CreateMap<TransactionEntity,Transaction>().ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id));
            CreateMap<CategoryEntity,Category>().ForMember(d => d.Code, opts => opts.MapFrom(s => s.Code));

            CreateMap<CreateTransactionCommand,TransactionEntity>().ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id));
            CreateMap<CreateCategoryCommand,CategoryEntity>().ForMember(d => d.Code, opts => opts.MapFrom(s => s.Code));

            CreateMap<PagedSortedList<TransactionEntity>, PagedSortedList<Transaction>>();

            CreateMap<SplitTransactionEntity, SingleTransactionSplit>().ReverseMap();
            CreateMap<TransactionEntity, SplitTransactionList>().ReverseMap();

        }
    }
}