using CadeODinheiro.Core.DTO;
using CadeODinheiro.Web.Infrastructure.Filters;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Mvc;
using System.Linq;
using PagedList;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using System.Data.Entity.Validation;
using CadeODinheiro.Web.App_Start;
using CadeODinheiro.Core.Entity.Enum;
using PagedList;
using CadeODinheiro.Web.Infrastructure.Utils;

namespace CadeODinheiro.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public IParameterBusiness parameterBusiness { get; set; }
        [Inject]
        public IAccountBusiness accountBusiness { get; set; }
        [Inject]
        public INotificationBusiness notificationBusiness { get; set; }
        [Inject]
        public IExpenseIncomeBusiness expenseIncomeBusiness { get; set; }
        [Inject]
        public ICategoryBusiness categoryBusiness { get; set; }

        public HomeController()
        {

        }

        [Auth()]
        public ActionResult Index()
        {
            if (AuthProvider != null)
            {
                TempData["NomeUsuario"] = AuthProvider.UserAntenticated.Login;
                TempData["MensagemBemVindo"] = "Seja Bem Vindo(a): " + AuthProvider.UserAntenticated.Nome + "!";
            }
            return View();
        }

        private HomeModel gerarConteudoHome()
        {
            HomeModel home = new HomeModel();
            home.listaContas = accountBusiness.Get.Where(a => a.sUserID == AuthProvider.UserAntenticated.sID && a.StatusType == StatusType.Ativo).ToList();
            #region ----- Notification ---------------------------------------------------------------------------------------------
            List<Notification> lstNotification = notificationBusiness.Get.Where(n => n.sUserID == AuthProvider.UserAntenticated.sID && n.StatusType == StatusType.Ativo).ToList();
            foreach (Notification not in lstNotification)
            {
                if (not.NotificationType == NotificationType.OcorrenciaUnica)
                {
                    if (!expenseIncomeBusiness.Get.Any(e => e.sNotificationID == not.sID) && not.dData < (DateTime.Now.AddDays(10)))
                    {
                        home.listaNotificacoes.Add(new HomeNotificacaoModel() { sID = not.sID, dData = not.dData, dValorPrevisto = not.dValor, sDescricao = not.sDescricao, sCategoriaDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).descricao });

                    }
                }
                else
                {
                    List<HomeNotificacaoModel> lstNotTemp = new List<HomeNotificacaoModel>();
                    // mês anterior
                    DateTime dDataCalculo = OperacaoData.retornoDataTimeValido(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, not.dData.Day);
                    if (dDataCalculo < (DateTime.Now.AddDays(10)) && !expenseIncomeBusiness.Get.Any(e => e.sNotificationID == not.sID && e.dDataBase.Month == dDataCalculo.Month))
                    {
                        lstNotTemp.Add(new HomeNotificacaoModel() { sID = not.sID, dData = dDataCalculo, dValorPrevisto = not.dValor, sDescricao = not.sDescricao, sCategoriaDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).descricao });
                    }
                    // mês atual
                    dDataCalculo = OperacaoData.retornoDataTimeValido(DateTime.Now.Year, DateTime.Now.Month, not.dData.Day);
                    if (dDataCalculo < (DateTime.Now.AddDays(10)) && !expenseIncomeBusiness.Get.Any(e => e.sNotificationID == not.sID && e.dDataBase.Month == dDataCalculo.Month))
                    {
                        lstNotTemp.Add(new HomeNotificacaoModel() { sID = not.sID, dData = dDataCalculo, dValorPrevisto = not.dValor, sDescricao = not.sDescricao, sCategoriaDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).descricao });
                    }
                    // verifica próximo mês
                    dDataCalculo = OperacaoData.retornoDataTimeValido(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, not.dData.Day);
                    if (dDataCalculo < (DateTime.Now.AddDays(10)) && !expenseIncomeBusiness.Get.Any(e => e.sNotificationID == not.sID && e.dDataBase.Month == dDataCalculo.Month))
                    {
                        lstNotTemp.Add(new HomeNotificacaoModel() { sID = not.sID, dData = dDataCalculo, dValorPrevisto = not.dValor, sDescricao = not.sDescricao, sCategoriaDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).descricao });
                    }

                    foreach (HomeNotificacaoModel hNot in lstNotTemp)
                    {
                        if (hNot.dData >= not.dData && ((not.NotificationType == NotificationType.OcorrenciaUnica) || (not.NotificationType == NotificationType.Mensal && hNot.dData <= not.dDataFim)))
                        {
                            home.listaNotificacoes.Add(hNot);
                        }
                    }
                }
            }
            #endregion
            #region ----- Resumo Categorias Mês Atual ------------------------------------------------------------------------------
            DateTime dDataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime dDataFim = dDataInicio.AddMonths(1) - (new TimeSpan(0, 0, 0, 1));
            List<ExpenseIncome> lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.Despesa).OrderBy(e => e.sCategoryID).ToList();

            double dTotal = 0;
            Dictionary<string, double> dcExpense = new Dictionary<string, double>();
            foreach (ExpenseIncome exp in lstMesAtual)
            {
                if (!dcExpense.ContainsKey(exp.sCategoryID)) dcExpense.Add(exp.sCategoryID, 0);
                dcExpense[exp.sCategoryID] += exp.dValor;
                dTotal += exp.dValor;
            }

            dcExpense = dcExpense.OrderByDescending(d => d.Value).ToDictionary(x => x.Key, x => x.Value);
            double dPercOutros = 0;
            int iTotElementos = dcExpense.Count, iOcorrencia = 0;
            foreach (var hr in dcExpense)
            {
                HomeResumoCategoriaModel homeResumo = new HomeResumoCategoriaModel();
                homeResumo.sMesAno = dDataInicio.ToString("MMyyyy");
                homeResumo.dValor = hr.Value;
                homeResumo.sCategoriaID = hr.Key;
                homeResumo.sDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == hr.Key).descricao;
                homeResumo.dPercentual = (hr.Value * 100) / dTotal;
                home.listaResumoMesAtual.Add(homeResumo);

                if (string.IsNullOrEmpty(home.sListaResumoMesAtual)) home.sListaResumoMesAtual = homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2); else home.sListaResumoMesAtual += "|&" + homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2);
                
            }
            #endregion
            #region ----- Resumo Categorias Mês Anterior ---------------------------------------------------------------------------
            DateTime dtTemp = DateTime.Now;
            dtTemp = dtTemp.AddMonths(-1);
            dDataInicio = new DateTime(dtTemp.Year, dtTemp.Month, 1);
            dDataFim = dDataInicio.AddMonths(1) - (new TimeSpan(0, 0, 0, 1));
            List<ExpenseIncome> lstMesAnterior = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.CategoryType == CategoryType.Despesa).OrderBy(e => e.sCategoryID).ToList();

            dTotal = 0;
            dcExpense = new Dictionary<string, double>();
            foreach (ExpenseIncome exp in lstMesAnterior)
            {
                if (!dcExpense.ContainsKey(exp.sCategoryID)) dcExpense.Add(exp.sCategoryID, 0);
                dcExpense[exp.sCategoryID] += exp.dValor;
                dTotal += exp.dValor;
            }

            dcExpense = dcExpense.OrderByDescending(d => d.Value).ToDictionary(x => x.Key, x => x.Value); ;
            dPercOutros = 0;
            iTotElementos = dcExpense.Count;
            iOcorrencia = 0;
            foreach (var hr in dcExpense)
            {
                HomeResumoCategoriaModel homeResumo = new HomeResumoCategoriaModel();
                homeResumo.sMesAno = dDataInicio.ToString("MMyyyy");
                homeResumo.dValor = hr.Value;
                homeResumo.sCategoriaID = hr.Key;
                homeResumo.sDescricao = categoryBusiness.Get.FirstOrDefault(c => c.sID == hr.Key).descricao;
                homeResumo.dPercentual = (hr.Value * 100) / dTotal;
                home.listaResumoMesAnterior.Add(homeResumo);
                
                if (string.IsNullOrEmpty(home.sListaResumoMesAnterior)) home.sListaResumoMesAnterior = homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2); else home.sListaResumoMesAnterior += "|&" + homeResumo.sDescricao + "|&" + Math.Round(homeResumo.dPercentual, 2);                
            }

            #endregion
            return home;
        }

        [Auth()]
        public ActionResult PaginaInicial()
        {
            
            return View(gerarConteudoHome());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaginaInicial(HomeModel model, string sNotificacaoID)
        {
            if (!string.IsNullOrEmpty(sNotificacaoID)) return RedirectToAction("Lancamento", "ExpenseIncome", new { sNotificacaoID = sNotificacaoID });
            return View(gerarConteudoHome());
        }

        private PagedList.IPagedList<ResumoCategoriaMesModel> gerarListaResumoCategoria(int pagina, string sMesAnoCategoriaID){
            string sMesAno = sMesAnoCategoriaID.Substring(0, 6);
            string sCategoriaID = sMesAnoCategoriaID.Substring(6);
            List<ResumoCategoriaMesModel> lstResumo = new List<ResumoCategoriaMesModel>();
            Category category = categoryBusiness.Get.FirstOrDefault(c => c.sID == sCategoriaID);
            if (string.IsNullOrEmpty(sMesAno)) sMesAno = DateTime.Today.ToString("MMyyyy");

            DateTime dDataInicio = new DateTime(Int32.Parse(sMesAno.Substring(2, 4)), Int32.Parse(sMesAno.Substring(0, 2)), 1);
            DateTime dDataFim = dDataInicio.AddMonths(1) - (new TimeSpan(0, 0, 0, 1));
            List<ExpenseIncome> lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.dData >= dDataInicio && e.dData <= dDataFim && e.sCategoryID == sCategoriaID && e.iNumeroOcorrencia == 1).OrderBy(e => e.dData).ThenBy(e => e.sDescricao).ToList();

            foreach (ExpenseIncome exp in lstMesAtual)
            {
                ResumoCategoriaMesModel rcm = new ResumoCategoriaMesModel();
                rcm.dData = exp.dData;
                rcm.dValor = exp.dValor;
                rcm.sDescricao = exp.sDescricao;
                rcm.sAgrupadorID = exp.sAgrupadorOcorrencia;
                if (exp.iTotalOcorrencia > 1) rcm.sDescricao += " (" + exp.iTotalOcorrencia + "x)";
                rcm.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == exp.sAccountID).sNome;
                lstResumo.Add(rcm);
            }
            foreach (ResumoCategoriaMesModel rcm in lstResumo)
            {
                if (!string.IsNullOrEmpty(rcm.sAgrupadorID))
                {
                    lstMesAtual = expenseIncomeBusiness.Get.Where(e => e.sAgrupadorOcorrencia == rcm.sAgrupadorID).ToList();
                    double dTotal = 0;
                    foreach (ExpenseIncome expInc in lstMesAtual) dTotal += expInc.dValor;
                    rcm.dValor = dTotal;
                }
            }

            return lstResumo.ToPagedList(pagina, 17);
        }

        [HttpGet]
        public PartialViewResult ListaResumoCategoria(int pagina, string sMesAnoCategoriaID)
        {
            string sCategoriaID = sMesAnoCategoriaID.Substring(6);
            if (string.IsNullOrEmpty(sCategoriaID)) return PartialView("_ListaResumoCategoria");
            Category category = categoryBusiness.Get.FirstOrDefault(c => c.sID == sCategoriaID);
            if (category == null) return PartialView("_ListaResumoCategoria");

            return PartialView("_ListaResumoCategoria", gerarListaResumoCategoria(pagina, sMesAnoCategoriaID));
        }
    }
}
