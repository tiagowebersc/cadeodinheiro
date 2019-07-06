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
    public class CategoryController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public ICategoryBusiness categoryBusiness { get; set; }

        [Auth()]
        public ActionResult Index()
        {
            return View(CriaListaCategoria(17, 1));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string navegaPagina, string alterarCategoria, string excluirCategoria)
        {
            if (!string.IsNullOrEmpty(navegaPagina) && navegaPagina.Equals("N")) return RedirectToAction("Cadastro", "Category");
            if (!string.IsNullOrEmpty(alterarCategoria)) return RedirectToAction("Cadastro", "Category", new { catID = alterarCategoria });
            if (!string.IsNullOrEmpty(excluirCategoria)) return RedirectToAction("Excluir", "Category", new { catID = excluirCategoria });
            int pagina = 1;
            int.TryParse(navegaPagina, out pagina);
            if (pagina <= 0) pagina = 1;
            return View(CriaListaCategoria(17, pagina));
        }

        [Auth()]
        public ActionResult Cadastro(string catID)
        {
            Category categoria = new Category();
            if (!string.IsNullOrEmpty(catID) && categoryBusiness.Get.Any(p => p.sID == catID))
            {
                categoria = categoryBusiness.Get.FirstOrDefault(p => p.sID == catID);
            }
            else
            {
                categoria.sUserID = AuthProvider.UserAntenticated.sID;                        
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(Category categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryBusiness.Get.Any(c => c.sID == categoria.sID))
                    {
                        Category catOld = categoryBusiness.Get.FirstOrDefault(c => c.sID == categoria.sID);
                        catOld.CategoryType = categoria.CategoryType;
                        catOld.descricao = categoria.descricao;
                        //alterar
                        categoryBusiness.Update(catOld);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Categoria alterada com sucesso!",
                            Titulo = "Sucesso",
                            Url = ""
                        });
                    }
                    else
                    {
                        if (categoryBusiness.Get.Any(c => c.descricao == categoria.descricao && c.sUserID == AuthProvider.UserAntenticated.sID && c.CategoryType == categoria.CategoryType))
                        {
                            return Json(new
                            {
                                Sucesso = false,
                                Mensagem = "Categoria já cadastrada!",
                                Titulo = "Erro"
                            });
                        }
                        //criar
                        categoryBusiness.Insert(categoria);
                        return Json(new
                        {
                            Sucesso = true,
                            Mensagem = "Categoria inserida com sucesso!",
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
                return View(categoria);
            }
        }

        [Auth()]
        public ActionResult Excluir(string catID)
        {
            if (!string.IsNullOrEmpty(catID) && categoryBusiness.Get.Any(p => p.sID == catID))
            {
                Category categoria = categoryBusiness.Get.FirstOrDefault(p => p.sID == catID);

                return View(categoria);
            }
            else
            {
                //não encontrou ou é vazio
                return RedirectToAction("Index", "Category");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Excluir(Category categoria, string confirmaExclusao)
        {
            if (!string.IsNullOrEmpty(confirmaExclusao) && confirmaExclusao.Equals("T"))
            {
                try
                {
                    categoryBusiness.Delete(categoria.sID);
                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Categoria excluída com sucesso!",
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

        private PagedList.IPagedList<Category> CriaListaCategoria(int paginaTam, int paginaNum)
        {
            var categorias = categoryBusiness.Get.Where(p => p.sUserID == AuthProvider.UserAntenticated.sID).OrderBy(p => p.descricao).ToList();
            return categorias.ToPagedList(paginaNum, paginaTam);
        }

    }
}
