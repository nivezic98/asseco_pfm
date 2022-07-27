using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Database.Entities
{
    public class CreateCategoryList
    {
        public List<CreateCategoryCommand> Categories { get; set; } = new List<CreateCategoryCommand>();
    }
}