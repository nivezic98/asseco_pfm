namespace PersonalFinanceManagement.API.Database.Entities
{
    public class TransactionEntity
    {
        public string Id { get; set; }

        public string BeneficiaryName { get; set; }

        public DateTime Date { get; set; }

        public Direction Direction { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public string Currency { get; set; }

        public string Mcc { get; set; }

        public TransactionKind Kind { get; set; }

        public string Catcode { get; set; }

        public CategoryEntity Category { get; set; }

        public ICollection<SplitTransactionEntity> SplitTransaction { get; set; }
    }
}