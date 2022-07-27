namespace PersonalFinanceManagement.API.Database.Entities
{
    public class SplitTransactionEntity
    {
        public string Id { get; set; }

        public string Catcode { get; set; }

        public double Amount { get; set; }

        public TransactionEntity Transaction { get; set; }

        public CategoryEntity Category { get; set; }

    }
}
