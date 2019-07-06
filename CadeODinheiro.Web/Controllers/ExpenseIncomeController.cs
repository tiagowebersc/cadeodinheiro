using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.DTO;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Entity.Enum;
using CadeODinheiro.Web.Infrastructure.Filters;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CadeODinheiro.Web.Controllers
{
    public class ExpenseIncomeController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public IAccountBusiness accountBusiness { get; set; }
        [Inject]
        public ICategoryBusiness categoryBusiness { get; set; }
        [Inject]
        public IExpenseIncomeBusiness expenseIncomeBusiness { get; set; }
        [Inject]
        public IExpenseIncomeReferenceBusiness expenseIncomeReferenceBusiness { get; set; }
        [Inject]
        public INotificationBusiness notificationBusiness { get; set; }

        private List<SelectListItem> getComboNotificacoes()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Value = string.Empty;
            item.Text = "-";
            lista.Add(item);

            var notification = notificationBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.StatusType == StatusType.Ativo).OrderBy(e => e.sDescricao).ToList();
            foreach (var not in notification)
            {
                SelectListItem item2 = new SelectListItem();
                item2.Value = not.sID;
                item2.Text = not.sDescricao;
                lista.Add(item2);
            }
            return lista;
        }

        private List<SelectListItem> getComboCategorias(List<CategoryType> lstCategoriaExpecifica)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            List<Category> categorias = new List<Category>();
            if (lstCategoriaExpecifica == null)
                categorias = categoryBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID).OrderBy(e => e.CategoryType).ThenBy(e => e.descricao).ToList();
            else
                foreach (CategoryType CategoriaExpecifica in lstCategoriaExpecifica)
                {
                    categorias.AddRange(categoryBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.CategoryType == CategoriaExpecifica).OrderBy(e => e.CategoryType).ThenBy(e => e.descricao).ToList());
                }
            foreach (var cat in categorias)
            {
                SelectListItem item = new SelectListItem();
                item.Value = cat.sID;
                if (cat.CategoryType == Core.Entity.Enum.CategoryType.Despesa) item.Text = "Despesa - " + cat.descricao;
                if (cat.CategoryType == Core.Entity.Enum.CategoryType.Receita) item.Text = "Receita - " + cat.descricao;
                if (cat.CategoryType == Core.Entity.Enum.CategoryType.Transferencia) item.Text = cat.descricao;
                if (cat.CategoryType == Core.Entity.Enum.CategoryType.PagamentoCartaoCredito) item.Text = cat.descricao;
                lista.Add(item);
            }
            return lista;
        }

        private List<SelectListItem> getComboContas(bool bGerarVazio, bool bIncluirCartaoCredito)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            if (bGerarVazio)
            {
                SelectListItem item = new SelectListItem();
                item.Value = string.Empty;
                item.Text = "Todas";
                lista.Add(item);
            }
            var account = accountBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.StatusType == StatusType.Ativo).OrderBy(e => e.sNome).ToList();
            foreach (var cont in account)
            {
                bool incluir = true;
                SelectListItem item = new SelectListItem();
                item.Value = cont.sID;
                item.Text = cont.sNome;
                if (cont.AccountType == AccountType.CartaoDeCredito && !bIncluirCartaoCredito) incluir = false;
                if (incluir) lista.Add(item);
            }
            return lista;
        }

        private string getConcatCartaoCredito()
        {
            string sConcatCartaoCredito = string.Empty;
            var account = accountBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.StatusType == StatusType.Ativo && e.AccountType == AccountType.CartaoDeCredito).OrderBy(e => e.sNome).ToList();
            foreach (var cont in account)
            {
                sConcatCartaoCredito += cont.sID + ";";
            }
            return sConcatCartaoCredito;
        }

        private List<SelectListItem> getComboCartaoCredito()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            var account = accountBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID && e.AccountType == AccountType.CartaoDeCredito).OrderBy(e => e.sNome).ToList();
            foreach (var cont in account)
            {
                SelectListItem item = new SelectListItem();
                item.Value = cont.sID;
                item.Text = cont.sNome;
                lista.Add(item);
            }
            return lista;
        }

        [Auth()]
        public ActionResult Index()
        {
            return RedirectToAction("Extrato");
        }

        //-Extrato-----------------------------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult Extrato()
        {
            ExtratoModel extratoModel = new ExtratoModel();
            extratoModel.dataInicioFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            extratoModel.dataFinalFiltro = (new DateTime((DateTime.Now.AddMonths(1)).Year, (DateTime.Now.AddMonths(1)).Month, 1)) - (new TimeSpan(1, 0, 0, 0));
            extratoModel.sContaID = string.Empty;
            //extratoModel.listaValor = gerarListaExtrato(extratoModel.dataInicioFiltro, extratoModel.dataFinalFiltro, extratoModel.sContaID);
            extratoModel.listaContas = getComboContas(true, true);
            return View(extratoModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Extrato(ExtratoModel extratoModel, string sLancamento, string sAgrupador)
        {
            if (!string.IsNullOrEmpty(sLancamento) && sLancamento.Equals("T")) return RedirectToAction("Lancamento", "ExpenseIncome");
            if (!string.IsNullOrEmpty(sLancamento) && sLancamento.Length == 36) return RedirectToAction("Lancamento", "ExpenseIncome", new { sExpenseIncomeID = sLancamento });
            if (!string.IsNullOrEmpty(sAgrupador) && sAgrupador.Length == 36) return RedirectToAction("LancamentoAgrupado", "ExpenseIncome", new { sAgrupadorID = sAgrupador });
            //extratoModel.listaValor = gerarListaExtrato(extratoModel.dataInicioFiltro, extratoModel.dataFinalFiltro, extratoModel.sContaID);
            extratoModel.listaContas = getComboContas(true, true);
            return View(extratoModel);
        }

        private PagedList.IPagedList<ExtratoValor> gerarListaExtrato(int pagina, DateTime dtInicio, DateTime dtFinal, string sContaID)
        {
            List<ExtratoValor> retorno = new List<ExtratoValor>();
            List<ExpenseIncome> lstExpInc = new List<ExpenseIncome>();
            bool bPositivo = false;

            if (string.IsNullOrEmpty(sContaID)) 
                lstExpInc = expenseIncomeBusiness.Get.Where(e => e.dData >= dtInicio && e.dData <= dtFinal && e.sUserID == AuthProvider.UserAntenticated.sID && e.iNumeroOcorrencia == 1).OrderBy(e => e.dData).ToList();
            else
                lstExpInc = expenseIncomeBusiness.Get.Where(e => e.dData >= dtInicio && e.dData <= dtFinal && e.sUserID == AuthProvider.UserAntenticated.sID && e.iNumeroOcorrencia == 1 && e.sAccountID == sContaID).OrderBy(e => e.dData).ToList();
            foreach (ExpenseIncome expInc in lstExpInc)
            {
                ExtratoValor ext = new ExtratoValor();
                ext.dData = expInc.dData;
                bPositivo = false;
                if (expInc.CategoryType == Core.Entity.Enum.CategoryType.Receita) bPositivo = true;
                if (expInc.CategoryType == Core.Entity.Enum.CategoryType.Transferencia && expInc.bTransferOrigem == false) bPositivo = true;

                if (bPositivo)
                    ext.dValor = expInc.dValor;
                else
                    ext.dValor = -(expInc.dValor);
                ext.sID = expInc.sID;
                ext.sAgrupadorID = expInc.sAgrupadorOcorrencia;
                ext.iAgrupadorTotalOcorrencia = expInc.iTotalOcorrencia;
                ext.sDescricao = expInc.sDescricao;
                ext.sDescConta = accountBusiness.Get.FirstOrDefault(a => a.sID == expInc.sAccountID).sNome;
                ext.sDescCategoria = categoryBusiness.Get.FirstOrDefault(c => c.sID == expInc.sCategoryID).descricao;
                retorno.Add(ext);
            }
            foreach (ExtratoValor ext in retorno)
            {
                if (!string.IsNullOrEmpty(ext.sAgrupadorID))
                {
                    lstExpInc = expenseIncomeBusiness.Get.Where(e => e.sAgrupadorOcorrencia == ext.sAgrupadorID).ToList();
                    double dTotal = 0;
                    bPositivo = false;
                    if (lstExpInc[0].CategoryType == Core.Entity.Enum.CategoryType.Receita) bPositivo = true;
                    if (lstExpInc[0].CategoryType == Core.Entity.Enum.CategoryType.Transferencia && lstExpInc[0].bTransferOrigem == false) bPositivo = true;

                    foreach (ExpenseIncome expInc in lstExpInc) if (bPositivo) dTotal += expInc.dValor; else dTotal -= expInc.dValor;
                    ext.dValor = dTotal;
                }
            }

            return retorno.ToPagedList(pagina, 17);
        }

        [HttpGet]
        public PartialViewResult ListaExtrato(int pagina, string dtInicio, string dtFinal, string sContaID)
        {
            if (string.IsNullOrEmpty(dtInicio) || string.IsNullOrEmpty(dtFinal)) return PartialView("_ListaExtrato");
            DateTime dataI = new DateTime(int.Parse(dtInicio.Substring(6, 4)), int.Parse(dtInicio.Substring(3, 2)), int.Parse(dtInicio.Substring(0, 2)));
            DateTime dataF = new DateTime(int.Parse(dtFinal.Substring(6, 4)), int.Parse(dtFinal.Substring(3, 2)), int.Parse(dtFinal.Substring(0, 2)));
            return PartialView("_ListaExtrato", gerarListaExtrato(pagina, dataI, dataF, sContaID));
        }

        [HttpGet]
        public ActionResult ExcluirLancamento(string sLancamentoID, string sAgrupadorID)
        {
            if (!string.IsNullOrEmpty(sLancamentoID))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        List<ExpenseIncome> lstExpExcluir = new List<ExpenseIncome>();
                        if (string.IsNullOrEmpty(sAgrupadorID))
                            lstExpExcluir = expenseIncomeBusiness.Get.Where(e => e.sID == sLancamentoID).ToList();
                        else
                            lstExpExcluir = expenseIncomeBusiness.Get.Where(e => e.sAgrupadorOcorrencia == sAgrupadorID).ToList();

                        if (lstExpExcluir == null || lstExpExcluir.Count == 0) throw new InvalidOperationException("Lançamento não identificado!");

                        foreach (ExpenseIncome expExcluir in lstExpExcluir)
                        {
                            if (expExcluir.bPagoCartaoCredito) throw new InvalidOperationException("Lançamento marcado como pago, excluir primeiro o lançamento de pagamento do cartão de crédito!");

                            List<ExpenseIncomeReference> expIncRef = expenseIncomeReferenceBusiness.Get.Where(e => e.sExpenseIncomeOriginID == expExcluir.sID).ToList();
                            if (expIncRef == null || expIncRef.Count == 0) expIncRef = expenseIncomeReferenceBusiness.Get.Where(e => e.sExpenseIncomeDestinyID == expExcluir.sID).ToList();
                            if (expIncRef != null && expIncRef.Count > 0)
                            {
                                string sDestino = string.Empty;
                                foreach (ExpenseIncomeReference eiRef in expIncRef)
                                {
                                    string sOrigem = eiRef.sExpenseIncomeOriginID;
                                    sDestino = eiRef.sExpenseIncomeDestinyID;
                                    expenseIncomeReferenceBusiness.Delete(eiRef.sID);
                                    if (expExcluir.CategoryType == CategoryType.PagamentoCartaoCredito)
                                    {
                                        ExpenseIncome expOrigem = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == sOrigem);
                                        expOrigem.bPagoCartaoCredito = false;
                                        expenseIncomeBusiness.Update(expOrigem);

                                        Account account = accountBusiness.Get.FirstOrDefault(a => a.sID == expOrigem.sAccountID);
                                        account.dSaldo -= expOrigem.dValor;
                                        accountBusiness.Update(account);
                                    }
                                    else
                                    {
                                        expenseIncomeBusiness.Delete(sOrigem);
                                    }
                                }
                                expenseIncomeBusiness.Delete(sDestino);
                            }
                            else
                            {
                                expenseIncomeBusiness.Delete(expExcluir.sID);
                            }
                        }
                        scope.Complete();
                        return Json(new { Sucesso = true, Titulo = "Sucesso", Mensagem = "Lançamento excluido com sucesso." }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        return Json(new { Sucesso = false, Titulo = "Erro", Mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { Sucesso = false, Titulo = "Erro", Mensagem = "Lançamento não informado!" }, JsonRequestBehavior.AllowGet);
            }
        }
        //-Lançamento--------------------------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult Lancamento(string sNotificacaoID, string sExpenseIncomeID)
        {
            ExpenseIncomeModel model = new ExpenseIncomeModel();
            if (!string.IsNullOrEmpty(sExpenseIncomeID) && expenseIncomeBusiness.Get.Any(e => e.sID == sExpenseIncomeID))
            {
                ExpenseIncome expInc = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID  == sExpenseIncomeID);
                // restrições -----------------------------------------------------------------------------------------------------------
                //if (expInc.CategoryType == CategoryType.PagamentoCartaoCredito) return Json(new { Sucesso = false, Mensagem = "Lançamentos de Pagamento de Cartão de Crédito não podem ser alterados!", Titulo = "Restrição" }, JsonRequestBehavior.AllowGet);
                if (expInc.CategoryType == CategoryType.Transferencia) return Json(new { Sucesso = false, Mensagem = "Lançamentos de Transferência não podem ser alterados!", Titulo = "Restrição" }, JsonRequestBehavior.AllowGet);
                if (expInc.bPagoCartaoCredito) return Json(new { Sucesso = false, Mensagem = "Lançamentos já marcados como pago no Cartão de Crédito não podem ser alterados!", Titulo = "Restrição" }, JsonRequestBehavior.AllowGet);
                //-----------------------------------------------------------------------------------------------------------------------
                model.sID = expInc.sID;
                model.sDescricao = expInc.sDescricao;
                model.sCategoryID = expInc.sCategoryID;
                model.eCategoryType = expInc.CategoryType;
                model.sNotificationID = expInc.sNotificationID;
                model.sAccountID = expInc.sAccountID;
                model.dData = expInc.dData;
                model.dDataBase = expInc.dDataBase;
                model.bDataDiferente = model.dData != model.dDataBase ? true : false;
                model.dValor = expInc.dValor;
            }
            else
            {
                if (!string.IsNullOrEmpty(sNotificacaoID))
                {
                    Notification not = notificationBusiness.Get.FirstOrDefault(n => n.sID == sNotificacaoID);
                    if (not != null)
                    {
                        model.sNotificationID = not.sID;
                        model.sDescricao = not.sDescricao;
                        model.dValor = not.dValor;
                        model.sCategoryID = not.sCategoryID;
                    }
                }
                model.dData = DateTime.Now;
                model.dDataBase = DateTime.Now;
                model.bDataDiferente = false;
            }
            if (model.eCategoryType == CategoryType.PagamentoCartaoCredito)
            {
                model.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.PagamentoCartaoCredito });
            }
            else
            {
                model.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.Despesa, CategoryType.Receita });
            }
            model.sConcatCartaoCredito = getConcatCartaoCredito();
            model.listaContas = getComboContas(false, true);
            model.listaNotifications = getComboNotificacoes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Lancamento(ExpenseIncomeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.dValor <= 0) throw new InvalidOperationException("Valor deve ser maior que 0 (zero)!");
                    ExpenseIncome expInc = null;
                    if (!string.IsNullOrEmpty(model.sID) && expenseIncomeBusiness.Get.Any(e => e.sID == model.sID))
                    {
                        // Alterar
                        expInc = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == model.sID);
                        if (!string.IsNullOrEmpty(expInc.sAgrupadorOcorrencia)) return Json(new { Sucesso = false, Mensagem = "Lançamento com Agrupador não podem ser alterados pela tela de alteração individual!", Titulo = "Restrição",  Url = "/Home/PaginaInicial" });
                        expInc.dData = model.dData;
                        expInc.dDataBase = model.bDataDiferente ? model.dDataBase : expInc.dData;
                        expInc.dValor = model.dValor;
                        expInc.sCategoryID = model.sCategoryID;
                        expInc.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == model.sCategoryID).CategoryType;
                        expInc.sAccountID = model.sAccountID;
                        expInc.sDescricao = model.sDescricao;
                        expInc.sNotificationID = model.sNotificationID;
                        
                        expenseIncomeBusiness.Update(expInc);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Lançamento Alterado com Sucesso!",
                            Titulo = "Sucesso",
                            Url = "/Home/PaginaInicial"
                        });
                    }
                    else
                    {
                        // Inserir
                        Account conta = accountBusiness.Get.FirstOrDefault(c => c.sID == model.sAccountID);
                        if (conta.AccountType != AccountType.CartaoDeCredito) model.iTotalParcelas = 1;
                        double dValorDivisao = model.dValor / model.iTotalParcelas;
                        if (dValorDivisao < 0.01) throw new InvalidOperationException("Valor da divisão menor que R$ 0,01!");
                        dValorDivisao = Math.Round(dValorDivisao, 2);
                        double dValorSobra = model.dValor;
                        string sAgrupadorParcelas = sAgrupadorParcelas = Guid.NewGuid().ToString();

                        for (int i = 1; i <= model.iTotalParcelas; i++)
                        {
                            expInc = new ExpenseIncome();
                            expInc.dData = model.dData;
                            expInc.dDataBase = model.bDataDiferente ? model.dDataBase : expInc.dData;
                            expInc.sCategoryID = model.sCategoryID;
                            expInc.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == model.sCategoryID).CategoryType;
                            expInc.sAccountID = model.sAccountID;
                            expInc.sDescricao = model.sDescricao;
                            expInc.sNotificationID = model.sNotificationID;
                            expInc.sUserID = AuthProvider.UserAntenticated.sID;
                            if (i == model.iTotalParcelas)
                            {
                                expInc.dValor = dValorSobra;   
                            }
                            else
                            {
                                expInc.dValor = dValorDivisao;
                                dValorSobra -= expInc.dValor;
                            }
                            expInc.iNumeroOcorrencia = i;
                            expInc.iTotalOcorrencia = model.iTotalParcelas;
                            if (model.iTotalParcelas > 1) expInc.sAgrupadorOcorrencia = sAgrupadorParcelas;

                            expenseIncomeBusiness.Insert(expInc);
                        }

                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Lançamento Gerado com Sucesso!",
                            Titulo = "Sucesso",
                            Url = "/Home/PaginaInicial"
                        });
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    string sErro = string.Empty;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            sErro += string.Format(" Property: {0} Error: {1} |", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    return Json(new
                    {
                        Sucesso = false,
                        Mensagem = sErro,
                        Titulo = "Erro"
                    });
                }
                catch (Exception ex)
                {
                    string sErro = ex.Message;
                    if (ex.InnerException != null) sErro += " - " + ex.InnerException.Message;
                    return Json(new
                    {
                        Sucesso = false,
                        Mensagem = sErro,
                        Titulo = "Erro"
                    });
                }
            }
            else
            {
                model.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.Despesa, CategoryType.Receita });
                model.sConcatCartaoCredito = getConcatCartaoCredito();
                model.listaContas = getComboContas(false, true);
                model.listaNotifications = getComboNotificacoes();
                return View(model);
            }
        }
        //-Lançamento Agrupado-----------------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult LancamentoAgrupado(string sAgrupadorID)
        {
            ExpIncAgrupadorModel model = new ExpIncAgrupadorModel();
            if (!string.IsNullOrEmpty(sAgrupadorID) && expenseIncomeBusiness.Get.Any(e => e.sAgrupadorOcorrencia == sAgrupadorID))
            {
                List<ExpenseIncome> lstExpInc = expenseIncomeBusiness.Get.Where(e => e.sAgrupadorOcorrencia == sAgrupadorID).OrderBy(e => e.iNumeroOcorrencia).ToList();

                model.sAgrupadorID = lstExpInc[0].sAgrupadorOcorrencia;
                model.sDescricao = lstExpInc[0].sDescricao;
                model.sCategoryID = lstExpInc[0].sCategoryID;
                model.sNotificationID = lstExpInc[0].sNotificationID;
                string sTemp = lstExpInc[0].sAccountID;
                model.sAccountDesc = accountBusiness.Get.FirstOrDefault(a => a.sID == sTemp).sNome;
                model.dData = lstExpInc[0].dData;
                model.dDataBase = lstExpInc[0].dDataBase;
                model.bDataDiferente = model.dData != model.dDataBase ? true : false;
                model.listaItens = new List<ExpIncAgrupadorItemModel>();
                foreach (ExpenseIncome expInc in lstExpInc)
                {
                    ExpIncAgrupadorItemModel item = new ExpIncAgrupadorItemModel();
                    item.sID = expInc.sID;
                    item.dValor = expInc.dValor;
                    item.bPago = expInc.bPagoCartaoCredito;
                    item.iParcela = expInc.iNumeroOcorrencia;
                    model.listaItens.Add(item);
                }
                model.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.Despesa, CategoryType.Receita });
                model.listaNotifications = getComboNotificacoes();
                return View(model);
            }
            else
            {
                return RedirectToAction("Lancamento", "ExpenseIncome");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LancamentoAgrupado(ExpIncAgrupadorModel model)
        {
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        foreach (ExpIncAgrupadorItemModel expIncAgrup in model.listaItens)
                        {

                            if (!string.IsNullOrEmpty(expIncAgrup.sID) && expenseIncomeBusiness.Get.Any(e => e.sID == expIncAgrup.sID))
                            {
                                // Alterar
                                ExpenseIncome expInc = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == expIncAgrup.sID);
                                expInc.dData = model.dData;
                                expInc.dDataBase = model.bDataDiferente ? model.dDataBase : expInc.dData;
                                if (expIncAgrup.dValor <= 0) throw new InvalidOperationException("Valor deve ser maior que 0 (zero)!");
                                expInc.dValor = expIncAgrup.dValor;
                                expInc.sCategoryID = model.sCategoryID;
                                expInc.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == model.sCategoryID).CategoryType;
                                expInc.sDescricao = model.sDescricao;
                                expInc.sNotificationID = model.sNotificationID;

                                expenseIncomeBusiness.Update(expInc);
                            }
                            else
                            {
                                scope.Dispose();
                                return Json(new
                                {
                                    Sucesso = false,
                                    Mensagem = "Erro na identificação do lançamento!",
                                    Titulo = "Erro",
                                    Url = "/Home/PaginaInicial"
                                });
                            }
                        }
                        scope.Complete();
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Lançamentos Alterados com Sucesso!",
                            Titulo = "Sucesso",
                            Url = "/Home/PaginaInicial"
                        });
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        scope.Dispose();
                        string sErro = string.Empty;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                sErro += string.Format(" Property: {0} Error: {1} |", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        return Json(new
                        {
                            Sucesso = false,
                            Mensagem = sErro,
                            Titulo = "Erro"
                        });
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        string sErro = ex.Message;
                        if (ex.InnerException != null) sErro += " - " + ex.InnerException.Message;
                        return Json(new
                        {
                            Sucesso = false,
                            Mensagem = sErro,
                            Titulo = "Erro"
                        });
                    }
                }
            }
            else
            {
                model.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.Despesa, CategoryType.Receita });
                model.listaNotifications = getComboNotificacoes();
                return View(model);
            }
        }
        //-Pagamento Cartão de Crédito---------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult PagamentoCartaoCredito()
        {
            CCPaymentModel ccPayment = new CCPaymentModel();
            ccPayment.listaCategorias = getComboCategorias(new List<CategoryType>(){CategoryType.PagamentoCartaoCredito});
            ccPayment.listaContas = getComboCartaoCredito();
            ccPayment.listaContasPagamento = getComboContas(false, true);
            ccPayment.dData = DateTime.Now;
            return View(ccPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagamentoCartaoCredito(CCPaymentModel ccPayment)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    if (string.IsNullOrEmpty(ccPayment.sListaGastosID)) throw new InvalidOperationException("Os gastos devem ser selecionados.");
                    if (string.IsNullOrEmpty(ccPayment.sDescricao)) throw new InvalidOperationException("A descrição deve ser preenchida.");
                    if (ccPayment.dData == DateTime.MinValue) throw new InvalidOperationException("A data deve ser preenchida.");

                    List<string> listaGastos = ccPayment.sListaGastosID.Split(';').ToList();
                    List<ExpenseIncome> listaExpenses = new List<ExpenseIncome>();
                    List<string> listaGastosProcessados = new List<string>();
                    double dTotalPagar = 0;
                    foreach (string gasto in listaGastos)
                    {
                        if (!string.IsNullOrEmpty(gasto) && !listaGastosProcessados.Contains(gasto))
                        {
                            ExpenseIncome exp = expenseIncomeBusiness.Get.FirstOrDefault(e => e.sID == gasto);
                            if (exp == null) throw new InvalidOperationException("Problemas na identificação do gasto (" + gasto + ").");
                            if (exp.bPagoCartaoCredito) throw new InvalidOperationException("Gasto (" + exp.sDescricao + ") já marcado como pago.");

                            double valorLancamento = exp.dValor;
                            if (exp.CategoryType == CategoryType.Receita) valorLancamento = valorLancamento * (-1);
                            dTotalPagar += valorLancamento;
                            listaExpenses.Add(exp);
                            listaGastosProcessados.Add(gasto);
                        }
                    }

                    ExpenseIncome expPagCC = new ExpenseIncome();
                    expPagCC.bPagoCartaoCredito = false;
                    expPagCC.bTransferOrigem = false;
                    expPagCC.CategoryType = CategoryType.PagamentoCartaoCredito;
                    expPagCC.dData = ccPayment.dData;
                    expPagCC.dDataBase = ccPayment.dData;
                    expPagCC.dValor = dTotalPagar;
                    expPagCC.sAccountID = ccPayment.sAccountPaymentID;
                    expPagCC.sCategoryID = ccPayment.sCategoryID;
                    expPagCC.sDescricao = ccPayment.sDescricao;
                    expPagCC.sUserID = AuthProvider.UserAntenticated.sID;
                    expenseIncomeBusiness.Insert(expPagCC);

                    foreach (ExpenseIncome exp in listaExpenses)
                    {
                        expenseIncomeBusiness.RegistraExpensePagoCartaoCredito(exp);
                        
                        ExpenseIncomeReference expRef = new ExpenseIncomeReference();
                        expRef.sExpenseIncomeOriginID = exp.sID;
                        expRef.sExpenseIncomeDestinyID = expPagCC.sID;
                        expenseIncomeReferenceBusiness.Insert(expRef);
                    }

                    scope.Complete();
                    return Json(new { Sucesso = true, Mensagem = "Pagamento de Cartão de Crédito realizado com sucesso.", Titulo = "Sucesso" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return Json(new { Sucesso = false, Mensagem = ex.Message, Titulo = "Erro" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public ActionResult ListaGastosCartaoCredito(string sCartaoCreditoID)
        {
            List<ExpenseIncome> listaGastos = new List<ExpenseIncome>();
            List<CCExpensePaymentModel> listaRetorno = new List<CCExpensePaymentModel>();
            listaGastos.AddRange(expenseIncomeBusiness.Get.Where(e => e.sAccountID == sCartaoCreditoID && e.bPagoCartaoCredito == false).OrderBy(e => e.dData).ThenBy(e => e.sDescricao).ThenBy(e => e.iNumeroOcorrencia).ToList());
            foreach (ExpenseIncome exp in listaGastos)
            {
                CCExpensePaymentModel ccExp = new CCExpensePaymentModel();
                ccExp.sID = exp.sID;
                ccExp.sDescricao = exp.sDescricao;
                if (exp.iTotalOcorrencia > 1) ccExp.sDescricao += " - " + exp.iNumeroOcorrencia + "/" + exp.iTotalOcorrencia;
                ccExp.sData = exp.dData.ToString("dd/MM/yyyy");
                double valorLancamento = exp.dValor;
                if (exp.CategoryType == CategoryType.Receita) valorLancamento = valorLancamento * (-1);
                ccExp.sValor = valorLancamento.ToString("0.00");
                listaRetorno.Add(ccExp);
            }

            return Json(new { Sucesso = true, Mensagem = "", Lista = listaRetorno }, JsonRequestBehavior.AllowGet);
        }
        //-Transferência-----------------------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult Transferencia()
        {
            TransferenciaModel transfer = new TransferenciaModel();
            transfer.listaCategorias = getComboCategorias(new List<CategoryType>(){CategoryType.Transferencia});
            transfer.listaContas = getComboContas(false, true);
            transfer.dData = DateTime.Now;
            return View(transfer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transferencia(TransferenciaModel transferModel)
        {
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        if (transferModel.dValor <= 0) throw new InvalidOperationException("Valor deve ser maior que 0 (zero)!");
                        ExpenseIncome expIncOrigem = new ExpenseIncome();
                        expIncOrigem.dData = transferModel.dData;
                        expIncOrigem.dDataBase = transferModel.dData;
                        expIncOrigem.dValor = transferModel.dValor;
                        expIncOrigem.sCategoryID = transferModel.sCategoryID;
                        expIncOrigem.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == transferModel.sCategoryID).CategoryType;
                        expIncOrigem.sAccountID = transferModel.sAccountOriginID;
                        expIncOrigem.sDescricao = transferModel.sDescricao;
                        expIncOrigem.sUserID = AuthProvider.UserAntenticated.sID;
                        expIncOrigem.bTransferOrigem = true;
                        expenseIncomeBusiness.Insert(expIncOrigem);

                        ExpenseIncome expIncDestino = new ExpenseIncome();
                        expIncDestino.dData = transferModel.dData;
                        expIncDestino.dDataBase = transferModel.dData;
                        expIncDestino.dValor = transferModel.dValor;
                        expIncDestino.sCategoryID = transferModel.sCategoryID;
                        expIncDestino.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == transferModel.sCategoryID).CategoryType;
                        expIncDestino.sAccountID = transferModel.sAccountDestinyID;
                        expIncDestino.sDescricao = transferModel.sDescricao;
                        expIncDestino.sUserID = AuthProvider.UserAntenticated.sID;
                        expIncDestino.bTransferOrigem = false;
                        expenseIncomeBusiness.Insert(expIncDestino);

                        ExpenseIncomeReference expIncRef = new ExpenseIncomeReference();
                        expIncRef.sExpenseIncomeOriginID = expIncOrigem.sID;
                        expIncRef.sExpenseIncomeDestinyID = expIncDestino.sID;
                        expenseIncomeReferenceBusiness.Insert(expIncRef);

                        scope.Complete();
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Transferência Gerado com Sucesso!",
                            Titulo = "Sucesso",
                            Url = "/Home/PaginaInicial"
                        });
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        string sErro = string.Empty;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                sErro += string.Format(" Property: {0} Error: {1} |", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        scope.Dispose();
                        return Json(new
                        {
                            Sucesso = false,
                            Mensagem = sErro,
                            Titulo = "Erro"
                        });
                    }
                    catch (Exception ex)
                    {
                        string sErro = ex.Message;
                        if (ex.InnerException != null) sErro += " - " + ex.InnerException.Message;
                        scope.Dispose();
                        return Json(new
                        {
                            Sucesso = false,
                            Mensagem = sErro,
                            Titulo = "Erro"
                        });
                    }
                }
            }
            else
            {
                transferModel.listaCategorias = getComboCategorias(new List<CategoryType>() { CategoryType.Transferencia });
                transferModel.listaContas = getComboContas(false, true);
                return View(transferModel);
            }
        }
        //-Saldo por data----------------------------------------------------------------------------------------------------------------
        [Auth()]
        public ActionResult SaldoData()
        {
            SaldoDataModel saldoModel = new SaldoDataModel();
            saldoModel.dataLimite = DateTime.Now.AddDays(-60);
            saldoModel.listaContas = getComboContas(false, false);
            saldoModel.sContaID = saldoModel.listaContas[0].Value;
            return View(saldoModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaldoData(SaldoDataModel saldoModel)
        {
            saldoModel.listaContas = getComboContas(false, false);
            return View(saldoModel);
        }

        private PagedList.IPagedList<SaldoDataItemModel> gerarListaSaldoData(int pagina, DateTime dtLimite, string sContaID)
        {
            List<SaldoDataItemModel> retorno = new List<SaldoDataItemModel>();
            List<ExpenseIncome> lstExpInc = new List<ExpenseIncome>();
            List<Account> lstContas = new List<Account>();
            if (string.IsNullOrEmpty(sContaID))
            {
                lstExpInc = expenseIncomeBusiness.Get.Where(e => e.dData >= dtLimite && e.sUserID == AuthProvider.UserAntenticated.sID).ToList();
                lstContas = accountBusiness.Get.Where(a => a.StatusType == StatusType.Ativo && a.sUserID == AuthProvider.UserAntenticated.sID).ToList();
            }
            else
            {
                lstExpInc = expenseIncomeBusiness.Get.Where(e => e.dData >= dtLimite && e.sUserID == AuthProvider.UserAntenticated.sID && e.sAccountID == sContaID).ToList();
                lstContas = accountBusiness.Get.Where(a => a.StatusType == StatusType.Ativo && a.sUserID == AuthProvider.UserAntenticated.sID && a.sID == sContaID).ToList();
            }

            DateTime dDataAtual = DateTime.Today;
            foreach (Account account in lstContas)
            {
                double dSaldo = account.dSaldo;
                while (dDataAtual >= dtLimite)
                {            
                    List<ExpenseIncome> lstTemp = lstExpInc.Where(l => l.dData.Date == dDataAtual.Date && l.sAccountID == account.sID).ToList();
                    SaldoDataItemModel saldoItem = new SaldoDataItemModel();
                    string sDescricao = string.Empty;
                    saldoItem.dSaldo = dSaldo;
                    saldoItem.dData = dDataAtual;
                    saldoItem.dValorDia = 0;
                    foreach (ExpenseIncome expTemp in lstTemp)
                    {
                        if (!string.IsNullOrEmpty(sDescricao)) sDescricao += " / ";
                        sDescricao += expTemp.sDescricao;
                        if (expTemp.CategoryType == CategoryType.Receita || (expTemp.CategoryType == CategoryType.Transferencia && !expTemp.bTransferOrigem))
                            saldoItem.dValorDia -= expTemp.dValor;
                        else
                            saldoItem.dValorDia += expTemp.dValor;
                    }
                    dSaldo += saldoItem.dValorDia;
                    saldoItem.sContaDesc = sDescricao;
                    if (saldoItem.sContaDesc.Length > 120) saldoItem.sContaDesc = saldoItem.sContaDesc.Substring(0, 116) + " ...";
                    saldoItem.dValorDia *= -1;
                    if (saldoItem.dValorDia != 0) retorno.Add(saldoItem);
                    dDataAtual = dDataAtual.AddDays(-1);
                }
                dDataAtual = DateTime.Today;
            }
            retorno = retorno.OrderBy(l => l.dData).ToList();
            return retorno.ToPagedList(pagina, 17);
        }

        [HttpGet]
        public PartialViewResult ListaSaldoData(int pagina, string dtLimite, string sContaID)
        {
            if (string.IsNullOrEmpty(dtLimite)) return PartialView("_ListaSaldoData");
            DateTime dataL = new DateTime(int.Parse(dtLimite.Substring(6, 4)), int.Parse(dtLimite.Substring(3, 2)), int.Parse(dtLimite.Substring(0, 2)));
            return PartialView("_ListaSaldoData", gerarListaSaldoData(pagina, dataL, sContaID));
        }

        //-------------------------------------------------------------------------------------------------------------------------------
    }
}
