using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using KingspModel;
using KingspModel.DB;
using KingspModel.Interface;
using KingspModel.Repository;
using web.Filters;
using KingspModel.Enum;
using System.IO;
using System.Web.Helpers;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace web.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    [HandleError]
    //[RequireHttps]
    [CookieCheck]
    public class BaseController : Controller
    {
        #region Interface

        /// <summary>
        /// IDB
        /// </summary>
        protected IDB iDB = new DBRepository();

        #endregion Interface

        #region const property

        protected const string APPLICATION_VND = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        /// <summary>
        /// 預設快取秒數 300
        /// </summary>
        protected const int DEFAULT_OUTPUTCACHE_DURATION = 300;

        /// <summary>
        /// 今天日期 to String ex:2021/01/01
        /// </summary>
        protected static readonly string DEFAULT_TODAY_STRING = DateTime.Today.ToDefaultString();

        #endregion const property

        #region property

        /// <summary>
        /// 列表分頁筆數
        /// </summary>
        protected object DefaultPage;

        /// <summary>
        /// 列表分頁筆數
        /// </summary>
        protected int ListPage;

        /// <summary>
        /// NODE_ID
        /// </summary>
        protected string NodeID;

        /// <summary>
        /// 目前是否英文語系
        /// </summary>
        protected bool IsEN { get; set; }

        /// <summary>
        /// 指定頁面的Title
        /// </summary>
        protected string PageTitle
        {
            get
            {
                return ViewBag.PageTitle;
            }
            set
            {
                ViewBag.PageTitle = value;
            }
        }

        #endregion property

        #region Const Property For ViewBag

        /// <summary>
        /// ViewBag.Keyword k
        /// </summary>
        protected const string VIEW_BAG_KEYWORD = "k";

        /// <summary>
        /// ViewBag.ListPage listPage
        /// </summary>
        protected const string VIEW_BAG_LISTPAGE = "listPage";

        /// <summary>
        /// ViewBag.Page page
        /// </summary>
        protected const string VIEW_BAG_PAGE = "page";

        #endregion Const Property For ViewBag

        #region Readonly Property For Webconfig

        /// <summary>
        /// 預設的每日分享後可投票數 (根據 webconfig 中 DefaultShareVote 設定)
        /// </summary>
        protected static readonly int DEFAULT_SHARE_VOTE = Function.GetConfigSetting("DefaultShareVote").ToInt();

        /// <summary>
        /// 預設的列表SIZE (根據 webconfig 中 DefaultPage 設定)
        /// </summary>
        //protected static readonly int DEFAULT_PAGE_SIZE = Function.GetConfigSetting("DefaultPage").ToInt();
        /// <summary>
        /// 預設的每日可投票數 (根據 webconfig 中 DefaultVote 設定)
        /// </summary>
        protected static readonly int DEFAULT_VOTE = Function.GetConfigSetting("DefaultVote").ToInt();

        /// <summary>
        /// 預設的投票結束日 (根據 webconfig 中 VoteEnd 設定)
        /// </summary>
        protected static readonly DateTime DEFAULT_VOTE_END = Function.GetConfigSetting("VoteEnd").ToDateTime();

        /// <summary>
        /// 預設的投票開始日 (根據 webconfig 中 VoteStart 設定)
        /// </summary>
        protected static readonly DateTime DEFAULT_VOTE_START = Function.GetConfigSetting("VoteStart").ToDateTime();

        #endregion Readonly Property For Webconfig

        /// <summary>
        /// Default 建構子
        /// </summary>
        public BaseController()
        {
            //PageTitle = PAGE_TITLE;
        }

        /// <summary>
        /// 設定語系
        /// </summary>
        /// <param name="culture"></param>
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            //清除OutputCache
            //Response.RemoveOutputCacheItem()
            //end
            culture = CultureHelper.GetImplementedCulture(culture);
            HttpCookie cultureCookie = new HttpCookie(Function.COOKIE_LANG);
            cultureCookie.HttpOnly = true;
            cultureCookie.Value = culture;
            cultureCookie.Expires = DateTime.Now.AddMonths(2);
            Response.Cookies.Add(cultureCookie);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return GoIndex();
        }

        public ActionResult UpdateNode()
        {
            SetNodeList();
            //SetSysUserList();
            SetData1List();
            //SetData2List();
            SetData3List();
            Msgbox_Toast("NODE更新成功！！");
            return GoIndex();
        }

        /// <summary>
        /// 判斷活動日期
        /// </summary>
        /// <returns></returns>
        protected bool CheckDateTime()
        {
            //return true;
            return (DEFAULT_VOTE_START <= DateTime.Now
                && DEFAULT_VOTE_END >= DateTime.Now);

            //return (Function.PROJECT_KEY[1].ToDateTime() <= DateTime.Now
            //&& Function.PROJECT_KEY[2].ToDateTime() >= DateTime.Now) ||
            //Request.UserHostAddress == "118.163.88.193" ||
            //Request.UserHostAddress == "192.168.0.196" ||
            //Request.UserHostAddress == "::1";
        }

        /// <summary>
        /// 導向預設的Error Page
        /// </summary>
        /// <returns></returns>
        protected ActionResult Error()
        {
            return RedirectToAction("Index", "Error");
        }

        /// <summary>
        /// 取得應用程式虛擬根路徑
        /// </summary>
        /// <returns></returns>
        protected string GetApplicationPath()
        {
            return Url.Content("~/");
        }

        /// <summary>
        /// 導向目前Controller下的Action Index
        /// </summary>
        /// <returns></returns>
        protected ActionResult GoIndex()
        {
            //return View("Error");
            return GoIndex(string.Empty);
        }

        /// <summary>
        /// 導向目前Controller下的Action Index
        /// </summary>
        /// <returns></returns>
        protected ActionResult GoIndex(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", new { id = id });
            }
        }

        /// <summary>
        /// 未知Action一律導回首頁
        /// </summary>
        /// <param name="actionName"></param>
        protected override void HandleUnknownAction(string actionName)
        {
            ResponseIndex();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            IsEN = CultureHelper.GetCurrentCulture() == CultureHelper.EN_US;
            //抓取瀏覽器的語言
            //string[] arr = Request.UserLanguages;

            //中英語系切換
            //string lang = requestContext.HttpContext.Request.Form["hidLang"];

            //if (!string.IsNullOrEmpty(lang))
            //{
            //    //Response.Cookies["_travelclick_lang"].Value = lang;
            //    //Response.Cookies["TIPN_lang"].Value = lang;
            //    CultureInfo currentInfo = new CultureInfo(lang);
            //    Thread.CurrentThread.CurrentCulture = currentInfo;
            //    Thread.CurrentThread.CurrentUICulture = currentInfo;
            //}

            ////初始化
            ////ViewBag.Lang = Request.Cookies["FCV_lang"].Value;

            //ViewBag.Lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(3); //TW
            //ViewBag.Hidden = false;
            ////LANGUAGE = (string)ViewBag.Lang;

            //初始化各種靜態資料
            if (Function.NodeList == null)
            {
                SetNodeList();
            }
            //if (Function.SysUserList == null)
            //{
            //    SetSysUserList();
            //}
            if (Function.Data1List == null)
            {
                SetData1List();
            }
            if (Function.Data3List == null)
            {
                SetData3List();
            }
            if (Function.DataRangeList == null)
            {
                Function.DataRangeList = new List<string>();
                DateTime _start = new DateTime(2022, 12, 5);
                for (int i = 0; i < 21; i++)//2022/12/05~2022/12/25 共21日
                {
                    DateTime _v = _start.AddDays(i);
                    string _vStr = _v.ToDefaultString();
                    Function.DataRangeList.Add(_vStr);
                }
            }
            //if (Function.Data2List == null)
            //{
            //    SetData2List();
            //}
            //end
            base.Initialize(requestContext);
        }

        /// <summary>
        /// 登出 或 重新來過時要清除的event
        /// </summary>
        protected void LogOffCleanEvent()
        {
            Session.Contents.RemoveAll();
            FormsAuthentication.SignOut();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResultBase;
            if (result == null)
            {
                // The controller action didn't return a view result
                // => no need to continue any further
                return;
            }
            //else if (!"118.163.88.193".Equals(Request.UserHostAddress)
            //    && !"192.168.0.197".Equals(Request.UserHostAddress)
            //    && !"192.168.0.90".Equals(Request.UserHostAddress))
            //{
            //    result.ViewName = "Comming";
            //}

            //var model = result.Model as MyViewModel;
            //if (model == null)
            //{
            //    // there's no model or the model was not of the expected type
            //    // => no need to continue any further
            //    return;
            //}

            // modify some property value
            //model.Foo = "bar";
        }

        /// <summary>
        /// 每個Action執行前(檢查權限)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //line瀏覽器需另開視窗
            //var isLine = Request.UserAgent.ToLower().Contains("line");
            //if (isLine && Request.HttpMethod == "GET" && string.IsNullOrEmpty(Request.QueryString["openExternalBrowser"]))
            //{
            //	if (string.IsNullOrEmpty(Request.Url.Query))
            //		Response.Redirect($"{Request.Url.AbsoluteUri}?openExternalBrowser=1");
            //	else
            //		Response.Redirect($"{Request.Url.AbsoluteUri}&openExternalBrowser=1");
            //	Response.End();
            //}

            if (!filterContext.IsChildAction && !Request.IsAjaxRequest())
            {
                //for預設列表分頁筆數
                if (filterContext.ActionParameters.TryGetValue(VIEW_BAG_LISTPAGE, out DefaultPage))
                {
                    ListPage = DefaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
                    ViewBag.ListPage = ListPage;
                }
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 檢查 returnUrl
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// 回Web首頁 呈現錯誤頁面
        /// </summary>
        protected void ResponseIndex()
        {
            Response.Redirect("~/");
            //this.RedirectToAction("Index", "Home");
            //this.View("Error").ExecuteResult(ControllerContext);
        }

        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="attFile">上傳的物件</param>
        /// <param name="fileName">檔案名稱</param>
        protected void SaveAtt(HttpPostedFileBase attFile, string fileName)
        {
            string uploadPath = SetUploadPath();
            attFile.SaveAs(uploadPath + fileName);
        }

        /// <summary>
        /// 處理原圖存檔與縮圖
        /// </summary>
        /// <param name="image">圖檔</param>
        /// <param name="pic">attachment model</param>
        protected void SavePicture(WebImage image, ATTACHMENT pic, int rotate = 0)
        {
            //image.RotateRight();
            //處理圖片轉向
            //if (rotate == 90)
            //{
            //    image.RotateRight();
            //}
            //else if (rotate == -90)
            //{
            //    image.RotateLeft();
            //}
            //else if (rotate == 180)
            //{
            //    image.RotateRight();
            //    image.RotateRight();
            //}
            string uploadPath = SetUploadPath();
            image.Save(uploadPath + pic.FILE_NAME);
            
            //防止webimage bug 圖片一律先以png格式另外儲存
            string _filePath = Function.GetRealityPath(Function.TEMP_UPLOAD_PATH) + pic.FILE_NAME;
            image.Save(_filePath, "png", false);

            WebImage imageOrg = new WebImage(_filePath);
            imageOrg.Save(uploadPath + "tmp" + pic.FILE_NAME, forceCorrectExtension: false);
            Function.CompressImage(uploadPath + "tmp" + pic.FILE_NAME, uploadPath + pic.FILE_NAME, size: 800);
            pic.DeleteFile(uploadPath + "tmp" + pic.FILE_NAME);

            //以下爲縮圖
            WebImage imageSmall = new WebImage(_filePath);
            WebImage imageMedium = new WebImage(_filePath);
            int maxWidth = Function.UPLOAD_PICTURES_SMALL_SIZE.Split(',')[0].ToInt();
            int maxHeight = Function.UPLOAD_PICTURES_SMALL_SIZE.Split(',')[1].ToInt();
            imageSmall = Function.SetImageSize(imageSmall, maxWidth, maxHeight);
            imageSmall.Save(uploadPath + "tmp" + pic.Small_PicName, forceCorrectExtension: false);
            Function.CompressImage(uploadPath + "tmp" + pic.Small_PicName, uploadPath + pic.Small_PicName);
            pic.DeleteFile(uploadPath + "tmp" + pic.Small_PicName);

            maxWidth = Function.UPLOAD_PICTURES_MEDIUM_SIZE.Split(',')[0].ToInt();
            maxHeight = Function.UPLOAD_PICTURES_MEDIUM_SIZE.Split(',')[1].ToInt();
            imageMedium = Function.SetImageSize(imageMedium, maxWidth, maxHeight);
            imageMedium.Save(uploadPath + "tmp" + pic.Medium_PicName, forceCorrectExtension: false);
            Function.CompressImage(uploadPath + "tmp" + pic.Medium_PicName, uploadPath + pic.Medium_PicName, size: 500);
            pic.DeleteFile(uploadPath + "tmp" + pic.Medium_PicName);
            //end
            //remove tmp png pic
            pic.DeleteFile(_filePath);
        }

        /// <summary>
        /// 轉換資料庫DATA1 to List
        /// </summary>
        protected void SetData1List()
        {
            Function.Data1List = iDB.GetAll<DATA1>().ToList();
            //Function.PhList = iDB.GetAll<PARAGRAPH>().ToList();
        }

        /// <summary>
        /// 轉換資料庫DATA2 to List
        /// </summary>
        protected void SetData2List()
        {
            Function.Data2List = iDB.GetAll<DATA2>().ToList();
        }

        /// <summary>
        /// 轉換資料庫DATA3 to List
        /// </summary>
        protected void SetData3List()
        {
            Function.Data3List = iDB.GetAll<DATA3>().ToList();
        }
        /// <summary>
		/// 取得報表相關的sample路徑
		/// </summary>
		/// <returns></returns>
		protected string GetRepotSamplePath()
        {
            string rsPath = Server.MapPath("~/Content/pOrder/");
            if (!Directory.Exists(rsPath))
            {
                Directory.CreateDirectory(rsPath);
            }
            return rsPath;
        }
        /// <summary>
		/// 取得報表相關的FileStream
		/// </summary>
		/// <param name="reportName">報表名稱</param>
		/// <returns></returns>
		protected FileStream GetReportFileStream(string reportName)
        {
            return new FileStream(GetRepotSamplePath() + reportName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        /// <summary>
        /// 設定FileUpload元件參數
        /// </summary>
        /// <param name="fileTypeExts">可選擇的檔案類型</param>
        /// <param name="type">上傳檔案類型</param>
        /// <param name="buttonText">按鈕文字</param>
        /// <param name="fileSizeLimit">檔案大小上限 單位= B, KB(預設), MB, or GB</param>
        /// <param name="queueSizeLimit">單次可上傳數量</param>
        /// <param name="atLeast">至少上傳 n 個</param>
        protected void SetFileUploadParamter(string fileTypeExts = Function.DEFAULT_FILEUPLOAD_EXT, string buttonText = "請選擇", string fileSizeLimit = "", int queueSizeLimit = 999, int atLeast = 0, AttachmentType uploadType = AttachmentType.File, string extraHTML = "", string fc1 = "")
        {
            //ViewBag.FileUploadFileTypeExts = fileTypeExts;
            ViewBag.FileUploadAcceptFiles = fileTypeExts;
            ViewBag.FileUploadButtonText = buttonText;
            ViewBag.FileUploadFileSizeLimit = fileSizeLimit.IsNullOrEmpty() ? Function.DEFAULT_ATTACHMENT_SIZE_LIMIT.ToMyString() : fileSizeLimit;
            ViewBag.FileUploadQueueSizeLimit = queueSizeLimit;
            ViewBag.FileUploadAtLeast = atLeast;
            ViewBag.FileUploadType = uploadType;
            ViewBag.ExtraHTML = extraHTML;
            ViewBag.fc1 = fc1;
        }

        /// <summary>
        /// 設定JavaScript Alert 訊息
        /// </summary>
        /// <param name="message">要跳出的訊息</param>
        protected void SetJSMessageBox(string message)
        {
            SetJSMessageBox(message, null);
        }

        /// <summary>
        /// 設定JavaScript Alert 訊息
        /// </summary>
        /// <param name="message">要跳出的訊息</param>
        /// <param name="url">外部url請加 http://</param>
        protected void SetJSMessageBox(string message, string url)
        {
            //TempData[Function.TEMPDATA_MESSAGE_KEY] = url.IsNullOrEmpty() ? Function.ShowMessage(message) : Function.ShowMessage(message, url);
            TempData[Function.TEMPDATA_MESSAGE_KEY] = url.IsNullOrEmpty() ? message : Function.ShowMessage(message, url);
        }

        /// <summary>
        /// 設定登入auth cookie
        /// </summary>
        /// <param name="user"></param>
        protected void SetLogOnAuthCookie(string user)
        {
            Session.Contents.RemoveAll();
            Session.Timeout = Function.SESSION_TIME_OUT;
            //Session[Function.PROJECT_KEY[5]] = Request.UserHostAddress;//存IP
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user, DateTime.Now, DateTime.Now.AddMinutes(Session.Timeout), false, string.Empty);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            this.Response.Cookies.Add(new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        /// <summary>
        /// 設定預設的 ModelState.AddModelError(string.Empty, Function.DEFAULT_ERROR);
        /// </summary>
        protected void SetModelStateError()
        {
            SetModelStateError(Function.DEFAULT_ERROR);
        }

        /// <summary>
        /// 設定預設的 ModelState.AddModelError(string.Empty, message);
        /// </summary>
        /// <param name="message">自訂訊息</param>
        protected void SetModelStateError(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        /// <summary>
        /// 轉換資料庫NODE to 記憶體 NODE List
        /// </summary>
        protected void SetNodeList()
        {
            Function.NodeList = iDB.GetAll<NODE>().ToList();
        }

        /// <summary>
        /// 轉換資料庫SYSUSER to List
        /// </summary>
        protected void SetSysUserList()
        {
            Function.SysUserList = iDB.GetAll<SYSUSER>().ToList();
        }

        /// <summary>
        /// 上傳前檢查目錄與產生路徑
        /// </summary>
        /// <param name="nid">功能id</param>
        /// <param name="type">上傳檔案類型 預設Image</param>
        /// <returns></returns>
        protected string SetUploadPath()
        {
            string uploadPath = Server.MapPath(Function.GetUploadPath());
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            return uploadPath;
        }

        #region Toast提示對話框系列

        /// <summary>
        /// 提示對話框(類型：自訂，位置：自訂)
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public void Msgbox_Toast(string Message, AlertMsgType type = AlertMsgType.Success, Position position = Position.Middle_center)
        {
            SetJSMessageBox(Msgbox.Toast(Message, type, position));
        }

        /// <summary>
        /// 提示對話框(全部自訂參數)
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="type">訊息類別：1.notice  2.warning 3.error 4.success</param>
        /// <param name="position">訊息位置：1.top-left  2.top-center 3.top-right 4.middle-left 5.middle-center 6.middle-right</param>
        /// <param name="nEffectDuration">動態效果持續時間  deafult:600</param>
        /// <param name="stayTime">停留時間  deafult:3000</param>
        /// <param name="sticky">是否要停留</param>
        /// <param name="closeText">關閉的訊息 預設為""</param>
        public void Msgbox_Toast(string message, AlertMsgType type, Position position, int nEffectDuration, int stayTime, bool sticky, string closeText)
        {
            SetJSMessageBox(Msgbox.Toast(message, type, position, nEffectDuration, stayTime, sticky, closeText));
        }

        /// <summary>
        /// 提示對話框(非跳轉頁面所使用，外層不會包&lt;script&gt;，要搭配Javascript())
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public string MsgboxThisPage(string Message, string id = "", AlertMsgType type = AlertMsgType.Success, Position position = Position.Middle_center)
        {
            return Msgbox.ToastNoJs(Message, type, position, id);
        }

        #endregion Toast提示對話框系列

        #region childAction

        /*[ChildActionOnly]
        [OutputCache(Duration = DEFAULT_OUTPUTCACHE_DURATION)]
        public ActionResult GetTopHotelData(bool type, string culture)
        {
            List<TopHotelModel> list = new List<TopHotelModel>();
            //第一筆存總數
            TopHotelModel total = new TopHotelModel();
            total.Totel = iHotelInfo.GetAll(type).Where(p => p.HotelStarLevel > 0).Count();
            list.Add(total);
            //end
            Random rnd = new Random();
            foreach (NODE n in Function.NodeList.Where(p => Function.NODE_ID_CITYINFO.CheckStringValue(p.PARENT_ID)).OrderBy(p => p.ORDER))
            {
                TopHotelModel _city = new TopHotelModel();
                _city.Title = n.TITLE;
                _city.HotelType = type ? 1 : 2;
                _city.Name = Function.GetCultureValueFromNODE(n);
                _city.Totel = iHotelInfo.GetAll(type).Where(p => p.HotelStarLevel > 0 && p.HotelCity.Equals(n.TITLE)).Count();
                _city.Image = Path.Combine(GetApplicationPath(), "images/city", n.TITLE, string.Format("0{0}.jpg", rnd.Next(1, n.CUSTOM5.ToInt())));
                list.Add(_city);
            }

            return PartialView(list);
        }*/

        #endregion childAction

        #region 建立viewbag參數

        /// <summary>
        /// 有值的話才會建立
        /// </summary>
        /// <param name="o"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        protected void CreateViewbag(int? o = null, DateTime? start = null, DateTime? end = null, int? page = null
            , string c3 = null, string k = null, string c5 = null)
        {
            if (o.HasValue)
            {
                ViewBag.o = o.ToMyString();
            }
            if (start.HasValue)
            {
                ViewBag.start = start.ToDefaultString();
            }
            if (end.HasValue)
            {
                ViewBag.end = end.ToDefaultString();
            }
            if (page.HasValue)
            {
                ViewBag.page = page;
            }
            ViewBag.c3 = c3;
            ViewBag.c5 = c5;
            ViewBag.k = k;
        }

        #endregion 建立viewbag參數
    }
}