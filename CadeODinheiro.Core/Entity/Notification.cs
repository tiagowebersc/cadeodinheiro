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
    [Table("tbNotification")]
    public class Notification : BaseEntity
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

        [Display(Name = "Valor Previsto")]
        [Required]
        public double dValor { get; set; }

        [Display(Name = "Frequência")]
        [Required(ErrorMessage = "Frequência deve ser informado!")]
        public NotificationType NotificationType { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime dData { get; set; }

        [ForeignKey("sUserID")]
        public User User { get; set; }
        [Required]
        public string sUserID { get; set; }

        [Display(Name = "Situação")]
        [Required]
        public StatusType StatusType { get; set; }

        [Display(Name = "Data Fim")]
        [Required]
        public DateTime dDataFim { get; set; }

    }
}
