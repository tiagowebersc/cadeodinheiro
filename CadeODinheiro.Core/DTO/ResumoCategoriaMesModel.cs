using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DTO
{
    public class ResumoCategoriaMesModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dData { get; set; }

        public string sDescricao { get; set; }

        public string sDescConta { get; set; }

        public string sCategoryID { get; set; }

        public string sDescCategoria { get; set; }

        public double dValor { get; set; }

        public string sAgrupadorID { get; set; }
    }
}
