using CadeODinheiro.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class NotificationModel
    {
        public string sID { get; set; }

        [Display(Name = "Descrição")]
        [Required]
        public string sDescricao { get; set; }

        [Display(Name = "Categoria")]
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

        [Display(Name = "Data Fim")]
        [Required]
        public DateTime dDataFim { get; set; }

        [Display(Name = "Situação")]
        [Required]
        public StatusType StatusType { get; set; }

        public List<SelectListItem> listaCategorias { get; set; }
    }
}
