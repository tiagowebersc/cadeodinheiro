using CadeODinheiro.Core.DTO;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Controllers
{
    public class LoginController : Controller
    {
        [Inject]
        public IAuthProvider AuthProvider { get; set; }
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AuthModel authModel)
        {
            if (ModelState.IsValid)
            {
                var msgError = string.Empty;
                if(!AuthProvider.Login(authModel, out msgError))
                {
                    TempData["msgLogin"] = msgError;
                    return View(authModel);
                }

                TempData["Apelido"] = authModel.Nome;
                return RedirectToAction("Index","Home");

            }
            else
            {
                TempData["msgLogin"] = "Usuário ou senha incorreto!";
            }            
            //return View(authModel);
            return RedirectToAction("", "Login", authModel);

            
        }

        public ActionResult EfetuarLogout()
        {
            AuthProvider.Logout();
            return RedirectToAction("");
        }
    }
}
