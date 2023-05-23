using KingspModel;
using KingspModel.DB;
using KingspModel.Enum;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace web.Controllers
{
	/// <summary>
	/// 不需權限驗證的皆可放在這
	/// </summary>
	public class JsonController : BaseController
    {
        #region 建構式

        #endregion

        #region 共用

        #region USER帳號管理
        /// <summary>
        /// 檢查USER_ID有無重覆
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <returns></returns>
        public JsonResult UserExist(string USER_ID)
        {
            bool isValidate = false;
            //if (Request.IsLocal)
            //{
            //    isValidate = !iUser.IsIDRepeat(USER_ID);
            //}
            isValidate = !iDB.IsIDRepeat<USER>(USER_ID);
            return Json(isValidate, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 檢查帳號有無重複
        /// </summary>
        /// <param name="USER_ID">Guid</param>
        /// <param name="CONTENT1">帳號(Email)</param>
        /// <returns></returns>
        public JsonResult UserEmailExist(string USER_ID, string CONTENT1)
        {
            bool isValidate = false;
            isValidate = iDB.CheckEmailRepeat<USER>(USER_ID, CONTENT1);
            return Json(isValidate, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 縣市切換鄉鎮

        public ActionResult CountrySearch(string cityType)
        {
            return PartialView("Search/_CountryResultPartial", Function.NodeList.Where(p => p.PARENT_ID.CheckStringValue(cityType) && p.ENABLE.IsEnable()).ToList());
        }

        public ActionResult GetCounty(string city, string selected)
        {
            if(!selected.IsNullOrEmpty())
            {
                string[] s = selected.Split('_');
                selected = s[1];
            }
            
            string strFormat = "<option value=\"{1}_{0}\"{3}>{1} {2}</option>";
            StringBuilder sb = new StringBuilder();
            if (!city.IsNullOrEmpty())
            {
                foreach (NODE county in Function.NodeList.Where(p => city.Equals(p.PARENT_ID)).OrderBy(p => p.ORDER))
                {
                    sb.AppendFormat(string.Format(strFormat, county.ID, county.CONTENT1, county.TITLE
                            , county.ID.Equals(selected) ? " selected" : ""));
                }
            }
            return Content(sb.ToString());
        }

        #endregion

        #region jQuery FileUpload 上傳

        /// <summary>
        /// 不在分類別 直接上傳至Upload
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="uploadType">檔案類型</param>
        /// <param name="c4"></param>
        /// <param name="c5"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUpload(string mid, AttachmentType? uploadType , string c1 = "", string c4 = "", string c5 = "",string fc1 ="")
        {
            foreach (string file in Request.Files)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase Filedata = Request.Files[i];
                    if (Filedata != null && Filedata.ContentLength > 0)
                    {
                        Image image = null;
                        try
                        {
                            image = Image.FromStream(Filedata.InputStream);
                        }
                        catch 
                        {
                            //用這種方式判斷是因為flash上傳檔案的ContentType 會被變更為 application/octet-stream
                        }
                        AttachmentType attType = AttachmentType.Image;

                        if (!uploadType.ToString().IsNullOrEmpty())
                        {
                            attType = (AttachmentType)uploadType;
                        }
                        else
                        {
                            attType = image != null ? AttachmentType.Image : AttachmentType.File;
                        }

                        ATTACHMENT att = new ATTACHMENT(Filedata.FileName);
                        att.ATT_TYPE = attType.ToIntValue();
                        if (attType == AttachmentType.Image && att.EXTENSION.IsNullOrEmpty())
                        {
                            att.GetRealExtension(image);
                        }
                        att.SetUpFileName();
                        att.MAIN_ID = mid;
                        att.CREATER = User.Identity.Name;
                        string _attC1 = Request["attC1"].ToMyString();
                        if (!_attC1.IsNullOrEmpty())
                        {
                            att.CONTENT1 = _attC1;
                        }
                        //備用
                        att.CONTENT4 = c4.IsNullOrEmpty() ? null : c4;
                        att.CONTENT5 = c5.IsNullOrEmpty() ? null : c5;

                        switch (attType)
                        {
                            case AttachmentType.File:
                                SaveAtt(Filedata, att.FILE_NAME);
                                break;
                            case AttachmentType.Image:
                                SavePicture(new WebImage(Filedata.InputStream), att);
                                break;
                        }

                        if (iDB.Add<ATTACHMENT>(att))
                            return Content(att.ID);
                    }
                }
            }
            return Content("檔案上傳失敗!!");

        }

        #endregion

        #endregion

        #region 專案用

        #region 搜尋DATA3

        public ActionResult GetC1Name(string id)
        {
            string _value = "旅宿不存在";
            if (!id.IsNullOrEmpty())
            {
                DATA3 d3 = Function.Data3List.FirstOrDefault(p => p.CONTENT2.Equals(id));
                if (d3 != null)
                {
                    _value = d3.CONTENT1;
                }
            }
            return Content(_value);
        }

        public ActionResult GetC2Name(string term)
        {
            string _value = "旅宿不存在";
            if (Function.Data3List.Any(p => p.CONTENT2.Contains(term)))
            {
                return Json(Function.Data3List.Where(p => p.CONTENT2.Contains(term))
                    .Select(p => new { label = p.CONTENT1, value = p.CONTENT2 }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { label = _value, value = _value }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion
    }
}
