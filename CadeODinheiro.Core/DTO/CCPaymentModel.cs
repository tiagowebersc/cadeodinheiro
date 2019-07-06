using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class CCPaymentModel
    {
        [Display(Name = "Descrição")]
        [Required]
        public string sDescricao { get; set; }

        [Display(Name = "Categoria")]
        [Required]
        public string sCategoryID { get; set; }

        [Display(Name = "Conta")]
        [Required]
        public string sAccountID { get; set; }

        [Display(Name = "Conta para o Pagamento")]
        [Required]
        public string sAccountPaymentID { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime dData { get; set; }

        [Display(Name = "Valot Total")]
        public Double dValorTotal { get; set; }

        public string sListaGastosID { get; set; }

        public List<SelectListItem> listaCategorias { get; set; }

        public List<SelectListItem> listaContas { get; set; }

        public List<SelectListItem> listaContasPagamento { get; set; }
    }

    public class CCExpensePaymentModel
    {
        public string sID { get; set; }

        public string sData { get; set; }

        public string sDescricao { get; set; }

        public string sValor { get; set; } 
    }
}
