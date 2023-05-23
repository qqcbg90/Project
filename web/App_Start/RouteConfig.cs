using System.Web.Mvc;
using System.Web.Routing;

namespace web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Captcha",
				url: "Captcha/{action}/{id}",
				defaults: new { controller = "Captcha", action = "Index", id = UrlParameter.Optional },
				constraints: new { controller = "Captcha" }
			);

			routes.MapRoute(
				name: "Json",
				url: "Json/{action}/{id}",
				defaults: new { controller = "Json", action = "Index", id = UrlParameter.Optional },
				constraints: new { controller = "Json" }
			);

            //routes.MapRoute(
            //	name: "En",
            //	url: "EN/{action}/{id}",
            //	defaults: new { controller = "EN", action = "Index", id = UrlParameter.Optional },
            //	constraints: new { controller = "EN" }
            //);

            routes.MapRoute(
				name: "Default",
				//url: "{controller}/{action}/{id}",
				url: "{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
