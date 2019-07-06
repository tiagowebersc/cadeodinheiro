using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Web.Infrastructure.Filters;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;

namespace CadeODinheiro.Web.Controllers
{
    public class AccountController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public IAccountBusiness accountBusiness { get; set; }

        [Auth()]
        public ActionResult Index()
        {
            return View(CriaListaContas(17, 1));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string navegaPagina, string alterarConta, string excluirConta)
        {
            if (!string.IsNullOrEmpty(navegaPagina) && navegaPagina.Equals("N")) return RedirectToAction("Cadastro", "Account");
            if (!string.IsNullOrEmpty(alterarConta)) return RedirectToAction("Cadastro", "Account", new { contID = alterarConta });
            if (!string.IsNullOrEmpty(excluirConta)) return RedirectToAction("Excluir", "Account", new { contID = excluirConta });
            int pagina = 1;
            int.TryParse(navegaPagina, out pagina);
            if (pagina <= 0) pagina = 1;
            return View(CriaListaContas(17, pagina));
        }

        [Auth()]
        public ActionResult Cadastro(string contID)
        {
            Account account = new Account();
            if (!string.IsNullOrEmpty(contID) && accountBusiness.Get.Any(p => p.sID == contID))
            {
                account = accountBusiness.Get.FirstOrDefault(p => p.sID == contID);
            }
            else
            {
                account.sUserID = AuthProvider.UserAntenticated.sID;
                account.iDiaFechamentoCC = 1;
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(Account account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (account.iDiaFechamentoCC < 1 || account.iDiaFechamentoCC > 31) throw new InvalidOperationException("Dia de fechamento incorreto!");
                    if (accountBusiness.Get.Any(c => c.sID == account.sID))
                    {
                        Account contaOld = accountBusiness.Get.FirstOrDefault(c => c.sID == account.sID);
                        contaOld.AccountType = account.AccountType;
                        contaOld.sDescricao = account.sDescricao;
                        contaOld.sNome = account.sNome;
                        contaOld.StatusType = account.StatusType;
                        contaOld.iDiaFechamentoCC = account.iDiaFechamentoCC;
                        //alterar
                        accountBusiness.Update(contaOld);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Conta alterada com sucesso!",
                            Titulo = "Sucesso",
                            Url = ""
                        });
                    }
                    else
                    {
                        if (accountBusiness.Get.Any(c => c.sNome == account.sNome && c.sUserID == AuthProvider.UserAntenticated.sID))
                        {
                            return Json(new
                            {
                                Sucesso = false,
                                Mensagem = "Conta já cadastrada!",
                                Titulo = "Erro"
                            });
                        }
                        account.sUserID = AuthProvider.UserAntenticated.sID;
                        //criar
                        accountBusiness.Insert(account);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Conta inserida com sucesso!",
                            Titulo = "Sucesso",
                            Url = ""
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
                return View(account);
            }
        }

        [Auth()]
        public ActionResult Excluir(string contID)
        {
            if (!string.IsNullOrEmpty(contID) && accountBusiness.Get.Any(p => p.sID == contID))
            {
                Account account = accountBusiness.Get.FirstOrDefault(p => p.sID == contID);

                return View(account);
            }
            else
            {
                //não encontrou ou é vazio
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Excluir(Account account, string confirmaExclusao)
        {
            if (!string.IsNullOrEmpty(confirmaExclusao) && confirmaExclusao.Equals("T"))
            {
                try
                {
                    accountBusiness.Delete(account.sID);
                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Conta excluída com sucesso!",
                        Titulo = "Sucesso",
                        Url = ""
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
            return RedirectToAction("Index", "Category");
        }

        private PagedList.IPagedList<Account> CriaListaContas(int paginaTam, int paginaNum)
        {
            var contas = accountBusiness.Get.Where(p => p.sUserID == AuthProvider.UserAntenticated.sID).OrderBy(p => p.sDescricao).ToList();
            return contas.ToPagedList(paginaNum, paginaTam);
        }

    }
}
