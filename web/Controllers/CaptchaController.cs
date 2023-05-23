using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using SRVTextToImage;
using System.Drawing.Imaging;
using System.Drawing;

namespace web.Controllers
{
    /// <summary>
    /// Captcha
    /// </summary>
    public class CaptchaController : BaseController
    {

        #region const property

        #endregion

        #region 建構式

        #endregion

        /// <summary>
        /// Session[Function.SESSION_CAPTCHA_IMAGE]
        /// </summary>
        /// <returns></returns>
         [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public FileResult GetCaptchaImage(int width = 200, int height = 75)
        {
            CaptchaRandomImage CI = new CaptchaRandomImage();
            //Session[Function.SESSION_CAPTCHA_IMAGE] = CI.GetRandomString(5);
            string _code = string.Empty;
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                _code += r.Next(10);
            }
            Session[Function.SESSION_CAPTCHA_IMAGE] = _code;
            CI.GenerateImage(Session[Function.SESSION_CAPTCHA_IMAGE].ToString(), width, height, Color.DarkGray, Color.White);
            MemoryStream stream = new MemoryStream();
            CI.Image.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(stream, "image/png");
        }

    }
}
