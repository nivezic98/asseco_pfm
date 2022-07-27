using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.API.Commands
{
    public class CreateCategoryCommand
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string ParentCode { get; set; }
    }
}