using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Data.SqlClient;
using MessagingToolkit.QRCode.Codec;
using System.Drawing.Imaging;
using System.Drawing;

namespace KingspModel
{
	/// <summary>
	/// 共用 Function
	/// </summary>
	public class Function
	{
		#region const property
		/// <summary>
		/// 預設興展帳號 kingsp
		/// </summary>
		public const string DEFAULT_ADMIN = "kingsp";
		/// <summary>
		/// 預設興展密碼 kingsp
		/// </summary>
		public const string DEFAULT_PASSWORD = "kingsp";
		/// <summary>
		/// Session Key (sysUser_role)
		/// </summary>
		public const string SESSION_ROLE = "sysUser_role";
		/// <summary>
		/// Session Key (sysUser_department_name)
		/// </summary>
		public const string SESSION_DEPARTMENT_NAME = "sysUser_department_name";
		/// <summary>
		/// Session Key (sysUser_name)
		/// </summary>
		public const string SESSION_NAME = "sysUser_name";
		/// <summary>
		/// Session Key (sysUser_group)
		/// </summary>
		public const string SESSION_GROUP = "sysUser_group";
		/// <summary>
		/// Session Key (sysUser_group_c1)
		/// </summary>
		public const string SESSION_GROUP_C1 = "sysUser_group_c1";
		/// <summary>
		/// Session Key (sysUser_c2)
		/// </summary>
		public const string SESSION_SYSUSER_C2 = "sysUser_c2";
		/// <summary>
		/// Session Key (sysUser_c30)
		/// </summary>
		public const string SESSION_SYSUSER_C30 = "sysUser_c30";
		/// <summary>
		/// Session Key (captchaImage)
		/// </summary>
		public const string SESSION_CAPTCHA_IMAGE = "captchaImage";
		/// <summary>
		/// TempData["tempDataMSG"]
		/// </summary>
		public const string TEMPDATA_MESSAGE_KEY = "tempDataMSG";
		/// <summary>
		/// javascript.jQuery 找不到時的訊息 undefined
		/// </summary>
		public const string UNDEFINED = "undefined";
		/// <summary>
		/// 上傳圖片中型檔名 圖片檔名 + "_medium"
		/// </summary>
		public const string PIC_MEDIUM = "_medium";
		/// <summary>
		/// 上傳圖片小型檔名 圖片檔名 + "_small"
		/// </summary>
		public const string PIC_SMALL = "_small";
		/// <summary>
		/// 預設可上傳的檔案格式 * (不限制) jquery file upload
		/// </summary>
		public const string DEFAULT_FILEUPLOAD_EXT = ".odf,.ods,.odp,.odt,.pdf,.zip,.rar";
		/// <summary>
		/// 預設可上傳的圖片格式 *.jpeg;*.jpg;*.gif;*.png;*.bmp
		/// </summary>
		public const string DEFAULT_FILEUPLOAD_PICTURE_EXT = ".jpg,.jfif,.png,.gif,.bmp,.tif,.tiff";
		/// <summary>
		/// 預設 confirm 訊息 確定要刪除嗎？
		/// </summary>
		public const string DEFAULT_CONFIRM_MESSAGE = "確定要刪除嗎？";
		/// <summary>
		/// 預設 alert訊息 資料新增成功！！
		/// </summary>
		public const string DEFAULT_ADD_MESSAGE = "資料新增成功！！";
		/// <summary>
		/// 預設 alert訊息 資料更新成功！！
		/// </summary>
		public const string DEFAULT_UPDATE_MESSAGE = "資料更新成功！！";
		/// <summary>
		/// 刪除時的錯誤訊息 刪除失敗！！
		/// </summary>
		public const string DELETE_ERROR_MESSAGE = "刪除失敗！！";
		/// <summary>
		/// 刪除後訊息 已刪除！！
		/// </summary>
		public const string DELETE_MESSAGE = "已刪除！！";
		/// <summary>
		/// Cookie "lang" 語系
		/// </summary>
		public const string COOKIE_LANG = "lang";
		/// <summary>
		/// 縣市 Node Parent_ID = CityInfo
		/// </summary>
		public const string NODE_ID_CITYINFO = "CityInfo";
		/// <summary>
		/// 「欄位名稱」 請勿超過 「設定的值」 個字 ({0} 請勿超過 {1} 個字)
		/// </summary>
		public const string DEFAULT_VALIDATION_MESSAGE = "{0} 請勿超過 {1} 個字";
		/// <summary>
		/// 電話格式範例 (02 - 12345678)
		/// </summary>
		public const string DEFAULT_FORMAT_TEL = "(02 - 12345678)";
		/// <summary>
		/// 手機格式範例 (0910 - 123456)
		/// </summary>
		public const string DEFAULT_FORMAT_PHONE = "(0910 - 123456)";
		/// <summary>
		/// 電話、手機格式 {0}-{1}
		/// </summary>
		public const string DEFAULT_FORMAT_TEL_PHONE = "{0}-{1}";
		/// <summary>
		/// 日期區間格式 {0} ～ {1}
		/// </summary>
		public const string DEFAULT_FORMAT_DATE_RANGE = "{0} ～ {1}";
		/// <summary>
		/// 時間區間格式 {0}{1}～{2}{3}
		/// </summary>
		public const string DEFAULT_FORMAT_TIME_RANGE = "{0}:{1}～{2}:{3}";

		#endregion

		#region const property for project
		///// <summary>
		///// sample &gt; sample
		///// </summary>
		//public const string CONST_SAMPLE = "sample";
		/// <summary>
		/// 分隔符號
		/// </summary>
		public const char DELIMITER = '＃';
		/// <summary>
		/// 公務車項目管理
		/// </summary>
		public const string CORPORATE_FLEET_VEHICLE = "CorporateFleetVehicle";
		/// <summary>
		/// 公務車項目管理>保養修配零件記錄
		/// </summary>
		public const string VEHICLE_REPAIR = "VehicleRepair";
		/// <summary>
		/// 行車前檢查紀錄
		/// </summary>
		public const string BEFORE_DRIVING = "BeforeDriving";
		/// <summary>
		/// 行車紀錄
		/// </summary>
		public const string AFTER_DRIVING = "AfterDriving";
		/// <summary>
		/// 光影文化館 > 光影電影院＞主題影展管理
		/// </summary>
		public const string FILM_EXHIBITION = "FilmExhibition";
		/// <summary>
		/// 光影文化館＞光影電影院＞單元放映管理
		/// </summary>
		public const string FILM_SCREENINGS = "FilmScreenings";
		/// <summary>
		/// 預設密碼 Ab123456~
		/// </summary>
		public const string DEFAULT_PASSWORD_SETUP = "Ab123456~";
		#endregion

		#region const property for Regular Expression
		/// <summary>
		/// Email 正則式 ^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$
		/// </summary>
		public const string EMAIL_REGEX = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
		/// <summary>
		/// 身份證字號 正則式 ([A-Z]|[a-z])([1-2])\d{8}
		/// </summary>
		public const string TAIWANID_REGEX = @"([A-Z]|[a-z])([1-2])\d{8}";
		/// <summary>
		/// 帳號 正則式 ^[a-zA-Z]\w+$ 首字英文字母+英數底線
		/// </summary>
		public const string USER_ID_REGEX = @"^[a-zA-Z]\w+$";
		/// <summary>
		/// 帳號 正則式 ^[a-zA-Z]\w{7,19}$ 首字英文字母+英數底線 8-20字元
		/// </summary>
		//public const string USER_ID_LENGTH_REGEX = @"^[a-zA-Z]\w{7,19}$";
		public const string USER_ID_LENGTH_REGEX = @"[a-zA-Z_]\w{2,49}$";
		/// <summary>
		/// 密碼 正則式 ^([a-zA-Z])(?=.*[a-zA-Z0-9]).{7,19}$ 首字英文字母+英數混合 8-20字元
		/// </summary>
		public const string PASSWORD_REGEX = @"^[a-zA-Z](?=.*[a-zA-Z0-9])(?=.*\d)(?!.*[\s]).{7,19}$";

		#endregion

		#region const property for export type

		/// <summary>
		/// 匯出格式 excel (application/vnd.ms-excel)
		/// </summary>
		public const string APPLICATION_EXCEL = "application/vnd.ms-excel";

		#endregion

		#region const property for Web View_Count Info

		/// <summary>
		/// 網站最近更新時間 NODE_ID = function_lastUpdate
		/// </summary>
		public const string WEB_LAST_UPDATE_DATE_NODE_ID = "function_lastUpdate";
		/// <summary>
		/// 網站總瀏覽人數 中文 viewcount
		/// </summary>
		public const string WEB_VIEW_TOTAL_TYPE = "viewcount";
		/// <summary>
		/// 網站每日瀏覽人數 中文 viewcount_today
		/// </summary>
		public const string WEB_VIEW_TOTAL_TYPE_TODAY = "viewcount_today";
		/// <summary>
		/// 網站總瀏覽人數 中文 viewcount2
		/// </summary>
		public const string WEB_VIEW_TOTAL_TYPE2 = "viewcount2";
		/// <summary>
		/// 網站每日瀏覽人數 中文 viewcount_today2
		/// </summary>
		public const string WEB_VIEW_TOTAL_TYPE_TODAY2 = "viewcount_today2";
		/// <summary>
		/// 網站瀏覽人數 中文 Session Key = Session_Start
		/// </summary>
		public const string WEB_VIEW_SESSION_START = "Session_Start";
		/// <summary>
		/// 網站瀏覽人數 中文 Session Key = A
		/// </summary>
		public const string WEB_VIEW_A = "A";
		/// <summary>
		/// 網站瀏覽人數 中文 Session Key = B
		/// </summary>
		public const string WEB_VIEW_B = "B";
		/// <summary>
		/// 網站瀏覽人數 中文 Application Key = total
		/// </summary>
		public const string WEB_VIEW_TOTAL = "total";
		/// <summary>
		/// 網站瀏覽人數 中文 Application Key = today
		/// </summary>
		public const string WEB_VIEW_TODAY = "today";
		/// <summary>
		/// 網站瀏覽人數 中文 Application Key = total2
		/// </summary>
		public const string WEB_VIEW_TOTAL2 = "total2";
		/// <summary>
		/// 網站瀏覽人數 中文 Application Key = today2
		/// </summary>
		public const string WEB_VIEW_TODAY2 = "today2";
		#endregion

		#region const property for AD
		/// <summary>
		/// AD sAMAccountName
		/// </summary>
		public const string LDAP_SAMACCOUNTNAME = "sAMAccountName";
		/// <summary>
		/// AD 中文姓名 cn
		/// </summary>
		public const string LDAP_CN = "cn";
		/// <summary>
		/// AD 部門 department
		/// </summary>
		public const string LDAP_DEPARTMENT = "department";
		/// <summary>
		/// AD E-mail mail
		/// </summary>
		public const string LDAP_MAIL = "mail";
		/// <summary>
		/// AD_Sync
		/// </summary>
		public const string LDAP_AD_SYNC = "AD_Sync";

		#endregion

		#region readonly property for Webconfig 前後台共用
		/// <summary>
		/// Default 頁面 Title (根據 webconfig 中 PageTitle 設定)
		/// </summary>
		public static readonly string PAGE_TITLE = GetConfigSetting("PageTitle");
		/// <summary>
		/// 預設的 Smtp Server
		/// </summary>
		public static readonly string DEFAULT_SMTP = GetConfigSetting("Smtp");
		/// <summary>
		/// 預設的寄件者
		/// </summary>
		public static readonly string DEFAULT_MAIL_FROM = GetConfigSetting("MailFrom");
		/// <summary>
		/// 預設的寄件者名稱
		/// </summary>
		public static readonly string DEFAULT_MAIL_FROM_NAME = GetConfigSetting("MailFromName");
		/// <summary>
		/// 預設的 Smtp Server User ID
		/// </summary>
		public static readonly string DEFAULT_SMTP_USER = GetConfigSetting("SmtpUser");
		/// <summary>
		/// 預設的 Smtp Server User PWD
		/// </summary>
		public static readonly string DEFAULT_SMTP_PWD = GetConfigSetting("SmtpPWD");
		/// <summary>
		/// 預設的 Smtp Server Enable SSL
		/// </summary>
		public static readonly bool DEFAULT_SMTP_SSL = GetConfigSetting("SmtpSSL").IsNullOrEmpty();
		/// <summary>
		/// 預設的 Smtp Server Port
		/// </summary>
		public static readonly string DEFAULT_SMTP_PORT = GetConfigSetting("SmtpPort");
		/// <summary>
		/// 上傳檔案路徑(根據webconfig中 UploadPath 的值)
		/// </summary>
		public static readonly string UPLOAD_PATH = GetConfigSetting("UploadPath");
		/// <summary>
		/// 上傳圖片中型縮圖Size (x,y)(根據webconfig中 UploadPicturesMediumSize 的值)
		/// </summary>
		public static readonly string UPLOAD_PICTURES_MEDIUM_SIZE = GetConfigSetting("UploadPicturesMediumSize");
		/// <summary>
		/// 上傳圖片小型縮圖Size (x,y)(根據webconfig中 UploadPicturesSmallSize 的值)
		/// </summary>
		public static readonly string UPLOAD_PICTURES_SMALL_SIZE = GetConfigSetting("UploadPicturesSmallSize");
		/// <summary>
		/// 預設可上傳的檔案大小(根據webconfig中 UploadAttachmentSize 的值)
		/// </summary>
		public static readonly int DEFAULT_ATTACHMENT_SIZE_LIMIT = GetConfigSetting("UploadAttachmentSize").ToInt();
		/// <summary>
		/// Uploadify 元件預設可上傳的檔案大小 (根據webconfig中 UploadifyUploadAttachmentSize 的值)
		/// </summary>
		public static readonly string DEFAULT_UPLOADIFY_ATTACHMENT_SIZE_LIMIT = GetConfigSetting("UploadifyUploadAttachmentSize");
		/// <summary>
		/// 預設圖片讀取網路絕對路徑(根據webconfig中 ImageUrl 的值)
		/// </summary>
		public static readonly string DEFAULT_IMAGE_URL = GetConfigSetting("ImageUrl");
		/// <summary>
		/// 預設網站根絕對路徑(根據webconfig中 RootHttp 的值) - 前台
		/// </summary>
		public static readonly string DEFAULT_ROOT_HTTP = GetConfigSetting("RootHttp");
		/// <summary>
		/// 預設網站根絕對路徑(根據webconfig中 RootHttp 的值) - 前台 
		/// </summary>
		public static readonly string DEFAULT_ROOT_HTTP_AFMC = GetConfigSetting("RootHttp_AFMC");
		/// <summary>
		/// 預設網站根絕對路徑(根據webconfig中 AdminHttp 的值) - 後台
		/// </summary>
		public static readonly string DEFAULT_ADMIN_HTTP = GetConfigSetting("AdminHttp");
		/// <summary>
		/// 預設的列表SIZE (根據 webconfig 中 DefaultPage 設定)
		/// </summary>
		public static readonly int DEFAULT_PAGE_SIZE = GetConfigSetting("DefaultPage").ToInt();
		/// <summary>
		/// 預設的 SessionTimeOut 分鐘(根據 webconfig 中 SessionTimeOut 設定)
		/// </summary>
		public static readonly int SESSION_TIME_OUT = Function.GetConfigSetting("SessionTimeOut").ToInt();
		/// <summary>
		/// 上傳檔案暫存實體路徑根目錄(根據config中 TempPath 的值)
		/// </summary>
		public static readonly string TEMP_UPLOAD_PATH = GetConfigSetting("TempPath");
		/// <summary>
		/// LDAP 連線路徑
		/// </summary>
		public static readonly string LDAP_PATH = GetConfigSetting("LDAD_Path");
		/// <summary>
		/// LDAP 連線Domain
		/// </summary>
		public static readonly string LDAP_DOMAIN = GetConfigSetting("LDAD_Domain");
		/// <summary>
		/// LDAP 連線帳號
		/// </summary>
		public static readonly string LDAP_ACCOUNT = GetConfigSetting("LDAD_Account");
		/// <summary>
		/// LDAP 連線密碼
		/// </summary>
		public static readonly string LDAP_PASSWORD = GetConfigSetting("LDAD_Password");
		/// <summary>
		/// 除錯模式(1:是 0:否)
		/// </summary>
		public static readonly string DEBUG_MODE = GetConfigSetting("DEBUG_MODE");
		/// <summary>
		/// 驗證模式(AD or DB)
		/// </summary>
		public static readonly string AUTH_MODE = GetConfigSetting("AUTH_MODE");
		/// <summary>
		/// 物品低於庫存量通知信 和 物品審核通知信 兩者寄送間隔時間 單位:毫秒(1000=1秒)
		/// </summary>
		public static readonly string TIME_GAP = GetConfigSetting("TIME_GAP");
		#endregion

		#region readonly property
		/// <summary>
		/// DB TABLE NAMES 集合
		/// <para>0:NODE</para>
		/// <para>1:ARTICLE</para>
		/// <para>2:ARTICLE_PLUG</para>
		/// <para>3:ATTACHMENT</para>
		/// <para>4:AUTHORITY</para>
		/// <para>5:COUNTER</para>
		/// <para>6:LOG</para>
		/// <para>7:MESSAGE</para>
		/// <para>8:MESSAGE_LOG</para>
		/// <para>9:PARAGRAPH</para>
		/// <para>10:PLUS</para>
		/// <para>11:ROLE_GROUP</para>
		/// <para>12:ROLE_USER_MAPPING</para>
		/// <para>13:SYSUSER</para>
		/// <para>14:USER</para>
		/// <para>15:DATA1</para>
		/// <para>16:DATA2</para>
		/// <para>17:DATA3</para>
		/// <para>18:DATA4</para>
		/// <para>19:DATA5</para>
		/// <para>20:DATA6</para>
		/// <para>21:DATA7</para>
		/// <para>22:DATA8</para>
		/// </summary>
		public static readonly string[] TABLE_NAMES = { "NODE", "ARTICLE","ARTICLE_PLUG","ATTACHMENT"
		,"AUTHORITY","COUNTER","LOG","MESSAGE","MESSAGE_LOG","PARAGRAPH","PLUS","ROLE_GROUP","ROLE_USER_MAPPING",
			"SYSUSER","USER","DATA1","DATA2","DATA3","DATA4","DATA5","DATA6","DATA7","DATA8"};
		/// <summary>
		/// 系統預設Error訊息(系統發生錯誤，請聯絡系統管理員!!)
		/// </summary>
		public static readonly string DEFAULT_ERROR = "系統發生錯誤，請聯絡系統管理員!!";
		/// <summary>
		/// (首字不分大小寫英文 + 英數底線組成)
		/// </summary>
		public static readonly string DEFAULT_USERID_ERROR = "(首字不分大小寫英文 + 英數底線組成)";
		/// <summary>
		/// 預設的ajax形式刪除回應訊息strFormat
		/// </summary>
		public static readonly string DEFAULT_AJAX_MESSAGE = "<td align='center' colspan='{0}' style='background-color:#ddd'><font color='red'>已刪除</font></td>";
		/// <summary>
		/// Error訊息(紅字的預設錯誤訊息!!)
		/// </summary>
		public static readonly string ERROR_MESSAGE = string.Format("<font color='red'>{0}</font>", DEFAULT_ERROR);

		/// <summary>
		/// 系統預設日期(資料庫最小日期 1753/01/01)
		/// </summary>
		public static readonly DateTime DEFAULT_TIME = DateTime.Parse("1753/01/01");
		/// <summary>
		/// 今年第一天(yyyy/01/01)
		/// </summary>
		public static readonly DateTime THIS_YEAR_FIRST_DATE = new DateTime(DateTime.Today.Year, 1, 1);
		/// <summary>
		/// 上個月第一天(yyyy/MM/01)
		/// </summary>
		public static readonly DateTime LAST_MONTH_FIRST_DATE = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
		/// <summary>
		/// 上個月最後一天(yyyy/MM/dd)
		/// </summary>
		public static readonly DateTime LAST_MONTHR_LAST_DATE = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, DateTime.Today.AddDays(1 - DateTime.Today.Day).AddSeconds(-1).Day);
		/// <summary>
		/// 本月第一天(yyyy/MM/01)
		/// </summary>
		public static readonly DateTime THIS_MONTH_FIRST_DATE = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
		/// <summary>
		/// 本月最後一天(yyyy/MM/dd)
		/// </summary>
		public static readonly DateTime THIS_MONTHR_LAST_DATE = DateTime.Today.AddMonths(1).AddDays(-DateTime.Today.AddMonths(1).Day);
		/// <summary>
		/// 常用影音格式 "mov", "avi", "mpg", "dat", "rm", "vob", "wmv", "rmvb", "asf", "mp4", "swf", "flv", "m2t/m2ts", "mid", "wav", "mp3", "ra", "aac", "ogg"
		/// </summary>
		public static readonly string[] AUDIO_FORMAT = { "mov", "avi", "mpg", "dat", "rm", "vob", "wmv", "rmvb", "asf", "mp4", "swf", "flv", "m2t/m2ts", "mid", "wav", "mp3", "ra", "aac", "ogg" };

		/// <summary>
		/// 預設 無圖片 路徑
		/// </summary>
		public static string DEFAULT_NOIMG_URL = Path.Combine(DEFAULT_ROOT_HTTP, "images/noimg.jpg");
		#endregion

		#region readonly property for project
		/// <summary>
		/// 活動相關參數 集合
		/// <para>0 活動識別ID active20190711_20190831</para>
		/// <para>1 活動日期起 yyyy/MM/dd</para>
		/// <para>2 活動日期迄 yyyy/MM/dd</para>
		/// <para>3 每日可遊玩次數 10</para>
		/// <para>4 獎品數目 100</para>
		/// <para>5 連線userIP session key logonIP</para>
		/// <para>6 活動2識別ID active20180716_20180831</para>
		/// <para>7 活動2日期起 yyyy/MM/dd</para>
		/// <para>8 活動2日期迄 yyyy/MM/dd</para>
		/// <para>9 登入者email</para>
		/// <para>10 活動3識別ID active20180716_20180930</para>
		/// <para>11 活動3日期起 yyyy/MM/dd</para>
		/// <para>12 活動3日期起 yyyy/MM/dd</para>
		/// <para>13 每日可投稿次數 8</para>
		/// </summary>
		public static readonly string[] PROJECT_KEY = {
 "active20190711_20190831",
 "2019/07/11 10:00",
 "2019/08/31 23:59",
 "10",
 "100",
 "logonIP",
 "active20180703_20180831",
 "2018/07/03 10:00",
 "2018/08/31 18:00",
 "email",
 "active20180716_20180930",
 "2018/07/16 10:00",
 "2018/09/30 18:00",
 "8"
			};
		#endregion

		#region Static List property

		/// <summary>
		/// 後端SYSUSER人員 對照
		/// </summary>
		public static List<SYSUSER> SysUserList;
		/// <summary>
		/// 所有NODE資料
		/// </summary>
		public static List<NODE> NodeList;
		/// <summary>
		/// 功能NODE資料 PARENT_ID='admin_function'
		/// </summary>
		public static List<NODE> FunctionNodeList;
		/// <summary>
		/// DATA1資料
		/// </summary>
		public static List<DATA1> Data1List;
		/// <summary>
		/// DATA2資料
		/// </summary>
		public static List<DATA2> Data2List;
		/// <summary>
		/// DATA3資料
		/// </summary>
		public static List<DATA3> Data3List;
		/// <summary>
		/// 活動日期range資料
		/// </summary>
		public static List<string> DataRangeList;
		/// <summary>
		/// 星期幾文字資料
		/// </summary>
		public static List<string> WeekDayList = new List<string> { "(日)", "(一)", "(二)", "(三)", "(四)", "(五)", "(六)" };
        /// <summary>
		/// PARAGRAPH資料
		/// </summary>
		public static List<PARAGRAPH> PhList;
        /// <summary>
        /// 所有使用者的權限 20201118 ting add
        /// </summary>
        public static Dictionary<string, List<AuthorityRight>> AllUsersAuthorityRight;
		#endregion

		#region Static property
		/// <summary>
		/// 今日是否有更新過
		/// </summary>
		public static bool IsTodayUpdated = false;
		/// <summary>
		/// 鎖定專用
		/// </summary>
		public static object ThisLock = new object();

		/// <summary>
		/// 目前所在的路徑
		/// </summary>
		public static string path;

		/// <summary>
		/// 在log檔新增備註
		/// </summary>
		public static string logRemark;

		#endregion

		#region Static Function For DB Model

		/// <summary>
		/// 取得產生的LOG
		/// </summary>
		/// <param name="tableName">操作的DB name</param>
		/// <param name="userName">操作者</param>
		/// <param name="msg">要記錄的訊息 會存於 CONTENT1 (500字內)</param>
		/// <param name="tableID">自行指定tableID</param>
		/// <returns></returns>
		public static LOG GetLog(string tableName, string userName, string msg, string tableID)
		{
			LOG log = new LOG();
			log.LOG_ID = GetGuid();
			log.TABLE_ID = tableID;
			log.TABLE_NAME = tableName;//操作的table name
			log.USER_ID = userName;//登入者的id
			log.CREATE_DATE = DateTime.Now;
			log.CONTENT1 = msg;
			return log;
		}

		/// <summary>
		/// 修改LOG記錄
		/// </summary>
		/// <param name="LogID"></param>
		/// <param name="USER_ID"></param>
		/// <param name="CONTENT2"></param>
		/// <returns></returns>
		public static bool UpdateLog(string LogID, string USER_ID, string CONTENT2)
		{
			bool bRet = false;
			if (!LogID.IsNullOrEmpty() && !CONTENT2.IsNullOrEmpty())
			{
				using (DBEntities db = new DBEntities())
				{
					LOG log = db.LOG.FirstOrDefault(p => p.LOG_ID == LogID);
					if (log != null)
					{
						if (!USER_ID.IsNullOrEmpty() && log.USER_ID.IsNullOrEmpty())
						{
							log.USER_ID = USER_ID;
						}
						log.CONTENT2 = CONTENT2.ToMyString();
						db.SaveChanges();
					}
				}
			}
			return bRet;
		}

		/// <summary>
		/// 取得子Node List
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="nodeID"></param>
		/// <param name="isIncludeSelf"></param>
		/// <returns></returns>
		public static List<NODE> GetChildrenNodes(List<NODE> nodes, string nodeID, bool isIncludeSelf = false)
		{
			List<NODE> _nodes = nodes == null ? new List<NODE>() : nodes;
			if (nodeID.IsNullOrEmpty())
				return _nodes;
			if (isIncludeSelf)
			{
				_nodes.Add(FunctionNodeList.Find(p => p.ID.Equals(nodeID)));
			}
			foreach (NODE n in FunctionNodeList.Where(p => p.PARENT_ID.CheckStringValue(nodeID)).OrderBy(p => p.ORDER))
			{
				_nodes.Add(n);
				_nodes = GetChildrenNodes(_nodes, n.ID, false);
			}
			return _nodes;
		}

		/// <summary>
		/// 取得母Node List
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="nodeID"></param>
		/// <param name="isIncludeSelf"></param>
		/// <returns></returns>
		public static List<NODE> GetParentNodes(List<NODE> nodes, string nodeID, bool isIncludeSelf = false)
		{
			List<NODE> _nodes = nodes == null ? new List<NODE>() : nodes;
			NODE n = FunctionNodeList.Find(p => p.ID.Equals(nodeID));
			if (n == null)
			{
				return _nodes;
			}
			else
			{
				if (isIncludeSelf)
				{
					_nodes.Add(n);
				}
				return GetParentNodes(_nodes, n.PARENT_ID, true);
			}
		}

		#endregion

		#region Static Function

		/// <summary>
		/// 取得NODE的TITLE
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetNodeTitle(string id)
		{
			NODE n = NodeList.FirstOrDefault(p => p.ID.Equals(id));
			return n == null ? string.Empty : n.TITLE;
		}

        /// <summary>
		/// 取得NODE的value by Lan
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetNodeValueByLang(string id)
        {
            NODE n = NodeList.FirstOrDefault(p => p.ID.Equals(id));
            if (n == null) return string.Empty;
            switch (Function.CultureName())
            {
                default:
                case CultureHelper.ZH_TW:
                    return n.TITLE;

                case CultureHelper.EN_US:
                    return n.CONTENT2;
            }
        }

        /// <summary>
        /// 從 NodeList 取得NODE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static NODE GetNode(string id)
		{
			NODE n = NodeList.FirstOrDefault(p => p.ID.Equals(id));
			return n ?? null;
		}
		/// <summary>
		/// 取得實際路徑
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetPhysicalPath(string path)
		{
			return Path.GetFullPath(string.Format("{0}{1}", System.AppDomain.CurrentDomain.BaseDirectory, path));
		}

		/// <summary>
		/// 取得SYSUSER的NAME
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetSysUserName(string id)
		{
			SYSUSER sys = SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(id));
			return sys == null ? string.Empty : sys.NAME;
		}

		/// <summary>
		/// 取得SYSUSER的Department
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetSysUserDept(string id)
		{
			SYSUSER sys = SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(id));
			return sys == null ? string.Empty : sys.CONTENT2;
		}

		/// <summary>
		/// 取得SYSUSER的EMAIL
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string GetSysUserEmail(string id)
		{
			SYSUSER sys = SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(id));
			return sys == null ? string.Empty : sys.EMAIL;
		}

		/// <summary>
		/// 取得SYSUSER From SysUserList
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static SYSUSER GetSysUserByID(string id)
		{
			SYSUSER sys = SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(id));
			return sys ?? null;
		}

		/// <summary>
		/// 取得 Appconfig/Webconfig 中 appSettings 的值
		/// </summary>
		/// <param name="key">key</param>
		/// <returns></returns>
		public static string GetConfigSetting(string key)
		{
			//System.Web.Configuration.WebConfigurationManager
			return ConfigurationManager.AppSettings[key];
		}
		/// <summary>
		/// 目前語系
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string CultureName()
		{
			return System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
		}

		/// <summary>
		/// 取得Guid format="N" without '-'
		/// </summary>
		/// <returns></returns>
		public static string GetGuid()
		{
			return GetGuid("N");
		}
		/// <summary>
		/// 取得Guid 指定format
		/// </summary>
		/// <param name="format">"N", "D", "B", "P", "X", null or "" ="D"</param>
		/// <returns></returns>
		public static string GetGuid(string format)
		{
			return Guid.NewGuid().ToString(format);
		}

		/// <summary>
		/// 產生類似 1534X-45S6C-A4597-C2786 的隨機字串 不會有數字0
		/// </summary>
		/// <returns></returns>
		public static string GetRandom()
		{
			StringBuilder sb = new StringBuilder();
			Random serial = new Random();
			Random s = new Random();
			char sn = new char();
			for (int i = 1; i < 5; i++)
			{
				for (int j = 1; j <= 5; j++)
				{
					if (s.Next(0, 2) == 1)
					{
						sn = (char)serial.Next(65, 91);//英文(65~91 A~Z)
					}
					else
					{
						sn = (char)serial.Next(49, 58);//數字(48~58 0~9)
					}
					sb.Append(sn);
				}
				sb.Append("-");
			}
			return sb.ToString().Substring(0, sb.Length - 1);
		}

		/// <summary>
		/// 跳出操作訊息(預設系統錯誤訊息)
		/// </summary>
		public static string ShowMessage()
		{
			return ShowMessage(DEFAULT_ERROR);
		}

		/// <summary>
		/// 跳出操作訊息
		/// </summary>
		/// <param name="msg">自訂訊息</param>
		public static string ShowMessage(string msg)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.AppendLine("$(function(){");
			sb.AppendFormat("alert('{0}');", msg);
			sb.Append("});");
			sb.AppendLine("</script>");
			return sb.ToString();
		}

		/// <summary>
		/// 跳出操作訊息前對client端的target給value
		/// </summary>
		/// <param name="target">目標id</param>
		/// <param name="value">值</param>
		/// <param name="msg">訊息</param>
		public static string ShowMessage(string target, string value, string msg)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.Append("setTimeout(function(){");
			sb.AppendFormat("$('#{0}').val('{1}');", target, value);
			sb.AppendFormat("alert('{0}');", msg);
			sb.Append("},0);");
			sb.AppendLine("</script>");
			return sb.ToString();
		}

		/// <summary>
		/// 跳出操作訊息後轉到url
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="url"></param>
		public static string ShowMessage(string msg, string url)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.Append("setTimeout(function(){");
			sb.AppendFormat("alert('{0}');", msg);
			sb.AppendFormat("location.href='{0}';", url);
			sb.Append("},0);");
			sb.AppendLine("</script>");
			return sb.ToString();
		}
		/// <summary>
		/// 跳出可選擇的操作訊息後轉到url
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="url"></param>
		public static string ShowConfirmMessage(string msg, string url)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.Append("setTimeout(function(){");
			sb.AppendFormat("var result=confirm('{0}');", msg);
			sb.Append("if (result == true){");
			sb.AppendFormat("location.href='{0}';", url);
			sb.Append("}},0);");
			sb.AppendLine("</script>");
			return sb.ToString();
		}

		/// <summary>
		/// 跳出Ajax操作訊息
		/// </summary>
		public static string ShowAjaxMessage()
		{
			return ShowAjaxMessage(Function.DEFAULT_ERROR);
		}

		/// <summary>
		/// 跳出Ajax操作訊息
		/// </summary>
		public static string ShowAjaxMessage(string msg)
		{
			return string.Format("alert('{0}')", msg);
		}

		/// <summary>
		/// 實作SHA1編碼(可直接改用 System.Web.Helpers.Crypto.SHA1)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string HashPassword(string str)
		{
			System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
			System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
			hash.ComputeHash(encoder.GetBytes(str.Trim()));
			return Convert.ToBase64String(hash.Hash);
		}

		/// <summary>
		/// 建立動態Type
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="moduleName"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static TypeBuilder CreateTypeBuilder(
			string assemblyName, string moduleName, string typeName)
		{
			TypeBuilder typeBuilder = AppDomain
				.CurrentDomain
				.DefineDynamicAssembly(new AssemblyName(assemblyName),
									   AssemblyBuilderAccess.Run)
				.DefineDynamicModule(moduleName)
				.DefineType(typeName, TypeAttributes.Public);
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			return typeBuilder;
		}

		/// <summary>
		/// 建立動態Type 欄位
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyType"></param>
		public static void CreateAutoImplementedProperty(
			TypeBuilder builder, string propertyName, Type propertyType)
		{
			const string PrivateFieldPrefix = "m_";
			const string GetterPrefix = "get_";
			const string SetterPrefix = "set_";

			// Generate the field.
			FieldBuilder fieldBuilder = builder.DefineField(
				string.Concat(PrivateFieldPrefix, propertyName),
							  propertyType, FieldAttributes.Private);

			// Generate the property
			PropertyBuilder propertyBuilder = builder.DefineProperty(
				propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

			// Property getter and setter attributes.
			MethodAttributes propertyMethodAttributes =
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig;

			// Define the getter method.
			MethodBuilder getterMethod = builder.DefineMethod(
				string.Concat(GetterPrefix, propertyName),
				propertyMethodAttributes, propertyType, Type.EmptyTypes);

			// Emit the IL code.
			// ldarg.0
			// ldfld,_field
			// ret
			ILGenerator getterILCode = getterMethod.GetILGenerator();
			getterILCode.Emit(OpCodes.Ldarg_0);
			getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
			getterILCode.Emit(OpCodes.Ret);

			// Define the setter method.
			MethodBuilder setterMethod = builder.DefineMethod(
				string.Concat(SetterPrefix, propertyName),
				propertyMethodAttributes, null, new Type[] { propertyType });

			// Emit the IL code.
			// ldarg.0
			// ldarg.1
			// stfld,_field
			// ret
			ILGenerator setterILCode = setterMethod.GetILGenerator();
			setterILCode.Emit(OpCodes.Ldarg_0);
			setterILCode.Emit(OpCodes.Ldarg_1);
			setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
			setterILCode.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getterMethod);
			propertyBuilder.SetSetMethod(setterMethod);
		}

		/// <summary>
		/// 建立動態List foreach需使用dynamic
		/// </summary>
		/// <param name="Type"></param>
		/// <returns></returns>
		public static IList CreateDynamicList(Type type)
		{
			Type listType = typeof(List<>).MakeGenericType(type);
			return (IList)Activator.CreateInstance(listType);
		}

		/// <summary>
		/// 取得圖片/檔案上傳路徑
		/// </summary>
		/// <param name="TrueUrl">真實路徑</param>
		/// <returns></returns>
		public static string GetUploadPath(bool TrueUrl = false)
		{
			if (TrueUrl)
				return Path.GetFullPath(string.Format("{0}{1}", System.AppDomain.CurrentDomain.BaseDirectory, GetUploadPath()));
			else
				return Path.Combine(UPLOAD_PATH).Replace("\\", "/");//為了firefox加的replace
		}

		/// <summary>
		/// 取得實際路徑(完整)
		/// </summary>
		/// <param name="directoryName"></param>
		/// <returns></returns>
		public static string GetRealityPath(string directoryName)
		{
			return System.Web.Hosting.HostingEnvironment.MapPath(Path.Combine(UPLOAD_PATH, directoryName));
		}

		/// <summary>
		/// Ajax形式的刪除回應訊息
		/// </summary>
		/// <param name="colspan">td的colspan數</param>
		/// <returns></returns>
		public static string AjaxDeleteMessage(int colspan)
		{
			return string.Format(DEFAULT_AJAX_MESSAGE, colspan);
		}

		/// <summary>
		/// 產生Google圖表的資料 Json
		/// </summary>
		/// <param name="cols">資料設定</param>
		/// <param name="rows">資料內容</param>
		/// <returns></returns>
		public static string ChartToJson(string[,] cols, string[,] rows)
		{
			//http://code.google.com/intl/zh-TW/apis/chart/interactive/docs/reference.html
			//type: 'string' 'number' 'boolean' 'date' 'datetime' 'timeofday'
			StringBuilder sb = new StringBuilder();
			sb.Append("{\"cols\": [");
			for (int i = 0; i < cols.GetLength(0); i++)
			{
				for (int j = 0; j < cols.GetLength(1); j += 2)
				{
					sb.Append((i == 0 ? string.Empty : ",") + "{\"id\":\"\",\"label\":\"" + cols[i, j] + "\",\"pattern\":\"\",\"type\":\"" + cols[i, j + 1] + "\"}");
				}
			}
			sb.Append("], \"rows\": [");
			for (int i = 0; i < rows.GetLength(0); i++)
			{
				sb.Append((i == 0 ? string.Empty : ",") + "{\"c\":[");
				for (int j = 0; j < rows.GetLength(1); j++)
				{
					string v = rows[i, j];
					int i_v = 0;
					if (int.TryParse(v, out i_v))
					{
						if (0 == i_v)
						{
							sb.Append((j == 0 ? string.Empty : ",") + "{\"v\":null,\"f\":null}");
						}
						else
						{
							sb.Append((j == 0 ? string.Empty : ",") + "{\"v\":" + rows[i, j] + ",\"f\":null}");
						}
					}
					else
					{
						sb.Append((j == 0 ? string.Empty : ",") + "{\"v\":\"" + rows[i, j] + "\",\"f\":null}");
					}
				}
				sb.Append("]}");
			}
			sb.Append("]}");

			return sb.ToString();
		}

		/// <summary>
		/// 產生GoogleCalendar網址-增加日曆
		/// </summary>
		/// <param name="start">start</param>
		/// <param name="end">end</param>
		/// <param name="text">text</param>
		/// <param name="details">details</param>
		/// <param name="location">location</param>
		/// <returns></returns>
		public static string GetGoogleCalendarShareString(DateTime? start, DateTime? end, string text, string details, string location)
		{
			//https://calendar.google.com/calendar/r/eventedit
			return $"https://calendar.google.com/calendar/r/eventedit?text={text}&dates={start.ToDefaultStringGoogleCalendar()}/{end.ToDefaultStringGoogleCalendar()}&details={details}&location={location}";
		}

		/// <summary>
		/// 縮圖
		/// </summary>
		/// <param name="image">原圖</param>
		/// <param name="maxWidth">最大寬</param>
		/// <param name="maxHeight">最大高</param>
		/// <returns>改變大小後的圖</returns>
		public static WebImage SetImageSize(WebImage image, int maxWidth, int maxHeight)
		{
			int _width = image.Width;
			int _height = image.Height;
			if (image.Width > 0 && image.Height > 0)
			{
				if (image.Width / image.Height >= maxWidth / maxHeight)
				{
					if (image.Width > maxWidth)
					{
						_width = maxWidth;
						_height = (image.Height * maxWidth) / image.Width;
					}
				}
				else
				{
					if (image.Height > maxHeight)
					{
						_height = maxHeight;
						_width = (image.Width * maxHeight) / image.Height;
					}
				}
			}
			return image.Resize(_width, _height, true, true).Crop(1, 1);
		}

		/// <summary>
		/// Use System.Net.Mail 寄mail (收件者多人)
		/// </summary>
		/// <param name="to">收件者集合</param>
		/// <param name="args">參數集合 0:from 1:fromName 2:subject 3:body</param>
		/// <returns></returns>
		public static bool SendMail(LetterModel model)
		{
			bool bDebug = Function.DEBUG_MODE.Equals("1") ? true : false;
			try
			{
				int PORT = DEFAULT_SMTP_PORT.IsNullOrEmpty() ? 25 : DEFAULT_SMTP_PORT.ToInt();
				using (SmtpClient smtp = new SmtpClient(DEFAULT_SMTP, PORT))
				{
					smtp.EnableSsl = DEFAULT_SMTP_SSL;
					if (!Function.DEFAULT_SMTP_USER.IsNullOrEmpty())
					{
						smtp.Credentials = new System.Net.NetworkCredential(Function.DEFAULT_SMTP_USER, Function.DEFAULT_SMTP_PWD);
					}
					using (MailMessage mail = new MailMessage())
					{
						mail.SubjectEncoding = Encoding.UTF8;
						mail.IsBodyHtml = true;
						mail.BodyEncoding = Encoding.UTF8;
						string m_from = model.Sender.IsNullOrEmpty() ? DEFAULT_MAIL_FROM : model.Sender;
						string m_fromName = model.SenderName.IsNullOrEmpty() ? DEFAULT_MAIL_FROM_NAME : model.SenderName;
						mail.From = new MailAddress(m_from, m_fromName, Encoding.UTF8);
						mail.Subject = model.Subject.ToMyString();
						mail.Body = model.Body.ToMyString();
						if (model.RecipientList != null && model.RecipientList.Count > 0)
						{
							if (bDebug)
							{
								mail.Body = mail.Body + "<br /><br /><br />原收件者：" + string.Join("、", model.RecipientList);
								foreach (string m_to in Function.GetConfigSetting("notice").ToSplit().ToList())
								{
									mail.To.Clear();
									mail.To.Add(new MailAddress(m_to.ToMyString(), m_to.ToMyString(), Encoding.UTF8));
									smtp.Send(mail);
								}
							}
							else
							{
								foreach (string m_to in model.RecipientList.AsParallel())
								{
									mail.To.Clear();
									if (!m_to.IsNullOrEmpty())
									{
										mail.To.Add(new MailAddress(m_to.ToMyString(), m_to.ToMyString(), Encoding.UTF8));
										smtp.Send(mail);
									}
								}
							}
						}
						else if (model.RecipientNameList != null && model.RecipientNameList.Count > 0)
						{
							if (bDebug)
							{
								mail.Body = mail.Body + "<br /><br /><br />原收件者：" + string.Join("、", model.RecipientNameList.Select(p => p.Key + ":" + p.Value));
								foreach (string m_to in Function.GetConfigSetting("notice").ToSplit().ToList())
								{
									mail.To.Add(new MailAddress(m_to.ToMyString(), m_to.ToMyString(), Encoding.UTF8));
									smtp.Send(mail);
								}
							}
							else
							{
								foreach (KeyValuePair<string, string> kvp in model.RecipientNameList.AsParallel())
								{
									mail.To.Clear();
									mail.To.Add(new MailAddress(kvp.Key.ToMyString(), kvp.Value.ToMyString(), Encoding.UTF8));
									smtp.Send(mail);
								}
							}
						}
						else
						{
							LogSystem.InitLogSystem();
							LogSystem.WriteLine(string.Format("SendMail 時發生錯誤！Message={0}", "沒有收件者！"));
							LogSystem.CloseUnderlayingStream();
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.InitLogSystem();
				LogSystem.WriteLine(string.Format("SendMail 時發生錯誤！Message={0}", ex.Message));
				LogSystem.CloseUnderlayingStream();
				return false;
			}
			return true;
		}



		/// <summary>
		/// 取得Dictionary string,string 的值
		/// </summary>
		/// <param name="dictionary">target dictionary</param>
		/// <param name="key">key</param>
		/// <returns>if key is not exist then return string.Empty</returns>
		public static string GetDictionaryValue(Dictionary<string, string> dictionary, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			if (dictionary.Any(k => k.Key.CheckStringValue(key)))
			{
				return dictionary.FirstOrDefault(k => k.Key.CheckStringValue(key)).Value;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// 取得IDictionary string,object 的值
		/// </summary>
		/// <param name="dictionary">target dictionary</param>
		/// <param name="key">key</param>
		/// <returns>if key is not exist then return string.Empty</returns>
		public static string GetDictionaryObjectValue(IDictionary<string, object> dictionary, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			if (dictionary.Any(k => k.Key.CheckStringValue(key)))
			{
				return dictionary.FirstOrDefault(k => k.Key.CheckStringValue(key)).Value.ToMyString();
			}
			else
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// 字串解密(DES)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		/// <returns></returns>
		public static string GetDecryptString(string source, string key, string iv)
		{
			byte[] sourceBytes = Convert.FromBase64String(source);
			MemoryStream ms = new MemoryStream(sourceBytes.Length);
			DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
			des.Key = Encoding.ASCII.GetBytes(key);
			des.IV = Encoding.ASCII.GetBytes(iv);
			ICryptoTransform CT = des.CreateDecryptor();
			CryptoStream cs = new CryptoStream(ms, CT, CryptoStreamMode.Write);
			cs.Write(sourceBytes, 0, sourceBytes.Length);
			cs.FlushFinalBlock();
			cs.Close();
			string rc = Encoding.UTF8.GetString(ms.ToArray());
			ms.Close();
			return rc;
		}

		/// <summary>
		/// 字串加密(DES)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		/// <returns></returns>
		public static string GetEncryotString(string source, string key, string iv)
		{
			MemoryStream ms = new MemoryStream();
			DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
			des.Key = Encoding.ASCII.GetBytes(key);
			des.IV = Encoding.ASCII.GetBytes(iv);
			ICryptoTransform CT = des.CreateEncryptor();
			CryptoStream cs = new CryptoStream(ms, CT, CryptoStreamMode.Write);

			byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
			cs.Write(sourceBytes, 0, sourceBytes.Length);
			cs.FlushFinalBlock();
			cs.Close();
			return Convert.ToBase64String(ms.ToArray());
		}
		/// <summary>
		/// MD5 加密
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <param name="isLower">true = 小寫 false = 大寫</param>
		/// <returns></returns>
		public static string GetMD5EncryotString(string source, string key, bool isLower = true)
		{
			MD5 md5 = MD5.Create();
			byte[] m = md5.ComputeHash(Encoding.UTF8.GetBytes(source + key));
			StringBuilder s = new StringBuilder();
			foreach (byte by in m)
			{
				if (isLower)
				{
					s.Append(by.ToString("x2"));
				}
				else
				{
					s.Append(by.ToString("X2"));
				}
			}
			return s.ToString();


			//MD5 md5 = MD5.Create();//建立一個MD5
			////byte[] source = Encoding.Default.GetBytes(source+ key);//將字串轉為Byte[]
			//byte[] crypto = md5.ComputeHash(Encoding.Default.GetBytes(source + key));//進行MD5加密
			//return Convert.ToBase64String(crypto) ?? "";//把加密後的字串從Byte[]轉為字串
			////Response.Write("MD5加密:  " + result);//輸出結果

			/*
			 參考官方網址:https://msdn.microsoft.com/zh-tw/library/system.security.cryptography.md5(v=vs.110).aspx
			 其他參考:https://dotblogs.com.tw/mrsunboss/2013/04/07/99955
			 */
		}
		/// <summary>
		/// 轉換 StopWatch Time To String
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		public static string ConvertStopWatchTime(TimeSpan ts)
		{
			return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
		}

		/// <summary>
		/// 取得兩個日期間的天數
		/// </summary>
		/// <param name="start">起始日</param>
		/// <param name="end">結束日</param>
		/// <returns></returns>
		public static int GetDaysBetweenTwoDates(DateTime start, DateTime end)
		{
			return new TimeSpan(end.Ticks - start.Ticks).Days;
		}

		/// <summary>
		/// 檢查value1 == value2
		/// </summary>
		/// <param name="value1">比較值1</param>
		/// <param name="value2">比較值2</param>
		/// <param name="isReturnSelect">回傳 True:selected；False:checked</param>
		/// <returns></returns>
		public static string IsSelect(string value1, string value2, bool isReturnSelect = true)
		{
			string _returnValue = isReturnSelect ? "selected" : "checked";
			return value1.CheckStringValue(value2) ? _returnValue : string.Empty;
		}

		/// <summary>
		/// 設定電話或手機顯示格式
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		public static string GetTelPhoneFormat(string value1, string value2)
		{
			if (!value1.IsNullOrEmpty() || !value2.IsNullOrEmpty())
			{
				return string.Format(DEFAULT_FORMAT_TEL_PHONE, value1, value2);
			}
			return string.Empty;
		}

		/// <summary>
		/// 設定日期區間顯示格式
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		public static string GetDateRangeFormat(string value1, string value2)
		{
			if (!value1.IsNullOrEmpty() || !value2.IsNullOrEmpty())
			{
				return string.Format(DEFAULT_FORMAT_DATE_RANGE, value1, value2);
			}
			return string.Empty;
		}

		/// <summary>
		/// 設定日期區間顯示格式 日期同一天則不顯示後面的日期
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		public static string GetDateRangeFormat2(DateTime? value1, DateTime? value2)
		{
			if (value1.HasValue && value2.HasValue)
			{
				return string.Format(DEFAULT_FORMAT_DATE_RANGE,
					value1.ToDefaultStringWithTime(),
					value1.Value.Date == value2.Value.Date ? value2.Value.ToString("HH:mm") : value2.ToDefaultStringWithTime());
			}
			return string.Empty;
		}

		/// <summary>
		/// 設定時間區間顯示格式
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		public static string GetTimeRangeFormat(string value1, string value2, string value3, string value4)
		{
			if (!value1.IsNullOrEmpty() || !value2.IsNullOrEmpty() || !value3.IsNullOrEmpty() || !value4.IsNullOrEmpty())
			{
				return string.Format(DEFAULT_FORMAT_TIME_RANGE, value1, value2, value3, value4);
			}
			return string.Empty;
		}

		/// <summary>
		/// 取得驗證錯誤訊息
		/// </summary>
		/// <param name="modelState"></param>
		/// <returns></returns>
		public static List<string> GetErrorListFromModelState(ModelStateDictionary modelState)
		{
			return modelState.Values.SelectMany(p => p.Errors)
				.Select(p => p.ErrorMessage).ToList();
		}

		/// <summary>
		/// 取得驗證錯誤訊息 key
		/// </summary>
		/// <param name="modelState"></param>
		/// <returns></returns>
		public static List<string> GetErrorKeyListFromModelState(ModelStateDictionary modelState)
		{
			return modelState.Where(p => p.Value.Errors.Count > 0).Select(p => p.Key).ToList();
		}

		#region AD相關

		/// <summary>
		/// Get AD Model List
		/// </summary>
		/// <returns></returns>
		public static List<AD_Sync_Model> GetAdDataList()
		{
			List<AD_Sync_Model> list = new List<AD_Sync_Model>();
			DirectoryEntry admin = new DirectoryEntry(LDAP_PATH, LDAP_ACCOUNT, LDAP_PASSWORD);
			DirectorySearcher ds = new DirectorySearcher(admin);
			ds.Filter = "(&(&(objectClass=user)(objectClass=person))(!objectClass=computer))";//找user
			ds.PropertiesToLoad.AddRange(new string[] { "cn", "sn", "whencreated", "title", "department", "samaccountname", "mail" });
			foreach (SearchResult sr in ds.FindAll())
			{
				if (sr.Properties["cn"] != null && sr.Properties["sn"] != null)
				{
					AD_Sync_Model ad = new AD_Sync_Model();
					if (sr.Properties["cn"].Count > 0)
						ad.EmployeeName = sr.Properties["cn"][0].ToString();
					//if (sr.Properties["sn"].Count > 0)
					//    sb.AppendFormat("sn={0}{1}", sr.Properties["sn"][0], Environment.NewLine);
					if (sr.Properties["whencreated"].Count > 0)
						ad.CreateTime = Convert.ToDateTime(sr.Properties["whencreated"][0].ToString());
					else
						ad.CreateTime = DEFAULT_TIME;
					if (sr.Properties["mail"].Count > 0)
						ad.Email = sr.Properties["mail"][0].ToString();
					if (sr.Properties[LDAP_DEPARTMENT].Count > 0)
						ad.Department = GetOuByString(sr.Properties[LDAP_DEPARTMENT][0].ToString(), "1");
					if (sr.Properties["sAMAccountName"].Count > 0)
						ad.Account = sr.Properties["sAMAccountName"][0].ToString();
					if (sr.Properties["title"].Count > 0)
						ad.TitleId = sr.Properties["title"][0].ToString();

					if (ad.Department.IsNullOrEmpty() || ad.TitleId.IsNullOrEmpty()) continue;
					list.Add(ad);
				}
			}
			return list;
		}

		/// <summary>
		/// 取得 AD Department List
		/// </summary>
		/// <returns></returns>
		public static List<AD_Department_Model> GetDepartmentList()
		{
			//LogSystem.SinitLogSystem();
			//司機室 Y0, GCB_Test GCB_Test,退離人員 ZZZZZ,保留辦公室 U1,辦公室 T3
			string[] excludeDep = { "廠商", "Domain Controllers", "Y0", "GCB_Test", "ZZZZZ", "U1", "T3" };
			List<AD_Department_Model> list = new List<AD_Department_Model>();
			DirectoryEntry admin = new DirectoryEntry(LDAP_PATH, LDAP_ACCOUNT, LDAP_PASSWORD);
			DirectorySearcher ds = new DirectorySearcher(admin);
			ds.Filter = "(objectClass=organizationalUnit)";//找組織、部門
			ds.PropertiesToLoad.AddRange(new string[] { LDAP_DEPARTMENT, "description" });
			foreach (SearchResult sr in ds.FindAll())
			{
				if (sr.Properties[LDAP_DEPARTMENT] != null && sr.Properties["description"] != null)
				{
					if (sr.Properties[LDAP_DEPARTMENT].Count > 0 && sr.Properties["description"].Count > 0)
					{
						//LogSystem.SwriteLine(string.Format("名稱：{0} ou：{1} 簡化ou：{2}", sr.Properties["description"][0].ToString(), sr.Properties[LDAP_DEPARTMENT][0].ToString(), GetOuByString(sr.Properties[LDAP_DEPARTMENT][0].ToString())));
						if (excludeDep.Contains(GetOuByString(sr.Properties[LDAP_DEPARTMENT][0].ToString())))
							continue;
						AD_Department_Model add = new AD_Department_Model();
						add.Ou = GetOuByString(sr.Properties[LDAP_DEPARTMENT][0].ToString());
						add.Description = sr.Properties["description"][0].ToString();
						list.Add(add);
					}
				}
			}
			//LogSystem.closeUnderlayingStream();
			return list;
		}

		/// <summary>
		/// 取得OU資訊 by 條件
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type">0=部門, 1=個人</param>
		/// <returns></returns>
		public static string GetOuByString(string value, string type = "0")
		{
			string _format = "0".Equals(type) ? LDAP_PATH + "OU=" : "OU=";
			string _value = string.Empty;
			if (!value.IsNullOrEmpty())
			{
				string[] list = value.Split(',');
				_value = list.FirstOrDefault(s => s.StartsWith(_format));
				if (!_value.IsNullOrEmpty())
				{
					_value = _value.Replace(_format, "");
				}
			}
			return _value;
		}

		#endregion

		#endregion

		#region Static Function For Check Authority in Session
		/// <summary>
		/// 檢查是否有權限
		/// </summary>
		/// <param name="id">功能id</param>
		/// <param name="authorities">權限集合</param>
		/// <param name="right">權限類型</param>
		/// <param name="isContinue">忽略權限，測試用</param>
		/// <returns></returns>
		public static bool CheckAuthority(string id, object authorities, Authority_Right right, bool isContinue = false)
		{
			List<AuthorityRight> _list = authorities as List<AuthorityRight> ?? new List<AuthorityRight>();
			bool isValidate = false;
			switch (right)
			{
				case Authority_Right.Search:
					isValidate = _list.Any(p => p.NODE_ID.Equals(id) && p.SEARCH);
					break;
				case Authority_Right.Add:
					isValidate = _list.Any(p => p.NODE_ID.Equals(id) && p.ADD);
					break;
				case Authority_Right.Update:
					isValidate = _list.Any(p => p.NODE_ID.Equals(id) && p.UPDATE);
					break;
				case Authority_Right.Delete:
					isValidate = _list.Any(p => p.NODE_ID.Equals(id) && p.DELETE);
					break;
			}
			return isContinue || isValidate;
		}

		#endregion

		#region Static Function For 特殊功能


		/// <summary>
		/// 取得圖片 exif 拍攝日期
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		//public static PicInfo GetExifDateInfo(Image image)
		//{
		//    PicInfo pic = new PicInfo();
		//    try
		//    {
		//        ExifTagCollection exif = new ExifTagCollection(image);

		//        //取得拍攝日期
		//        ExifTag data = exif.Where(e => e.Id == 36867).FirstOrDefault();
		//        if (data == null)
		//            data = exif.Where(e => e.Id == 36868).FirstOrDefault();
		//        if (data == null)
		//            data = exif.Where(e => e.Id == 306).FirstOrDefault();
		//        pic.ShootDate = data == null ? Function.DEFAULT_TIME : Function.ConvertExifDate(data.Value);
		//        //最大尺寸
		//        pic.MaxSize = Regex.Replace(exif.Where(e => e.Id == 40962).FirstOrDefault().ToString(), "[^0-9]", "")
		//            + " * " + Regex.Replace(exif.Where(e => e.Id == 40963).FirstOrDefault().ToString(), "[^0-9]", "");
		//        //拍攝地點

		//    }
		//    catch (Exception ex)
		//    {
		//        LogSystem.InitLogSystem();
		//        LogSystem.WriteLine(string.Format("取得照片資訊發生錯誤{0}{1}", Environment.NewLine, ex.ToString()));
		//        LogSystem.CloseUnderlayingStream();
		//    }
		//    return pic;
		//}


		/// <summary>
		/// 轉換EXIF的日期string to DateTime
		/// </summary>
		/// <param name="exifDate"></param>
		/// <returns></returns>
		public static DateTime ConvertExifDate(string exifDate)
		{
			if (exifDate.IsNullOrEmpty()) return Function.DEFAULT_TIME;
			string[] year = exifDate.Split(' ')[0].Split(':');
			string[] time = exifDate.Split(' ')[1].Split(':');
			if (year.Length < 3 || time.Length < 3) return Function.DEFAULT_TIME;
			DateTime tmpDateTime;
			if (!DateTime.TryParse(string.Format("{0}/{1}/{2} {3}:{4}:{5}", year[0], year[1], year[2], time[0], time[1], time[2]), out tmpDateTime))
			{
				tmpDateTime = Function.DEFAULT_TIME;
			}
			if (tmpDateTime < Function.DEFAULT_TIME || tmpDateTime > DateTime.MaxValue)
				return Function.DEFAULT_TIME;
			return tmpDateTime;
		}

		#endregion

		#region 專案用
		/// <summary>
		/// 20180208 ting add
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sqlQuery"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static DataTable getDataTable(DBEntities db, string sqlQuery, params object[] parameters)
		{
			DataTable dt = new DataTable();
			var conn = db.Database.Connection;
			var connState = conn.State;
			try
			{
				if (connState != ConnectionState.Open) conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = sqlQuery;
					cmd.CommandType = CommandType.Text;
					if (parameters != null)
						cmd.Parameters.AddRange(parameters);
					using (var reader = cmd.ExecuteReader())
					{
						dt.Load(reader);
					}
				}
			}
			catch (Exception ex)
			{
				//LogSystem.InitLogSystem();
				//LogSystem.WriteLine("(Message) => " + ex.Message);
				//LogSystem.WriteLine("(Source) => " + ex.Source);
				//LogSystem.WriteLine("(StackTrace) => " + ex.StackTrace);
				//LogSystem.WriteLine("(TargetSite) => " + ex.TargetSite);
				//LogSystem.WriteLine("(sqlQuery) => " + sqlQuery);
				//LogSystem.CloseUnderlayingStream();
				//throw;
			}
			finally //成功失敗都會路過
			{
				if (connState != ConnectionState.Closed) conn.Close();
			}
			return dt;
		}

		/// <summary>
		/// 20191204 一開始帳號未限制格式，導致特殊符號（空白、.、&），在管理時會出現異常
		/// 對帳號進行編碼
		/// </summary>
		public static string encodeUserID(string sUserID)
		{
			if (!sUserID.IsNullOrEmpty())
			{
				sUserID = sUserID.Replace(" ", "＄").Replace("&", "＆").Replace(".", "。");
			}
			return sUserID;
		}

		/// <summary>
		/// 20191204 一開始帳號未限制格式，導致特殊符號（空白、.、&），在管理時會出現異常
		/// 對帳號進行解碼
		/// </summary>
		public static string decodeUserID(string sUserID)
		{
			if (!sUserID.IsNullOrEmpty())
			{
				sUserID = sUserID.Replace("＄", " ").Replace("＆", "&").Replace("。", ".");
			}
			return sUserID;
		}
        #endregion

        #region 壓縮圖片

        /// <summary>
        /// 無損壓縮圖片
        /// https://www.zendei.com/article/26681.html
        /// https://blog.csdn.net/qq_16542775/article/details/51792149
        /// </summary>
        /// <param name="sFile">原圖片地址</param>
        /// <param name="dFile">壓縮後保存圖片地址</param>
        /// <param name="flag">壓縮質量（數字越小壓縮率越高）1-100</param>
        /// <param name="size">壓縮後圖片的最大大小</param>
        /// <param name="sfsc">是否是第一次調用</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true)
        {
            using (Image iSource = Image.FromFile(sFile))
            {
                ImageFormat tFormat = iSource.RawFormat;

                //如果是第一次調用，原始圖像的大小小於要壓縮的大小，則直接複製文件，並且返回true
                FileInfo firstFileInfo = new FileInfo(sFile);
                if (sfsc && firstFileInfo.Length < size * 1024)
                {
                    firstFileInfo.CopyTo(dFile, true);
                    return true;
                }

                //int dHeight = iSource.Height / 2;
                //int dWidth = iSource.Width / 2;
                int dHeight = iSource.Height;
                int dWidth = iSource.Width;
                int sW = 0, sH = 0;

                //按比例縮放
                Size tem_size = new Size(iSource.Width, iSource.Height);
                if (tem_size.Width > dHeight || tem_size.Width > dWidth)
                {
                    if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = (dWidth * tem_size.Height) / tem_size.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (tem_size.Width * dHeight) / tem_size.Height;
                    }
                }
                else
                {
                    sW = tem_size.Width;
                    sH = tem_size.Height;
                }

                using (Bitmap ob = new Bitmap(dWidth, dHeight))
                {
                    using (Graphics g = Graphics.FromImage(ob))
                    {
                        g.Clear(Color.WhiteSmoke);
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
                    }

                    //以下代碼為保存圖片時，設置壓縮質量
                    EncoderParameters ep = new EncoderParameters();
                    long[] qy = new long[1];
                    qy[0] = flag;//設置壓縮的比例1-100
                    EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
                    ep.Param[0] = eParam;

                    try
                    {
                        ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                        ImageCodecInfo jpegICIinfo = null;
                        for (int x = 0; x < arrayICI.Length; x++)
                        {
                            if (arrayICI[x].FormatDescription.Equals("JPEG"))
                            {
                                jpegICIinfo = arrayICI[x];
                                break;
                            }
                        }
                        if (jpegICIinfo != null)
                        {
                            ob.Save(dFile, jpegICIinfo, ep); //dFile是壓縮後的新路徑
                            FileInfo fi = new FileInfo(dFile);
                            if (fi.Length > 1024 * size)
                            {
                                flag = flag - 10;
                                CompressImage(sFile, dFile, flag, size, false);
                            }
                        }
                        else
                        {
                            ob.Save(dFile, tFormat);
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        #endregion
        /// <summary>
        /// 取得對應的語系值
        /// </summary>
        /// <param name="twValue"></param>
        /// <param name="enValue"></param>
        /// <returns></returns>
        public static string LangFilter(string twValue, string enValue)
		{
			if (string.IsNullOrEmpty(enValue))
				enValue = twValue;

			switch (CultureHelper.GetCurrentCulture())
			{
				default:
				case CultureHelper.ZH_TW:
					return twValue;
				case CultureHelper.EN_US:
					return enValue;
			}
		}

		public static string GetFilePath(string fileName, bool isTrueUrl = false)
		{
			return Path.Combine(GetUploadPath(isTrueUrl), fileName);
		}

		#region NPOI
		/// <summary>
		/// 建立樣式
		/// </summary>
		/// <param name="book"></param>
		/// <param name="align">水平位置</param>
		/// <param name="valign">垂直位直</param>
		/// <param name="border">框線</param>
		/// <param name="wrap">true:多行文字</param>
		/// <param name="bgColor">背景色，例：HSSFColor.GREY_25_PERCENT.index</param>
		/// <param name="dataFormat">資料顯示格式，如：HSSFDataFormat.GetBuiltinFormat("#,##0")</param>
		/// <param name="fontSize">字型大小</param>
		/// <param name="fontColor">字型顏色，例：HSSFColor.BLUE.index</param>
		/// <param name="fontBold">true:粗體</param>
		/// <param name="fontName">字型</param>
		/// <returns></returns>
		public static HSSFCellStyle NPOI_SetCellStyle(HSSFWorkbook book
			, HorizontalAlignment align = HorizontalAlignment.CENTER, VerticalAlignment valign = VerticalAlignment.CENTER
			, bool border = false, bool wrap = true, short bgColor = -1, short dataFormat = -1
			, short fontSize = -1, short fontColor = -1, bool fontBold = false, string fontName = "")
		{
			HSSFCellStyle cs = book.CreateCellStyle() as HSSFCellStyle;
			cs.Alignment = align; //水平
			cs.VerticalAlignment = valign; //垂直
			if (border) //框線
			{
				cs.BorderBottom = BorderStyle.THIN;
				cs.BorderLeft = BorderStyle.THIN;
				cs.BorderTop = BorderStyle.THIN;
				cs.BorderRight = BorderStyle.THIN;
			}
			cs.WrapText = wrap;
			if (bgColor >= 0) //背景色
			{
				//注意 要設定背景色，需先設定前景色，否則程式不會報錯也無法設定背景色
				cs.FillForegroundColor = bgColor;
				//注意 設定背景色後，一定要設定填充模式，否則背景色不會顯示
				cs.FillPattern = FillPatternType.SOLID_FOREGROUND;
			}
			if (dataFormat > 0) //顯示方式（如：千分位）
			{
				cs.DataFormat = dataFormat;
			}
			if (fontSize > 0 || fontColor > 0 || fontBold || !fontName.IsNullOrEmpty())
			{
				HSSFFont font = book.CreateFont() as HSSFFont;
				if (fontSize > 0) //字型大小
				{
					font.FontHeightInPoints = fontSize;
				}
				if (fontColor > 0) //字體顏色
				{
					font.Color = fontColor;
				}
				if (fontBold) //粗體
				{
					font.Boldweight = (short)FontBoldWeight.BOLD;
				}
				if (!fontName.IsNullOrEmpty()) //字型
				{
					font.FontName = fontName;
				}
				cs.SetFont(font);
			}
			return cs;
		}

		/// <summary>
		/// 建立所有樣式清單
		/// HEADER：資料欄位名稱
		/// 第1個英文字：水平(L:左 R:右 C:中)
		/// 第2個英文字：垂直(Ｔ:上 Ｂ:下 C:中)
		/// _BORDER：加框線
		/// _NUM：數字樣式
		/// </summary>
		public static Dictionary<string, HSSFCellStyle> NPOI_GetCellStyle(HSSFWorkbook book)
		{
			Dictionary<string, HSSFCellStyle> cellStyle = new Dictionary<string, HSSFCellStyle>();

			cellStyle.Add("TOPIC", NPOI_SetCellStyle(book, wrap: false, fontSize: 12));

			//HEADER 格線+垂直水平置中+背景灰色
			cellStyle.Add("HEADER", NPOI_SetCellStyle(book, border: true, bgColor: HSSFColor.GREY_25_PERCENT.index, fontBold: true));

			//CC 無格線+垂直水平置中
			cellStyle.Add("CC", NPOI_SetCellStyle(book, wrap: false));

			//RC 無格線+靠右+垂直置中
			cellStyle.Add("RC", NPOI_SetCellStyle(book, align: HorizontalAlignment.RIGHT));

			//LT 無格線+左上
			cellStyle.Add("LT", NPOI_SetCellStyle(book, align: HorizontalAlignment.LEFT, valign: VerticalAlignment.TOP));

			//CC_BORDER 格線+垂直水平置中
			cellStyle.Add("CC_BORDER", NPOI_SetCellStyle(book, border: true));

			cellStyle.Add("CC_BORDER_NUM", NPOI_SetCellStyle(book, border: true, dataFormat: HSSFDataFormat.GetBuiltinFormat("#,##0")));

			return cellStyle;
		}

		/// <summary>
		/// 套用樣式至一列
		/// </summary>
		/// <param name="_row"></param>
		/// <param name="style"></param>
		/// <param name="_columnCount"></param>
		/// <returns></returns>
		public static void NPOI_SetRowStyle(HSSFRow row, HSSFCellStyle style, int cellCount)
		{
			for (int i = 0; i < cellCount; i++)
			{
				if (row.GetCell(i) == null)
				{
					row.CreateCell(i).SetCellValue("");
				}
				row.GetCell(i).CellStyle = style;
			}
		}

		/// <summary>
		/// 套用樣式到全部列
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="startRow">起始列</param>
		/// <param name="cellCount">總欄位</param>
		/// <param name="style">共用樣式</param>
		/// <param name="spec_style">特殊欄位指定樣式</param>
		public static void NPOI_SetRowStyle_ALL(HSSFSheet sheet, int startRow, int cellCount
			, HSSFCellStyle style, Dictionary<int, HSSFCellStyle> spec_style = null)
		{
			HSSFRow row;
			HSSFCell cell;
			for (int r = startRow; r <= sheet.LastRowNum; r++)
			{
				row = sheet.GetRow(r) as HSSFRow;
				for (int c = 0; c < cellCount; c++)
				{
					cell = row.GetCell(c) as HSSFCell;
					if (cell == null)
					{
						cell = row.CreateCell(c) as HSSFCell;
						cell.SetCellValue("");
					}
					cell.CellStyle = (spec_style != null && spec_style.ContainsKey(c)) ? spec_style[c] : style;
				}
			}
		}
		#endregion

		public static string GetIPAddress()
		{
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(sIPAddress))
			{
				return context.Request.ServerVariables["REMOTE_ADDR"];
			}
			else
			{
				string[] ipArray = sIPAddress.Split(new Char[] { ',' });
				return ipArray[0];
			}
		}

		#region 問卷調查

		/// <summary>
		/// 活動日期
		/// </summary>
		public static SelectList getSurveyActDate(string id, string tableName = "DATA2")
		{
			List<SelectListItem> list;
			using (DBEntities db = new DBEntities())
			{
				string SqlStr = string.Format(@"SELECT ST
FROM {0} tb
JOIN PLUS p ON p.MAIN_ID = tb.ID AND p.PLUS_TYPE = 'TIME' AND p.[ORDER] = 1 AND p.[ENABLE] = 1
CROSS APPLY dbo.fnSplitDate2Table(p.ID, p.DATETIME1, p.[DATETIME2])
WHERE tb.[ENABLE] = 1
AND tb.ID = @ID
ORDER BY ST", tableName);
				list = db.Database.SqlQuery<DateTime>(SqlStr, new SqlParameter("ID", id))
					.Select(p => new SelectListItem() { Value = p.ToDateString(), Text = p.ToDateString(week: true) }).ToList();
			}
			if (list == null)
			{
				list = new List<SelectListItem>();
				list.Add(new SelectListItem() { Value = string.Empty, Text = "請選擇" });
			}
			else if (list.Count > 1)
			{
				list.Insert(0, new SelectListItem() { Value = string.Empty, Text = "請選擇" });
			}
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 現居地－縣市
		/// </summary>
		public static SelectList getSurveyLoc(bool bFirst = false, string sText = "請選擇")
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (bFirst)
			{
				list.Add(new SelectListItem() { Value = string.Empty, Text = sText });
			}
			list.Add(new SelectListItem() { Value = "1", Text = "桃園市" });
			list.Add(new SelectListItem() { Value = "2", Text = "臺北市" });
			list.Add(new SelectListItem() { Value = "3", Text = "新北市" });
			list.Add(new SelectListItem() { Value = "4", Text = "竹苗地區" });
			list.Add(new SelectListItem() { Value = "5", Text = "其他縣市" });
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 現居地－桃園市 - 區域
		/// </summary>
		public static SelectList getSurveyLocX(bool bFirst = false, string sText = "請選擇")
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (bFirst)
			{
				list.Add(new SelectListItem() { Value = string.Empty, Text = sText });
			}
			list.Add(new SelectListItem() { Value = "1", Text = "中壢" });
			list.Add(new SelectListItem() { Value = "2", Text = "平鎮" });
			list.Add(new SelectListItem() { Value = "3", Text = "龍潭" });
			list.Add(new SelectListItem() { Value = "4", Text = "楊梅" });
			list.Add(new SelectListItem() { Value = "5", Text = "新屋" });
			list.Add(new SelectListItem() { Value = "6", Text = "觀音" });
			list.Add(new SelectListItem() { Value = "7", Text = "桃園" });
			list.Add(new SelectListItem() { Value = "8", Text = "龜山" });
			list.Add(new SelectListItem() { Value = "9", Text = "八德" });
			list.Add(new SelectListItem() { Value = "10", Text = "大溪" });
			list.Add(new SelectListItem() { Value = "11", Text = "復興" });
			list.Add(new SelectListItem() { Value = "12", Text = "大園" });
			list.Add(new SelectListItem() { Value = "13", Text = "蘆竹" });
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 年齡層
		/// </summary>
		public static SelectList getSurveyAge(bool bFirst = false, string sText = "請選擇")
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (bFirst)
			{
				list.Add(new SelectListItem() { Value = string.Empty, Text = sText });
			}
			list.Add(new SelectListItem() { Value = "1", Text = "20歲(含)以下" });
			list.Add(new SelectListItem() { Value = "2", Text = "21~30歲" });
			list.Add(new SelectListItem() { Value = "3", Text = "31~40歲" });
			list.Add(new SelectListItem() { Value = "4", Text = "41~50歲" });
			list.Add(new SelectListItem() { Value = "5", Text = "51~60歲" });
			list.Add(new SelectListItem() { Value = "6", Text = "61歲以上" });
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 演出類型
		/// </summary>
		public static SelectList getSurveyPerformanceType(bool bFirst = false, string sText = "請選擇")
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (bFirst)
			{
				list.Add(new SelectListItem() { Value = string.Empty, Text = sText });
			}
			list.Add(new SelectListItem() { Value = "1", Text = "音樂" });
			list.Add(new SelectListItem() { Value = "2", Text = "戲劇" });
			list.Add(new SelectListItem() { Value = "3", Text = "舞蹈" });
			list.Add(new SelectListItem() { Value = "4", Text = "親子" });
			list.Add(new SelectListItem() { Value = "5", Text = "其他" });
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 入場方式
		/// </summary>
		public static SelectList getSurveyEntry(bool bFirst = false, string sText = "請選擇")
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (bFirst)
			{
				list.Add(new SelectListItem() { Value = string.Empty, Text = sText });
			}
			list.Add(new SelectListItem() { Value = "sellTicket", Text = "售票" });
			list.Add(new SelectListItem() { Value = "ropeTicket", Text = "報名或索票" });
			list.Add(new SelectListItem() { Value = "freeTicket", Text = "自由入場" });
			return new SelectList(list, "Value", "Text");
		}

		/// <summary>
		/// 最得問卷的題庫
		/// </summary>
		/// <param name="type">1:表演藝術(預設值) 2:展覽藝術 3:桃園鐵玫瑰藝術節 4:電影活動 5:索票</param>
		/// <returns></returns>
		public static List<SurveyQues> getSurveyQAData(string type)
		{
			int iType = type.ToInt();
			if (iType > 0)
			{
				switch (iType)
				{
					case 2:
						return Function.getSurveyExhibitionQA(); //展覽藝術
					case 3:
						return Function.getSurveyRoseQA(); //桃園鐵玫瑰藝術節
					case 4:
						return Function.getSurveyMovieQA(); //電影活動
					case 5:
						return Function.getSurveyTicketQA(); //索票
					default:
						return Function.getSurveyPerformanceQA(); //預設:表演藝術
				}
			}
			return null;
		}

		/// <summary>
		/// 20201023 ting add
		/// 問卷調查 - 表演藝術
		/// </summary>
		public static List<SurveyQues> getSurveyPerformanceQA()
		{
			return new List<SurveyQues>() {
				new SurveyQues { Type = QuesType.checkbox, Name = "您如何得知本節目演出訊息？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "館舍內得知", Val = "A001", hasOth = false },
					new SurveyOpt { Name = "藝文店家", Val = "A002", hasOth = false },
					new SurveyOpt { Name = "學校", Val = "A003", hasOth = false },
					new SurveyOpt { Name = "平面媒體(報章雜誌、藝文月刊等)", Val = "A004", hasOth = false },
					new SurveyOpt { Name = "大眾媒體(廣播、電視、實體或電子廣告看板等)", Val = "A005", hasOth = false },
					new SurveyOpt { Name = "網路媒體(本場館網站、Facebook、Instagram、LINE、藝術入口網站等)", Val = "A006", hasOth = false },
					new SurveyOpt { Name = "親友揪團", Val = "A007", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "A008", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "吸引您前來欣賞本節目演出之原因？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "親友介紹", Val = "B001", hasOth = false },
					new SurveyOpt { Name = "交通方便", Val = "B002", hasOth = false },
					new SurveyOpt { Name = "票價優惠", Val = "B003", hasOth = false },
					new SurveyOpt { Name = "節目內容優質", Val = "B004", hasOth = false },
					new SurveyOpt { Name = "團隊知名度", Val = "B005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "B006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您對於今天的演出活動整體表現是否滿意？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "C001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "C002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "C003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "C004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館服務人員提供之解說及服務表現？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "D001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "D002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "D003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "D004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館的整體環境整潔？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "E001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "E002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "E003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "E004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館的演出場地設備及環境？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "F001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "F002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "F003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "F004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您可以接受的表演活動門票價格為？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "500元以下", Val = "G001", hasOth = false },
					new SurveyOpt { Name = "500~1200元", Val = "G002", hasOth = false },
					new SurveyOpt { Name = "1200~2000元", Val = "G003", hasOth = false },
					new SurveyOpt { Name = "2000~3000元", Val = "G004", hasOth = false },
					new SurveyOpt { Name = "3000元以上", Val = "G005", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "請問影響您再次蒞臨本場館的因素是？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "演出/活動品質", Val = "H001", hasOth = false },
					new SurveyOpt { Name = "演出時段", Val = "H002", hasOth = false },
					new SurveyOpt { Name = "服務態度", Val = "H003", hasOth = false },
					new SurveyOpt { Name = "環境設施", Val = "H004", hasOth = false },
					new SurveyOpt { Name = "售票與否", Val = "H005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "H006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您希望本場館多舉辦哪些類型的表演節目？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "音樂類", Val = "I100", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "管樂", Val = "I101", hasOth = false },
						new SurveyOpt { Name = "交響樂", Val = "I102", hasOth = false },
						new SurveyOpt { Name = "室內樂", Val = "I103", hasOth = false },
						new SurveyOpt { Name = "國樂", Val = "I104", hasOth = false },
						new SurveyOpt { Name = "合唱", Val = "I105", hasOth = false },
						new SurveyOpt { Name = "流行音樂", Val = "I106", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "I107", hasOth = true },
					} },
					new SurveyOpt { Name = "舞蹈類", Val = "I200", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "現代舞", Val = "I201", hasOth = false },
						new SurveyOpt { Name = "傳統舞蹈", Val = "I202", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "I203", hasOth = true },
					} },
					new SurveyOpt { Name = "戲劇類", Val = "I300", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "現代戲劇", Val = "I301", hasOth = false },
						new SurveyOpt { Name = "傳統戲曲", Val = "I302", hasOth = false },
						new SurveyOpt { Name = "歌劇", Val = "I303", hasOth = false },
						new SurveyOpt { Name = "音樂劇", Val = "I304", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "I305", hasOth = true },
					} },
					new SurveyOpt { Name = "親子類", Val = "I400", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "I500", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.textarea, Name = "您對於今天的演出、服務的整體表現、相關硬體設施及未來提供活動內容，有無其他的批評、指教及建議？" },
			};
		}

		/// <summary>
		/// 20201023 ting add
		/// 問卷調查 - 展覽藝術
		/// </summary>
		public static List<SurveyQues> getSurveyExhibitionQA()
		{
			return new List<SurveyQues>()
			{
				new SurveyQues { Type = QuesType.checkbox, Name = "您如何得知此次展出訊息？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "館舍內得知", Val = "A001", hasOth = false },
					new SurveyOpt { Name = "藝文店家", Val = "A002", hasOth = false },
					new SurveyOpt { Name = "學校", Val = "A003", hasOth = false },
					new SurveyOpt { Name = "平面媒體(報章雜誌、藝文月刊等)", Val = "A004", hasOth = false },
					new SurveyOpt { Name = "大眾媒體(廣播、電視、實體或電子廣告看板等)", Val = "A005", hasOth = false },
					new SurveyOpt { Name = "網路媒體(本場館網站、Facebook、Instagram、LINE、藝術入口網站等)", Val = "A006", hasOth = false },
					new SurveyOpt { Name = "親友揪團", Val = "A007", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "A008", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "吸引您前來欣賞本次展覽之原因？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "親友介紹", Val = "B001", hasOth = false },
					new SurveyOpt { Name = "交通方便", Val = "B002", hasOth = false },
					new SurveyOpt { Name = "免費或票價優惠", Val = "B003", hasOth = false },
					new SurveyOpt { Name = "展出內容優質", Val = "B004", hasOth = false },
					new SurveyOpt { Name = "展出者知名度", Val = "B005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "B006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您對今天的展覽活動整體表現是否滿意？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "C001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "C002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "C003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "C004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館服務人員提供之解說及服務表現？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "D001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "D002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "D003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "D004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館的整體環境整潔？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "E001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "E002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "E003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "E004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本場館的展出場地設備及環境？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "F001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "F002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "F003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "F004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您可接受的展覽門票價格為？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "100元以下", Val = "G001", hasOth = false },
					new SurveyOpt { Name = "100~300元", Val = "G002", hasOth = false },
					new SurveyOpt { Name = "300元以上", Val = "G003", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "請問影響您再次蒞臨本場館的因素是？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "展覽/活動品質", Val = "H001", hasOth = false },
					new SurveyOpt { Name = "展出時段", Val = "H002", hasOth = false },
					new SurveyOpt { Name = "服務態度", Val = "H003", hasOth = false },
					new SurveyOpt { Name = "環境設施", Val = "H004", hasOth = false },
					new SurveyOpt { Name = "售票與否", Val = "H005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "H006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您希望本場館多舉辦那些類型的展覽？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "平面視覺藝術類(例:書畫、水彩、油畫、版畫、插畫、攝影...等)", Val = "I001", hasOth = false },
					new SurveyOpt { Name = "立體視覺藝術類(例:雕塑、陶藝、竹藝...等)", Val = "I002", hasOth = false },
					new SurveyOpt { Name = "裝置藝術或空間設計類", Val = "I003", hasOth = false },
					new SurveyOpt { Name = "數位或新媒體藝術", Val = "I004", hasOth = false },
					new SurveyOpt { Name = "工藝類(例:陶瓷、珠寶、皮件、金工、模型...等)", Val = "I005", hasOth = false },
					new SurveyOpt { Name = "裝置藝術或空間設計類", Val = "I006", hasOth = false },
					new SurveyOpt { Name = "動漫相關", Val = "I007", hasOth = false },
					new SurveyOpt { Name = "歷史文獻類", Val = "I008", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "I009", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.textarea, Name = "您對於今天的展覽、服務的整體表現、相關硬體設施及未來提供活動內容，有無其他的批評、指教及建議？" },
			};
		}

		/// <summary>
		/// 20201023 ting add
		/// 問卷調查 - 桃園鐵玫瑰藝術節
		/// </summary>
		public static List<SurveyQues> getSurveyRoseQA()
		{
			return new List<SurveyQues>()
			{
				new SurveyQues { Type = QuesType.radio, Name = "您是否因鐵玫瑰藝術節「首次」蒞臨桃園展演中心(簡稱鐵玫瑰)？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "是", Val = "A001", hasOth = false },
					new SurveyOpt { Name = "1-5次", Val = "A002", hasOth = false },
					new SurveyOpt { Name = "5-10次", Val = "A003", hasOth = false },
					new SurveyOpt { Name = "10次以上", Val = "A004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您是如何「認識」鐵玫瑰？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "鐵玫瑰藝術節活動文宣", Val = "B001", hasOth = false },
					new SurveyOpt { Name = "其他藝文展演活動文宣", Val = "B002", hasOth = false },
					new SurveyOpt { Name = "鐵玫瑰藝術節策展人介紹", Val = "B003", hasOth = false },
					new SurveyOpt { Name = "其他新聞露出、網路資訊", Val = "B004", hasOth = false },
					new SurveyOpt { Name = "家住附近", Val = "B005", hasOth = false },
					new SurveyOpt { Name = "親友介紹", Val = "B006", hasOth = false },
					new SurveyOpt { Name = "其它", Val = "B007", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您是如何得知本演出活動「訊息」？ (可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "鐵玫瑰藝術節FB粉專、官網 ", Val = "C001", hasOth = false },
					new SurveyOpt { Name = "藝術節策展人介紹", Val = "C002", hasOth = false },
					new SurveyOpt { Name = "參與藝術節表演團體介紹", Val = "C003", hasOth = false },
					new SurveyOpt { Name = "館舍內海報、戶外帆布", Val = "C004", hasOth = false },
					new SurveyOpt { Name = "桃園藝文", Val = "C005", hasOth = false },
					new SurveyOpt { Name = "桃園市政府LINE", Val = "C006", hasOth = false },
					new SurveyOpt { Name = "藝文店家紙本文宣", Val = "C007", hasOth = false },
					new SurveyOpt { Name = "兩廳院藝文指南針、售票系統", Val = "C008", hasOth = false },
					new SurveyOpt { Name = "Par表演藝術雜誌", Val = "C009", hasOth = false },
					new SurveyOpt { Name = "迷誠品系列通路", Val = "C010", hasOth = false },
					new SurveyOpt { Name = "Youtube、IG廣告", Val = "C011", hasOth = false },
					new SurveyOpt { Name = "公車車體、路口LED、桃園火車站看板", Val = "C012", hasOth = false },
					new SurveyOpt { Name = "麥當勞麥TV ", Val = "C013", hasOth = false },
					new SurveyOpt { Name = "親友介紹", Val = "C014", hasOth = false },
					new SurveyOpt { Name = "其它", Val = "C015", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您認為甚麼樣「行銷通路」是較為有效的？ (可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "活動官網、社群、電子報", Val = "D001", hasOth = false },
					new SurveyOpt { Name = "IG、FB、Google、Line廣告", Val = "D002", hasOth = false },
					new SurveyOpt { Name = "策展人介紹", Val = "D003", hasOth = false },
					new SurveyOpt { Name = "參與藝術節表演團體、演出者介紹", Val = "D004", hasOth = false },
					new SurveyOpt { Name = "紙本海報、文宣 ", Val = "D005", hasOth = false },
					new SurveyOpt { Name = "親友介紹 ", Val = "D006", hasOth = false },
					new SurveyOpt { Name = "其它", Val = "D007", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "吸引您來參與活動的「關鍵因素」為何？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "鐵玫瑰藝術節口碑", Val = "E001", hasOth = false },
					new SurveyOpt { Name = "鐵玫瑰藝術節策展人經歷", Val = "E002", hasOth = false },
					new SurveyOpt { Name = "桃園展演中心館舍形象", Val = "E003", hasOth = false },
					new SurveyOpt { Name = "場館硬體更新重新啟用", Val = "E004", hasOth = false },
					new SurveyOpt { Name = "表演團體或演出者知名度", Val = "E005", hasOth = false },
					new SurveyOpt { Name = "對活動或演出內容有興趣", Val = "E006", hasOth = false },
					new SurveyOpt { Name = "社交需求、陪同親友出席", Val = "E007", hasOth = false },
					new SurveyOpt { Name = "閒暇休閒娛樂", Val = "E008", hasOth = false },
					new SurveyOpt { Name = "免費或票價合理，您覺得合理票價是___元", Val = "E009", hasOth = true },
					new SurveyOpt { Name = "其它", Val = "E010", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您認為什麼「時段」 較能吸引您觀賞藝文活動？ (可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "1-3月", Val = "F001", hasOth = false },
					new SurveyOpt { Name = "4-6月", Val = "F002", hasOth = false },
					new SurveyOpt { Name = "7-9月", Val = "F003", hasOth = false },
					new SurveyOpt { Name = "10-12月", Val = "F004", hasOth = false },
					new SurveyOpt { Name = "週六上午", Val = "F005", hasOth = false },
					new SurveyOpt { Name = "週六下午", Val = "F006", hasOth = false },
					new SurveyOpt { Name = "週六晚間", Val = "F007", hasOth = false },
					new SurveyOpt { Name = "週日上午", Val = "F008", hasOth = false },
					new SurveyOpt { Name = "週日下午", Val = "F009", hasOth = false },
					new SurveyOpt { Name = "週日晚間", Val = "F010", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您認為藝術節節目是否會影響館舍「形象」？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "會（續填第8題） ", Val = "G001", hasOth = false },
					new SurveyOpt { Name = "不會（續填第9題） ", Val = "G002", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "參與完今日的活動，您認為鐵玫瑰的「形象」是？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "有國際視野的 ", Val = "H001", hasOth = false },
					new SurveyOpt { Name = "與桃園在地連結的 ", Val = "H002", hasOth = false },
					new SurveyOpt { Name = "創新前衛的", Val = "H003", hasOth = false },
					new SurveyOpt { Name = "有深度內涵的 ", Val = "H004", hasOth = false },
					new SurveyOpt { Name = "適合普遍大眾的 ", Val = "H005", hasOth = false },
					new SurveyOpt { Name = "保守通俗的 ", Val = "H006", hasOth = false },
					new SurveyOpt { Name = "脫離地方脈絡", Val = "H007", hasOth = false },
					new SurveyOpt { Name = "冷僻高深的", Val = "H008", hasOth = false },
					new SurveyOpt { Name = "內容膚淺的", Val = "H009", hasOth = false },
					new SurveyOpt { Name = "適合特定族群", Val = "H010", hasOth = false },
					new SurveyOpt { Name = "其它", Val = "H011", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您對鐵玫瑰認知「既有的形象」？ (可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "有國際視野的 ", Val = "I001", hasOth = false },
					new SurveyOpt { Name = "與桃園在地連結的 ", Val = "I002", hasOth = false },
					new SurveyOpt { Name = "創新前衛的", Val = "I003", hasOth = false },
					new SurveyOpt { Name = "有深度內涵的 ", Val = "I004", hasOth = false },
					new SurveyOpt { Name = "適合普遍大眾的 ", Val = "I005", hasOth = false },
					new SurveyOpt { Name = "保守通俗的 ", Val = "I006", hasOth = false },
					new SurveyOpt { Name = "脫離地方脈絡", Val = "I007", hasOth = false },
					new SurveyOpt { Name = "冷僻高深的", Val = "I008", hasOth = false },
					new SurveyOpt { Name = "內容膚淺的", Val = "I009", hasOth = false },
					new SurveyOpt { Name = "適合特定族群", Val = "I010", hasOth = false },
					new SurveyOpt { Name = "其它", Val = "I011", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "您對鐵玫瑰藝術節未來的「期待」？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "更多國際團隊", Val = "J001", hasOth = false },
					new SurveyOpt { Name = "更多桃園在地團隊", Val = "J002", hasOth = false },
					new SurveyOpt { Name = "更多新作首演", Val = "J003", hasOth = false },
					new SurveyOpt { Name = "更多通俗內容", Val = "J004", hasOth = false },
					new SurveyOpt { Name = "更多闔家觀賞的演出", Val = "J005", hasOth = false },
					new SurveyOpt { Name = "更多免費入場的活動 ", Val = "J006", hasOth = false },
					new SurveyOpt { Name = "更多工作坊或推廣活動", Val = "J007", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "J008", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.textarea, Name = "您對於今天的展覽、服務的整體表現、相關硬體設施及未來提供活動內容，有無其他的批評、指教及建議？" },
			};
		}

		/// <summary>
		/// 20210315 ting add
		/// 問卷調查 - 電影活動
		/// </summary>
		public static List<SurveyQues> getSurveyMovieQA()
		{
			return new List<SurveyQues>() {
				new SurveyQues { Type = QuesType.checkbox, Name = "您如何得知此次電影放映訊息？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "館舍內得知", Val = "A001", hasOth = false },
					new SurveyOpt { Name = "藝文店家", Val = "A002", hasOth = false },
					new SurveyOpt { Name = "學校", Val = "A003", hasOth = false },
					new SurveyOpt { Name = "平面媒體(報章雜誌、藝文月刊等)", Val = "A004", hasOth = false },
					new SurveyOpt { Name = "大眾媒體(廣播、電視、實體或電子廣告看板等)", Val = "A005", hasOth = false },
					new SurveyOpt { Name = "網路媒體(本中心網站、Facebook、Instagram、LINE、藝術入口網站等)", Val = "A006", hasOth = false },
					new SurveyOpt { Name = "親友揪團", Val = "A007", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "A008", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "吸引您前來欣賞本次電影之原因？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "親友介紹", Val = "B001", hasOth = false },
					new SurveyOpt { Name = "交通方便", Val = "B002", hasOth = false },
					new SurveyOpt { Name = "票價優惠", Val = "B003", hasOth = false },
					new SurveyOpt { Name = "喜歡此部電影", Val = "B004", hasOth = false },
					new SurveyOpt { Name = "電影類型吸引人", Val = "B005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "B006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您對今天的電影活動整體表現是否滿意？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "C001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "C002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "C003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "C004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本館服務人員提供之解說及服務表現？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "D001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "D002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "D003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "D004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本館的整體環境整潔？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "E001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "E002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "E003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "E004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "本館的場地設備及環境？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "非常滿意", Val = "F001", hasOth = false },
					new SurveyOpt { Name = "滿意", Val = "F002", hasOth = false },
					new SurveyOpt { Name = "不滿意", Val = "F003", hasOth = false },
					new SurveyOpt { Name = "很不滿意", Val = "F004", hasOth = false },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "請問影響您再次蒞臨本館的因素是？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "電影/活動品質", Val = "G001", hasOth = false },
					new SurveyOpt { Name = "放映時段", Val = "G002", hasOth = false },
					new SurveyOpt { Name = "服務態度", Val = "G003", hasOth = false },
					new SurveyOpt { Name = "環境設施", Val = "G004", hasOth = false },
					new SurveyOpt { Name = "售票與否", Val = "G005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "G006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "請問您最喜歡看的電影類型？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "劇情片", Val = "H001", hasOth = false },
					new SurveyOpt { Name = "動作片", Val = "H002", hasOth = false },
					new SurveyOpt { Name = "愛情片", Val = "H003", hasOth = false },
					new SurveyOpt { Name = "恐怖片", Val = "H004", hasOth = false },
					new SurveyOpt { Name = "驚悚片", Val = "H005", hasOth = false },
					new SurveyOpt { Name = "史詩片", Val = "H006", hasOth = false },
					new SurveyOpt { Name = "科幻片", Val = "H007", hasOth = false },
					new SurveyOpt { Name = "動畫片", Val = "H008", hasOth = false },
					new SurveyOpt { Name = "紀錄片", Val = "H009", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "H010", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "請問您喜歡哪些電影活動？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "演員見面會", Val = "I001", hasOth = false },
					new SurveyOpt { Name = "電影特映會", Val = "I002", hasOth = false },
					new SurveyOpt { Name = "贈送電影票", Val = "I003", hasOth = false },
					new SurveyOpt { Name = "映後座談會", Val = "I004", hasOth = false },
					new SurveyOpt { Name = "影評人活動", Val = "I005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "I006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.textarea, Name = "您對於今天的放映、服務的整體表現、相關硬體設施及未來提供活動內容，有無其他的批評、指教及建議？" },
			};
		}

		/// <summary>
		/// 20210315 ting add
		/// 問卷調查 - 桃園展演中心 演出索票單
		/// </summary>
		/// <returns></returns>
		public static List<SurveyQues> getSurveyTicketQA()
		{
			return new List<SurveyQues>() {
				new SurveyQues { Type = QuesType.checkbox, Name = "您如何得知本節目演出訊息？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "館舍內得知", Val = "A001", hasOth = false },
					new SurveyOpt { Name = "藝文店家", Val = "A002", hasOth = false },
					new SurveyOpt { Name = "學校", Val = "A003", hasOth = false },
					new SurveyOpt { Name = "報章雜誌", Val = "A004", hasOth = false },
					new SurveyOpt { Name = "電子媒體(電台、電視、廣播)", Val = "A005", hasOth = false },
					new SurveyOpt { Name = "兩廳院或年代售票系統月刊", Val = "A006", hasOth = false },
					new SurveyOpt { Name = "本場館網站、FB、電子報、市府LINE", Val = "A007", hasOth = false },
					new SurveyOpt { Name = "親友揪團", Val = "A008", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "A009", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "吸引您前來索取本節目演出票券之原因？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "親友介紹", Val = "B001", hasOth = false },
					new SurveyOpt { Name = "交通方便", Val = "B002", hasOth = false },
					new SurveyOpt { Name = "票價優惠", Val = "B003", hasOth = false },
					new SurveyOpt { Name = "節目內容優質", Val = "B004", hasOth = false },
					new SurveyOpt { Name = "團隊知名度", Val = "B005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "B006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "您可接受的表演活動門票價格為？"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "500元以下", Val = "C001", hasOth = false },
					new SurveyOpt { Name = "500~1200元", Val = "C002", hasOth = false },
					new SurveyOpt { Name = "1200~2000元", Val = "C003", hasOth = false },
					new SurveyOpt { Name = "2000~3000元", Val = "C004", hasOth = false },
					new SurveyOpt { Name = "3000元以上", Val = "C005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "C006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.radio, Name = "請問影響您願意蒞臨本館的因素是？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "演出/活動品質", Val = "D001", hasOth = false },
					new SurveyOpt { Name = "演出時段", Val = "D002", hasOth = false },
					new SurveyOpt { Name = "服務態度", Val = "D003", hasOth = false },
					new SurveyOpt { Name = "環境設施", Val = "D004", hasOth = false },
					new SurveyOpt { Name = "售票與否", Val = "D005", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "D006", hasOth = true },
				} },
				new SurveyQues { Type = QuesType.checkbox, Name = "請問哪些類型的表演節目您較為感興趣，並希望未來本館多舉辦？(可複選)"
				, Opts = new List<SurveyOpt>() {
					new SurveyOpt { Name = "音樂類", Val = "E100", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "管樂", Val = "E101", hasOth = false },
						new SurveyOpt { Name = "交響樂", Val = "E102", hasOth = false },
						new SurveyOpt { Name = "室內樂", Val = "E103", hasOth = false },
						new SurveyOpt { Name = "國樂", Val = "E104", hasOth = false },
						new SurveyOpt { Name = "合唱", Val = "E105", hasOth = false },
						new SurveyOpt { Name = "流行音樂", Val = "E106", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "E107", hasOth = true },
					} },
					new SurveyOpt { Name = "舞蹈類", Val = "E200", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "現代舞", Val = "E201", hasOth = false },
						new SurveyOpt { Name = "傳統舞蹈", Val = "E202", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "E203", hasOth = true },
					} },
					new SurveyOpt { Name = "戲劇類", Val = "E300", hasOth = false, SubOpts = new List<SurveyOpt>() {
						new SurveyOpt { Name = "現代戲劇", Val = "E301", hasOth = false },
						new SurveyOpt { Name = "傳統戲曲", Val = "E302", hasOth = false },
						new SurveyOpt { Name = "歌劇", Val = "E303", hasOth = false },
						new SurveyOpt { Name = "音樂劇", Val = "E304", hasOth = false },
						new SurveyOpt { Name = "其他", Val = "E305", hasOth = true },
					} },
					new SurveyOpt { Name = "親子類", Val = "E400", hasOth = false },
					new SurveyOpt { Name = "其他", Val = "E500", hasOth = true }
				} }
			};
		}
        #endregion

        #region QRCode產生

        /// <summary>
        /// 建立QRCode
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="uploadPath">儲存路徑</param>
        public static void CreateQRcode(string url, string uploadPath)
        {
            var encoder = new QRCodeEncoder();
            using (var qrcode = encoder.Encode(url))
            {
                qrcode.Save(uploadPath, ImageFormat.Png);
            }
        }


        #endregion
    }
}
