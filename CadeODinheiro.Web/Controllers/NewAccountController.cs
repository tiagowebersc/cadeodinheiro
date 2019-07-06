using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.DTO;
using CadeODinheiro.Core.Entity;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Controllers
{
    public class NewAccountController : Controller
    {
        //
        // GET: /NewAccount/
        [Inject]
        public IUserBusiness userBusiness { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MyDataModel model)
        {
            try
            {
                model.login = model.login.Trim().ToUpper();
                if (string.IsNullOrEmpty(model.login)) throw new InvalidOperationException("Login deve ser informado!");
                if (string.IsNullOrEmpty(model.nome)) throw new InvalidOperationException("Nome deve ser informado!");
                if (string.IsNullOrEmpty(model.senha)) throw new InvalidOperationException("Senhas deve ser informada!");
                if (model.senha != model.confirmarSenha) throw new InvalidOperationException("Senhas não conferem!");
                if (model.senha.Length < 5) throw new InvalidOperationException("Senha deve ter no mínimo 5 caracteres!");
                    
                if (ModelState.IsValid)
                {
                    if (userBusiness.Get.Any(u => u.login == model.login)) throw new InvalidOperationException("Login já cadastrado!");
                    User user = new User();
                    user.login = model.login;
                    user.nome = model.nome;
                    user.senha = model.senha;
                    userBusiness.Insert(user);
                    return Json(new
                    {
                        Sucesso = true,
                        Url = Url.Action("Index", "Login"),
                        Mensagem = "Conta criada!",
                        Titulo = "Sucesso"
                    });

                }
                return View(model);
            }
            catch (Exception ex)
            {
                string sErro;
                sErro = ex.Message;
                if (ex.InnerException != null) sErro += " - " + ex.InnerException.Message;

                return Json(new
                {
                    Sucesso = false,
                    Url = "",
                    Mensagem = sErro,
                    Titulo = "Erro"
                });
            }
        }

    }
}
