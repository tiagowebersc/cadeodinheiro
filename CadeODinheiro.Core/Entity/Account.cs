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
    [Table("tbAccount")]
    public class Account : BaseEntity
    {
        [Display(Name = "Nome")]
        [StringLength(30)]
        [Required(ErrorMessage = "Nome deve ser informado!")]
        public string sNome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(255)]
        [Required(ErrorMessage = "Descrição deve ser informado!")]
        public string sDescricao { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Tipo deve ser informado!")]
        public AccountType AccountType { get; set; }

        [Display(Name = "Saldo")]
        [Required(ErrorMessage = "Saldo deve ser informado!")]
        public double dSaldo { get; set; }

        [ForeignKey("sUserID")]
        public User User { get; set; }
        [Required]
        public string sUserID { get; set; }

        [Display(Name = "Situação")]
        [Required]
        [DefaultValue(StatusType.Ativo)]
        public StatusType StatusType { get; set; }

        [Display(Name = "Dia fechamento do Cartão de Crédito")]
        [Required]
        [DefaultValue(1)]
        public int iDiaFechamentoCC { get; set; }
        
    }
}
