using CadeODinheiro.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbLogExpenseIncome")]
    public class LogExpenseIncome : BaseEntity
    {
        [Display(Name = "Tipo Log")]
        public LogType LogType { get; set; }

        [Display(Name = "Descrição")]
        public string sDescricao { get; set; }

        [Display(Name = "Tipo")]
        public CategoryType CategoryType { get; set; }

        [Display(Name = "Categoria")]
        public string sCategoryDesc { get; set; }

        [Display(Name = "Conta")]
        public string sAccountDesc { get; set; }

        [Display(Name = "Valor")]
        public double dValor { get; set; }

        [Display(Name = "Data")]
        public DateTime dData { get; set; }

        [DefaultValue(false)]
        public bool bPagoCartaoCredito { get; set; }

        [Display(Name = "Notificação")]
        public string sNotificationDesc { get; set; }

        [ForeignKey("sUserID")]
        public User User { get; set; }
        public string sUserID { get; set; }

        [Display(Name = "Data Base")]
        public DateTime dDataBase { get; set; }
    }
}
