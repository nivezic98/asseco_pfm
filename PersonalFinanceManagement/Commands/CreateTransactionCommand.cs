using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersonalFinanceManagement.API.Models;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Commands
{
    public class CreateTransactionCommand
    {
        [Required]
        public string Id { get; set; }
        public string BeneficiaryName{get;set;}
        [Required]
        public DateTime Date{get;set;}
        [Required]
        public Direction? Direction{get;set;}  
        [Required]
        public double Amount{get;set;}
        public string Description{get;set;}
        [Required]
        [StringLength(3, MinimumLength = 3,ErrorMessage = "Currency must be 3 characters long.")]
        public string Currency{get;set;}
        public string Mcc{get;set;}
        [Required]
        public TransactionKind? Kind{get;set;}

        public string CatCode{get;set;}
    }
}