using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using Autofac.Integration.Mvc;
using KingspModel;
using StackExchange.Profiling;

namespace admin
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			//前端驗證自訂錯誤訊息 App_GlobalResources > Messages.resx
			ClientDataTypeModelValidatorProvider.ResourceClassKey = "Messages";
			DefaultModelBinder.ResourceClassKey = "Messages";
			//end
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			MvcHandler.DisableMvcResponseHeader = true;
			//for autofac
			var builder = new ContainerBuilder();
			SetupResolveRules(builder);
			builder.RegisterControllers(Assembly.GetExecutingAssembly());
			IContainer container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
			//end

			//remove aspx view engine
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());

			//AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
			//AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;

			StackExchange.Profiling.EntityFramework6.MiniProfilerEF6.Initialize();
			MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
		}

		protected void Application_End()
		{

		}

		void SetupResolveRules(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(Assembly.Load("KingspModel")).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
			{
				FormsIdentity id = (FormsIdentity)User.Identity;
				FormsAuthenticationTicket ticket = id.Ticket;
				string[] roles = ticket.UserData.Split(',');
				Context.User = new GenericPrincipal(id, roles);
			}
		}

		protected void Application_BeginRequest()
		{
			//for 語系設定
			//HttpCookie cLang = Request.Cookies[Function.COOKIE_LANG];
			//if (cLang != null)
			//{
			//	CultureInfo currentInfo = new CultureInfo(cLang.Value);
			//	Thread.CurrentThread.CurrentCulture = currentInfo;
			//	Thread.CurrentThread.CurrentUICulture = currentInfo;
			//}
			if (Request.IsLocal)
			{
				MiniProfiler.Start();
			}
		}

		protected void Application_EndRequest()
		{
			MiniProfiler.Stop();
		}

		protected void Session_Start()
		{
			//HttpContext.Current.Items[Function.WEB_VIEW_SESSION_START] = string.Empty;
			//Session["A"] = string.Empty;
		}

		protected void Session_End()
		{
			//Application.Lock();
			//total
			//if (Application[Function.WEB_VIEW_TOTAL_ARTICLE_ID] == null) Application[Function.WEB_VIEW_TOTAL_ARTICLE_ID] = 0;
			//Application.UnLock();
		}
	}
}
