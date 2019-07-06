using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    public class ExtratoModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataInicioFiltro { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataFinalFiltro { get; set; }

        public List<SelectListItem> listaContas { get; set; }

        public string sContaID { get; set; }

        public List<ExtratoValor> listaValor { get; set; }

        public string sMensagemErro { get; set; }
    }

    public class ExtratoValor
    {
        public string sID { get; set; }

        public string sAgrupadorID { get; set; }

        public int iAgrupadorTotalOcorrencia { get; set; }

        public string sDescricao { get; set; }

        public string sDescConta { get; set; }

        public string sDescCategoria { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dData { get; set; }

        public double dValor { get; set; }
    }
}
