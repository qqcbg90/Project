using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KingspModel
{
    /// <summary>
    /// 自訂的HtmlHelper Extension
    /// </summary>
    public static class MyHtmlHelperExtenson
    {
        /// <summary>
        /// 產生可上傳檔案的 HttpPost Form
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="defaultRouteValues">預設的routeValues</param>
        /// <param name="routeValues">自訂的routeValues</param>
        /// <returns></returns>
        public static MvcForm BeginFileForm(this HtmlHelper helper, string actionName = null, string controllerName = null, Dictionary<string, string> defaultRouteValues = null, object routeValues = null)
        {
            var newRouteValues = new RouteValueDictionary(routeValues);
            if (defaultRouteValues != null)
            {
                foreach (KeyValuePair<string, string> item in defaultRouteValues)
                {
                    if (!newRouteValues.ContainsKey(item.Key))
                    {
                        newRouteValues.Add(item.Key, item.Value);
                    }
                }
            }
            var newHttpAttributes = new RouteValueDictionary();
            newHttpAttributes.Add("enctype", "multipart/form-data");
            return helper.BeginForm(actionName, controllerName, newRouteValues, FormMethod.Post, newHttpAttributes);
        }

        /// <summary>
        /// 產生可上傳檔案的 HttpPost Form
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="defaultRouteValues">預設的routeValues</param>
        /// <param name="routeValues">自訂的routeValues</param>
        /// <returns></returns>
        public static MvcForm BeginFileForm(this HtmlHelper helper, Dictionary<string, string> defaultRouteValues, object routeValues = null)
        {
            var newRouteValues = new RouteValueDictionary(routeValues);
            if (defaultRouteValues != null)
            {
                foreach (KeyValuePair<string, string> item in defaultRouteValues)
                {
                    if (!newRouteValues.ContainsKey(item.Key))
                    {
                        newRouteValues.Add(item.Key, item.Value);
                    }
                }
            }
            var newHttpAttributes = new RouteValueDictionary();
            newHttpAttributes.Add("enctype", "multipart/form-data");
            return helper.BeginForm(null, null, newRouteValues, FormMethod.Post, newHttpAttributes);
        }

        /// <summary>
        /// 同時產生EditorFor and ValidationMessageFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EditorValidationFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.EditorValidationFor(expression, null);
        }

        /// <summary>
        /// 同時產生EditorFor and ValidationMessageFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="additionalViewData"></param>
        /// <returns></returns>
        public static MvcHtmlString EditorValidationFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Object additionalViewData)
        {
            var str = html.EditorFor(expression, additionalViewData).ToString() + html.ValidationMessageFor(expression).ToString();
            return MvcHtmlString.Create(str);
        }

        /// <summary>
        /// 同時產生LabelFor and EditorFor and ValidationMessageFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelEditorValidationFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.LabelEditorValidationFor(expression, null);
        }

        /// <summary>
        /// 同時產生LabelFor and EditorFor and ValidationMessageFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="additionalViewData"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelEditorValidationFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Object additionalViewData)
        {
            var str = html.LabelFor(expression).ToString() + html.EditorFor(expression, additionalViewData).ToString() + html.ValidationMessageFor(expression).ToString();
            return MvcHtmlString.Create(str);
        }

        /// <summary>
        /// 同時產生 TextBoxFor and ValidationMessageFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxValidationFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Object htmlAttributes)
        {
            var str = html.TextBoxFor(expression, htmlAttributes).ToString() + html.ValidationMessageFor(expression).ToString();
            return MvcHtmlString.Create(str);
        }

        /// <summary>
        /// 自動加入預設的ViewBag屬性
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="actionName"></param>
        /// <param name="defaultRouteValues"></param>
        /// /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper helper, string actionName, Dictionary<string, string> defaultRouteValues, object routeValues = null, string controllerName = null)
        {
            var newRouteValues = new RouteValueDictionary(routeValues);
            if (defaultRouteValues != null)
            {
                foreach (KeyValuePair<string, string> item in defaultRouteValues)
                {
                    if (!newRouteValues.ContainsKey(item.Key))
                    {
                        newRouteValues.Add(item.Key, item.Value);
                    }
                }
            }
            return helper.Action(actionName, controllerName, newRouteValues);
        }

        /// <summary>
        /// Ajax 包含圖片的Link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="imageUrl"></param>
        /// <param name="altText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="ajaxOptions"></param>
        /// <returns></returns>
        public static string ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions);
            return link.ToHtmlString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Html 包含圖片的Link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="imageUrl"></param>
        /// <param name="altText"></param>
        /// <param name="actionName"></param>
        /// <param name="controller"></param>
        /// <param name="routeValues"></param>
        /// <param name="_imageClass"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionImageLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, string controller, object routeValues, string _imageClass = "", object htmlAttributes = null)
        {
            var image = new TagBuilder("img");
            image.MergeAttribute("src", imageUrl);
            image.MergeAttribute("alt", altText);
            image.MergeAttribute("title", altText);
            if (string.IsNullOrEmpty(_imageClass) == false) image.MergeAttribute("class", _imageClass);
            var link = helper.ActionLink("[replaceme]", actionName, controller, routeValues, htmlAttributes);
            return new MvcHtmlString(link.ToHtmlString().Replace("[replaceme]", image.ToString(TagRenderMode.SelfClosing)));
        }

		/// <summary>
		/// list model
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="html"></param>
		/// <param name="expression"></param>
		/// <param name="htmlFieldName"></param>
		/// <returns></returns>
		public static MvcHtmlString EditorForMany<TModel, TValue>(this HtmlHelper<TModel> html,Expression<Func<TModel, IEnumerable<TValue>>> expression,
		string htmlFieldName = null) where TModel : class
		{
			var items = expression.Compile()(html.ViewData.Model);
			var sb = new StringBuilder();
			var hasPrefix = false;

			if (String.IsNullOrEmpty(htmlFieldName))
			{
				var prefix = html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
				hasPrefix = !String.IsNullOrEmpty(prefix);
				htmlFieldName = (prefix.Length > 0 ? (prefix + ".") : String.Empty) + ExpressionHelper.GetExpressionText(expression);
			}

			if (items != null)
			{
				foreach (var item in items)
				{
					var dummy = new { Item = item };
					//var guid = Guid.NewGuid().ToString();
					var guid = Function.GetGuid();

					var memberExp = Expression.MakeMemberAccess(Expression.Constant(dummy), dummy.GetType().GetProperty("Item"));
					var singleItemExp = Expression.Lambda<Func<TModel, TValue>>(memberExp, expression.Parameters);

					sb.Append(String.Format(@"<input type=""hidden"" name=""{0}.Index"" value=""{1}"" />", htmlFieldName, guid));
					sb.Append(html.EditorFor(singleItemExp, null, String.Format("{0}[{1}]", hasPrefix ? ExpressionHelper.GetExpressionText(expression) : htmlFieldName, guid)));
				}
			}
			return new MvcHtmlString(sb.ToString());
		}

		/// <summary>
		/// 20200616 add by ting
		/// https://cpratt.co/html-editorfor-and-htmlattributes/
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
		{
			var concatKeys = new string[] { "class" };

			var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
			var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

			RouteValueDictionary htmlAttributes = (htmlAttributesDict != null) ? new RouteValueDictionary(htmlAttributesDict) : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
			RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null) ? new RouteValueDictionary(defaultHtmlAttributesDict) : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

			foreach (var item in htmlAttributes)
			{
				if (concatKeys.Contains(item.Key))
				{
					defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null) ? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value) : item.Value;
				}
				else
				{
					defaultHtmlAttributes[item.Key] = item.Value;
				}
			}
			return defaultHtmlAttributes;
		}
	}
}
