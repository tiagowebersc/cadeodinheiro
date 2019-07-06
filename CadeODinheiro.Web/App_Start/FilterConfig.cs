using CadeODinheiro.Web.Infrastructure.Filters;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogCustomAttribute());
        }
    }
}