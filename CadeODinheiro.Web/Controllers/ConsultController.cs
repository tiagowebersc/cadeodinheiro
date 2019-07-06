using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.DTO;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Entity.Enum;
using CadeODinheiro.Web.Infrastructure.Filters;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CadeODinheiro.Web.Infrastructure.Utils;

namespace CadeODinheiro.Web.Controllers
{
    public class ConsultController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public IExpenseIncomeBusiness expenseIncomeBusiness { get; set; }
        [Inject]
        public IExpenseIncomeReferenceBusiness expenseIncomeReferenceBusiness { get; set; }
        [Inject]
        public IAccountBusiness accountBusiness { get; set; }
        [Inject]
        public ICategoryBusiness categoryBusiness { get; set; }
        [Inject]
        public INotificationBusiness notificationBusiness { get; set; }
        
        private List<ResumoCategoriaMesModel> gerarListaDespesa(string sMesAno)
        {
            List<ResumoCategoriaMesModel> retorno = new List<ResumoCategoriaMesModel>();
            DateTime dDataInicio = new DateTime(int.Parse(sMesAno.Substring(2, 4)), int.Parse(sMesAno.Substring(0, 2)), 1);
            DateTime dDataFim = dDataInicio.AddMonths(1) - (new TimeSpan(0, 0, 0, 1));
            // tudo do mês sem cartão de crédito
            List<ExpenseIncome> lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.Despesa).OrderBy(e => e.sCategoryID).ToList();
            foreach (ExpenseIncome exp in lstMesAtual)
            {
                Account conta = accountBusiness.Get.FirstOrDefault(c => c.sID == exp.sAccountID);
                if (conta.AccountType != AccountType.CartaoDeCredito)
                {
                    ResumoCategoriaMesModel resumo = new ResumoCategoriaMesModel();
                    resumo.dData = exp.dData;
                    resumo.dValor = exp.dValor;
                    resumo.sCategoryID = exp.sCategoryID;
                    resumo.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == exp.sCategoryID).descricao;
                    resumo.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == exp.sAccountID).sNome;
                    resumo.sDescricao = exp.sDescricao;
                    if (exp.iTotalOcorrencia > 1) resumo.sDescricao += " - " + exp.iNumeroOcorrencia + "/" + exp.iTotalOcorrencia;
                    retorno.Add(resumo);
                }
            }
            // tudo do mês pago por cartão de crédito
            lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.PagamentoCartaoCredito).OrderBy(e => e.sCategoryID).ToList();
            foreach (ExpenseIncome exp in lstMesAtual)
            {
                List<ExpenseIncomeReference> lstCartao = expenseIncomeReferenceBusiness.Get.Where(e => e.sExpenseIncomeDestinyID == exp.sID).ToList();
                foreach (ExpenseIncomeReference expRef in lstCartao)
                {
                    ExpenseIncome expPago = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == expRef.sExpenseIncomeOriginID);
                    if (expPago.CategoryType == CategoryType.Despesa)
                    {
                        ResumoCategoriaMesModel resumo = new ResumoCategoriaMesModel();
                        resumo.dData = expPago.dData;
                        resumo.dValor = expPago.dValor;
                        resumo.sCategoryID = expPago.sCategoryID;
                        resumo.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == expPago.sCategoryID).descricao;
                        resumo.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == expPago.sAccountID).sNome;
                        resumo.sDescricao = expPago.sDescricao;
                        if (expPago.iTotalOcorrencia > 1) resumo.sDescricao += " - " + expPago.iNumeroOcorrencia + "/" + expPago.iTotalOcorrencia;
                        retorno.Add(resumo);
                    }
                }
            }
            return retorno;
        }

        private void gerarListaCompletaDespesa(ref ResumoMensalModel resumoMensal)
        {
            List<ResumoCategoriaMesModel> listaDetalhada = gerarListaDespesa(resumoMensal.sMesAno);
            
            double dTotal = 0;
            Dictionary<string, double> dcExpense = new Dictionary<string, double>();
            foreach (ResumoCategoriaMesModel exp in listaDetalhada)
            {
                if (!dcExpense.ContainsKey(exp.sCategoryID)) dcExpense.Add(exp.sCategoryID, 0);
                dcExpense[exp.sCategoryID] += exp.dValor;
                dTotal += exp.dValor;
            }
            
            dcExpense = dcExpense.OrderByDescending(d => d.Value).ToDictionary(x => x.Key, x => x.Value); ;
            int iTotElementos = dcExpense.Count;
            foreach (var hr in dcExpense)
            {
                HomeResumoCategoriaModel homeResumo = new HomeResumoCategoriaModel();
                homeResumo.sMesAno = resumoMensal.sMesAno;
                homeResumo.dValor = hr.Value;
                homeResumo.sCategoriaID = hr.Key;
                homeResumo.sDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == hr.Key).descricao;
                homeResumo.dPercentual = (hr.Value * 100) / dTotal;
                resumoMensal.lstCategoriaGastos.Add(homeResumo);
                
                if (string.IsNullOrEmpty(resumoMensal.sLstCategoriaGastos)) resumoMensal.sLstCategoriaGastos = homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2); else resumoMensal.sLstCategoriaGastos += "|&" + homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2);
            }
            resumoMensal.dTotalDespesa = dTotal;
        }

        private List<ResumoCategoriaMesModel> gerarListaReceita(string sMesAno)
        {
            List<ResumoCategoriaMesModel> retorno = new List<ResumoCategoriaMesModel>();
            DateTime dDataInicio = new DateTime(int.Parse(sMesAno.Substring(2, 4)), int.Parse(sMesAno.Substring(0, 2)), 1);
            DateTime dDataFim = dDataInicio.AddMonths(1) - (new TimeSpan(0, 0, 0, 1));
            List<ExpenseIncome> lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.Receita).OrderBy(e => e.sCategoryID).ToList();

            foreach (ExpenseIncome exp in lstMesAtual)
            {
                ResumoCategoriaMesModel homeResumo = new ResumoCategoriaMesModel();
                homeResumo.dData = exp.dData;
                homeResumo.dValor = exp.dValor;
                homeResumo.sCategoryID = exp.sCategoryID;
                homeResumo.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == exp.sCategoryID).descricao;
                homeResumo.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == exp.sAccountID).sNome;
                homeResumo.sDescricao = exp.sDescricao;
                retorno.Add(homeResumo);
            }
            // crédito do mês pago por cartão de crédito
            lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.PagamentoCartaoCredito).OrderBy(e => e.sCategoryID).ToList();
            foreach (ExpenseIncome exp in lstMesAtual)
            {
                List<ExpenseIncomeReference> lstCartao = expenseIncomeReferenceBusiness.Get.Where(e => e.sExpenseIncomeDestinyID == exp.sID).ToList();
                foreach (ExpenseIncomeReference expRef in lstCartao)
                {
                    ExpenseIncome expPago = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == expRef.sExpenseIncomeOriginID);
                    if (expPago.CategoryType == CategoryType.Receita)
                    {
                        ResumoCategoriaMesModel homeResumo = new ResumoCategoriaMesModel();
                        homeResumo.dData = expPago.dData;
                        homeResumo.dValor = expPago.dValor;
                        homeResumo.sCategoryID = expPago.sCategoryID;
                        homeResumo.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == expPago.sCategoryID).descricao;
                        homeResumo.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == expPago.sAccountID).sNome;
                        homeResumo.sDescricao = expPago.sDescricao;
                        retorno.Add(homeResumo);
                    }
                }
            }
            return retorno;
        }

        private void gerarListaCompletaReceita(ref ResumoMensalModel resumoMensal)
        {
            List<ResumoCategoriaMesModel> lista = gerarListaReceita(resumoMensal.sMesAno);
            double dTotal = 0;
            foreach (ResumoCategoriaMesModel exp in lista)
            {
                dTotal += exp.dValor;
            }
            resumoMensal.dTotalReceita = dTotal;
        }

        [Auth()]
        public ActionResult ResumoMensal()
        {
            string sMesAno = DateTime.Now.ToString("MMyyyy");
            return View(criaResumoMensal(sMesAno));
        }

        [Auth()]
        public ActionResult Previsao()
        {
            PrevisaoModel previsao = new PrevisaoModel();
            DateTime dataBase = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            previsao.sMes1 = dataBase.ToString("yyyyMM");
            previsao.lstMes1 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes1)
            {
                if (pfm.bPago) previsao.totalPago1 += pfm.dValor; else previsao.totalAPagar1 += pfm.dValor;
                previsao.total1 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes2 = dataBase.ToString("yyyyMM");
            previsao.lstMes2 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes2)
            {
                if (pfm.bPago) previsao.totalPago2 += pfm.dValor; else previsao.totalAPagar2 += pfm.dValor;
                previsao.total2 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes3 = dataBase.ToString("yyyyMM");
            previsao.lstMes3 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes3)
            {
                if (pfm.bPago) previsao.totalPago3 += pfm.dValor; else previsao.totalAPagar3 += pfm.dValor;
                previsao.total3 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes4 = dataBase.ToString("yyyyMM");
            previsao.lstMes4 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes4)
            {
                if (pfm.bPago) previsao.totalPago4 += pfm.dValor; else previsao.totalAPagar4 += pfm.dValor;
                previsao.total4 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes5 = dataBase.ToString("yyyyMM");
            previsao.lstMes5 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes5)
            {
                if (pfm.bPago) previsao.totalPago5 += pfm.dValor; else previsao.totalAPagar5 += pfm.dValor;
                previsao.total5 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes6 = dataBase.ToString("yyyyMM");
            previsao.lstMes6 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes6)
            {
                if (pfm.bPago) previsao.totalPago6 += pfm.dValor; else previsao.totalAPagar6 += pfm.dValor;
                previsao.total6 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes7 = dataBase.ToString("yyyyMM");
            previsao.lstMes7 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes7)
            {
                if (pfm.bPago) previsao.totalPago7 += pfm.dValor; else previsao.totalAPagar7 += pfm.dValor;
                previsao.total7 += pfm.dValor;
            }
            dataBase = dataBase.AddMonths(1);
            previsao.sMes8 = dataBase.ToString("yyyyMM");
            previsao.lstMes8 = ListaPrevisao(dataBase.Month, dataBase.Year);
            foreach (PrevisaoMesModel pfm in previsao.lstMes8)
            {
                if (pfm.bPago) previsao.totalPago8 += pfm.dValor; else previsao.totalAPagar8 += pfm.dValor;
                previsao.total8 += pfm.dValor;
            }
            List<string> lstCategorias = new List<string>();
            #region Gera lista de todas as categorias entre os meses
            foreach (PrevisaoMesModel frm in previsao.lstMes1) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes2) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes3) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes4) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes5) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes6) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes7) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            foreach (PrevisaoMesModel frm in previsao.lstMes8) if (!lstCategorias.Contains(frm.sCategoryID)) lstCategorias.Add(frm.sCategoryID);
            #endregion
            Dictionary<string, Dictionary<string, decimal>> listaGrafico = new Dictionary<string, Dictionary<string, decimal>>();
            Dictionary<string, string> listaNomeCategoria = new Dictionary<string, string>();
            previsao.sDataGrafico = string.Empty;
            int indice = 1;
            decimal dTotal1 = 0, dTotal2 = 0, dTotal3 = 0, dTotal4 = 0, dTotal5 = 0, dTotal6 = 0, dTotal7 = 0, dTotal8 = 0;
            foreach (string sCat in lstCategorias)
            {
                Category catPrevisao = categoryBusiness.Get.FirstOrDefault(c => c.sID == sCat);
                if (catPrevisao.CategoryType == CategoryType.Despesa)
                {
                    Dictionary<string, decimal> lstTemp = new Dictionary<string, decimal>();
                    lstTemp.Add(previsao.sMes1, retornaTotalCategoriaLista(sCat, previsao.lstMes1));
                    dTotal1 += lstTemp[previsao.sMes1];
                    lstTemp.Add(previsao.sMes2, retornaTotalCategoriaLista(sCat, previsao.lstMes2));
                    dTotal2 += lstTemp[previsao.sMes2];
                    lstTemp.Add(previsao.sMes3, retornaTotalCategoriaLista(sCat, previsao.lstMes3));
                    dTotal3 += lstTemp[previsao.sMes3];
                    lstTemp.Add(previsao.sMes4, retornaTotalCategoriaLista(sCat, previsao.lstMes4));
                    dTotal4 += lstTemp[previsao.sMes4];
                    lstTemp.Add(previsao.sMes5, retornaTotalCategoriaLista(sCat, previsao.lstMes5));
                    dTotal5 += lstTemp[previsao.sMes5];
                    lstTemp.Add(previsao.sMes6, retornaTotalCategoriaLista(sCat, previsao.lstMes6));
                    dTotal6 += lstTemp[previsao.sMes6];
                    lstTemp.Add(previsao.sMes7, retornaTotalCategoriaLista(sCat, previsao.lstMes7));
                    dTotal7 += lstTemp[previsao.sMes7];
                    lstTemp.Add(previsao.sMes8, retornaTotalCategoriaLista(sCat, previsao.lstMes8));
                    dTotal8 += lstTemp[previsao.sMes8];
                    listaGrafico.Add(sCat, lstTemp);
                    listaNomeCategoria.Add(sCat, catPrevisao.descricao);
                    //previsao.sDataGrafico += indice + "&|" + listaNomeCategoria[sCat] + "&|" + previsao.sMes1 + "&|" + lstTemp[previsao.sMes1] + "&|" + previsao.sMes2 + "&|" + lstTemp[previsao.sMes2] + "&|" + previsao.sMes3 + "&|" + lstTemp[previsao.sMes3] + "&|" + previsao.sMes4 + "&|" + lstTemp[previsao.sMes4] + "&|" + previsao.sMes5 + "&|" + lstTemp[previsao.sMes5] + "&|" + previsao.sMes6 + "&|" + lstTemp[previsao.sMes6] + "&|" + previsao.sMes7 + "&|" + lstTemp[previsao.sMes7] + "&|" + previsao.sMes8 + "&|" + lstTemp[previsao.sMes8] + "&&|";
                    previsao.sDataGrafico += indice + "&|" + listaNomeCategoria[sCat] + "&|1&|" + lstTemp[previsao.sMes1] + "&|2&|" + lstTemp[previsao.sMes2] + "&|3&|" + lstTemp[previsao.sMes3] + "&|4&|" + lstTemp[previsao.sMes4] + "&|5&|" + lstTemp[previsao.sMes5] + "&|6&|" + lstTemp[previsao.sMes6] + "&|7&|" + lstTemp[previsao.sMes7] + "&|8&|" + lstTemp[previsao.sMes8] + "&&|";
                    indice++;
                }
            }
            previsao.sDataGrafico += 0 + "&|Todos&|1&|" + dTotal1 + "&|2&|" + dTotal2 + "&|3&|" + dTotal3 + "&|4&|" + dTotal4 + "&|5&|" + dTotal5 + "&|6&|" + dTotal6 + "&|7&|" + dTotal7 + "&|8&|" + dTotal8 + "&&|";

                

            return View(previsao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResumoMensal(String sMesAno)
        {
            string sMesAnoBase = string.IsNullOrEmpty(sMesAno) ? DateTime.Now.ToString("MM/yyyy") : sMesAno;
            return View(criaResumoMensal(sMesAnoBase));
        }

        private ResumoMensalModel criaResumoMensal(string sMesAno)
        {
            ResumoMensalModel model = new ResumoMensalModel();
            model.lstCategoriaGastos = new List<HomeResumoCategoriaModel>();
            model.dTotalReceita = 0;
            model.dTotalDespesa = 0;
            model.sMesAno = sMesAno;
            model.sMesAnoAnterior = (new DateTime(int.Parse(sMesAno.Substring(2, 4)), int.Parse(sMesAno.Substring(0, 2)), 1)).AddMonths(-1).ToString("MMyyyy");
            model.sMesAnoPosterior = (new DateTime(int.Parse(sMesAno.Substring(2, 4)), int.Parse(sMesAno.Substring(0, 2)), 1)).AddMonths(1).ToString("MMyyyy");
            gerarListaCompletaDespesa(ref model);
            gerarListaCompletaReceita(ref model);
            model.dTotalDiferenca = model.dTotalReceita - model.dTotalDespesa;
            return model;
        }

        [HttpGet]
        public PartialViewResult ListaResumoReceita(int pagina, string sMesAno)
        {
            if (string.IsNullOrEmpty(sMesAno)) return PartialView("_ListaResumoReceita");
            List<ResumoCategoriaMesModel> lista = gerarListaReceita(sMesAno);
            lista = lista.OrderBy(a => a.dData).ToList();
            return PartialView("_ListaResumoReceita", lista.ToPagedList(pagina, 17));
        }

        [HttpGet]
        public PartialViewResult ListaResumoDespesa(int pagina, string sMesAno)
        {
            if (string.IsNullOrEmpty(sMesAno)) return PartialView("_ListaResumoDespesa");
            List<ResumoCategoriaMesModel> lista = gerarListaDespesa(sMesAno);
            lista = lista.OrderBy(a => a.dData).ThenBy(a => a.sDescricao).ToList();
            return PartialView("_ListaResumoDespesa", lista.ToPagedList(pagina, 17));
        }

        [HttpGet]
        public PartialViewResult ListaResumoCategoria(int pagina, string sMesAnoCategoriaID)
        {
            string sCategoriaID = sMesAnoCategoriaID.Substring(6);
            if (string.IsNullOrEmpty(sCategoriaID)) return PartialView("_ListaResumoCategoria");
            Category category = categoryBusiness.Get.FirstOrDefault(c => c.sID == sCategoriaID);
            if (category == null) return PartialView("_ListaResumoCategoria");

            List<ResumoCategoriaMesModel> lista = gerarListaDespesa(sMesAnoCategoriaID.Substring(0, 6));
            List<ResumoCategoriaMesModel> listaRetorno = new List<ResumoCategoriaMesModel>();
            foreach (ResumoCategoriaMesModel res in lista) if (res.sCategoryID == sCategoriaID) listaRetorno.Add(res);
            listaRetorno = listaRetorno.OrderBy(a => a.dData).ToList();
            return PartialView("_ListaResumoCategoria", listaRetorno.ToPagedList(pagina, 17));
        }

        private List<PrevisaoMesModel> ListaPrevisao(int iMes, int iAno)
        {
            DateTime dInicioMes = new DateTime(iAno, iMes, 1);
            DateTime dFimMes = dInicioMes.AddMonths(1).AddSeconds(-1);
            List<PrevisaoMesModel> lista = new List<PrevisaoMesModel>();
            #region ----------------------------------------------------------------------- Notificações -----------------------------------------------------------------------
            List<Notification> lstNotification = notificationBusiness.Get.Where(n => n.sUserID == AuthProvider.UserAntenticated.sID && n.StatusType == StatusType.Ativo).ToList();
            foreach (Notification not in lstNotification)
            {
                if (categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).CategoryType == CategoryType.Despesa)
                {
                    DateTime dataBaseNot = new DateTime(not.dData.Year, not.dData.Month, 1);
                    DateTime dataFimBaseNot = new DateTime(not.dDataFim.Year, not.dDataFim.Month, 1);

                    if (dataBaseNot <= dInicioMes && dataFimBaseNot >= dInicioMes)
                    {
                        PrevisaoMesModel previsao = new PrevisaoMesModel();
                        previsao.dData = OperacaoData.retornoDataTimeValido(iAno, iMes, not.dData.Day);
                        previsao.dValor = not.dValor;
                        Category catTemp = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID);
                        if (catTemp.CategoryType == CategoryType.Receita) previsao.dValor *= (-1);
                        previsao.sAccountID = string.Empty;
                        previsao.sCategoryID = not.sCategoryID;
                        previsao.sDescricao = not.sDescricao;
                        previsao.sAgrupadorID = string.Empty;
                        previsao.bPago = false;
                        DateTime dDataCalculo = OperacaoData.retornoDataTimeValido(iAno, iMes, not.dData.Day);
                        if (expenseIncomeBusiness.Get.Any(e => e.sNotificationID == not.sID && e.dDataBase.Month == dDataCalculo.Month && e.dDataBase.Year == dDataCalculo.Year))
                        {
                            ExpenseIncome expInc = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sNotificationID == not.sID && e.dDataBase.Month == dDataCalculo.Month && e.dDataBase.Year == dDataCalculo.Year);
                            previsao.sDescricao = expInc.sDescricao;
                            previsao.dData = expInc.dDataBase;
                            previsao.dValor = expInc.dValor;
                            if (expInc.CategoryType == CategoryType.Receita) previsao.dValor *= (-1);
                            previsao.bPago = true;
                            previsao.sAccountID = expInc.sAccountID;
                            previsao.sCategoryID = expInc.sCategoryID;
                            previsao.sAgrupadorID = expInc.sAgrupadorOcorrencia;
                        }
                        previsao.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == previsao.sCategoryID).descricao;
                        if (!string.IsNullOrEmpty(previsao.sAccountID)) previsao.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == previsao.sAccountID).sNome; else previsao.sDescConta = string.Empty;
                        lista.Add(previsao);
                    }
                }
            }
            #endregion
            #region ----------------------------------------------------------------------- Cartão de Crédito ------------------------------------------------------------------
            List<Account> lstCartoes = accountBusiness.Get.Where(a => a.sUserID == AuthProvider.UserAntenticated.sID && a.AccountType == AccountType.CartaoDeCredito && a.StatusType == StatusType.Ativo).ToList();
            foreach (Account conta in lstCartoes)
            {
                string sExpenseIncomePagCartaoID = string.Empty;
                // Verifica se o Cartão já foi pago este mês
                List<ExpenseIncome> lstPagCartao = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.CategoryType == CategoryType.PagamentoCartaoCredito && e.dData >= dInicioMes && e.dData <= dFimMes).ToList();
                foreach (ExpenseIncome pagCartao in lstPagCartao)
                {
                    ExpenseIncomeReference expRef = expenseIncomeReferenceBusiness.Get.FirstOrDefault(e => e.sExpenseIncomeDestinyID == pagCartao.sID);
                    if (expRef != null)
                    {
                        if (expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == expRef.sExpenseIncomeOriginID).sAccountID.Equals(conta.sID))
                        {
                            sExpenseIncomePagCartaoID = pagCartao.sID;
                            break;
                        }
                    }
                }
                // se já tem um registro de pagamento só catabiliza oque foi pago, senão pega os possíveis itens para o mês
                if (string.IsNullOrEmpty(sExpenseIncomePagCartaoID))
                {
                    DateTime dataFechamento = CadeODinheiro.Web.Infrastructure.Utils.OperacaoData.retornoDataTimeValido(iAno, iMes, conta.iDiaFechamentoCC);
                    //adiciona os dias de prazo para pagamento e ver se fica no mesmo mês se não um mês é reduzido
                    if (dataFechamento.AddDays(15).Month != iMes) dataFechamento = dataFechamento.AddMonths(-1);
                    DateTime dataInicioVigencia = dataFechamento.AddMonths(-1).AddDays(1);
                    // pega os itens avulsos
                    List<ExpenseIncome> lstGastos = expenseIncomeBusiness.Get.Where(e => e.sAccountID == conta.sID && e.dData >= dataInicioVigencia && e.dData <= dataFechamento && e.iTotalOcorrencia == 1).ToList();
                    foreach (ExpenseIncome gasto in lstGastos)
                    {
                        PrevisaoMesModel previsao = new PrevisaoMesModel();
                        previsao.dData = gasto.dData;
                        previsao.dValor = gasto.dValor;
                        if (gasto.CategoryType == CategoryType.Receita) previsao.dValor *= (-1);
                        previsao.sAccountID = gasto.sAccountID;
                        previsao.sCategoryID = gasto.sCategoryID;
                        previsao.sDescricao = gasto.sDescricao;
                        previsao.sAgrupadorID = gasto.sAgrupadorOcorrencia;
                        previsao.bPago = false;
                        previsao.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == previsao.sCategoryID).descricao;
                        previsao.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == previsao.sAccountID).sNome;
                        lista.Add(previsao);
                    }
                    // calcula os parcelados
                    lstGastos = expenseIncomeBusiness.Get.Where(e => e.sAccountID == conta.sID && e.iTotalOcorrencia > 1).ToList();
                    foreach (ExpenseIncome gasto in lstGastos)
                    {
                        DateTime dataCalculada = gasto.dData.AddMonths((gasto.iNumeroOcorrencia-1));
                        if (dataCalculada >= dataInicioVigencia && dataCalculada <= dataFechamento)
                        {
                            PrevisaoMesModel previsao = new PrevisaoMesModel();
                            previsao.dData = gasto.dData;
                            previsao.dValor = gasto.dValor;
                            if (gasto.CategoryType == CategoryType.Receita) previsao.dValor *= (-1);
                            previsao.sAccountID = gasto.sAccountID;
                            previsao.sCategoryID = gasto.sCategoryID;
                            previsao.sDescricao = gasto.sDescricao + " - " + gasto.iNumeroOcorrencia + "/" + gasto.iTotalOcorrencia;
                            previsao.sAgrupadorID = gasto.sAgrupadorOcorrencia;
                            previsao.bPago = false;
                            previsao.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == previsao.sCategoryID).descricao;
                            previsao.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == previsao.sAccountID).sNome;
                            lista.Add(previsao);
                        }
                    }

                }
                else
                {
                    List<ExpenseIncomeReference> lstGastos = expenseIncomeReferenceBusiness.Get.Where(e => e.sExpenseIncomeDestinyID == sExpenseIncomePagCartaoID).ToList();
                    foreach (ExpenseIncomeReference gasto in lstGastos)
                    {
                        ExpenseIncome expInc = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == gasto.sExpenseIncomeOriginID);
                            
                        PrevisaoMesModel previsao = new PrevisaoMesModel();
                        previsao.dData = expInc.dData;
                        previsao.dValor = expInc.dValor;
                        if (expInc.CategoryType == CategoryType.Receita) previsao.dValor *= (-1);
                        previsao.sAccountID = expInc.sAccountID;
                        previsao.sCategoryID = expInc.sCategoryID;
                        previsao.sDescricao = expInc.sDescricao;
                        if (expInc.iTotalOcorrencia > 1) previsao.sDescricao += " - " + expInc.iNumeroOcorrencia + "/" + expInc.iTotalOcorrencia;
                        previsao.sAgrupadorID = expInc.sAgrupadorOcorrencia;
                        previsao.bPago = true;
                        previsao.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == previsao.sCategoryID).descricao;
                        previsao.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == previsao.sAccountID).sNome;
                        lista.Add(previsao);
                    }
                }
            }
            #endregion
            return lista;
        }

        private decimal retornaTotalCategoriaLista (string sCategoryID, List<PrevisaoMesModel> lista){
            decimal total = 0;
            foreach (PrevisaoMesModel pfm in lista)
            {
                if (pfm.sCategoryID.Equals(sCategoryID))
                    total += Convert.ToDecimal(pfm.dValor);
            }
            return total;
        }

    }
}
