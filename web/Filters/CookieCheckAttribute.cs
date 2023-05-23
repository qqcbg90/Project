using System.Web.Mvc;
using KingspModel;

namespace web.Filters
{
    /// <summary>
    /// 瀏覽人次相關
    /// </summary>
    public class CookieCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string _start = string.Empty, _a = string.Empty, _b = string.Empty;
            string _total = string.Empty, _today = string.Empty;
            string _total2 = string.Empty, _today2 = string.Empty;
            //不是中文才設定 都再加上(_語系)
            if (!Function.CultureName().Equals(CultureHelper.ZH_TW))
            {
                _start = $"{Function.WEB_VIEW_SESSION_START}_{Function.CultureName()}";
                _a = $"{Function.WEB_VIEW_A}_{Function.CultureName()}";
                _b = $"{Function.WEB_VIEW_B}_{Function.CultureName()}";
                _total = $"{Function.WEB_VIEW_TOTAL}_{Function.CultureName()}";
                _today = $"{Function.WEB_VIEW_TODAY}_{Function.CultureName()}";
            }
            else
            {
                _start = Function.WEB_VIEW_SESSION_START;
                _a = Function.WEB_VIEW_A;
                _b = Function.WEB_VIEW_B;
                _total = Function.WEB_VIEW_TOTAL;
                _today = Function.WEB_VIEW_TODAY;
                _total2 = Function.WEB_VIEW_TOTAL2;
                _today2 = Function.WEB_VIEW_TODAY2;
            }
            filterContext.HttpContext.Application.Lock();
            if (filterContext.HttpContext.Application[_total] == null)
                filterContext.HttpContext.Application[_total] = 1;
            filterContext.HttpContext.Application[_total] = filterContext.HttpContext.Application[_total].ToMyString().ToInt() + 1;
            if (filterContext.HttpContext.Application[_today] == null)
                filterContext.HttpContext.Application[_today] = 1;
            filterContext.HttpContext.Application[_today] = filterContext.HttpContext.Application[_today].ToMyString().ToInt() + 1;
            filterContext.HttpContext.Application.UnLock();


            //"Protect".CheckStringValue(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName)

            //if (filterContext.HttpContext.Items[_start] == null
            //    && filterContext.HttpContext.Session[_a] != null
            //    && filterContext.HttpContext.Session[_b] == null)
            //{
            //    filterContext.HttpContext.Application.Lock();
            //    if (filterContext.HttpContext.Application[_total] == null)
            //        filterContext.HttpContext.Application[_total] = 1;
            //    filterContext.HttpContext.Application[_total] = filterContext.HttpContext.Application[_total].ToMyString().ToInt() + 1;
            //    if (filterContext.HttpContext.Application[_today] == null)
            //        filterContext.HttpContext.Application[_today] = 1;
            //    filterContext.HttpContext.Application[_today] = filterContext.HttpContext.Application[_today].ToMyString().ToInt() + 1;
            //    filterContext.HttpContext.Application.UnLock();
            //    filterContext.HttpContext.Session[_b] = string.Empty;
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}