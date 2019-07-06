using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class ExpIncAgrupadorModel
    {
        public string sAgrupadorID { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string sDescricao { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public string sCategoryID { get; set; }

        [Display(Name = "Conta")]
        public string sAccountDesc { get; set; }

        [Required]
        [Display(Name = "Data")]
        public DateTime dData { get; set; }

        [Required]
        [Display(Name = "Data Base")]
        public DateTime dDataBase { get; set; }

        [Display(Name = "Alterar Data Base")]
        [Required]
        public bool bDataDiferente { get; set; }

        [Display(Name = "Notificação")]
        public string sNotificationID { get; set; }

        public List<SelectListItem> listaCategorias { get; set; }

        public List<SelectListItem> listaNotifications { get; set; }

        public List<ExpIncAgrupadorItemModel> listaItens { get; set; }

    }

    public class ExpIncAgrupadorItemModel
    {
        public string sID { get; set; }

        public int iParcela { get; set; }

        public bool bPago { get; set; }

        [Required]
        [Display(Name = "Valor")]
        public double dValor { get; set; }

    }
}
