using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CadeODinheiro.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           
            routes.MapRoute(
                name: "Boleto",
                url: "{controller}/{action}/{sPessoaID}/{sKitID}",
                defaults: new { controller = "NovaConta", action = "VisualizarBoleto", sPessoaID = UrlParameter.Optional, sKitID = UrlParameter.Optional }
            );

            
        }
    }
}