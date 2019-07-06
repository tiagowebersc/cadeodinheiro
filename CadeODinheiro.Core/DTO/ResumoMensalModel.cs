using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DTO
{
    public class ResumoMensalModel
    {
        public string sMesAno { get; set; }

        public string sMesAnoAnterior { get; set; }

        public string sMesAnoPosterior { get; set; }

        public List<HomeResumoCategoriaModel> lstCategoriaGastos { get; set; }

        //public List<ResumoCategoriaMesModel> lstDetalhadaReceitas { get; set; }

        //public List<ResumoCategoriaMesModel> lstDetalhadaGastos { get; set; }

        public string sLstCategoriaGastos { get; set; }

        public double dTotalDespesa { get; set; }

        public double dTotalReceita { get; set; }

        public double dTotalDiferenca { get; set; }
    }
}
