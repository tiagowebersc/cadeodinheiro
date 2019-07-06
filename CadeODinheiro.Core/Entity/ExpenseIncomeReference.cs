using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbExpenseIncomeReference")]
    public class ExpenseIncomeReference :BaseEntity
    {
        [ForeignKey("sExpenseIncomeOriginID")]
        public ExpenseIncome ExpenseIncomeOrigin { get; set; }
        [Required]
        public string sExpenseIncomeOriginID { get; set; }

        [ForeignKey("sExpenseIncomeDestinyID")]
        public ExpenseIncome ExpenseIncomeDestiny { get; set; }
        [Required]
        public string sExpenseIncomeDestinyID { get; set; }
    }
}
