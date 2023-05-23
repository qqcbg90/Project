using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using KingspModel;
using SRVTextToImage;

namespace admin.Controllers
{
	[AllowAnonymous]
	public class CaptchaController : BaseController
	{
		/// <summary>
		/// Session[Function.SESSION_CAPTCHA_IMAGE]
		/// </summary>
		/// <returns></returns>
		[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
		public FileResult GetCaptchaImage(int width = 200, int height = 64)
		{
			CaptchaRandomImage CI = new CaptchaRandomImage();

			//英數混合
			//Session[Function.SESSION_CAPTCHA_IMAGE] = CI.GetRandomString(5); 

			//純數字
			Random r = new Random();
			Session[Function.SESSION_CAPTCHA_IMAGE] = r.Next(10000, 100000);

			CI.GenerateImage(Session[Function.SESSION_CAPTCHA_IMAGE].ToString(), width, height, Color.DarkGray, Color.White);
			MemoryStream stream = new MemoryStream();
			CI.Image.Save(stream, ImageFormat.Png);
			stream.Seek(0, SeekOrigin.Begin);
			return new FileStreamResult(stream, "image/png");
		}
	}
}
