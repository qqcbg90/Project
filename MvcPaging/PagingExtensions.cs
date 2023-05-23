using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace MvcPaging
{
    public static class PagingExtensions
    {
        #region AjaxHelper extensions

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, AjaxOptions ajaxOptions)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, null, ajaxOptions);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, AjaxOptions ajaxOptions)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, actionName, null, ajaxOptions);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, object values, AjaxOptions ajaxOptions)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values), ajaxOptions);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values, AjaxOptions ajaxOptions)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values), ajaxOptions);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions)
        {
            return Pager(ajaxHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary, ajaxOptions);
        }

        public static HtmlString Pager(this AjaxHelper ajaxHelper, int pageSize, int currentPage, int totalItemCount, string actionName, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            var pager = new Pager(ajaxHelper.ViewContext, pageSize, currentPage, totalItemCount, valuesDictionary, ajaxOptions);
            return pager.RenderHtml();
        }

        #endregion

        #region HtmlHelper extensions

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, null, pageName);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, null, pageName);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, object values, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values), pageName);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values), pageName);
        }

        //20131112 Add By danny
        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, int htmlType, string actionName, object values, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, htmlType, actionName, new RouteValueDictionary(values), pageName);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, string pageName = "page")
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary, pageName);
        }

        public static HtmlString Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, int htmlType, string actionName, RouteValueDictionary valuesDictionary, string pageName = "page")
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            var pager = new Pager(htmlHelper.ViewContext, pageSize, currentPage, totalItemCount, valuesDictionary, null, pageName);
            if (htmlType == 0)//後台
                return pager.RenderHtml();
            else//前台
                return pager.RenderHtml2();
        }

        #endregion

        #region IQueryable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}