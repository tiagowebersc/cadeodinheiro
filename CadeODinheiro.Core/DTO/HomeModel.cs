using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DTO
{
    public class HomeModel
    {
        public HomeModel()
        {
            listaContas = new List<Account>();
            listaNotificacoes = new List<HomeNotificacaoModel>();
            listaResumoMesAnterior = new List<HomeResumoCategoriaModel>();
            listaResumoMesAtual = new List<HomeResumoCategoriaModel>();
            sListaResumoMesAnterior = string.Empty;
            sListaResumoMesAtual = string.Empty;
        }

        public List<Account> listaContas { get; set; }

        public List<HomeResumoCategoriaModel> listaResumoMesAnterior { get; set; }

        public List<HomeResumoCategoriaModel> listaResumoMesAtual { get; set; }

        public List<HomeNotificacaoModel> listaNotificacoes { get; set; }

        public string sListaResumoMesAnterior { get; set; }

        public string sListaResumoMesAtual { get; set; }
    }

    public class HomeResumoCategoriaModel
    {
        public string sCategoriaID { get; set; }

        public string sMesAno { get; set; }

        public string sDescricao { get; set; }

        public double dValor { get; set; }

        public double dPercentual { get; set; }
    }

    public class HomeNotificacaoModel
    {
        public string sID { get; set; }

        public string sDescricao { get; set; }

        public string sCategoriaDescricao { get; set; }

        public double dValorPrevisto { get; set; }

        public DateTime dData { get; set; }
    }

}
