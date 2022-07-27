using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Database.Entities
{
    public class CreateTransactionList
    {
        public List<CreateTransactionCommand> Transactions { get; set; } = new List<CreateTransactionCommand>();

    }
}