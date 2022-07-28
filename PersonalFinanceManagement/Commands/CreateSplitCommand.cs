using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.API.Commands
{
    public class CreateSplitCommand
    {
        [Required]
        public string Catcode { get; set; }
        [Required]
        public double Amount { get; set; }
        
        
    }
}