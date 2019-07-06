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
    [Table("tbExpenseIncome")]
    public class ExpenseIncome : BaseEntity
    {
        [Display(Name = "Descrição")]
        [Required]
        public string sDescricao { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Tipo deve ser informado!")]
        public CategoryType CategoryType { get; set; }

        [ForeignKey("sCategoryID")]
        public Category Category { get; set; }
        [Required]
        public string sCategoryID { get; set; }

        [ForeignKey("sAccountID")]
        public Account Account { get; set; }
        [Required]
        public string sAccountID { get; set; }

        [Display(Name = "Valor")]
        [Required]
        public double dValor { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime dData { get; set; }
        
        [DefaultValue(false)]
        [Required]
        public bool bTransferOrigem { get; set; }

        [DefaultValue(false)]
        [Required]
        public bool bPagoCartaoCredito { get; set; }

        [ForeignKey("sNotificationID")]
        public Notification Notification { get; set; }
        public string sNotificationID { get; set; }

        [ForeignKey("sUserID")]
        public User User { get; set; }
        [Required]
        public string sUserID { get; set; }

        [Display(Name = "Data Base")]
        [Required]
        public DateTime dDataBase { get; set; }

        [Display(Name = "Número da Ocorrência")]
        [Required]
        [DefaultValue(1)]
        public int iNumeroOcorrencia { get; set; }

        [Display(Name = "Total de Ocorrências")]
        [Required]
        [DefaultValue(1)]
        public int iTotalOcorrencia { get; set; }

        [Display(Name = "Agrupador das Ocorrências")]
        public string sAgrupadorOcorrencia { get; set; }

    }
}
