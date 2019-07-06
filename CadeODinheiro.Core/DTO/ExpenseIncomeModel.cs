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
    public class ExpenseIncomeModel
    {
        public string sID { get; set; }

        [Display(Name = "Descrição")]
        [Required]
        public string sDescricao { get; set; }

        [Display(Name = "Categoria")]
        [Required]
        public string sCategoryID { get; set; }

        public CategoryType eCategoryType { get; set; }

        [Display(Name = "Conta")]
        [Required]
        public string sAccountID { get; set; }

        [Display(Name = "Valor")]
        [Required]
        public double dValor { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime dData { get; set; }

        [Display(Name = "Data Base")]
        [Required]
        public DateTime dDataBase { get; set; }

        [Display(Name = "Alterar Data Base")]
        [Required]
        public bool bDataDiferente { get; set; }

        [Display(Name="Notificação")]
        public string sNotificationID { get; set; }

        [Display(Name = "Total de Parcelas")]
        public int iTotalParcelas { get; set; }

        public string sConcatCartaoCredito { get; set; }

        public List<SelectListItem> listaCategorias { get; set; }

        public List<SelectListItem> listaContas { get; set; }

        public List<SelectListItem> listaNotifications { get; set; }

    }
}