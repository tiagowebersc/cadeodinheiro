using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Web.Infrastructure.Filters;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Validation;
using CadeODinheiro.Core.DTO;

namespace CadeODinheiro.Web.Controllers
{
    public class NotificationController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public INotificationBusiness notificationBusiness { get; set; }
        [Inject]
        public ICategoryBusiness categoryBusiness { get; set; }

        private List<SelectListItem> getComboCategorias()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            var categorias = categoryBusiness.Get.Where(e => e.sUserID == AuthProvider.UserAntenticated.sID).OrderBy(e => e.CategoryType).ThenBy(e => e.descricao).ToList();
            foreach (var cat in categorias)
            {
                if (cat.CategoryType == Core.Entity.Enum.CategoryType.Despesa || cat.CategoryType == Core.Entity.Enum.CategoryType.Receita)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = cat.sID;
                    if (cat.CategoryType == Core.Entity.Enum.CategoryType.Despesa)
                        item.Text = "Despesa - " + cat.descricao;
                    else
                        item.Text = "Receita - " + cat.descricao;
                    lista.Add(item);
                }
            }
            return lista;
        }

        [Auth()]
        public ActionResult Index()
        {
            return View(CriaListaNotificacoes(17, 1));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string navegaPagina, string alterarNotificacao, string excluirNotificacao)
        {
            if (!string.IsNullOrEmpty(navegaPagina) && navegaPagina.Equals("N")) return RedirectToAction("Cadastro", "Notification");
            if (!string.IsNullOrEmpty(alterarNotificacao)) return RedirectToAction("Cadastro", "Notification", new { notID = alterarNotificacao });
            if (!string.IsNullOrEmpty(excluirNotificacao)) return RedirectToAction("Excluir", "Notification", new { notID = excluirNotificacao });
            int pagina = 1;
            int.TryParse(navegaPagina, out pagina);
            if (pagina <= 0) pagina = 1;
            return View(CriaListaNotificacoes(17, pagina));
        }

        [Auth()]
        public ActionResult Cadastro(string notID)
        {
            NotificationModel notification = new NotificationModel();
            if (!string.IsNullOrEmpty(notID) && notificationBusiness.Get.Any(p => p.sID == notID))
            {
                Notification not = notificationBusiness.Get.FirstOrDefault(p => p.sID == notID);
                notification.dData = not.dData;
                notification.dDataFim = not.dDataFim;
                notification.dValor = not.dValor;
                notification.NotificationType = not.NotificationType;
                notification.sCategoryID = not.sCategoryID;
                notification.sDescricao = not.sDescricao;
                notification.sID = not.sID;
                notification.StatusType = not.StatusType;
            }
            else
            {
                notification.dData = DateTime.Now;
                notification.dDataFim = DateTime.Now.AddYears(1);
                notification.dValor = 0;
                notification.StatusType = Core.Entity.Enum.StatusType.Ativo;
            }
            notification.listaCategorias = getComboCategorias();
            
            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(NotificationModel notification)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (notificationBusiness.Get.Any(c => c.sID == notification.sID))
                    {
                        Notification notOld = notificationBusiness.Get.FirstOrDefault(c => c.sID == notification.sID);
                        notOld.dData = notification.dData;
                        notOld.dDataFim = notification.dDataFim;
                        notOld.dValor = notification.dValor;
                        notOld.NotificationType = notification.NotificationType;
                        notOld.sCategoryID = notification.sCategoryID;
                        notOld.sDescricao = notification.sDescricao;
                        notOld.StatusType = notification.StatusType;
                        notificationBusiness.Update(notOld);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Notificação alterada com sucesso!",
                            Titulo = "Sucesso",
                            Url = ""
                        });
                    }
                    else
                    {
                        Notification not = new Notification();
                        not.dData = notification.dData;
                        not.dDataFim = notification.dDataFim;
                        not.dValor = notification.dValor;
                        not.NotificationType = notification.NotificationType;
                        not.sCategoryID = notification.sCategoryID;
                        not.sDescricao = notification.sDescricao;
                        not.StatusType = notification.StatusType;
                        not.sUserID = AuthProvider.UserAntenticated.sID;
                        not.CategoryType = categoryBusiness.Get.FirstOrDefault(c => c.sID == not.sCategoryID).CategoryType;
                        //criar
                        notificationBusiness.Insert(not);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Notificação inserida com sucesso!",
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
                return View(notification);
            }
        }

        [Auth()]
        public ActionResult Excluir(string notID)
        {
            if (!string.IsNullOrEmpty(notID) && notificationBusiness.Get.Any(p => p.sID == notID))
            {
                Notification notification = notificationBusiness.Get.FirstOrDefault(p => p.sID == notID);

                return View(notification);
            }
            else
            {
                //não encontrou ou é vazio
                return RedirectToAction("Index", "Notification");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Excluir(Notification notification, string confirmaExclusao)
        {
            if (!string.IsNullOrEmpty(confirmaExclusao) && confirmaExclusao.Equals("T"))
            {
                try
                {
                    notificationBusiness.Delete(notification.sID);
                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Notificação excluída com sucesso!",
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
            return RedirectToAction("Index", "Notification");
        }

        private PagedList.IPagedList<Notification> CriaListaNotificacoes(int paginaTam, int paginaNum)
        {
            var categorias = notificationBusiness.Get.Where(p => p.sUserID == AuthProvider.UserAntenticated.sID).OrderBy(p => p.sDescricao).ToList();
            return categorias.ToPagedList(paginaNum, paginaTam);
        }

    }
}
