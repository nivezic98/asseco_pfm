using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Database.Entities
{
    public class CreateSplitTransactionList
    {
        public List<CreateSplitCommand> Splits { get; set; } = new List<CreateSplitCommand>();
    }
}