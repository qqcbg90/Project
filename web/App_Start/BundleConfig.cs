using System.Web;
using System.Web.Optimization;

namespace web
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //base Jquery
            bundles.Add(new ScriptBundle("~/bundles/basejquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery-ui-{version}.js"
            ));

            //custom Jquery
            bundles.Add(new ScriptBundle("~/bundles/customjquery").Include(
                "~/Content/toastmessage/jquery.toastmessage.js",
                "~/Scripts/jqueryPlugin.js",
                "~/Scripts/custom.js"
            ));

            //base css
            bundles.Add(new StyleBundle("~/BaseCss/css").Include(
                "~/Content/reset.css",
                "~/Content/bootstrap.min.css",
                "~/Content/Site.css",
            "~/Content/toastmessage/css/jquery.toastmessage.css",
            "~/Content/jquery-ui-themes/lightness/jquery-ui.css"
            ));

            //base encss
            //bundles.Add(new StyleBundle("~/BaseCss/encss").Include(
            //"~/Content/toastmessage/css/jquery.toastmessage.css",
            //"~/Content/jquery-ui-themes/lightness/jquery-ui.css"
            //));
        }
    }
}
