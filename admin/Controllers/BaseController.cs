using admin.Filters;
using CKFinder.Settings;
using Ionic.Zip;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using KingspModel.Interface;
using KingspModel.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;


namespace admin.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    //[Authorize]
    [HandleError]
    public class BaseController : Controller
    {
        #region protected Interface
        /// <summary>
        /// iDB 直接在BaseController new
        /// </summary>
        protected IDB iDB = new DBRepository();
        #endregion

        #region Base Const Property
        /// <summary>
        /// 預設功能NODE root id = admin_function
        /// </summary>
        protected const string FUNCTION_ROOT_NODE_ID = "admin_function";
        /// <summary>
        /// 動作紀錄LOG 刪除
        /// </summary>
        protected const string ACTIONLOG_DISPLAY_NAME_DELETE = "刪除";
        /// <summary>
        /// 動作紀錄LOG 編輯
        /// </summary>
        protected const string ACTIONLOG_DISPLAY_NAME_EDIT = "編輯";
        #endregion

        #region Project Const Property

        #endregion

        #region Const Property For ViewBag
        /// <summary>
        /// ViewBag.NodeID nid
        /// </summary>
        protected const string VIEW_BAG_NODEID = "nid";
        /// <summary>
        /// ViewBag.defaultPage defaultPage
        /// </summary>
        protected const string VIEW_BAG_DEFAULTPAGE = "defaultPage";

        /// <summary>
        /// ViewBag.Page page
        /// </summary>
        protected const string VIEW_BAG_PAGE = "page";
        /// <summary>
        /// ViewBag.Keyword k
        /// </summary>
        protected const string VIEW_BAG_KEYWORD = "k";

        /// <summary>
        /// ViewBag.Page page
        /// </summary>
        protected const string VIEW_BAG_PAGE_X = "pageX";
        /// <summary>
        /// ViewBag.Keyword k
        /// </summary>
        protected const string VIEW_BAG_KEYWORD_X = "kX";
        #endregion

        #region Readonly Property From Webconfig
        /// <summary>
        /// 預設的 Root Http Url ex:http://localhost:3291/ (根據 webconfig 中 PageTitle 設定)
        /// </summary>
        //protected static readonly string DEFAULT_ROOT_HTTP = Function.GetConfigSetting("RootHttp");
        #endregion

        #region Readonly Property
        /// <summary>
        /// 不受權限控制的 Controller 集合 Home,Node,Admin,Json,Error,_Sample,Captcha
        /// </summary>
        protected static readonly string[] CONTROLLER_LIST = new string[] { "Home", "Node", "Admin", "Json", "Error", "_Sample", "Captcha" };
        /// <summary>
        /// 不受權限控制的 Action 集合 LogOn,LogOff,LeftMenu,SetCulture
        /// </summary>
        protected static readonly string[] ACTION_LIST = new string[] { "LogOn", "LogOff", "LeftMenu", "SetCulture", "PreviewNewsletter", "AddCroppieAtt", "Exchange" };
        #endregion

        #region protected Property
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
        /// <summary>
        /// 新增或修改 有無成功
        /// </summary>
        protected bool IsSuccessful;
        /// <summary>
        /// 判斷是新增或修改
        /// </summary>
        protected bool IsAdd;
        /// <summary>
        /// 列表分頁筆數
        /// </summary>
        protected object DefaultPage;
        /// <summary>
        /// 功能權限 NodeID
        /// </summary>
        protected string NodeID;
        /// <summary>
        /// 忽略功能權限 NodeID 測試用
        /// </summary>
        protected bool IsContinue;
        /// <summary>
        /// AlertMsg
        /// </summary>
        protected string AlertMsg;
        #endregion

        #region protected Property For Project

        #endregion

        #region Protected Function
        /// <summary>
        /// 設定登入auth cookie
        /// </summary>
        /// <param name="user"></param>
        protected void SetLogOnAuthCookie(string user)
        {
            Session.Contents.RemoveAll();
            Session.Timeout = Function.SESSION_TIME_OUT;
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user, DateTime.Now, DateTime.Now.AddMinutes(Function.SESSION_TIME_OUT), false, string.Empty);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.Secure = Request.IsSecureConnection;
            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 登出 或 重新來過時要清除的event
        /// </summary>
        protected void LogOffCleanEvent()
        {
            Session.Contents.RemoveAll();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// 取得應用程式絕對路徑
        /// </summary>
        /// <returns></returns>
        protected string GetApplicationPath()
        {
            return Url.Content("~/");
        }

        /// <summary>
        /// 設定 ViewBag.ContentTitle 1~3
        /// <para>1 同時會設定 PageTitle </para>
        /// </summary>
        /// <param name="title"></param>
        protected void SetContentTitle(string title, int location = 1)
        {
            switch (location)
            {
                case 2:
                    ViewBag.ContentTitle2 = title;
                    break;
                case 3:
                    ViewBag.ContentTitle3 = title;
                    break;
                default:
                    ViewBag.ContentTitle = title;
                    PageTitle = title;
                    break;
            }
        }

        /// <summary>
        /// 設定 ViewBag.IsEdit
        /// </summary>
        /// <param name="isEdit">是否有編輯權限</param>
        protected void SetIsEdit(bool isEdit)
        {
            ViewBag.IsEdit = isEdit;
        }

        /// <summary>
        /// 取得Session key = key 的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetSessionStringValue(string key)
        {
            return !key.IsNullOrEmpty() && Session[key] != null ? Session[key].ToMyString() : string.Empty;
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
            string ERRORs = string.Join("; ", ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(p => p.Key + "：" + string.Join("、", p.Value.Errors.SelectMany(e => e.ErrorMessage).ToArray())));
            ModelState.AddModelError(string.Empty, message + " " + ERRORs);
        }
        /// <summary>
        /// 產生非預設的參數傳遞 所用的方式
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> CreateDict(string id = "", string did = "", string c1 = "", string c2 = "", string c3 = "", string pid = ""
            , string k = "", string k2 = "", string dt = "", string dType = "", DateTime? start = null, DateTime? end = null, string _start = "", string _end = "", string y = "", string m = "", string c = "", string c9 = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (!pid.IsNullOrEmpty())
            {
                dict.Add(nameof(pid), pid);
            }

            if (!id.IsNullOrEmpty())
            {
                dict.Add(nameof(id), id);
            }
            if (!did.IsNullOrEmpty())
            {
                dict.Add(nameof(did), did);
            }
            if (!c1.IsNullOrEmpty())
            {
                dict.Add(nameof(c1), c1);
            }
            if (!c2.IsNullOrEmpty())
            {
                dict.Add(nameof(c2), c2);
            }
            if (!c3.IsNullOrEmpty())
            {
                dict.Add(nameof(c3), c3);
            }
            if (!k.IsNullOrEmpty())
            {
                dict.Add(nameof(k), k);
            }
            if (!k2.IsNullOrEmpty())
            {
                dict.Add(nameof(k2), k2);
            }
            if (!dt.IsNullOrEmpty())
            {
                dict.Add(nameof(dt), dt);
            }
            if (!dType.IsNullOrEmpty())
            {
                dict.Add(nameof(dType), dType);
            }

            if (start.HasValue)
            {
                dict.Add(nameof(start), start.ToDefaultString());
            }
            if (end.HasValue)
            {
                dict.Add(nameof(end), end.ToDefaultString());
            }

            if (!_start.IsNullOrEmpty())
            {
                dict.Add(nameof(start), _start);
            }
            if (!_end.IsNullOrEmpty())
            {
                dict.Add(nameof(end), _end);
            }
            if (!y.IsNullOrEmpty())
            {
                dict.Add(nameof(y), y);
            }
            if (!m.IsNullOrEmpty())
            {
                dict.Add(nameof(m), m);
            }

            if (!c.IsNullOrEmpty())
            {
                dict.Add(nameof(c), c);
            }

            if (!c9.IsNullOrEmpty())
            {
                dict.Add(nameof(c9), c9);
            }
            return dict;
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

        /// <summary>
        /// 處理原圖存檔與縮圖
        /// </summary>
        /// <param name="image">圖檔</param>
        /// <param name="pic">attachment model</param>
        protected void SavePicture(WebImage image, ATTACHMENT pic)
        {
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

        //#region 無損壓縮圖片 2021-12-09 add ting
        ///// <summary>
        ///// 無損壓縮圖片
        ///// https://www.zendei.com/article/26681.html
        ///// https://blog.csdn.net/qq_16542775/article/details/51792149
        ///// </summary>
        ///// <param name="sFile">原圖片地址</param>
        ///// <param name="dFile">壓縮後保存圖片地址</param>
        ///// <param name="flag">壓縮質量（數字越小壓縮率越高）1-100</param>
        ///// <param name="size">壓縮後圖片的最大大小</param>
        ///// <param name="sfsc">是否是第一次調用</param>
        ///// <returns></returns>
        //public static bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true)
        //{
        //    using (Image iSource = Image.FromFile(sFile))
        //    {
        //        ImageFormat tFormat = iSource.RawFormat;

        //        //如果是第一次調用，原始圖像的大小小於要壓縮的大小，則直接複製文件，並且返回true
        //        FileInfo firstFileInfo = new FileInfo(sFile);
        //        if (sfsc && firstFileInfo.Length < size * 1024)
        //        {
        //            firstFileInfo.CopyTo(dFile, true);
        //            return true;
        //        }

        //        //int dHeight = iSource.Height / 2;
        //        //int dWidth = iSource.Width / 2;
        //        int dHeight = iSource.Height;
        //        int dWidth = iSource.Width;
        //        int sW = 0, sH = 0;

        //        //按比例縮放
        //        Size tem_size = new Size(iSource.Width, iSource.Height);
        //        if (tem_size.Width > dHeight || tem_size.Width > dWidth)
        //        {
        //            if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
        //            {
        //                sW = dWidth;
        //                sH = (dWidth * tem_size.Height) / tem_size.Width;
        //            }
        //            else
        //            {
        //                sH = dHeight;
        //                sW = (tem_size.Width * dHeight) / tem_size.Height;
        //            }
        //        }
        //        else
        //        {
        //            sW = tem_size.Width;
        //            sH = tem_size.Height;
        //        }

        //        using (Bitmap ob = new Bitmap(dWidth, dHeight))
        //        {
        //            using (Graphics g = Graphics.FromImage(ob))
        //            {
        //                g.Clear(Color.WhiteSmoke);
        //                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //                g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
        //            }

        //            //以下代碼為保存圖片時，設置壓縮質量
        //            EncoderParameters ep = new EncoderParameters();
        //            long[] qy = new long[1];
        //            qy[0] = flag;//設置壓縮的比例1-100
        //            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
        //            ep.Param[0] = eParam;

        //            try
        //            {
        //                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
        //                ImageCodecInfo jpegICIinfo = null;
        //                for (int x = 0; x < arrayICI.Length; x++)
        //                {
        //                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
        //                    {
        //                        jpegICIinfo = arrayICI[x];
        //                        break;
        //                    }
        //                }
        //                if (jpegICIinfo != null)
        //                {
        //                    ob.Save(dFile, jpegICIinfo, ep); //dFile是壓縮後的新路徑
        //                    FileInfo fi = new FileInfo(dFile);
        //                    if (fi.Length > 1024 * size)
        //                    {
        //                        flag = flag - 10;
        //                        CompressImage(sFile, dFile, flag, size, false);
        //                    }
        //                }
        //                else
        //                {
        //                    ob.Save(dFile, tFormat);
        //                }
        //                return true;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //}
        //#endregion

        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="attFile">上傳的物件</param>
        /// <param name="fileName">檔案名稱</param>
        protected void SaveAtt(HttpPostedFileBase attFile, string fileName)
        {
            string uploadPath = SetUploadPath();
            string sFile = uploadPath + fileName;
            string sExt = System.IO.Path.GetExtension(attFile.FileName);
            attFile.SaveAs(sFile);

            if (System.IO.File.Exists(sFile) && IsImage(attFile))
            {
                compressImage(fileName);
            }
        }

        #region Croppie功能 2020-12-17 add

        [HttpPost]
        public ActionResult AddCroppieAtt(string id, string width, string height)
        {
            return PartialView("_CroppieFileUploadExPartial", new string[] { id, width, height });
        }

        /// <summary>
        /// 儲存Croppie形式的圖片,僅存原圖
        /// </summary>
        /// <param name="base64img"></param>
        /// <param name="orginalFileName"></param>
        /// <returns></returns>
        protected void SaveAttachmentFromCroppie(string base64img, string fileName)
        {
            string _img = base64img.Substring(22);
            byte[] bytes = Convert.FromBase64String(_img);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
                string uploadPath = SetUploadPath();
                string fullPath = uploadPath + fileName;
                image.Save(fullPath);
            }
        }

        #endregion

        #region 圖片壓縮
        protected bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }
            string[] formats = new string[] { ".jpg", ".jpeg", ".bmp", ".gif" };
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        protected bool compressImage(String fileName, long quality = 70L)
        {
            string uploadPath = SetUploadPath();
            string sExt = System.IO.Path.GetExtension(fileName);
            string fileNameNew = fileName.Replace(sExt, "_ORI" + sExt);
            System.IO.File.Copy(uploadPath + fileName, uploadPath + fileNameNew);

            using (System.Drawing.Image bmp = System.Drawing.Image.FromFile(uploadPath + fileNameNew)) //原圖
            {
                System.Drawing.Imaging.ImageCodecInfo codecInfo = GetEncoder(bmp.RawFormat); //圖片編解碼資訊
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                System.Drawing.Imaging.EncoderParameters encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                System.Drawing.Imaging.EncoderParameter encoderParameter = new System.Drawing.Imaging.EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter; //編碼器參數

                string sExtension = string.Empty; //副檔名
                System.Drawing.Imaging.ImageFormat format = bmp.RawFormat;
                if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                {
                    sExtension = ".jpg";
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Png))
                {
                    sExtension = ".png";
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                {
                    sExtension = ".bmp";
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                {
                    sExtension = ".gif";
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                {
                    sExtension = ".icon";
                }
                else
                {
                    sExtension = ".jpg";
                }
                try
                {
                    int maxWidth = 1920;
                    int maxHeight = 1080;
                    int _width = bmp.Width;
                    int _height = bmp.Height;
                    if (bmp.Width > 0 && bmp.Height > 0)
                    {
                        if (bmp.Width / bmp.Height >= maxWidth / maxHeight)
                        {
                            if (bmp.Width > maxWidth)
                            {
                                _width = maxWidth;
                                _height = (bmp.Height * maxWidth) / bmp.Width;
                            }
                        }
                        else
                        {
                            if (bmp.Height > maxHeight)
                            {
                                _height = maxHeight;
                                _width = (bmp.Width * maxHeight) / bmp.Height;
                            }
                        }
                    }

                    using (System.Drawing.Bitmap bmpNew = new System.Drawing.Bitmap(_width, _height))
                    {
                        using (System.Drawing.Graphics grap = System.Drawing.Graphics.FromImage(bmpNew))
                        {
                            grap.Clear(System.Drawing.Color.Transparent);
                            grap.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, _width, _height));
                            bmpNew.Save(uploadPath + fileNameNew.Replace("_ORI", string.Empty), codecInfo, encoderParameters); //保存壓縮圖
                            return true;
                        }
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    bmp.Dispose();
                    GC.Collect();
                    if (System.IO.File.Exists(uploadPath + fileNameNew))
                    {
                        System.IO.File.Delete(uploadPath + fileNameNew);
                    }
                }
                return false;
            }
        }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat rawFormat)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == rawFormat.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 取得登入者的權限 AuthorityRight 集合
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        protected List<AuthorityRight> GetSysUserAuthorities(SYSUSER sysUser)
        {
            List<AuthorityRight> authorities = new List<AuthorityRight>();
            /*if (Session[Function.SESSION_GROUP] == null) //群組名稱在這裡設定
			{
				Session[Function.SESSION_GROUP] = string.Join("、", sysUser.ROLE_USER_MAPPING.Select(p => p.ROLE_GROUP).Select(p => p.TITLE).Distinct().ToArray());
			}*/
            foreach (ROLE_USER_MAPPING mapping in sysUser.ROLE_USER_MAPPING)
            {
                foreach (AUTHORITY authority in mapping.ROLE_GROUP.AUTHORITY)
                {
                    AuthorityRight ar = authorities.FirstOrDefault(p => p.NODE_ID.Equals(authority.NODE_ID));
                    if (ar != null)//有重覆的權限要檢查各自的增刪改查 以有權限的為準
                    {
                        ar.ADD = authority.ADD.IsTrue();
                        ar.SEARCH = authority.SEARCH.IsTrue();
                        ar.UPDATE = authority.UPDATE.IsTrue();
                        ar.DELETE = authority.DELETE.IsTrue();
                    }
                    else
                    {
                        ar = new AuthorityRight();
                        ar.ROLE_GROUP_ID = authority.ROLE_GROUP_ID;
                        ar.NODE_ID = authority.NODE_ID;
                        ar.ADD = authority.ADD.IsTrue();
                        ar.SEARCH = authority.SEARCH.IsTrue();
                        ar.UPDATE = authority.UPDATE.IsTrue();
                        ar.DELETE = authority.DELETE.IsTrue();
                        authorities.Add(ar);
                    }
                }
            }
            return authorities;
        }

        /// <summary>
        /// 取得 Session[Function.SESSION_ROLE] 中的 List[AuthorityRight]
        /// </summary>
        /// <returns></returns>
        protected List<AuthorityRight> GetSysUserAuthorities()
        {
            List<AuthorityRight> lsAR = Session[Function.SESSION_ROLE] as List<AuthorityRight> ?? new List<AuthorityRight>();
            if (lsAR.Count == 0)
            {
                if (Function.AllUsersAuthorityRight == null)
                {
                    UpdateAllUsersAuthorityRight(true);
                }
                string sUSER_ID = User.Identity.Name;
                Dictionary<string, List<AuthorityRight>> dict = Function.AllUsersAuthorityRight;
                if (dict.ContainsKey(sUSER_ID))
                {
                    lsAR = dict[sUSER_ID];
                }
            }
            return lsAR;
        }

        /// <summary>
        /// 檢查是否擁有node_id的權限
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        protected bool IsAuthority(string node_id)
        {
            return GetSysUserAuthorities().Any(p => p.NODE_ID.Equals(node_id));
        }

        /// <summary>
        /// 檢查權限
        /// </summary>
        /// <param name="right"></param>
        protected bool IsAuthority(Authority_Right right)
        {
            List<AuthorityRight> lsAR = Session[Function.SESSION_ROLE] as List<AuthorityRight> ?? GetSysUserAuthorities();
            return Function.CheckAuthority(NodeID, lsAR ?? new List<AuthorityRight>(), right, IsContinue);
        }

        /// <summary>
        /// 檢查權限 無權限會導回登入頁
        /// </summary>
        /// <param name="right"></param>
        protected void CheckAuthority(Authority_Right right)
        {
            if (!IsAuthority(right))
            {
                ReturnToLogOn();
            }
            SetIsEdit(true);
        }

        /// <summary>
        /// 導回登入頁
        /// </summary>
        protected ActionResult ReturnToLogOn()
        {
            LogOffCleanEvent();
            FormsAuthentication.RedirectToLoginPage();
            return null;
            //return RedirectToAction("LogOn");
            //LogOffCleanEvent();
            //FormsAuthentication.RedirectToLoginPage();
        }

        /// <summary>
        /// 如果已登入，轉到登入後的首頁
        /// </summary>
        protected void CheckLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                GoIndex();
            }
        }

        /// <summary>
        /// 回Web首頁
        /// </summary>
        protected void ResponseIndex()
        {
            Response.Redirect("~/");
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
        /// 導向目前Controller下的Action Index
        /// </summary>
        /// <returns></returns>
        protected ActionResult GoIndex()
        {
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
        /// 導向目前Controller下的Action Index 加上搜尋參數
        /// </summary>
        /// <returns></returns>
        protected ActionResult GoIndex(string nid = null, int? page = null, int? defaultPage = null, string k = null, Dictionary<string, string> routeValue = null, string actionName = "Index")
        {
            var newRouteValues = new RouteValueDictionary();
            if (!newRouteValues.ContainsKey(VIEW_BAG_NODEID)) newRouteValues.Add(VIEW_BAG_NODEID, nid);
            if (!newRouteValues.ContainsKey(VIEW_BAG_PAGE)) newRouteValues.Add(VIEW_BAG_PAGE, page);
            if (!newRouteValues.ContainsKey(VIEW_BAG_DEFAULTPAGE)) newRouteValues.Add(VIEW_BAG_DEFAULTPAGE, defaultPage);
            if (!newRouteValues.ContainsKey(VIEW_BAG_KEYWORD)) newRouteValues.Add(VIEW_BAG_KEYWORD, k);
            if (routeValue != null)
            {
                foreach (KeyValuePair<string, string> item in routeValue)
                {
                    if (!newRouteValues.ContainsKey(item.Key))
                    {
                        newRouteValues.Add(item.Key, item.Value);
                    }
                }
            }
            if (!AlertMsg.IsNullOrEmpty())
            {
                Msgbox_Toast(AlertMsg);
            }
            return RedirectToAction(actionName, newRouteValues);
        }

        /// <summary>
        /// 產生群組權限字串,以逗號分隔
        /// </summary>
        /// <param name="rg"></param>
        /// <param name="isCRUD">Create Read Update Delete</param>
        /// <returns></returns>
        protected string CreateAuthorityString(ROLE_GROUP rg, bool isCRUD = false)
        {
            List<string> _list = new List<string>();
            string _tmpFormat = "{0}{1}";
            foreach (AUTHORITY a in rg.AUTHORITY)
            {
                _list.Add(a.NODE_ID);
                if (isCRUD)
                {
                    if (a.SEARCH.IsTrue())
                    {
                        _list.Add(string.Format(_tmpFormat, a.NODE_ID, Authority_Right.Search.ToIntValue()));
                    }
                    if (a.ADD.IsTrue())
                    {
                        _list.Add(string.Format(_tmpFormat, a.NODE_ID, Authority_Right.Add.ToIntValue()));
                    }
                    if (a.UPDATE.IsTrue())
                    {
                        _list.Add(string.Format(_tmpFormat, a.NODE_ID, Authority_Right.Update.ToIntValue()));
                    }
                    if (a.DELETE.IsTrue())
                    {
                        _list.Add(string.Format(_tmpFormat, a.NODE_ID, Authority_Right.Delete.ToIntValue()));
                    }
                }
            }
            return string.Join(",", _list);
        }

        /// <summary>
        /// 產生群組權限List
        /// </summary>
        /// <param name="authorityString"></param>
        /// <param name="isCRUD">Create Read Update Delete</param>
        /// <returns></returns>
        protected List<AUTHORITY> CreateAuthorityList(string authorityString, bool isCRUD = false)
        {
            string _nid = string.Empty;
            string _authority_right = string.Empty;
            List<AUTHORITY> _authorities = new List<AUTHORITY>();
            foreach (string _authority in authorityString.ToSplit().OrderBy(p => p))
            {
                _nid = _authority.Substring(0, _authority.Length - 1);//扣掉最後1個字元
                AUTHORITY authority = _authorities.FirstOrDefault(p => _nid.Equals(p.NODE_ID));
                if (authority == null)
                {
                    authority = new AUTHORITY();
                    authority.ID = Function.GetGuid();
                    authority.CREATER = User.Identity.Name;
                    authority.CREATE_DATE = DateTime.Now;
                    authority.NODE_ID = _authority;
                    authority.SEARCH = isCRUD ? bool.FalseString : bool.TrueString;
                    authority.ADD = isCRUD ? bool.FalseString : bool.TrueString;
                    authority.UPDATE = isCRUD ? bool.FalseString : bool.TrueString;
                    authority.DELETE = isCRUD ? bool.FalseString : bool.TrueString;
                    _authorities.Add(authority);
                }
                if (isCRUD)
                {
                    _authority_right = _authority.Replace(_nid, string.Empty);//留下權限字元
                    if (_authority_right.IsInt())
                    {
                        switch ((Authority_Right)_authority_right.ToInt())
                        {
                            case Authority_Right.Search:
                                _authorities.Find(p => p.NODE_ID.Equals(_nid)).SEARCH = bool.TrueString;
                                break;
                            case Authority_Right.Add:
                                _authorities.Find(p => p.NODE_ID.Equals(_nid)).ADD = bool.TrueString;
                                break;
                            case Authority_Right.Update:
                                _authorities.Find(p => p.NODE_ID.Equals(_nid)).UPDATE = bool.TrueString;
                                break;
                            case Authority_Right.Delete:
                                _authorities.Find(p => p.NODE_ID.Equals(_nid)).DELETE = bool.TrueString;
                                break;
                        }
                    }
                }
            }
            return _authorities;
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
        /// 設定FileUpload元件參數2
        /// </summary>
        /// <param name="fileTypeExts">可選擇的檔案類型</param>
        /// <param name="type">上傳檔案類型</param>
        /// <param name="buttonText">按鈕文字</param>
        /// <param name="fileSizeLimit">檔案大小上限 單位= B, KB(預設), MB, or GB</param>
        /// <param name="queueSizeLimit">單次可上傳數量</param>
        /// <param name="atLeast">至少上傳 n 個</param>
        protected void SetFileUploadParamter2(string fileTypeExts = Function.DEFAULT_FILEUPLOAD_EXT, string buttonText = "請選擇", string fileSizeLimit = "", int queueSizeLimit = 999, int atLeast = 0, AttachmentType uploadType = AttachmentType.File, string extraHTML = "", string fc2 = "")
        {
            //ViewBag.FileUploadFileTypeExts = fileTypeExts;
            ViewBag.FileUploadAcceptFiles2 = fileTypeExts;
            ViewBag.FileUploadButtonText2 = buttonText;
            ViewBag.FileUploadFileSizeLimit2 = fileSizeLimit.IsNullOrEmpty() ? Function.DEFAULT_ATTACHMENT_SIZE_LIMIT.ToMyString() : fileSizeLimit;
            ViewBag.FileUploadQueueSizeLimit2 = queueSizeLimit;
            ViewBag.FileUploadAtLeast2 = atLeast;
            ViewBag.FileUploadType2 = uploadType;
            ViewBag.ExtraHTML2 = extraHTML;
            ViewBag.fc2 = fc2;
        }

        /// <summary>
        /// 設定FileUpload元件參數3
        /// </summary>
        /// <param name="fileTypeExts">可選擇的檔案類型</param>
        /// <param name="type">上傳檔案類型</param>
        /// <param name="buttonText">按鈕文字</param>
        /// <param name="fileSizeLimit">檔案大小上限 單位= B, KB(預設), MB, or GB</param>
        /// <param name="queueSizeLimit">單次可上傳數量</param>
        /// <param name="atLeast">至少上傳 n 個</param>
        protected void SetFileUploadParamter3(string fileTypeExts = Function.DEFAULT_FILEUPLOAD_EXT, string buttonText = "請選擇", string fileSizeLimit = "", int queueSizeLimit = 999, int atLeast = 0, AttachmentType uploadType = AttachmentType.File, string extraHTML = "", string fc3 = "")
        {
            //ViewBag.FileUploadFileTypeExts = fileTypeExts;
            ViewBag.FileUploadAcceptFiles3 = fileTypeExts;
            ViewBag.FileUploadButtonText3 = buttonText;
            ViewBag.FileUploadFileSizeLimit3 = fileSizeLimit.IsNullOrEmpty() ? Function.DEFAULT_ATTACHMENT_SIZE_LIMIT.ToMyString() : fileSizeLimit;
            ViewBag.FileUploadQueueSizeLimit3 = queueSizeLimit;
            ViewBag.FileUploadAtLeast3 = atLeast;
            ViewBag.FileUploadType3 = uploadType;
            ViewBag.ExtraHTML3 = extraHTML;
            ViewBag.fc3 = fc3;
        }

        /// <summary>
        /// 設定FileUpload元件參數4
        /// </summary>
        /// <param name="fileTypeExts">可選擇的檔案類型</param>
        /// <param name="type">上傳檔案類型</param>
        /// <param name="buttonText">按鈕文字</param>
        /// <param name="fileSizeLimit">檔案大小上限 單位= B, KB(預設), MB, or GB</param>
        /// <param name="queueSizeLimit">單次可上傳數量</param>
        /// <param name="atLeast">至少上傳 n 個</param>
        protected void SetFileUploadParamter4(string fileTypeExts = Function.DEFAULT_FILEUPLOAD_EXT, string buttonText = "請選擇", string fileSizeLimit = "", int queueSizeLimit = 999, int atLeast = 0, AttachmentType uploadType = AttachmentType.File, string extraHTML = "", string fc4 = "")
        {
            //ViewBag.FileUploadFileTypeExts = fileTypeExts;
            ViewBag.FileUploadAcceptFiles4 = fileTypeExts;
            ViewBag.FileUploadButtonText4 = buttonText;
            ViewBag.FileUploadFileSizeLimit4 = fileSizeLimit.IsNullOrEmpty() ? Function.DEFAULT_ATTACHMENT_SIZE_LIMIT.ToMyString() : fileSizeLimit;
            ViewBag.FileUploadQueueSizeLimit4 = queueSizeLimit;
            ViewBag.FileUploadAtLeast4 = atLeast;
            ViewBag.FileUploadType4 = uploadType;
            ViewBag.ExtraHTML4 = extraHTML;
            ViewBag.fc4 = fc4;
        }

        /// <summary>
        /// 儲存 ModelState.IsValid = false 的訊息
        /// </summary>
        /// <param name="model"></param>
        protected void SaveModelValidMSG(ModelStateDictionary model)
        {
            List<string> error = Function.GetErrorKeyListFromModelState(model);
            List<string> error2 = Function.GetErrorListFromModelState(model);
            LogSystem.InitLogSystem();
            foreach (string s in error)
            {
                LogSystem.WriteLine(s);
            }
            foreach (string s in error2)
            {
                LogSystem.WriteLine(s);
            }
            LogSystem.CloseUnderlayingStream();
        }

        /// <summary>
        /// 轉換下載檔案的檔名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected string SetFileDownLoadName(string fileName)
        {
            string browser = Request.Browser.Browser;
            if (!"Firefox".CheckStringValue(browser))
            {
                fileName = Server.UrlPathEncode(fileName);
            }
            return fileName;
        }

        /// <summary>
        /// 是否POST
        /// </summary>
        protected bool IsPost()
        {
            return "post".CheckStringValue(Request.HttpMethod);
        }

        /// <summary>
        /// 是否Ajax Request
        /// </summary>
        protected bool IsAjaxRequest()
        {
            return Request.IsAjaxRequest();
        }

        /// <summary>
        /// 取得登入者資訊
        /// </summary>
        /// <returns></returns>
        protected SYSUSER GetLogOnSysUser()
        {
            return iDB.GetByID<SYSUSER>(User.Identity.Name);
        }

        /// <summary>
        /// Word 套印用
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion

        #region Protected Function for Project
        /// <summary>
        /// 建立會員送的5點
        /// </summary>
        /// <param name="d1"></param>
        protected void CreateMemberPlus(USER fun6000)
        {
            PLUS data = new PLUS();
            data.ID = Function.GetGuid();
            data.MAIN_ID = fun6000.USER_ID;
            data.CREATER = "createMember";
            data.PLUS_TYPE = "fun5002";
            data.STATUS = "1";//預設1
            data.CREATE_DATE = DateTime.Now;
            data.CONTENT1 = "";
            data.CONTENT2 = fun6000.CONTENT1;
            data.CONTENT3 = fun6000.CONTENT2 + fun6000.CONTENT3;
            data.CONTENT5 = "加入會員送5點";
            data.CONTENT6 = fun6000.USER_ID;
            data.DATETIME1 = DateTime.Today;
            data.DATETIME2 = data.DATETIME1.Value.AddYears(1).AddDays(-1);
            data.DECIMAL5 = 5;
            iDB.Add<PLUS>(data);
        }
        /// <summary>
        /// 計算會員等級
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string ResetMemberLevel(string id)
        {
            string _value = string.Empty;
            if (!id.IsNullOrEmpty())
            {
                USER model = iDB.GetByID<USER>(id);
                NODE _C15Node = Function.GetNode(model.CONTENT6);//目前等級
                if ("level99".Equals(_C15Node.ID))
                {
                    _value = _C15Node.ID;//維持原狀
                    return _value;
                }
                NODE _nextNode = null;
                string SqlStr = $"select sum(DECIMAL5)  as d5 from plus where CONTENT6='{model.USER_ID}' and [ENABLE]=1 and ([PLUS_TYPE]='fun4000' or [PLUS_TYPE]='fun5002' ) ";
                double d5 = 0;
                using (DBEntities db = new DBEntities())
                {
                    DataRow dr = Function.getDataTable(db, SqlStr).AsEnumerable().FirstOrDefault();
                    if (dr != null)
                    {
                        d5 = dr["d5"].ToString().ToDouble();
                    }
                }
                foreach (var node in Function.NodeList.Where(p => "MemberLevel".Equals(p.PARENT_ID) && p.ENABLE.IsEnable())
                    .OrderByDescending(p => p.ORDER))
                {
                    if (d5 < node.CONTENT1.ToInt())
                    {
                        _nextNode = node;
                    }
                }

                if (_nextNode == null)
                {
                    _value = _C15Node.ID;//維持原狀
                }
                else
                {
                    _value = _nextNode.ID;
                    model.CONTENT6 = _value;
                    iDB.Save();
                }
            }
            return _value;
        }

        #endregion

        #region public Action

        //[ChildActionOnly]
        //public ActionResult GetMenus(int menuType)
        //{
        //    string _viewName = string.Empty;
        //    if (menuType == 0)//top
        //    {
        //        _viewName = "Menu/_TopMenuPartial";
        //    }
        //    else//footer
        //    {
        //        _viewName = "Menu/_FooterMenuPartial";
        //    }
        //    return PartialView(_viewName, GetMenus());
        //}

        /// <summary>
        /// 根據使用者權限取得Menu
        /// </summary>
        /// <returns></returns>
        //List<NODE> GetMenus()
        //{
        //    List<NODE> menus = new List<NODE>();
        //    foreach (NODE n in Function.FunctionNodeList.Where(p => p.PARENT_ID.CheckStringValue(FUNCTION_ROOT_NODE_ID) && p.ENABLE.IsEnable()).OrderBy(p => p.ORDER))
        //    {
        //        if (IsAuthority(n.ID))
        //            menus.Add(n);
        //    }
        //    return menus;
        //}
        /// <summary>
        /// 左方功能列表
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public JsonResult LeftMenu(string pid)
        {
            pid = FUNCTION_ROOT_NODE_ID;
            List<zNode> zNodes = new List<zNode>();
            foreach (NODE n in Function.GetChildrenNodes(null, pid).Where(p => p.ENABLE.IsEnable()).OrderBy(p => p.ORDER))
            {
                if (!IsAuthority(n.ID)) { continue; }

                //母功能關掉，子功能也要一併關掉
                NODE parent = Function.GetNode(n.PARENT_ID);
                if (parent != null && parent.ENABLE.IsDisable())
                {
                    continue;
                }

                zNode z = new zNode();
                z.id = n.ID;
                z.name = n.TITLE;
                z.pId = n.PARENT_ID;
                z.url = !IsAuthority(n.ID) || (parent != null && parent.ENABLE.IsDisable()) ? "javascript:NoAuth();" : (n.URL.IsNullOrEmpty() ? string.Empty : Url.Content(n.URL));
                //z.url = (n.URL.IsNullOrEmpty() ? string.Empty : Url.Content(n.URL));
                zNodes.Add(z);
            }
            return Json(zNodes, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region BaceController Function

        /// <summary>
        /// Default 建構子 會指定PageTitle預設值
        /// </summary>
        public BaseController()
        {
            PageTitle = Function.PAGE_TITLE;
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
            UpdateSysUserList(true);
            UpdateNodeList(true);
            UpdateAllUsersAuthorityRight(true);
            base.Initialize(requestContext);
        }

        /// <summary>
        /// 更新 Function.FunctionNodeList and Function.NodeList
        /// </summary>
        /// <param name="isFirst">初始化:true 更新:false</param>
        /// <param name="isWeb">更新前台node 預設false</param>
        protected void UpdateNodeList(bool isFirst = false, bool isWeb = false)
        {
            if (isFirst)
            {
                //初始化功能NODE List
                if (Function.FunctionNodeList == null)
                    Function.FunctionNodeList = iDB.GetAllNodesBySQL(FUNCTION_ROOT_NODE_ID);
                //初始化All NODE List
                if (Function.NodeList == null)
                    Function.NodeList = iDB.GetAllAsNoTracking<NODE>(false).ToList();
            }
            else
            {
                Function.FunctionNodeList = iDB.GetAllNodesBySQL(FUNCTION_ROOT_NODE_ID);
                Function.NodeList = iDB.GetAllAsNoTracking<NODE>(false).ToList();
            }

            if (isWeb)
            {
                #region 更新前台 NODE
                //建立 WebRequest 並指定目標的 uri
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    WebRequest request = WebRequest.Create(Function.DEFAULT_ROOT_HTTP + "UpdateNode");
                    request.Method = "GET";
                    //使用 GetResponse 方法將 request 送出，如果不是用 using 包覆，請記得手動 close WebResponse 物件，避免連線持續被佔用而無法送出新的 request
                    using (var httpResponse = (HttpWebResponse)request.GetResponse())
                    {
                        httpResponse.Close();
                    }

                    request = WebRequest.Create(Function.DEFAULT_ROOT_HTTP_AFMC + "UpdateNode");
                    request.Method = "GET";
                    //使用 GetResponse 方法將 request 送出，如果不是用 using 包覆，請記得手動 close WebResponse 物件，避免連線持續被佔用而無法送出新的 request
                    using (var httpResponse = (HttpWebResponse)request.GetResponse())
                    {
                        httpResponse.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.InitLogSystem();
                    LogSystem.WriteLine(ex.Message);
                    LogSystem.WriteLine(ex.StackTrace);
                    LogSystem.CloseUnderlayingStream();
                }
                #endregion
            }
        }

        /// <summary>
        /// 更新 Function.SysUserList
        /// </summary>
        /// <param name="isFirst">初始化:true 更新:false</param>
        protected void UpdateSysUserList(bool isFirst = false)
        {
            if (isFirst)
            {
                if (Function.SysUserList == null)
                    Function.SysUserList = iDB.GetAllAsNoTracking<SYSUSER>(false).ToList();
            }
            else
            {
                Function.SysUserList = iDB.GetAllAsNoTracking<SYSUSER>(false).ToList();
            }
        }

        /// <summary>
        /// 更新 Function.UpdateAllUsersAuthorityRight
        /// </summary>
        /// <param name="isFirst">初始化:true 更新:false</param>
        protected void UpdateAllUsersAuthorityRight(bool isFirst = false)
        {
            Dictionary<string, List<AuthorityRight>> dict = new Dictionary<string, List<AuthorityRight>>();
            if (isFirst)
            {
                if (Function.AllUsersAuthorityRight == null)
                {
                    List<SYSUSER> users = iDB.GetAllAsNoTracking<SYSUSER>(true).ToList();
                    foreach (SYSUSER user in users)
                    {
                        if (!dict.ContainsKey(user.USER_ID))
                        {
                            dict.Add(user.USER_ID, GetSysUserAuthorities(user));
                        }
                    }
                    Function.AllUsersAuthorityRight = dict;
                }
            }
            else
            {
                List<SYSUSER> users = iDB.GetAllAsNoTracking<SYSUSER>(true).ToList();
                foreach (SYSUSER user in users)
                {
                    if (!dict.ContainsKey(user.USER_ID))
                    {
                        dict.Add(user.USER_ID, GetSysUserAuthorities(user));
                    }
                }
                Function.AllUsersAuthorityRight = dict;
            }
        }

        /// <summary>
        /// 更新 Function.FunctionNodeList, Function.NodeList 
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateNode()
        {
            UpdateNodeList(isWeb: true);
            return GoIndex();
        }

        /// <summary>
        /// 更新 會員等級
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateMemberLevel(string id)
        {
            ResetMemberLevel(id);
            Msgbox_Toast(Function.DEFAULT_UPDATE_MESSAGE);
            return GoIndex();
        }

        /// <summary>
        /// 每個Action執行前(檢查權限)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.ControllerName = filterContext.RouteData.Values["controller"].ToString();
            ViewBag.ActionName = filterContext.RouteData.Values["action"].ToString();

            if (!filterContext.IsChildAction) //只有母Action才來做比對跟轉換
            {
                if (!filterContext.IsChildAction &&
                !filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.CheckStringValue(CONTROLLER_LIST) &&
                !filterContext.ActionDescriptor.ActionName.CheckStringValue(ACTION_LIST))
                {
                    //for NodeID and LeftMenu Title For PNodeID
                    NodeID = filterContext.HttpContext.Request.QueryString[VIEW_BAG_NODEID];
                    ViewBag.NodeID = NodeID;
                    if (IsAuthority(NodeID))
                    {
                        //過瀘 jquery=UNDEFINED
                        List<string> keys = filterContext.ActionParameters.Where(p => Function.UNDEFINED.Equals(p.Value.ToMyString())).Select(p => p.Key).ToList();
                        foreach (string key in keys)
                        {
                            filterContext.ActionParameters[key] = string.Empty;
                        }
                        //end
                        //setup LeftMenu Title,SiteMap,ContentTitle from nid
                        List<NODE> maps = Function.GetParentNodes(null, NodeID, true);
                        maps = maps.Where(p => !p.ID.Equals(FUNCTION_ROOT_NODE_ID)).ToList();//最上層ID剔除
                        maps.Reverse();//排序反轉
                        ViewBag.SiteMap = string.Join(" > ", maps.Select(p => p.TITLE).ToArray());
                        NODE _n = maps.FirstOrDefault();
                        ViewBag.LeftMenuTitle = _n.TITLE;
                        ViewBag.PNodeID = _n.ID;
                        NODE _n2 = maps.Find(p => p.ID.Equals(NodeID));
                        SetContentTitle(_n2 == null ? string.Empty : _n2.TITLE);

                        //for預設列表分頁筆數
                        int _defaultPage = Function.DEFAULT_PAGE_SIZE;
                        if (filterContext.ActionParameters.TryGetValue(VIEW_BAG_DEFAULTPAGE, out DefaultPage))
                        {
                            _defaultPage = DefaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
                            ViewBag.DefaultPage = _defaultPage;
                        }

                        //for ViewBag.Keyword ViewBag.Page
                        string _k = Function.GetDictionaryObjectValue(filterContext.ActionParameters, VIEW_BAG_KEYWORD);
                        string _page = Function.GetDictionaryObjectValue(filterContext.ActionParameters, VIEW_BAG_PAGE);
                        ViewBag.Keyword = _k;
                        ViewBag.Page = _page;

                        string _kX = Function.GetDictionaryObjectValue(filterContext.ActionParameters, VIEW_BAG_KEYWORD_X);
                        string _pageX = Function.GetDictionaryObjectValue(filterContext.ActionParameters, VIEW_BAG_PAGE_X);
                        ViewBag.KeywordX = _kX;
                        ViewBag.PageX = _pageX;

                        //create custom routeValues
                        Dictionary<string, string> defaultViewBag = new Dictionary<string, string>();
                        defaultViewBag.Add(VIEW_BAG_NODEID, NodeID);
                        defaultViewBag.Add(VIEW_BAG_DEFAULTPAGE, _defaultPage.ToString());

                        defaultViewBag.Add(VIEW_BAG_PAGE, _page);
                        defaultViewBag.Add(VIEW_BAG_KEYWORD, _k);

                        defaultViewBag.Add(VIEW_BAG_PAGE_X, _pageX);
                        defaultViewBag.Add(VIEW_BAG_KEYWORD_X, _kX);

                        ViewBag.DefaultRouteValues = defaultViewBag;
                    }
                    else
                    {
                        //filterContext.Result = ReturnToLogOn();
                        filterContext.Result = RedirectToAction("Index", "Home");
                        return;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 將檔案壓縮(單檔/多檔)
        /// A content result which can accepts a DotNetZip ZipFile object to write to the output stream
        /// (可以接受DotNetZip ZipFile對象寫入輸出流的內容結果)
        /// ----
        /// return new ZipFileResult(zip, filename);
        /// </summary>
        public class ZipFileResult : ActionResult
        {
            public ZipFile zip { get; set; }
            public string filename { get; set; }

            public ZipFileResult(ZipFile zip)
            {
                this.zip = zip;
                this.filename = null;
            }
            public ZipFileResult(ZipFile zip, string filename)
            {
                this.zip = zip;
                this.filename = filename;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var Response = context.HttpContext.Response;
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment;" + (string.IsNullOrEmpty(filename) ? "" : "filename=" + filename));
                zip.Save(Response.OutputStream);
                Response.End();
            }
        }

        #endregion

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

        #endregion

        #region Delete
        /// <summary>
        /// 刪除 NODE (ting add)
        /// </summary>
        [ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult DeleteNode(string id, int? page, int? defaultPage, string k, bool really = false, string actionName = "Index")
        {
            CheckAuthority(Authority_Right.Delete);

            //不是真的刪除時，記錄刪除人及刪除時間
            if (!really)
            {
                NODE n = iDB.GetByID<NODE>(id, false);
                if (n != null)
                {
                    n.CONTENT10 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
                }
            }
            if (!iDB.Delete<NODE>(id, really))
            {
                AlertMsg = Function.DELETE_ERROR_MESSAGE;
            }
            UpdateNodeList(isWeb: true);
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), actionName);
        }

        /// <summary>
        /// 刪除 DATA1 (ting add)
        /// </summary>
        [ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult DeleteData1(string id, int? page, int? defaultPage, string k, string k1, string k2, bool really = false, string actionName = "Index")
        {
            CheckAuthority(Authority_Right.Delete);

            //不是真的刪除時，記錄刪除人及刪除時間
            if (!really)
            {
                DATA1 d1 = iDB.GetByID<DATA1>(id, false);
                if (d1 != null)
                {
                    d1.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
                }
            }
            if (!iDB.Delete<DATA1>(id, really))
            {
                AlertMsg = Function.DELETE_ERROR_MESSAGE;
            }
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2" }), actionName);
        }

        /// <summary>
        /// 刪除 PLUS (ting add)
        /// </summary>
        [ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult DeletePlus(string id, int? page, int? defaultPage, string pid, string k, string k1, string k2, bool really = false, string actionName = "Index")
        {
            CheckAuthority(Authority_Right.Delete);

            //不是真的刪除時，記錄刪除人及刪除時間
            if (!really)
            {
                PLUS plus = iDB.GetByID<PLUS>(pid, false);
                if (plus != null)
                {
                    plus.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
                }
            }
            if (!iDB.Delete<PLUS>(pid, really))
            {
                AlertMsg = Function.DELETE_ERROR_MESSAGE;
            }
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1", "k2" }), actionName);
        }

        /// <summary>
        /// 刪除 ATTACHMENT
        /// </summary>
        [ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult DeleteAttachment(string id, string attid, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = true, string actionName = "Edit")
        {
            CheckAuthority(Authority_Right.Delete);
            ATTACHMENT atta = iDB.GetByID<ATTACHMENT>(attid);
            if (atta != null)
            {
                if (really)
                {
                    if (!iDB.Delete<ATTACHMENT>(attid))
                    {
                        AlertMsg = Function.DELETE_ERROR_MESSAGE;
                    }
                }
                else
                {
                    //不是真的刪除時，記錄刪除人及刪除時間
                    atta.CONTENT9 = EnableType.Disable.ToIntValue();
                    atta.CONTENT10 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
                    iDB.Save();
                }
                if (atta.NODE != null)
                {
                    //UpdateNodeList(isWeb: true);
                }
            }
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1", "k2", "start", "end" }), actionName);
        }
        #endregion

        #region excel匯出

        protected const string APPLICATION_VND = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
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
        /// 設定需傳遞的參數
        /// </summary>
        /// <param name="targets">目標參數名稱</param>
        /// <returns></returns>
        protected Dictionary<string, string> SetRouteValue(params string[] targets)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in targets)
            {
                result.Add(item, SetRouteValue(item));
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 設定需傳遞的參數(包含常用參數)
        /// </summary>
        /// <param name="targets">目標參數名稱</param>
        /// <returns></returns>
        protected Dictionary<string, string> SetRouteValueIncludeCommon(params string[] targets)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] arrCommon = new string[] { VIEW_BAG_NODEID, VIEW_BAG_PAGE, VIEW_BAG_DEFAULTPAGE, VIEW_BAG_KEYWORD };
            foreach (string item in arrCommon)
            {
                result.Add(item, SetRouteValue(item));
            }
            foreach (string item in targets)
            {
                result.Add(item, SetRouteValue(item));
            }
            return result;
        }

        private string SetRouteValue(string name)
        {
            string result = string.Empty;
            result = result.IsNullOrEmpty() ? Request.QueryString[name].ToMyString() : result;
            result = result.IsNullOrEmpty() ? Url.RequestContext.RouteData.Values[name].ToMyString() : result;

            if (IsPost() && Request.Form.Count > 0 && Request.Form.Keys.Cast<string>().Any(x => x.CheckStringValue(name)))
                result = Request.Form[name].ToMyString();

            ViewData[name] = result;
            return result;
        }

        /// <summary>
        /// 設定語系
        /// </summary>
        [AllowAnonymous]
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            //清除OutputCache
            //Response.RemoveOutputCacheItem()
            //end
            culture = CultureHelper.GetImplementedCulture(culture);
            HttpCookie cultureCookie = Request.Cookies[Function.COOKIE_LANG];
            if (cultureCookie != null)
            {
                cultureCookie.Value = culture;
            }
            else
            {
                cultureCookie = new HttpCookie(Function.COOKIE_LANG)
                {
                    Secure = Request.IsSecureConnection,
                    HttpOnly = true,
                    Value = culture,
                    Expires = DateTime.Now.AddMonths(2)
                };
            }
            Response.Cookies.Add(cultureCookie);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            if (Session[Function.SESSION_ROLE] != null)
            {
                string sID = ((List<AuthorityRight>)Session[Function.SESSION_ROLE]).Select(p => p.ROLE_GROUP_ID).Distinct().FirstOrDefault();
                Session[Function.SESSION_GROUP] = iDB.GetByIDAsNoTracking<ROLE_GROUP>(sID).TITLE;
            }
            return Redirect(returnUrl);
        }

        /// <summary>
        /// 取得子NODE 20200713 ting add
        /// </summary>
        /// <returns>List<SelectListItem></returns>
        public List<SelectListItem> GetSelectListItem(string id, string v = "", string first = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (!first.IsNullOrEmpty())
            {
                list.Add(new SelectListItem() { Value = string.Empty, Text = first });
            }
            if (!id.IsNullOrEmpty())
            {
                list.AddRange(Function.NodeList.Where(p => id.CheckStringValue(p.PARENT_ID) && p.ENABLE.IsEnable()).OrderBy(p => p.ORDER)
                    .Select(p => new SelectListItem() { Value = p.ID, Text = p.TITLE, Selected = p.ID.Equals(v) }).ToList());
            }
            return list;
        }

        /// <summary>
        /// 修改 DATETIME9 & DATETIME10 （20200723 ting add）
        /// </summary>
        /// <param name="TABLE">表格名稱</param>
        /// <param name="NODE_ID">功能編號</param>
        /// <param name="ID">資料編號</param>
        public void UpdatePlusDT9AndDT10(string TABLE, string NODE_ID, string ID, int ORDER)
        {
            using (DBEntities db = new DBEntities())
            {
                string SqlStr = string.Format(@"UPDATE {0}
SET DATETIME9 = (SELECT MIN(DATETIME1) FROM PLUS WHERE PLUS_TYPE = 'TIME' AND [ORDER] = @ORDER AND PLUS.MAIN_ID = {0}.ID)
, DATETIME10 = (SELECT MAX([DATETIME2]) FROM PLUS WHERE PLUS_TYPE = 'TIME' AND [ORDER] = @ORDER AND PLUS.MAIN_ID = {0}.ID)
FROM {0} WHERE NODE_ID = @NODE_ID AND ID = @ID;", TABLE);
                db.Database.ExecuteSqlCommand(SqlStr, new SqlParameter("NODE_ID", NODE_ID), new SqlParameter("ID", ID), new SqlParameter("ORDER", ORDER));
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 寄信通知擁有審核功能的使用者
        /// </summary>
        /// <param name="funList">功能編號</param>
        /// <param name="sSubject">主旨</param>
        /// <param name="sBody">內容</param>
        public void SendMail2Auditor(string[] funList, string sSubject, string sBody)
        {
            DataTable dt = new DataTable();
            using (DBEntities db = new DBEntities())
            {
                string SqlStr = string.Format(@"SELECT u.EMAIL
FROM ROLE_GROUP rg
JOIN AUTHORITY au ON au.ROLE_GROUP_ID = rg.ID AND rg.[ENABLE] = 1
JOIN ROLE_USER_MAPPING m ON m.ROLE_GROUP_ID = rg.ID
JOIN SYSUSER u ON m.[USER_ID] = u.[USER_ID] AND u.[ENABLE] = 1
WHERE GROUP_TYPE = 'SYSUSER'
AND au.NODE_ID IN ({0})
GROUP BY u.EMAIL", "'" + string.Join("','", funList) + "'");
                dt = Function.getDataTable(db, SqlStr);
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Function.SendMail(new LetterModel { RecipientList = new List<string> { dr["EMAIL"].ToString() }, Subject = sSubject, Body = sBody });
                }
            }
        }

        /// <summary>
        /// 取得 NODE 上層節點的完整中文路徑 20200819 ting add
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFullParentNodeText(string id)
        {
            ViewBag.TreeLevel = 0;
            string SqlStr = @";WITH tmp AS
(
	SELECT ID, PARENT_ID, TITLE, CONVERT(nvarchar(1000), TITLE) as TITLEs
	, 1 as TreeLevel
	FROM NODE
	WHERE ID = @SelectID AND [ENABLE] = '1'
	UNION ALL
	SELECT n.ID, n.PARENT_ID, n.TITLE, CONVERT(nvarchar(1000), n.TITLE + ' > ' + p.TITLEs) as TITLEs
	, TreeLevel + 1 as TreeLevel
	FROM NODE n
	JOIN tmp p ON n.ID = p.PARENT_ID AND n.[ENABLE] = '1'
	WHERE n.PARENT_ID IS NOT NULL
)
SELECT TOP 1 TITLEs, TreeLevel FROM tmp ORDER BY TreeLevel DESC
OPTION (MAXRECURSION 0)";
            string sText = string.Empty;
            using (DBEntities db = new DBEntities())
            {
                DataRow dr = Function.getDataTable(db, SqlStr, new SqlParameter("SelectID", id)).AsEnumerable().FirstOrDefault();
                if (dr != null)
                {
                    sText = dr["TITLEs"].ToString();
                    ViewBag.TreeLevel = Convert.ToInt32(dr["TreeLevel"]);
                }
            }
            return sText;
        }


        /// <summary>
        /// 取得 SQL密碼剩餘天數,逾期改密碼 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSqlpsd_date(string id)
        {
            string SqlStr = @"DECLARE @login varchar(50) = '" + id +
            @"'
            SELECT @login = name 
            FROM sys.sql_logins
            WHERE (type = 's') AND (is_expiration_checked = 1)
            -- 傳回密碼到期之前的剩餘天數
            --SELECT LOGINPROPERTY(@login, 'DaysUntilExpiration') AS 'days_until_expiration'
            
            DECLARE @days int;
            SELECT @days = CONVERT(int,ISNULL(LOGINPROPERTY(@login, 'DaysUntilExpiration'),0)) --AS 'days_until_expiration'
            SELECT LOGINPROPERTY(@login, 'DaysUntilExpiration') AS 'days_until_expiration', DATEADD(d, @days, getdate()) AS 'date',LOGINPROPERTY(@login, 'PasswordLastSetTime') AS 'days_lastday'";
            string date = string.Empty;
            string lastdt = string.Empty;
            using (DBEntities db = new DBEntities())
            {
                DataRow dr = Function.getDataTable(db, SqlStr).AsEnumerable().FirstOrDefault();
                if (dr != null)
                {
                    date = dr["date"].ToDateString();
                    lastdt = dr["days_lastday"].ToDateString();
                    if (DateTime.Today >= date.ToDateTime().AddDays(-43) && !(DateTime.Today == lastdt.ToDateTime()))
                    {
                        string newpassword = "kingsp" + DateTime.Today.ToDefaultString2();
                        var pattern = @"password=([^;]+)";
                        var regex = new Regex(pattern);
                        //admin.config
                        Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                        var connString = config.ConnectionStrings.ConnectionStrings["DBEntities"].ToString();
                        var newConnString = regex.Replace(connString, "password="+ newpassword);
                        //config.ConnectionStrings.ConnectionStrings["DBEntities"].ConnectionString = newConnString;
                        //config.Save();

                        //web.config
                        //PhysicalPath(@"E:\專案\72_SINYI\src\web\Web.config", "72_SINYI");

                        //app.config
                        //PhysicalPath(@"E:\專案\72_SINYI\src\KingspModel\App.config", "72_SINYI");

                        //改sql 密碼
                        string SqlStr2 = @"ALTER LOGIN "+id+" WITH PASSWORD = N'" + newpassword + "'";
                        //db.Database.ExecuteSqlCommand(SqlStr2);
                        //db.SaveChanges();

                        date = newpassword;
                    }
                    else
                    {
                        date = "密碼尚未逾期";
                    }
                }
            }
            return date;
        }

        public string GetPSW()
        {
            //取得web.config
            string oldcon = ConfigurationManager.ConnectionStrings["DBEntities"].ToString();
            string ID = @"id=([^;]+)";
            string Password = @"password=([^;]+)";
            string old_password = string.Empty;
            string con_id = string.Empty;
            Regex regex_id = new Regex(ID);
            Match match_id = regex_id.Match(oldcon);
            Regex regex_psw = new Regex(Password);
            Match match_psw = regex_psw.Match(oldcon);
            if (match_id.Success)
            {
                con_id = match_id.Groups[1].Value;
            }
            if (match_psw.Success)
            {
                old_password = match_psw.Groups[1].Value;
            }
            string newpsw = GetSqlpsd_date(con_id);
            string result = "New：" + newpsw +"Old：" + old_password;
            return result;
            
        }

       
        /// <param name="path">檔案實體路徑</param>
        /// <param name="websitename">IIS網站名稱</param>
        /// <returns></returns>
        public string PhysicalPath(string path,string websitename)
        {
            string result = "ok";
            try {
                string newpassword = "kingsp" + DateTime.Today.ToDefaultString2();
                var configFile = new FileInfo(path);
                var pattern = @"password=([^;]+)";
                var regex = new Regex(pattern);
                var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
                var wcfm = new WebConfigurationFileMap();
                wcfm.VirtualDirectories.Add("/", vdm);
                Configuration config = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/", websitename);
                var connString = config.ConnectionStrings.ConnectionStrings["DBEntities"].ToString();
                var newConnString = regex.Replace(connString, "password=" + newpassword);
                config.ConnectionStrings.ConnectionStrings["DBEntities"].ConnectionString = newConnString;
                config.Save();
            }
            catch(Exception ex) {
                result = ex.ToString();
            }
            return result;
        }
    }
}

