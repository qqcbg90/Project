using Autofac;
using Autofac.Integration.Mvc;
using KingspModel;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
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
using KingspModel.Interface;
using KingspModel.Repository;
using KingspModel.DB;

namespace web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        #region 多語系計數器

        /// <summary>
        /// 計數器 全部 type 預設 tw
        /// </summary>
        string allCounterType = Function.WEB_VIEW_TOTAL_TYPE;
        /// <summary>
        /// 計數器 今日 type 預設 tw
        /// </summary>
        string todayCounterType = Function.WEB_VIEW_TOTAL_TYPE_TODAY;
        /// <summary>
        /// 計數器 全部 type2 預設 tw2
        /// </summary>
        string allCounterType2 = Function.WEB_VIEW_TOTAL_TYPE2;
        /// <summary>
        /// 計數器 今日 type2 預設 tw2
        /// </summary>
        string todayCounterType2 = Function.WEB_VIEW_TOTAL_TYPE_TODAY2;
        /// <summary>
        /// 計數器 webViewSessionStart 預設 tw
        /// </summary>
        string webViewSessionStart = Function.WEB_VIEW_SESSION_START;
        /// <summary>
        /// 計數器 webViewA 預設 tw
        /// </summary>
        string webViewA = Function.WEB_VIEW_A;
        /// <summary>
        /// 計數器 webViewTotal 預設 tw
        /// </summary>
        string webViewTotal = Function.WEB_VIEW_TOTAL;
        /// <summary>
        /// 計數器 webViewToday 預設 tw
        /// </summary>
        string webViewToday = Function.WEB_VIEW_TODAY;
        /// <summary>
        /// 計數器 webViewTotal2 預設 tw2
        /// </summary>
        string webViewTotal2 = Function.WEB_VIEW_TOTAL2;
        /// <summary>
        /// 計數器 webViewToday2 預設 tw2
        /// </summary>
        string webViewToday2 = Function.WEB_VIEW_TODAY2;

        #endregion

        protected void Application_Start()
        {
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

            //start miniprofilerEF6
            StackExchange.Profiling.EntityFramework6.MiniProfilerEF6.Initialize();
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();

            //啟動計數器
            //CounterStart();
            //CounterStart2();
        }

        protected void Application_End()
        {
            //儲存計數器
            //CounterEnd();
            //CounterEnd2();
        }

        void SetupResolveRules(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("KingspModel"))
                .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)User.Identity;

                        FormsAuthenticationTicket ticket = id.Ticket;

                        string[] roles = ticket.UserData.Split(',');

                        Context.User = new GenericPrincipal(id, roles);

                    }
                }
            }
        }

        protected void Application_BeginRequest()
        {
            //for 語系設定
            HttpCookie cLang = Request.Cookies[Function.COOKIE_LANG];
            if (cLang != null)
            {
                CultureInfo currentInfo = new CultureInfo(cLang.Value);
                Thread.CurrentThread.CurrentCulture = currentInfo;
                Thread.CurrentThread.CurrentUICulture = currentInfo;
            }
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
            SetUpCounterType();
            HttpContext.Current.Items[webViewSessionStart] = string.Empty;
            Session[webViewA] = string.Empty;
        }

        protected void Session_End()
        {
            SetUpCounterType();
            Application.Lock();
            //total
            if (Application[webViewTotal] == null) Application[webViewTotal] = 1;
            //if (Application[webViewTotal2] == null) Application[webViewTotal2] = 1;
            //today
            if (Application[webViewToday] == null) Application[webViewToday] = 1;
            //if (Application[webViewToday2] == null) Application[webViewToday2] = 1;
            Application.UnLock();
        }

        #region function

        /// <summary>
        /// 根據語系設定 counter type key
        /// </summary>
        void SetUpCounterType()
        {
            //不是中文才設定 都再加上(語系)
            if (!Function.CultureName().Equals(CultureHelper.ZH_TW))
            {
                allCounterType = $"{Function.WEB_VIEW_TOTAL_TYPE}{Function.CultureName()}";
                todayCounterType = $"{Function.WEB_VIEW_TOTAL_TYPE_TODAY}{Function.CultureName()}";
                webViewSessionStart = $"{Function.WEB_VIEW_SESSION_START}_{Function.CultureName()}";
                webViewA = $"{Function.WEB_VIEW_A}_{Function.CultureName()}";
                webViewTotal = $"{Function.WEB_VIEW_TOTAL}_{Function.CultureName()}";
                webViewToday = $"{Function.WEB_VIEW_TODAY}_{Function.CultureName()}";
            }
        }

        /// <summary>
        /// 瀏覽人次計算 開始
        /// </summary>
        void CounterStart()
        {
            //SaveLog();
            SetUpCounterType();
            IDB iDB = new DBRepository();
            COUNTER tw = iDB.GetAll<COUNTER>().FirstOrDefault(p => allCounterType.Equals(p.TYPE));
            if (tw == null)
            {
                tw = new COUNTER();
                tw.TYPE = allCounterType;
                tw.LOG_DATE = DateTime.Today;
                tw.COUNT = 1;
                iDB.Add<COUNTER>(tw);
            }
            COUNTER today = iDB.GetAll<COUNTER>()
                .FirstOrDefault(p => todayCounterType.Equals(p.TYPE)
                && DateTime.Today == p.LOG_DATE);
            if (today == null)
            {
                today = new COUNTER();
                today.TYPE = todayCounterType;
                today.LOG_DATE = DateTime.Today;
                today.COUNT = 1;
                iDB.Add<COUNTER>(today);
            }
            Application[webViewTotal] = tw.COUNT;
            Application[webViewToday] = today.COUNT;

        }
        /// <summary>
        /// 瀏覽人次計算 結束
        /// </summary>
        void CounterEnd()
        {
            //SaveLog("1");
            SetUpCounterType();
            IDB iDB = new DBRepository();
            int _totalCount = Application[webViewTotal].ToMyString().ToInt();
            int _todayCount = Application[webViewToday].ToMyString().ToInt();
            COUNTER tw = iDB.GetAll<COUNTER>().FirstOrDefault(p => allCounterType.Equals(p.TYPE));
            if (_totalCount > tw.COUNT)
            {
                tw.COUNT = _totalCount;
            }
            DateTime _today = DateTime.Today;
            if (DateTime.Now.Hour == 0)//零時要更新昨天的人數
            {
                _today = DateTime.Today.AddDays(-1);
            }
            COUNTER today = iDB.GetAll<COUNTER>()
                .FirstOrDefault(p => todayCounterType.Equals(p.TYPE)
                && _today == p.LOG_DATE);
            if (_todayCount > today.COUNT)
            {
                today.COUNT = _todayCount;
            }
            iDB.Save();
        }

        /// <summary>
        /// 瀏覽人次計算2 開始
        /// </summary>
        void CounterStart2()
        {
            //SaveLog();
            IDB iDB = new DBRepository();
            COUNTER tw = iDB.GetAll<COUNTER>().FirstOrDefault(p => allCounterType2.Equals(p.TYPE));
            if (tw == null)
            {
                tw = new COUNTER();
                tw.TYPE = allCounterType2;
                tw.LOG_DATE = DateTime.Today;
                tw.COUNT = 305;
                iDB.Add<COUNTER>(tw);
            }
            COUNTER today = iDB.GetAll<COUNTER>()
                .FirstOrDefault(p => todayCounterType2.Equals(p.TYPE)
                && DateTime.Today == p.LOG_DATE);
            if (today == null)
            {
                today = new COUNTER();
                today.TYPE = todayCounterType2;
                today.LOG_DATE = DateTime.Today;
                today.COUNT = 1;
                iDB.Add<COUNTER>(today);
            }
            Application[webViewTotal2] = tw.COUNT;
            Application[webViewToday2] = today.COUNT;

        }
        /// <summary>
        /// 瀏覽人次計算2 結束
        /// </summary>
        void CounterEnd2()
        {
            //SaveLog("1");
            IDB iDB = new DBRepository();
            int _totalCount = Application[webViewTotal2].ToMyString().ToInt();
            int _todayCount = Application[webViewToday2].ToMyString().ToInt();
            COUNTER tw = iDB.GetAll<COUNTER>().FirstOrDefault(p => allCounterType2.Equals(p.TYPE));
            if (_totalCount > tw.COUNT)
            {
                tw.COUNT = _totalCount;
            }
            DateTime _today = DateTime.Today;
            if (DateTime.Now.Hour == 0)//零時要更新昨天的人數
            {
                _today = DateTime.Today.AddDays(-1);
            }
            COUNTER today = iDB.GetAll<COUNTER>()
                .FirstOrDefault(p => todayCounterType2.Equals(p.TYPE)
                && _today == p.LOG_DATE);
            if (_todayCount > today.COUNT)
            {
                today.COUNT = _todayCount;
            }
            iDB.Save();
        }

        /// <summary>
        /// 儲存log
        /// </summary>
        /// <param name="t"></param>
        void SaveLog(string t="")
        {
            LogSystem.InitLogSystem();
            if (t.IsNullOrEmpty())//開始
            {
                LogSystem.WriteLine("CounterStart");
            }
            else
            {
                LogSystem.WriteLine("CounterEnd");
            }
            LogSystem.WriteLine($"{DateTime.Now.ToDefaultStringWithTime1()}");
            LogSystem.CloseUnderlayingStream();
        }


        #endregion
    }
}
