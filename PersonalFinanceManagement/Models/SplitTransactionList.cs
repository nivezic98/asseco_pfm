namespace PersonalFinanceManagement.API.Models
{
    public class SplitTransactionList : Transaction
    {
        public List<SingleTransactionSplit> SplitTranList { get; set; }
    }
}