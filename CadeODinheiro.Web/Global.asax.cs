using CadeODinheiro.Web.App_Start;
using CadeODinheiro.Web.Infrastructure.DependencyResolver;
using CadeODinheiro.Web.Infrastructure.FilterProvider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CadeODinheiro.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            DependencyResolver.SetResolver(new NinjectDependencyResolver());
            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterProviders.Providers.Clear();
            FilterProviders.Providers.Add(new FilterProviderCustom());

            DataConfig dc = new DataConfig();
            dc.PopularBancoDados();
        }

        protected void Application_BeginRequest()
        {
            CultureInfo cInf = new CultureInfo("pt-BR", false);

            cInf.DateTimeFormat.DateSeparator = "/";
            cInf.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cInf.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss tt";

            System.Threading.Thread.CurrentThread.CurrentCulture = cInf;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cInf;
        }
    }
}