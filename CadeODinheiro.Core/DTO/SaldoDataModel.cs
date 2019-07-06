using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class SaldoDataModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataLimite { get; set; }

        public List<SelectListItem> listaContas { get; set; }

        public string sContaID { get; set; }

        public string sMensagemErro { get; set; }
    }

    public class SaldoDataItemModel
    {
        public string sContaDesc { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dData { get; set; }

        public double dValorDia { get; set; }

        public double dSaldo { get; set; }
    }
}
