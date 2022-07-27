namespace PersonalFinanceManagement.API.Database.Entities
{
    public class CategoryEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string ParentCode { get; set; }

        public  ICollection<TransactionEntity> Transactions { get; set; }

        public ICollection<SplitTransactionEntity> SplitTransactions { get; set; }

    }
}