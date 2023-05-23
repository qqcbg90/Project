using System.Web.Optimization;

namespace admin
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
				"~/Scripts/custom.js",
				"~/Scripts/jqueryPlugin.js",
				"~/Content/toastmessage/jquery.toastmessage.js",
				"~/Content/ckeditor/ckeditor.js",
				"~/Content/ckeditor/styles.js",
				"~/Content/ckeditor/ckfinder/ckfinder.js",
				"~/Content/zTree/jquery.ztree.all-3.5.js",
				"~/Scripts/JQueryDatePickerTW.js",
				"~/Scripts/vue.js",
				"~/Scripts/components.js",
				"~/Scripts/jquery.fancybox.min.js"
			));

			//後台
			bundles.Add(new StyleBundle("~/BaseCss/css").Include(
				"~/Content/reset.css",
				"~/Content/style.css",
				"~/Content/zTree/zTreeStyle.css",
				"~/Content/font-awesome.css",
				"~/Content/jquery.fancybox.min.css"
			));

			//前台
			bundles.Add(new StyleBundle("~/BaseCss/css_web").Include(
				"~/Content/reset.css",
				"~/Content/style_web.css"
			));

			//活動報名
			bundles.Add(new StyleBundle("~/BaseCss/css_act").Include(
				"~/Content/reset.css",
				"~/Content/style_act.css"
			));

			//other css
			bundles.Add(new StyleBundle("~/OtherCss/css").Include(
				"~/Content/font-awesome.css",
				"~/Content/jquery-ui-themes/lightness/jquery-ui.css",
				"~/Content/toastmessage/css/jquery.toastmessage.css"
			));
			
			#region amCharts

			bundles.Add(new ScriptBundle("~/amCharts/js").Include( //是名稱非真正路徑 但要加入~/
				"~/Content/amCharts/amcharts.js",
				"~/Content/amCharts/pie.js",
				"~/Content/amCharts/export.js",
				"~/Content/amCharts/light.js",
				"~/Content/amCharts/serial.js"
			));

			#endregion
		}
	}
}
