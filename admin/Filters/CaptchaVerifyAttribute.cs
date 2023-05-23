using System.Web.Mvc;
using KingspModel;

namespace admin.Filters
{
	/// <summary>
	/// Captcha 驗證
	/// </summary>
	public sealed class CaptchaVerifyAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// 驗證碼ID
		/// </summary>
		public string CaptchaID { get; set; }
		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string ErrorMessage { get; set; }

		public CaptchaVerifyAttribute(string captchaID, string errorMessage)
		{
			this.CaptchaID = captchaID;
			this.ErrorMessage = errorMessage;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.ActionParameters.ContainsKey(CaptchaID))
			{
				string captcha = filterContext.ActionParameters[CaptchaID].ToMyString();
				if (!captcha.CheckStringValue(filterContext.HttpContext.Session[Function.SESSION_CAPTCHA_IMAGE].ToMyString()))
				{
					filterContext.Controller.ViewData.ModelState.AddModelError(CaptchaID, ErrorMessage);
				}
			}
			else
			{
				filterContext.Controller.ViewData.ModelState.AddModelError(CaptchaID, CaptchaID + " not found.");
			}
			base.OnActionExecuting(filterContext);
		}
	}
}