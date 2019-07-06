using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.DTO;
using CadeODinheiro.Web.Infrastructure.Filters;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadeODinheiro.Core.Entity;

namespace CadeODinheiro.Web.Controllers
{
    public class MyDataController : Controller
    {
        //
        // GET: /MyData/
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        [Inject]
        public IUserBusiness userBusiness { get; set; }
        
        [Auth()]
        public ActionResult Index()
        {
            MyDataModel model = new MyDataModel();
            User user = userBusiness.Get.FirstOrDefault(u => u.sID == AuthProvider.UserAntenticated.sID);
            model.login = user.login;
            model.nome = user.nome;            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MyDataModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = userBusiness.Get.FirstOrDefault(u => u.sID == AuthProvider.UserAntenticated.sID);
                    user.nome = model.nome;
                    if (!string.IsNullOrEmpty(model.senha) || !string.IsNullOrEmpty(model.confirmarSenha))
                    {
                        if (model.senha != model.confirmarSenha) throw new InvalidOperationException("Senhas não conferem!");
                        if (model.senha.Length < 5) throw new InvalidOperationException("Senha deve ter no mínimo 5 caracteres!");
                        user.senha = model.senha;
                    }
                    userBusiness.Update(user);

                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Informações atualizadas!",
                        Titulo = "Sucesso"
                    });
                }
                catch (Exception ex)
                {
                    string sErro = ex.Message;
                    if (ex.InnerException != null) sErro += " - " + ex.InnerException.Message;
                    TempData["mensagemErroAlterarConta"] = sErro;
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
                return View(model);
            }
        }
    }
}
