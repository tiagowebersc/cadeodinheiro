using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Infrastructure.Helpers
{
    public static class TBOSHelperFactory
    {
        public static TBOSHelpers TBOS(this HtmlHelper helper)
        {
            return new TBOSHelpers(helper);
        }
    }
}