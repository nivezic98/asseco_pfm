using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.API.Commands
{
    public class CreateCategorizeCommand
    {
        [Required]
        public string Catcode { get; set; }
    }
}