using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using KingspModel.Attributes;
using KingspModel.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace KingspModel.DataModel
{
	#region Demo Model
	/// <summary>
	/// 這是測試Model
	/// </summary>
	public sealed class DemoModel : BaseMetadata
	{
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DataType(DATA_TYPE_TITLE)]
		//[Email(ErrorMessage = FORMAT_ERROR_MESSAGE)]
		public string ID { get; set; }

		/// <summary>
		/// 文字
		/// </summary>
		[DisplayName("文字")]
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT { get; set; }
		/// <summary>
		/// 多行文字
		/// </summary>
		[DisplayName("多行文字")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT_MAX { get; set; }
		/// <summary>
		/// 日期
		/// </summary>
		[DisplayName("日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? DATE { get; set; }
		/// <summary>
		/// 日期時分
		/// </summary>
		[DisplayName("日期時分")]
		[DataType(DATA_TYPE_DATETIME)]
		public DateTime? DATETIME { get; set; }
		/// <summary>
		/// 編輯器
		/// </summary>
		[DisplayName("編輯器")]
		[DataType(DATA_TYPE_CKEDITOR)]
		[AllowHtml]
		public string CKEDITOR { get; set; }
		/// <summary>
		/// 數字有小數
		/// </summary>
		[DataType(DATA_TYPE_TITLE)]
		public Decimal? DECIMAL { get; set; }
		/// <summary>
		/// 數字無小數
		/// </summary>
		[DataType(DATA_TYPE_TITLE)]
		public int? INT { get; set; }
		/// <summary>
		/// DATA_TYPE_FILEUPLOAD
		/// </summary>
		[DisplayName("檔案")]
		[DataType(DATA_TYPE_FILEUPLOAD)]
		public string ATT { get; set; }
        /// <summary>
        /// 已上傳的ATTACHMENT
        /// </summary>
        public List<ATTACHMENT> AttList { get; set; }
    }
	#endregion

	#region 會員管理 User Model
	/// <summary>
	/// 會員管理 User Model
	/// </summary>
	public sealed class UserModel : BaseMetadata
	{
		public string MAIN_ID { get; set; }
		public string USER_ID { get; set; }

		/// <summary>
		/// 帳號
		/// </summary>
		[DisplayName("帳號")]
		[DataType(DATA_TYPE_TITLE)]
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[Email(ErrorMessage = "Email格式錯誤")]
		//[Remote("UserEmailExist", "Json", AdditionalFields = "USER_ID", ErrorMessage = DEFAULT_REPEAT_KEY)]
		public string CONTENT1 { get; set; }

		/// <summary>
		/// 密碼
		/// </summary>
		[DisplayName("密碼")]
		[DataType(DataType.Password)]
		//[StringLength(30, MinimumLength = 6, ErrorMessage = "會員帳號的長度需再6~30個字元內！")]
		//[RegularExpression(Function.PASSWORD_REGEX, ErrorMessage = "密碼必須 首字英文字母 + 英數混合 8-20字元")]
		public string PASSWORD { get; set; }

		/// <summary>
		/// 姓
		/// </summary>
		[DisplayName("姓")]
		[DataType(DATA_TYPE_TITLE)]
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		public string CONTENT2 { get; set; }

		/// <summary>
		/// 名字
		/// </summary>
		[DisplayName("名字")]
		[DataType(DATA_TYPE_TITLE)]
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		public string CONTENT3 { get; set; }

		/// <summary>
		/// 性別
		/// </summary>
		[DisplayName("性別")]
		[DataType(DATA_TYPE_TITLE)]
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		public string CONTENT4 { get; set; }

		/// <summary>
		/// 電話
		/// </summary>
		[DisplayName("電話")]
		[DataType(DATA_TYPE_TITLE)]
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		public string CONTENT5 { get; set; }

        /// <summary>
		/// 會員等級
		/// </summary>
		[DisplayName("會員等級")]
        [DataType(DATA_TYPE_TITLE)]
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        public string CONTENT6 { get; set; }

        /// <summary>
		/// 來源
		/// </summary>
		[DisplayName("來源")]
        [DataType(DATA_TYPE_TITLE)]
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        public string CONTENT7 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT8 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT9 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT10 { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        [DisplayName("生日")]
		[DataType(DATA_TYPE_DATE)]
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		public DateTime? DATETIME1 { get; set; }
        public DateTime? DATETIME2 { get; set; }
        public DateTime? DATETIME3 { get; set; }
        public DateTime? DATETIME4 { get; set; }
        public DateTime? DATETIME5 { get; set; }

        /// <summary>
        /// FB ID
        /// </summary>
        public string CONTENT11 { get; set; }
        /// <summary>
        /// Line
        /// </summary>
        public string CONTENT12 { get; set; }
        /// <summary>
        /// Google
        /// </summary>
        public string CONTENT13 { get; set; }
        public string CONTENT14 { get; set; }
        public string CONTENT15 { get; set; }
        public string CONTENT16 { get; set; }
        public string CONTENT17 { get; set; }
        public string CONTENT18 { get; set; }
        public string CONTENT19 { get; set; }
        public string CONTENT20 { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [DisplayName("備註")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string PARAGRAPH { get; set; }
        /// <summary>
        /// 屬於此user的PLUS集合
        /// </summary>
        public List<PLUS> PlusList { get; set; }
        /// <summary>
        /// 屬於此user的訂單PLUS集合
        /// </summary>
        public List<PLUS> PlusFun4000 { get; set; }
        /// <summary>
        /// 屬於此user的紅利點數PLUS集合
        /// </summary>
        public List<PLUS> PlusFun5002 { get; set; }
        /// <summary>
        /// 屬於此user的兌換紀錄PLUS集合
        /// </summary>
        public List<PLUS> PlusFun5003 { get; set; }

        public List<ATTACHMENT> AttachmentList { get; set; }

        public ATTACHMENT AttPic { get; set; }
    }

	/// <summary>
	/// 會員忘記密碼 Model
	/// </summary>
	public sealed class ForgetModel : BaseMetadata
	{
		/// <summary>
		/// 帳號 email
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("帳號")]
		[DataType(DATA_TYPE_TITLE)]
		public string email { get; set; }
	}

	#endregion

	#region FB Model

	/// <summary>
	/// Facebook Json 轉 Object 用
	/// </summary>
	public class FBUserData
	{
		public FBUserData(JObject jObject)
		{
			id = (string)jObject["id"];
			email = (string)jObject["email"];
			name = (string)jObject["name"];
		}

		public string name { get; set; }
		public string id { get; set; }
		public string email { get; set; }

	}

    #endregion

    #region Line model
    /// <summary>
    /// Line用的 (Google也適用)
    /// </summary>
    public class LineLoginToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
    /// <summary>
    /// Line用的 (Google也用同一個)
    /// </summary>
    public class LineProfile
    {
        public string userId { get; set; }
        public string displayName { get; set; }
        public string pictureUrl { get; set; }
        public string statusMessage { get; set; }
        public string email { get; set; }
        #region google
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }
        #endregion
    }

    #endregion

    #region IG 取得USER Json

    public class Rootobject
	{
		public Data data { get; set; }
		public Meta meta { get; set; }
	}

	public class Data
	{
		public string id { get; set; }
		public string username { get; set; }
		public string profile_picture { get; set; }
		public string full_name { get; set; }
		public string bio { get; set; }
		public string website { get; set; }
		public bool is_business { get; set; }
		public Counts counts { get; set; }
	}

	public class Counts
	{
		public int media { get; set; }
		public int follows { get; set; }
		public int followed_by { get; set; }
	}

	public class Meta
	{
		public int code { get; set; }
	}

	public class RootobjectServer
	{
		public string access_token { get; set; }
		public User user { get; set; }
	}

	public class User
	{
		public string id { get; set; }
		public string username { get; set; }
		public string full_name { get; set; }
		public string profile_picture { get; set; }
	}
	#endregion

	#region EXCEL
	/// <summary>
	/// Excel
	/// </summary>
	public sealed class ExcelModel : BaseMetadata
	{
		public byte[] ms { get; set; }
		public string contentType { get; set; }
		public string fileName { get; set; }
	}

	/// <summary>
	/// ImportModel(EPPlus OfficeOpenXml)
	/// </summary>
	public sealed class ImportModel : BaseMetadata
	{
		/// <summary>
		/// Excel
		/// </summary>
		public ExcelPackage ep { get; set; }
		/// <summary>
		/// 頁籤
		/// </summary>
		public ExcelWorksheet sheet { get; set; }
		/// <summary>
		/// 起始列編號
		/// </summary>
		public int startRowNumber { get; set; }
		/// <summary>
		/// 結束列編號
		/// </summary>
		public int endRowNumber { get; set; }
		/// <summary>
		/// 開始欄編號
		/// </summary>
		public int startColumn { get; set; }
		/// <summary>
		/// 結束欄編號
		/// </summary>
		public int endColumn { get; set; }
		/// <summary>
		/// 第幾個頁籤
		/// </summary>
		public int i { get; set; }
		/// <summary>
		/// 是否包含標題列
		/// </summary>
		public bool isHeader { get; set; }

		public ImportModel()
		{
			i = 1;
			isHeader = true;
		}

		/// <summary>
		/// SetImportModel
		/// </summary>
		/// <returns></returns>
		public ImportModel SetImportModel()
		{
			if (this.ep != null)
			{
				this.sheet = this.ep.Workbook.Worksheets[i];

				this.startRowNumber = (this.isHeader) ? this.sheet.Dimension.Start.Row + 1 : this.sheet.Dimension.Start.Row;//起始列編號，從1算起
				this.endRowNumber = this.sheet.Dimension.End.Row;//結束列編號，從1算起
				this.startColumn = this.sheet.Dimension.Start.Column;//開始欄編號，從1算起
				this.endColumn = this.sheet.Dimension.End.Column;//結束欄編號，從1算起
			}
			return this;
		}
	}
	#endregion

	#region Send Mail Model
	/// <summary>
	/// 信件
	/// </summary>
	public sealed class LetterModel : BaseMetadata
	{
		/// <summary>
		/// 寄件者E-mail
		/// </summary>
		[DisplayName("寄件者E-mail")]
		public string Sender { get; set; }
		/// <summary>
		/// 寄件者姓名
		/// </summary>
		[DisplayName("寄件者姓名")]
		public string SenderName { get; set; }
		/// <summary>
		/// 主旨
		/// </summary>
		[DisplayName("主旨")]
		public string Subject { get; set; }
		/// <summary>
		/// 內容
		/// </summary>
		[DisplayName("內容")]
		public string Body { get; set; }
		/// <summary>
		/// 收件者(多位) 不包含名稱
		/// </summary>
		[DisplayName("收件者")]
		public List<string> RecipientList { get; set; }
		/// <summary>
		/// 收件者(多位) 包含名稱 (Key=Email, Value=Name)
		/// </summary>
		[DisplayName("收件者")]
		public Dictionary<string, string> RecipientNameList { get; set; }
	}
	#endregion

	#region WaterAll

	#region 主視覺＆連結＆廣告&跑馬燈
    /// <summary>
    /// 首頁管理系列Model
    /// </summary>
	public sealed class BannerModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string MAIN_ID { get; set; }
		/// <summary>
		/// 名稱、文字(英文)
		/// </summary>
		[DisplayName("英文版")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
        /// <summary>
        /// 圖片上傳
        /// </summary>
        [DisplayName("圖片上傳")]
		public HttpPostedFileBase hpf { get; set; }
		public string ImgUrl { get; set; }
		/// <summary>
		/// 文字
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("文字說明")]
		[DataType(DATA_TYPE_TITLE)]
		public string DESCRIPTION { get; set; }
		/// <summary>
		/// 連結
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
        /// <summary>
        /// 連結en
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("連結英文版")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
        /// <summary>
        /// 備用
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("備用")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT3 { get; set; }
        /// <summary>
        /// 備用
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("備用")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT4 { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
		public int ORDER { get; set; }
		/// <summary>
		/// 發佈日期
		/// </summary>
		[DisplayName("發佈日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime CONTENT9 { get; set; }
		/// <summary>
		/// 下線日期 無則表示永久上線
		/// </summary>
		[DisplayName("下線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT10 { get; set; }
	}
    #endregion

    #region ARTICLE 資料通用Model 活動訊息&常見問題

    /// <summary>
	/// Article Model
	/// </summary>
	public sealed class ArticleModel : BaseMetadata
    {
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        //[DisplayName("ID")]
        [DataType(DATA_TYPE_TITLE)]
        public string ID { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string NODE_ID { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime CREATE_DATE { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CREATER { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime UPDATE_DATE { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string UPDATER { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string ARTICLE_TYPE { get; set; }
        public byte ENABLE { get; set; }
        public int ORDER { get; set; }
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT2 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT3 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT4 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT5 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT6 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT7 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT8 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT9 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT10 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT11 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT12 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT13 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT14 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT15 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT16 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT17 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT18 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT19 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT20 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public bool Bool_DECIMAL1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL2 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL3 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL4 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL5 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL6 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL7 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL8 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL9 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public int? DECIMAL10 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME1 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME2 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME3 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME4 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME5 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME6 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME7 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME8 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME9 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME10 { get; set; }

        #region Attachment

        public List<ATTACHMENT> Atts { get; set; }
        public List<ATTACHMENT> Pics { get; set; }

        #endregion

        #region Paragraph
        [AllowHtml]
        [DataType(DATA_TYPE_CKEDITOR_REQUIRED)]
        public string PH0 { get; set; }
        [AllowHtml]
        [DataType(DATA_TYPE_CKEDITOR)]
        public string PH1 { get; set; }

        #endregion
    }

    #endregion

    #region DATA 資料通用Model

    /// <summary>
	/// Data Model
	/// </summary>
	public sealed class DataModel : BaseMetadata
    {
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        //[DisplayName("ID")]
        [DataType(DATA_TYPE_TITLE)]
        public string ID { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string MAIN_ID { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string NODE_ID { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime CREATE_DATE { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CREATER { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime UPDATE_DATE { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string UPDATER { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string DATA_TYPE { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string STATUS { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public bool Bool_STATUS { get; set; }
        public byte ENABLE { get; set; }
        public int ORDER { get; set; }
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT2 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT3 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT4 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT5 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT6 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT7 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT8 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT9 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT10 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT11 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT12 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT13 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT14 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT15 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT16 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT17 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT18 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT19 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT20 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT21 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT22 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT23 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT24 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT25 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT26 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT27 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT28 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT29 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT30 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public bool Bool_DECIMAL1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public bool Bool_DECIMAL2 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public bool Bool_DECIMAL3 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL1 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL2 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL3 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL4 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL5 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL6 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL7 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL8 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL9 { get; set; }
        [DataType(DATA_TYPE_TITLE)]
        public decimal? DECIMAL10 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME1 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME2 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME3 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME4 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME5 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME6 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME7 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME8 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME9 { get; set; }
        [DataType(DATA_TYPE_DATE)]
        public DateTime? DATETIME10 { get; set; }
		/// <summary>
		/// 內容
		/// </summary>
		[DisplayName("內容")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT { get; set; }
		#region PLUS

		public List<PLUS> plusList { get; set; }

        #endregion

        #region Attachment
        [DisplayName("檔案")]
        [DataType(DATA_TYPE_FILEUPLOAD)]
        public string ATT { get; set; }
        /// <summary>
        /// 已上傳的ATTACHMENT
        /// </summary>
        public List<ATTACHMENT> AttList { get; set; }
        public List<ATTACHMENT> Atts { get; set; }
        public List<ATTACHMENT> Pics { get; set; }

        #endregion

        #region Paragraph
        [AllowHtml]
        [DataType(DATA_TYPE_CKEDITOR_REQUIRED)]
        public string PH0 { get; set; }
        [AllowHtml]
        [DataType(DATA_TYPE_CKEDITOR)]
        public string PH1 { get; set; }

        #endregion

        #region DATA1-3系列
        public DATA1 Data1 { get; set; }
        public DATA2 Data2 { get; set; }
        public DATA3 Data3 { get; set; }
        public DATA4 Data4 { get; set; }
        #endregion
    }

	#endregion

	#region ChatGPTModel
	public sealed class ChatGPTModel : BaseMetadata
	{
		[Required]
		public string Message { get; set; }
	}
	#endregion

	#region 電子手冊
		public sealed class EManualModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string MAIN_ID { get; set; }
		/// <summary>
		/// 封面上傳
		/// </summary>
		[DisplayName("封面上傳")]
		public HttpPostedFileBase hpf { get; set; }
		public string ImgUrl { get; set; }
		/// <summary>
		/// 手冊名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("手冊名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string DESCRIPTION { get; set; }
		/// <summary>
		/// 電子手冊上傳
		/// </summary>
		public HttpPostedFileBase hpfFile { get; set; }
		/// <summary>
		/// 手冊系統檔名(包含副檔名)
		/// </summary>
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		[DisplayName("排序")]
		public int ORDER { get; set; }
		/// <summary>
		/// 刪除(0:是、1:否)
		/// </summary>
		[DisplayName("刪除")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// 上線日期
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("上線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT6 { get; set; }
		/// <summary>
		/// 下線日期
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("下線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT7 { get; set; }
	}
	#endregion

	#region 工程標案圖片上傳
	public sealed class BidPicModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string NODE_ID { get; set; }
		/// <summary>
		/// 標案名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("標案名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 標案案號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("標案案號")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		[DisplayName("備註")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 標案日期
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		[DisplayName("標案日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime DATETIME1 { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<HttpPostedFileBase> HPFs { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
	}
	#endregion

	#region 最新公告
	public sealed class NewsModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string NODE_ID { get; set; }
        /// <summary>
        /// 呈現類型
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("呈現類型")]
		public string ARTICLE_TYPE { get; set; }
        /// <summary>
        /// 置頂
        /// </summary>
        [DisplayName("置頂")]
        public int? ORDER { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("標題")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 內容
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("內容")]
        [DataType(DATA_TYPE_TEXTAREA)]
        //[DataType(DATA_TYPE_TEXTAREA_REQUIRED)]
        //[DataType(DATA_TYPE_CKEDITOR_REQUIRED)]
        [AllowHtml]
		public string CONTENT { get; set; }
        /// <summary>
        /// 網址
        /// </summary>
        [DisplayName("網址")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT11 { get; set; }
		/// <summary>
		/// 上線日期(起)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		[DisplayName("上線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime DATETIME1 { get; set; }
        /// <summary>
        /// 上線日期(迄)
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		[DisplayName("上線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime DATETIME2 { get; set; }
		/// <summary>
		/// 附件上傳
		/// </summary>
		public List<HttpPostedFileBase> HPF1s { get; set; }
		/// <summary>
		/// 附件上傳
		/// </summary>
		public List<ATTACHMENT> ATTAs { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<HttpPostedFileBase> HPF2s { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
        /// <summary>
		/// 附件上傳
		/// </summary>
		public HttpPostedFileBase HPF { get; set; }
    }
	#endregion

	#region 主題影展管理&單元放映管理
	public sealed class CategoryModel : BaseMetadata
	{
		/// <summary>
		/// 主題影展(只有「單元放映管理」必填)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("主題影展")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// 介紹
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("介紹")]
		[DataType(DATA_TYPE_TEXTAREA_REQUIRED)]
		public string URL { get; set; }
		/// <summary>
		/// 影展日期(起)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT1 { get; set; }
		/// <summary>
		/// 影展日期(迄)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT2 { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		[DisplayName("圖片上傳")]
		public HttpPostedFileBase hpf { get; set; }
		public ATTACHMENT atta { get; set; }
		/// <summary>
		/// 索票說明 20201012
		/// </summary>
		[DisplayName("索票說明")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 索票連結 20201012
		/// </summary>
		[DisplayName("索票連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 上線日期(起)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("上線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT5 { get; set; }
		/// <summary>
		/// 下線日期(迄)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("下線日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? CONTENT6 { get; set; }
	}
	#endregion

	#region 影片管理
	public sealed class FilmModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string NODE_ID { get; set; }
        /// <summary>
        /// 映後座談(0:是 1:否)
        /// </summary>
        [DisplayName("映後座談")]
		[DataType(DATA_TYPE_TITLE)]
		public int? ORDER { get; set; }
        /// <summary>
        /// 映前導讀(0:是 1:否)
        /// </summary>
        [DisplayName("映前導讀")]
		public int? DECIMAL3 { get; set; }
		/// <summary>
		/// 片名
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("片名")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 片名(英)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("片名(英)")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT14 { get; set; }
		/// <summary>
		/// 類型
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("類型")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 分級
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("分級")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// 導演
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("導演")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// 國家
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("國家")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }
		/// <summary>
		/// 其他_自行新增
		/// </summary>
		[DisplayName("其他_自行新增")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7_OTHER { get; set; }
		/// <summary>
		/// 放映規格
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("放映規格")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
		/// <summary>
		/// 其他_自行新增
		/// </summary>
		[DisplayName("其他_自行新增")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8_OTHER { get; set; }
		/// <summary>
		/// 發音
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("發音")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// 其他_自行新增
		/// </summary>
		[DisplayName("其他_自行新增")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9_OTHER { get; set; }
		/// <summary>
		/// 字幕
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("字幕")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT10 { get; set; }
		/// <summary>
		/// 其他_自行新增
		/// </summary>
		[DisplayName("其他_自行新增")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT10_OTHER { get; set; }
		/// <summary>
		/// 獎項
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("獎項")]
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public string CONTENT11 { get; set; }
		/// <summary>
		/// 色彩
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("色彩")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT12 { get; set; }
		/// <summary>
		/// 預告片連結
		/// </summary>
		[DisplayName("預告片連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT13 { get; set; }
		/// <summary>
		/// 影片說明
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("影片介紹")]
		[DataType(DATA_TYPE_TEXTAREA_REQUIRED_MAXLENGTH)]
		public string CONTENT21 { get; set; }
		/// <summary>
		/// 年份
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("年份")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL1 { get; set; }
		/// <summary>
		/// 片長
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("片長")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL2 { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<HttpPostedFileBase> HPFs { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
		/// <summary>
		/// 放映時間（新增用）
		/// </summary>
		[DisplayName("放映時間")]
		[DataType(DATA_TYPE_DATETIME)]
		public DateTime TIME { get; set; }
	}
	#endregion

	#region 影片放映清單
	public sealed class FilmPlayModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string NODE_ID { get; set; }
		/// <summary>
		/// 影片編號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("影片編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string MAIN_ID { get; set; }
		public string FILM_NAME { get; set; }
		/// <summary>
		/// 映後座談(1:是 0:否)
		/// </summary>
		[DisplayName("映後座談")]
		[DataType(DATA_TYPE_TITLE)]
		public string STATUS { get; set; }
		/// <summary>
		/// 出席座談(1:是 0:否) (導演或影人出席映後座談或映前導讀)
		/// </summary>
		[DisplayName("出席座談")]
		[DataType(DATA_TYPE_TITLE)]
		public int? ORDER { get; set; }
		/// <summary>
		/// 主題/單元編號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("主題/單元編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 索票說明
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("索票說明")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 索票連結
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("索票連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
	}
	#endregion

	#region 藝文講堂
	public sealed class LectureModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		public string NODE_ID { get; set; }
		/// <summary>
		/// 分類
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("分類")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// 標題
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("標題")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 講者
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("講者")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 活動地點
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動地點")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 適合對象
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("適合對象")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 報名方式
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("報名方式")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// 報名日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("報名日期")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// 報名人數
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("報名人數")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }
		/// <summary>
		/// 入場方式
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("入場方式")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
		/// <summary>
		/// 說明
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("說明")]
		//[DataType(DATA_TYPE_TEXTAREA_REQUIRED)]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT21 { get; set; }
		/// <summary>
		/// 注意事項
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("注意事項")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT22 { get; set; }
		/// <summary>
		/// 洽詢說明
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("洽詢說明")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT23 { get; set; }
		/// <summary>
		/// 0:電影講座：名稱是「影片介紹連結」
		/// 2:親子活動：名稱是「相關連結」
		/// </summary>
		[DisplayName("影片介紹連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT13_0 { get; set; }
		/// <summary>
		/// 0:電影講座：名稱是「影片介紹連結」
		/// 2:親子活動：名稱是「相關連結」
		/// </summary>
		[DisplayName("相關連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT13_2 { get; set; }
		/// <summary>
		/// 報名網址
		/// </summary>
		[DisplayName("報名網址")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT14 { get; set; }
		/// <summary>
		/// 出席者
		/// </summary>
		[DisplayName("出席者")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<HttpPostedFileBase> HPFs { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
	}
	#endregion

	#region 系統參數設定
	public sealed class SetupModel : BaseMetadata
	{
		public string ID { get; set; }
		public string NODE_ID { get; set; }
		/// <summary>
		/// 最新訊息筆數
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("最新訊息筆數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? CONTENT1 { get; set; }
	}
	#endregion

	#region 展演分類&展演場地
	public sealed class ExhibitionCategoryModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// PARENT_ID
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("PARENT_ID")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// 展演場地＞申請類別（0:兩者皆選 1:審查補助 2:一般租借）
		/// </summary>
		[DisplayName("申請類別")]
		[DataType(DATA_TYPE_DATE)]
		public string CONTENT1 { get; set; }
	}
	#endregion

	#region 檔期管理
	public sealed class ExhibitionModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 展演團隊
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("展演團隊")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 主辦類型
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("主辦類型")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 演出館別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("演出館別")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 演出場地
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("演出場地")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 展演類型
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("展演類型")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// 活動類型
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動類型")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// 活動名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }
		/// <summary>
		/// 聯絡人
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("聯絡人")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
		/// <summary>
		/// 聯絡電話
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("聯絡電話")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// 聯絡手機
		/// </summary>
		[DisplayName("聯絡手機")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT10 { get; set; }
		/// <summary>
		/// 電子郵件
		/// </summary>
		[Email]
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("電子郵件")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT11 { get; set; }
		/// <summary>
		/// 傳真號碼
		/// </summary>
		[DisplayName("傳真號碼")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT12 { get; set; }
		/// <summary>
		/// 展演說明
		/// </summary>
		[DisplayName("展演說明")]
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public string CONTENT13 { get; set; }
		/// <summary>
		/// 相關連結
		/// </summary>
		[DisplayName("相關連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT14 { get; set; }
		/// <summary>
		/// 入場方式
		/// </summary>
		[DisplayName("入場方式")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT15 { get; set; }
		/// <summary>
		/// 入場方式-其他
		/// </summary>
		[DisplayName("入場方式")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT15_OTHER { get; set; }
		/// <summary>
		/// 入場方式-索票連結
		/// </summary>
		[DisplayName("索票連結")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT16 { get; set; }
		/// <summary>
		/// 公開演出
		/// </summary>
		[DisplayName("公開演出")]
		//[Range(typeof(bool), "false", "true")]
		public bool DECIMAL1 { get; set; }
		/// <summary>
		/// 首頁最新展演活動
		/// </summary>
		[DisplayName("首頁最新展演活動")]
		//[Range(typeof(bool), "false", "true")]
		public bool DECIMAL2 { get; set; }
		/// <summary>
		/// 首頁活動預告
		/// </summary>
		[DisplayName("首頁活動預告")]
		//[Range(typeof(bool), "false", "true")]
		public bool DECIMAL7 { get; set; }
		/// <summary>
		/// 身心障礙索票
		/// </summary>
		[DisplayName("身心障礙索票")]
		//[Range(typeof(bool), "false", "true")]
		public bool DECIMAL3 { get; set; }
		/// <summary>
		/// 參加人數
		/// </summary>
		[DisplayName("參加人數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL4 { get; set; }
		/// <summary>
		/// 入場方式-發送張數
		/// </summary>
		[DisplayName("發送張數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL5 { get; set; }
		/// <summary>
		/// 便民服務＞索票資訊：索取張數
		/// </summary>
		[DisplayName("索取張數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL6 { get; set; }
		/// <summary>
		/// 入場方式-開放索票時間
		/// </summary>
		[DisplayName("開放索票時間")]
		[DataType(DATA_TYPE_DATETIME)]
		public DateTime? DATETIME1 { get; set; }
		/// <summary>
		/// 活動內容 & 入場方式說明
		/// </summary>
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public List<string> MAX_CONTENT { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<HttpPostedFileBase> HPFs { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
	}
	#endregion

	#region 身心障礙索票
	public sealed class DisabilityModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		//[DisplayName("")]
		[DataType(DATA_TYPE_TITLE)]
		public string ID { get; set; }
		/// <summary>
		/// 固定值：DISABILITY
		/// </summary>
		//[DisplayName("")]
		[DataType(DATA_TYPE_TITLE)]
		public string PLUS_TYPE { get; set; }
		/// <summary>
		/// 固定值：0
		/// </summary>
		//[DisplayName("")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// 殘障手冊編號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("殘障手冊編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("姓名")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 性別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("性別")]
		[DataType(DATA_TYPE_SEXTYPE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 聯絡電話
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("聯絡電話")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// E-mail
		/// </summary>
		[DisplayName("E-mail")]
		[DataType(DATA_TYPE_TITLE)]
		[Email(ErrorMessage = "Email格式不正確")]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// 索票票種
		/// 0:陪同席
		/// 1:陪同席+輪椅席
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("索票票種")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// 演出時間編號
		/// </summary>
		[DisplayName("演出時間編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }
		/// <summary>
		/// 演出開始時間
		/// </summary>
		[DisplayName("演出開始時間")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime DATETIME1 { get; set; }
	}
	#endregion

	#region 下載專區
	public sealed class DownloadModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// 類別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("類別")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// 新增類別
		/// </summary>
		[DisplayName("新增類別")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID_OTHER { get; set; }
		/// <summary>
		/// 附件上傳
		/// </summary>
		public List<HttpPostedFileBase> HPFs { get; set; }
		/// <summary>
		/// 附件上傳
		/// </summary>
		public List<ATTACHMENT> ATTAs { get; set; }
	}
	#endregion

	#region 電子報
	public sealed class NewsletterModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 標題
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("標題")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// 預計發送日期
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("預計發送日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime CONTENT1 { get; set; }
		/// <summary>
		/// 實際發送日期
		/// </summary>
		[DisplayName("實際發送日期")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 資料抓取日期（起）
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("資料抓取日期")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 資料抓取日期（迄）
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("資料抓取日期")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 電子報類別（0:原電子報 1:自訂電子報）
		/// </summary>
		[DisplayName("電子報類別")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// 發送群組
		/// </summary>
		[DisplayName("發送群組")]
		[DataType(DATA_TYPE_TITLE)]
		public List<string> MEMO { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		public List<ATTACHMENT> PICs { get; set; }
	}
	#endregion

	#region 可領用物品項目管理
	public sealed class InventoryItemModel : BaseMetadata
	{
		/// <summary>
		/// 物品類別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("物品類別")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 物品名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("物品名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// 安全數量
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("安全數量")]
		[DataType(DATA_TYPE_TITLE)]
		public int? ORDER { get; set; }
		/// <summary>
		/// 物品單位
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("物品單位")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 目前數量
		/// </summary>
		[DisplayName("目前數量")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// 圖片上傳
		/// </summary>
		[DisplayName("圖片上傳")]
		public HttpPostedFileBase hpf { get; set; }
		public ATTACHMENT atta { get; set; }
		/// <summary>
		/// 開放領用申請(1:是、0:否)
		/// </summary>
		[DisplayName("開放領用申請")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
	}
	#endregion

	#region 物品領品/出入庫
	/// <summary>
	/// 主單
	/// </summary>
	public sealed class InventoryRequisitionModel : BaseMetadata
	{
		/// <summary>
		/// 請領單/出入庫單編號 (年月日+流水號5碼)
		/// </summary>
		[DisplayName("編號")]
		public string ID { get; set; }
		/// <summary>
		/// INVENTORY_REQUISITION
		/// </summary>
		public string NODE_ID { get; set; }
		/// <summary>
		/// 請領日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領日期")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
		public DateTime CREATE_DATE { get; set; }
		/// <summary>
		/// 請領人
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領人")]
		[DataType(DATA_TYPE_TITLE)]
		public string CREATER { get; set; }
		/// <summary>
		/// 請領單位
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領單位")]
		[DataType(DATA_TYPE_TITLE)]
		public string DATA_TYPE { get; set; }
		/// <summary>
		/// 功能別(1:庫存管理 2:物品領用)
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("功能別")]
		[DataType(DATA_TYPE_TITLE)]
		public string STATUS { get; set; }
		/// <summary>
		/// 請領原因
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領原因")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 審核狀態(0:待簽核 1:核准 2:駁回)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("審核狀態")]
		[DataType(DATA_TYPE_TITLE)]
		public int? ORDER { get; set; }
		/// <summary>
		/// 駁回原因
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("駁回原因")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT28 { get; set; }
		/// <summary>
		/// 審核者(帳號)
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("審核者")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT29 { get; set; }
		/// <summary>
		/// 審核日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("審核日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? DATETIME1 { get; set; }
		/// <summary>
		/// 細項
		/// </summary>
		public List<InventoryRequisitionDetailModel> DETAILs { get; set; }
	}
	/// <summary>
	/// 細項
	/// </summary>
	public sealed class InventoryRequisitionDetailModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 請領單/出入庫單編號 (年月日+流水號5碼)
		/// </summary>
		public string MAIN_ID { get; set; }
		/// <summary>
		/// 請領日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime CREATE_DATE { get; set; }
		/// <summary>
		/// 請領人
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領人")]
		[DataType(DATA_TYPE_TITLE)]
		public string CREATER { get; set; }
		/// <summary>
		/// 物品編號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("物品編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string PLUS_TYPE { get; set; }
		/// <summary>
		/// 物品名稱
		/// </summary>
		[DisplayName("物品名稱")]
		public string PLUS_TYPE_TITLE { get; set; }
		/// <summary>
		/// 物品單位
		/// </summary>
		[DisplayName("物品單位")]
		public string PLUS_TYPE_UNIT { get; set; }
		/// <summary>
		/// 物品圖片
		/// </summary>
		[DisplayName("物品圖片")]
		public string PLUS_TYPE_IMG { get; set; }
		/// <summary>
		/// 固定值：INVENTORY_REQUISITION_ITEM
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("固定值")]
		[DataType(DATA_TYPE_TITLE)]
		public string STATUS { get; set; }
		/// <summary>
		/// 0:入庫 1:出庫
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("出入庫")]
		[DataType(DATA_TYPE_TITLE)]
		public int? ORDER { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("數量")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL1 { get; set; }
		/// <summary>
		/// 目前數量
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("目前數量")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL2 { get; set; }
		/// <summary>
		/// 出入庫原因
		/// </summary>
		[DisplayName("出入庫原因")]
		[DataType(DATA_TYPE_TEXTAREA)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 開放領用申請(1:是 0:否)
		/// </summary>
		[DisplayName("開放領用申請")]
		public string CONTENT8 { get; set; }
	}
	#endregion

	#region 報表
	public class ReportModel
	{
		public string CONTENT1 { get; set; }
		public string CONTENT2 { get; set; }
		public string CONTENT3 { get; set; }
		public string CONTENT4 { get; set; }
		public string CONTENT5 { get; set; }
		public string CONTENT6 { get; set; }
		public string CONTENT7 { get; set; }
		public string CONTENT8 { get; set; }
		public string CONTENT9 { get; set; }
		public string CONTENT10 { get; set; }
		public string CONTENT11 { get; set; }
		public string CONTENT12 { get; set; }
		public string CONTENT13 { get; set; }
		public string CONTENT14 { get; set; }
		public string CONTENT15 { get; set; }
		public string CONTENT16 { get; set; }
		public string CONTENT17 { get; set; }
		public string CONTENT18 { get; set; }
		public string CONTENT19 { get; set; }
		public string CONTENT20 { get; set; }
		public decimal? DECIMAL1 { get; set; }
		public decimal? DECIMAL2 { get; set; }
		public decimal? DECIMAL3 { get; set; }
		public decimal? DECIMAL4 { get; set; }
		public decimal? DECIMAL5 { get; set; }
		public decimal? DECIMAL6 { get; set; }
		public decimal? DECIMAL7 { get; set; }
		public decimal? DECIMAL8 { get; set; }
		public decimal? DECIMAL9 { get; set; }
		public decimal? DECIMAL10 { get; set; }
		public decimal? DECIMAL11 { get; set; }
		public decimal? DECIMAL12 { get; set; }
		public decimal? DECIMAL13 { get; set; }
		public decimal? DECIMAL14 { get; set; }
		public decimal? DECIMAL15 { get; set; }
		public decimal? DECIMAL16 { get; set; }
		public decimal? DECIMAL17 { get; set; }
		public decimal? DECIMAL18 { get; set; }
		public decimal? DECIMAL19 { get; set; }
		public decimal? DECIMAL20 { get; set; }
	}
	#endregion

	#region 動支登記
	public sealed class BudgetRegModel : BaseMetadata
	{
		/// <summary>
		/// 申請單編號 (年月日+流水號3碼)
		/// </summary>
		[DisplayName("編號")]
		public string ID { get; set; }
		/// <summary>
		/// fun10_02
		/// </summary>
		public string NODE_ID { get; set; }
		/// <summary>
		/// 申請日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("申請日期")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
		public DateTime CREATE_DATE { get; set; }
		/// <summary>
		/// 請領人
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("請領人")]
		[DataType(DATA_TYPE_TITLE)]
		public string CREATER { get; set; }
		/// <summary>
		/// 申請單位
		/// </summary>
		[DisplayName("申請單位")]
		[DataType(DATA_TYPE_TITLE)]
		public string DATA_TYPE { get; set; }
		/// <summary>
		/// 年份
		/// </summary>
		[DisplayName("年份")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// 組別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("組別")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT11 { get; set; }
		/// <summary>
		/// 類別
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("類別")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT12 { get; set; }
		/// <summary>
		/// 細目
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("細目")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT13 { get; set; }
		/// <summary>
		/// 說明
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("說明")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 組別>類別>細目>說明
		/// </summary>
		public string CONTENT1_NAME { get; set; }
		/// <summary>
		/// 執行細目
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("執行細目")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 動支字號
		/// </summary>
		[DisplayName("動支字號")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 承辦人
		/// </summary>
		[DisplayName("承辦人")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 廠商
		/// </summary>
		[DisplayName("廠商")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// 簽會數
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("簽會數")]
		[DataType(DATA_TYPE_TITLE)]
		[Range(1, int.MaxValue, ErrorMessage = "不可為 0！")]
		public int? DECIMAL1 { get; set; }
		/// <summary>
		/// 執行數
		/// </summary>
		[DisplayName("執行數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL2 { get; set; }
		/// <summary>
		/// 承辦日期
		/// </summary>
		[DisplayName("承辦日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime? DATETIME1 { get; set; }

		//==========僅頁面顯示,未儲存至資料庫==========
		/// <summary>
		/// 預算數
		/// </summary>
		[DisplayName("預算數")]
		[DataType(DATA_TYPE_TITLE)]
		public decimal? DECIMAL3 { get; set; }
		/// <summary>
		/// 剩餘數
		/// </summary>
		[DisplayName("剩餘數")]
		[DataType(DATA_TYPE_TITLE)]
		public decimal? DECIMAL4 { get; set; }
		/// <summary>
		/// 已執行數
		/// </summary>
		[DisplayName("已執行數")]
		[DataType(DATA_TYPE_TITLE)]
		public decimal? DECIMAL5 { get; set; }
	}
	#endregion

	#region 動支預算科目
	public sealed class BudgetSubjectModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 上層編號
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("上層編號")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// 上層完整中文路徑
		/// </summary>
		public string PARENT_TEXT { get; set; }
		/// <summary>
		/// 科目名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("科目名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// 年度
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("年度")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// 年度預算
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("年度預算")]
		[DataType(DATA_TYPE_TITLE)]
		[Range(1, int.MaxValue, ErrorMessage = "不可為 0！")]
		public int? CONTENT1 { get; set; }
		/// <summary>
		/// 下層分類 (1:有 0:無)
		/// </summary>
		[DisplayName("下層分類")]
		[DataType(DATA_TYPE_DATE)]
		public string URL { get; set; }
		/// <summary>
		/// 網址 20201012 add
		/// </summary>
		[DisplayName("網址")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
	}
	#endregion

	#region 團隊調查統計系統
	public sealed class TeamSurveyModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 上層編號
		/// </summary>
		public string NODE_ID { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime CREATE_DATE { get; set; }
		public string CREATER { get; set; }

		/// <summary>
		/// 團隊名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("團隊名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 節目名稱
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("節目名稱")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 負責舞監
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("負責舞監")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// Q1
		/// </summary>
		public List<string> CONTENT4 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4_OTH { get; set; }
		/// <summary>
		/// Q2
		/// </summary>
		public string CONTENT5 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5_OTH { get; set; }
		/// <summary>
		/// Q3
		/// </summary>
		public List<string> CONTENT6 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6_OTH { get; set; }
		/// <summary>
		/// Q4
		/// </summary>
		public List<string> CONTENT7 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7_OTH { get; set; }
		/// <summary>
		/// Q5
		/// </summary>
		public List<string> CONTENT8 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8_OTH { get; set; }
		/// <summary>
		/// Q6
		/// </summary>
		public List<string> CONTENT9 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9_OTH { get; set; }
		/// <summary>
		/// Q7
		/// </summary>
		public List<string> CONTENT10 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT10_OTH { get; set; }
		/// <summary>
		/// Q8
		/// </summary>
		public List<string> CONTENT11 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT11_OTH { get; set; }
		/// <summary>
		/// Q9
		/// </summary>
		public List<string> CONTENT12 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT12_OTH { get; set; }
		/// <summary>
		/// Q10
		/// </summary>
		public List<string> CONTENT13 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT13_OTH { get; set; }
		/// <summary>
		/// Q11
		/// </summary>
		public List<string> CONTENT14 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT14_OTH { get; set; }
		/// <summary>
		/// Q12
		/// </summary>
		public string CONTENT15 { get; set; }
		/// <summary>
		/// Q13
		/// </summary>
		public List<string> CONTENT16 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT16_OTH { get; set; }
		/// <summary>
		/// Q14
		/// </summary>
		public string CONTENT17 { get; set; }
		/// <summary>
		/// Q15 意見與建議
		/// </summary>
		[DisplayName("意見與建議")]
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public string CONTENT110 { get; set; }
		/// <summary>
		/// 演出日期(開始)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("演出日期")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime DATETIME1 { get; set; }
		/// <summary>
		/// 演出日期(結束)
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("演出日期")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime DATETIME2 { get; set; }
		/// <summary>
		/// 填寫日期
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("填寫日期")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime DATETIME3 { get; set; }
	}
	#endregion

	#region 觀眾問卷調查統計
	public sealed class AudienceSurveyModel : BaseMetadata
	{
		public string ExhibitionInfo1 { get; set; }
		public string ExhibitionInfo2 { get; set; }
		public string ExhibitionInfo3 { get; set; }
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 上層編號
		/// </summary>
		public string NODE_ID { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime CREATE_DATE { get; set; }
		public string CREATER { get; set; }
		/// <summary>
		/// 檔期編號
		/// </summary>
		public string DATA_TYPE { get; set; }
		/// <summary>
		/// 居住地
		/// </summary>
		public string CONTENT31 { get; set; }
		/// <summary>
		/// 性別(1:男 2:女)
		/// </summary>
		public string CONTENT32 { get; set; }
		/// <summary>
		/// 婚姻狀況(0:未婚 1:已婚)
		/// </summary>
		public string CONTENT33 { get; set; }
		/// <summary>
		/// 年齡
		/// </summary>
		public string CONTENT34 { get; set; }
		/// <summary>
		/// 學歷
		/// </summary>
		public string CONTENT35 { get; set; }
		/// <summary>
		/// 職業
		/// </summary>
		public string CONTENT36 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT36_OTH { get; set; }
		/// <summary>
		/// E-mail
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[Email]
		[DisplayName("E-mail")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// Q15 E-mail
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[Email]
		[DisplayName("E-mail")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// Q15 通訊地址
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("通訊地址")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// Q1
		/// </summary>
		public List<string> CONTENT4 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4_OTH { get; set; }
		/// <summary>
		/// Q2
		/// </summary>
		public List<string> CONTENT5 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5_OTH { get; set; }
		/// <summary>
		/// Q3
		/// </summary>
		public string CONTENT6 { get; set; }
		/// <summary>
		/// Q4
		/// </summary>
		public List<string> CONTENT7 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7_OTH { get; set; }
		/// <summary>
		/// Q5
		/// </summary>
		public string CONTENT8 { get; set; }
		/// <summary>
		/// Q6
		/// </summary>
		public string CONTENT9 { get; set; }
		/// <summary>
		/// Q7
		/// </summary>
		public string CONTENT10 { get; set; }
		/// <summary>
		/// Q8
		/// </summary>
		public string CONTENT11 { get; set; }
		/// <summary>
		/// Q9
		/// </summary>
		public string CONTENT12 { get; set; }
		/// <summary>
		/// Q10
		/// </summary>
		public string CONTENT13 { get; set; }
		/// <summary>
		/// Q11
		/// </summary>
		public string CONTENT14 { get; set; }
		/// <summary>
		/// Q12
		/// </summary>
		public string CONTENT15 { get; set; }
		/// <summary>
		/// Q13
		/// </summary>
		public List<string> CONTENT16 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT16_OTH { get; set; }
		/// <summary>
		/// Q14
		/// </summary>
		public List<string> CONTENT17 { get; set; }
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT17_OTH { get; set; }
		/// <summary>
		/// Q16 意見與建議
		/// </summary>
		[DisplayName("意見與建議")]
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public string CONTENT110 { get; set; }
		/// <summary>
		/// 活動日期 / 演出時間
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動日期 / 演出時間")]
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime DATETIME1 { get; set; }
	}
	#endregion

	#region 觀眾問卷調查統計(前台)
	public sealed class SurveyModel : BaseMetadata
	{
		/// <summary>
		/// 編號(GUID)
		/// </summary>
		public string ID { get; set; }
		/// <summary>
		/// 上層編號
		/// </summary>
		public string NODE_ID { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[DataType(DATA_TYPE_DATE)]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime CREATE_DATE { get; set; }
		public string CREATER { get; set; }
		/// <summary>
		/// 館別
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("館別")]
		public string DATA_TYPE { get; set; }
		/// <summary>
		/// 檔期編號
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("檔期編號")]
		public string STATUS { get; set; }
		/// <summary>
		/// 問卷類型
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("問卷類型")]
		public int ORDER { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("姓名")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// 性別(1:男 2:女 -1:其他)
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("性別")]
		[DataType(DATA_TYPE_SEXTYPE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// 電話
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("電話")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// 現居地
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("現居地")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// 現居地-區域
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("現居地-區域")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// E-mail
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[Email(ErrorMessage = "E-mail 格式不正確")]
		[DisplayName("E-mail")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// 年齡層
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("年齡層")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }

		/// <summary>
		/// 演出類型
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		//[DisplayName("演出類型")]
		//[DataType(DATA_TYPE_TITLE)]
		//public string CONTENT8 { get; set; }
		/// <summary>
		/// 入場方式
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		//[DisplayName("入場方式")]
		//[DataType(DATA_TYPE_TITLE)]
		//public string CONTENT9 { get; set; }
		
		/// <summary>
		/// 意見與建議
		/// </summary>
		[DisplayName("意見與建議")]
		[DataType(DATA_TYPE_TEXTAREA_MAXLENGTH)]
		public string CONTENT109 { get; set; }
		/// <summary>
		/// 問卷總題數(不包含 意見與建議)
		/// </summary>
		[DisplayName("問卷總題數")]
		[DataType(DATA_TYPE_TITLE)]
		public int DECIMAL1 { get; set; }
		/// <summary>
		/// 索取張數 20210323 add
		/// </summary>
		[DisplayName("索取張數")]
		[DataType(DATA_TYPE_TITLE)]
		public int? DECIMAL2 { get; set; }
		/// <summary>
		/// 活動日期
		/// </summary>
		//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動日期")]
		[DataType(DATA_TYPE_DATE)]
		public DateTime DATETIME1 { get; set; }
		/// <summary>
		/// 活動日期 DDL
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("活動日期")]
		[DataType(DATA_TYPE_TITLE)]
		public string DATE_SELECT { get; set; }
		/// <summary>
		/// 問題
		/// </summary>
		public List<SurveyQues> Ques { get; set; }
	}

	public enum QuesType
	{
		/// <summary>
		/// 單選
		/// </summary>
		[Description("單選")]
		radio,
		/// <summary>
		/// 複選
		/// </summary>
		[Description("複選")]
		checkbox,		
		/// <summary>
		/// 問答
		/// </summary>
		[Description("問答")]
		textarea
	}

	public sealed class SurveyQues
	{
		/// <summary>
		/// 流水號
		/// </summary>
		public int Seq { get; set; }
		/// <summary>
		/// 題型
		/// </summary>
		public QuesType Type { get; set; }
		/// <summary>
		/// 問題
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 問題 - 選項
		/// </summary>
		public List<SurveyOpt> Opts { get; set; }
		/// <summary>
		/// 答案 - 問答題 
		/// </summary>
		public string Ans { get; set; }
	}

	public sealed class SurveyOpt
	{
		/// <summary>
		/// 流水號
		/// </summary>
		public int Seq { get; set; }
		/// <summary>
		/// 選項 - 文字
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 選項 - 值
		/// </summary>
		public string Val { get; set; }
		/// <summary>
		/// 其他(true:有 false:無)
		/// </summary>
		public bool hasOth { get; set; }
		/// <summary>
		/// 答案
		/// </summary>
		public string Ans { get; set; }
		/// <summary>
		/// 答案 - 其他
		/// </summary>
		public string Other { get; set; }
		/// <summary>
		/// 問題 - 選項 20210315 ting add
		/// </summary>
		public List<SurveyOpt> SubOpts { get; set; }
	}

	public sealed class SurveyOther
	{
		/// <summary>
		/// 答案編號
		/// </summary>
		public string Val { get; set; }
		/// <summary>
		/// 答案 - 其他
		/// </summary>
		public string Other { get; set; }
	}
	
	#endregion

	public sealed class AttaNotesModel
	{
		/// <summary>
		/// true:顯示多檔上傳說明
		/// </summary>
		public bool bMultiple { get; set; }
		/// <summary>
		/// 說明類型：1=圖片說明 2=檔案說明 3=圖片和檔案說明
		/// </summary>
		public int? iDescType { get; set; }
		/// <summary>
		/// 圖片大小限制
		/// </summary>
		public int? iPicSizeLimit { get; set; }
		/// <summary>
		/// 檔案大小限制
		/// </summary>
		public int? iFileSizeLimit { get; set; }
		/// <summary>
		/// 建議圖片的寬高
		/// </summary>
		public string sPicWH { get; set; }
	}

    #endregion

    
}
