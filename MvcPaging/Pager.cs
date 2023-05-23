using KingspModel.Resources;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace MvcPaging
{
	public class Pager
    {
        private ViewContext viewContext;
        private readonly int pageSize;
        //private readonly int currentPage;
        private int currentPage;
        private readonly int totalItemCount;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
        private readonly AjaxOptions ajaxOptions;
		private string pageName;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount
			, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions, string pageName = "")
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
            this.ajaxOptions = ajaxOptions;
			this.pageName = pageName;
        }
        //後台
        public HtmlString RenderHtml()
        {
            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            //const int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            // Previous
            //sb.Append(currentPage > 1 ? GeneratePageLink("&lt;", currentPage - 1) : "<span class=\"disabled\">&lt;</span>");

            sb.Append(currentPage > 1 ? GeneratePageLink(Resource.PrevPage.ToString(), currentPage - 1) : string.Empty);
            // Next
            sb.Append(currentPage < pageCount ? GeneratePageLink(Resource.NextPage.ToString(), (currentPage + 1)) : string.Empty);

            /*取消中間的頁數選擇
            var start = 1;
            var end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }
            //sb.Append("<div class='ajax-links'>");
            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append(GeneratePageLink("2", 2));
                sb.Append("...");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i));
                }

            }
            if (end < (pageCount - 3))
            {
                sb.Append("...");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }
            中間頁數選擇 end */
            //sb.Append("</div>");
            // Next
            //sb.Append(currentPage < pageCount ? GeneratePageLink("&gt;", (currentPage + 1)) : "<span class=\"disabled\">&gt;</span>");
            //sb.Append(currentPage < pageCount ? GeneratePageLink("下一頁", (currentPage + 1)) : "<span class=\"disabled\">下一頁</span>");



            //總共 x 頁 / x 筆資料
            //sb.AppendFormat(Resource.Total_Page, pageCount, totalItemCount);
            sb.AppendFormat(Resource.AdminTotalPage.ToString(), pageCount, totalItemCount);

            //add select 
            var select = new StringBuilder();
            select.AppendFormat("<select name=\"{0}\" onchange=\"SelectGo($(this).val())\">", pageName);
            //select.Append("<select onchange=\"SelectGo()\" >");
            for (int i = 1; i <= pageCount; i++)
            {
                if (i == currentPage)
                    select.AppendFormat("<option selected='selected'>{0}</option>", i);
                else
                    select.AppendFormat("<option>{0}</option>", i);
            }
            select.Append("</select>");

            sb.Append(select.ToString());
            //end
            sb.Append(Resource.AdminListPage.ToString());

            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary) { { pageName, pageNumber } };
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<a");

            if (ajaxOptions != null)
                foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())
                    stringBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);

            stringBuilder.AppendFormat(" href=\"{0}\">{1}</a>", virtualPathForArea.VirtualPath, linkText);

            return stringBuilder.ToString();
        }

        //前台
        public HtmlString RenderHtml2()
        {
            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);

            var sb = new StringBuilder();
            //sb.AppendLine("<div>");
            // Previous
            //sb.Append(currentPage > 1 ? GeneratePageLink("&lt;", currentPage - 1) : "<span class=\"disabled\">&lt;</span>");

            sb.Append(currentPage > 1 ? GeneratePageLink2(Resource.PrevPage, currentPage - 1) : string.Empty);


            /*取消中間的頁數選擇
            var start = 1;
            var end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }
            //sb.Append("<div class='ajax-links'>");
            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append(GeneratePageLink("2", 2));
                sb.Append("...");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i));
                }

            }
            if (end < (pageCount - 3))
            {
                sb.Append("...");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }
            中間頁數選擇 end */
            //sb.Append("</div>");
            // Next
            //sb.Append(currentPage < pageCount ? GeneratePageLink("&gt;", (currentPage + 1)) : "<span class=\"disabled\">&gt;</span>");
            //sb.Append(currentPage < pageCount ? GeneratePageLink("下一頁", (currentPage + 1)) : "<span class=\"disabled\">下一頁</span>");

            //新的中間的頁數選擇
            var start = 1;
            var end = pageCount;
            const int nrOfPagesToDisplay = 5;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }
            //sb.Append("<p class='num'>");
            if (start > 3)
            {
                sb.Append(GeneratePageLink2("1", 1));
                sb.Append(GeneratePageLink2("2", 2));
                sb.Append("<span><p><a href=\"javascript:;\">...</a></p></span>");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat("<span class=\"current\"><p>{0}</p></span>", i);
                }
                else
                {
                    sb.Append(GeneratePageLink2(i.ToString(), i));
                }

            }
            if (end < (pageCount - 3))
            {
                sb.Append("<span><p><a href=\"javascript:;\">...</a></p></span>");
                sb.Append(GeneratePageLink2((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink2(pageCount.ToString(), pageCount));
            }
            //sb.Append("</p>");


            //add select 
            //var select = new StringBuilder();
            //select.AppendFormat("<select name=\"{0}\" onchange=\"SelectGo($(this).val())\" >", pageName);
            ////select.Append("<select onchange=\"SelectGo()\" >");
            //for (int i = 1; i <= pageCount; i++)
            //{
            //    if (i == currentPage)
            //        select.AppendFormat("<option selected='selected'>{0}</option>", i);
            //    else
            //        select.AppendFormat("<option>{0}</option>", i);
            //}
            //select.Append("</select>");
            //sb.AppendFormat(Resource.TotalPage, select.ToString(), pageCount);


            // Next
            sb.Append(currentPage < pageCount ? GeneratePageLink2(Resource.NextPage, (currentPage + 1)) : string.Empty);
            //sb.AppendLine("</div>");
            //總共 x 頁 / x 筆資料
            //sb.AppendFormat("總共 {0} 頁 / {1} 筆資料", pageCount, totalItemCount);

            //sb.AppendFormat(Resource.ListPage, totalItemCount, "<select name=\"listPage\" onchange=\"ChangeListPage($(this).val())\" ><option>15</option><option>50</option><option>100</option></select>");
            //sb.AppendFormat($"<p>第{currentPage}頁 / 共{pageCount}頁</p>");
            //end


            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink2(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary) { { pageName, pageNumber } };
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<span");

            if (ajaxOptions != null)
                foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())
                    stringBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);
            string _aCss = string.Empty;
            if (linkText.Contains("上一頁"))
            {
                _aCss = "btn_prev";
            }
            else if (linkText.Contains("下一頁"))
            {
                _aCss = "btn_next";
            }
            if (string.IsNullOrEmpty(_aCss))
            {
                stringBuilder.AppendFormat(" onclick=\"javascript:GoUrl('{0}');\"><p>{1}</p></span>", virtualPathForArea.VirtualPath, linkText);
            }
            else
            {
                stringBuilder.AppendFormat(" onclick=\"javascript:GoUrl('{0}');\" id=\"{1}\" ></span>", virtualPathForArea.VirtualPath, _aCss);
            }

            return stringBuilder.ToString();
        }
    }
}