using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.API.Commands
{
    public class CreateSplitCommand
    {
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string Catcode { get; set; }
    }
}