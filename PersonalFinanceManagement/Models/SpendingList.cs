namespace PersonalFinanceManagement.API.Models
{
    public class SpendingList
    {
        public List<SpendingInCategory> Group { get; set; } = new List<SpendingInCategory>();
    }
}