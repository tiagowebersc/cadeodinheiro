using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Infrastructure.Filters
{
    public class AuthAttribute : AuthorizeAttribute
    {
        private string msgErro;

        [Inject]
        public IAuthProvider authProvider { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!authProvider.Authenticate)
            {
                msgErro = "Usuário não está autenticado";
                return false;
            }
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData["msgLogin"] = msgErro;
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}