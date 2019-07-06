using System.Web;
using System.Web.Optimization;

namespace CadeODinheiro.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.PrintArea.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            //  ***** SmartAdmin *****
            
            // CSS
            bundles.Add(new StyleBundle("~/Content/themes/smartadmin").Include(
                "~/Content/themes/bootstrap.css",
                "~/Content/themes/font-awesome.css",
                "~/Content/themes/smartadmin*"));

            bundles.Add(new StyleBundle("~/Content/CadeODinheiro").Include("~/Content/CadeODinheiro_css/style.css", "~/Content/datepicker.css"));
            bundles.Add(new StyleBundle("~/Content/boleto").Include("~/Content/Boleto.css"));
            
            //// JS
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").IncludeDirectory("~/Scripts/smartadmin/bootstrap", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymask").IncludeDirectory("~/Scripts/smartadmin/masked-input", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerytouch").IncludeDirectory("~/Scripts/smartadmin/jquery-touch", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/pace").IncludeDirectory("~/Scripts/smartadmin/pace", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/notification").IncludeDirectory("~/Scripts/smartadmin/notification", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/smartwidgets").IncludeDirectory("~/Scripts/smartadmin/smartwidgets", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include("~/Scripts/smartadmin/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include("~/Scripts/custom.js", "~/Scripts/smartadmin/select2/select2*", "~/Scripts/smartadmin/msie-fix/jquery*"));

            //System.Web.Optimization.BundleTable.EnableOptimizations = false;
        }
    }
}

   // <script src="~/Scripts/smartadmin/plugin/easy-pie-chart/jquery.easy-pie-chart.min.js"></script>
   // <script src="~/Scripts/smartadmin/plugin/sparkline/jquery.sparkline.min.js"></script>
   // <script src="~/Scripts/smartadmin/plugin/bootstrap-slider/bootstrap-slider.min.js"></script>
   // <script src="~/Scripts/smartadmin/plugin/smartclick/smartclick.js"></script>