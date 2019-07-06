using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class TransferenciaModel
    {
        public string sID { get; set; }

        [Display(Name = "Descrição")]
        [Required]
        public string sDescricao { get; set; }

        [Display(Name = "Conta Origem")]
        [Required]
        public string sAccountOriginID { get; set; }

        [Display(Name = "Conta Destino")]
        [Required]
        public string sAccountDestinyID { get; set; }

        [Display(Name = "Categoria")]
        [Required]
        public string sCategoryID { get; set; }

        [Display(Name = "Valor")]
        [Required]
        public double dValor { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime dData { get; set; }

        public List<SelectListItem> listaContas { get; set; }

        public List<SelectListItem> listaCategorias { get; set; }

    }
}
