using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KingspModel.Attributes;
using System.Web.Mvc;

namespace KingspModel.DataModel
{
    #region 登入Model LogOnModel

    /// <summary>
    /// 登入Model
    /// </summary>
    public sealed class LogOnModel : BaseMetadata
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("帳  號")]
        [DataType(DATA_TYPE_TITLE)]
		//[RegularExpression("^\\w+$", ErrorMessage = "*")]
		public string UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DataType(DataType.Password)]
        [DisplayName("密  碼")]
        public string Password { get; set; }

        [DisplayName("記住我?")]
        bool RememberMe { get; set; }

    }

    #endregion

    #region SYSUSER 編輯 model SysUserModel

    /// <summary>
    /// SYSUSER 編輯 model
    /// </summary>
    public sealed class SysUserModel : BaseMetadata
    {
		public byte ENABLE { get; set; }
		/// <summary>
		/// User_Id
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("帳號")]
        [DataType(DATA_TYPE_TITLE)]
        [RegularExpression(Function.USER_ID_REGEX, ErrorMessage = "帳號必須 首字英文字母")]
        [Remote("SysUserExist", "Json", ErrorMessage = DEFAULT_REPEAT_KEY)]
        //[StringLength(20, ErrorMessage = FORMAT_ERROR_MESSAGE, MinimumLength = 8)]
        public string USER_ID { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("變更密碼")]
        [DataType(DataType.Password)]
        [RegularExpression(Function.PASSWORD_REGEX, ErrorMessage = "密碼必須 首字英文字母 + 英數混合 8-20 字元")]
		public string PASSWORD { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("姓名")]
        [DataType(DATA_TYPE_TITLE)]
        public string NAME { get; set; }
        /// <summary>
        /// Email
        /// </summary>        
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [Remote("SysEmailExist", "Json", AdditionalFields = "USER_ID", ErrorMessage = DEFAULT_REPEAT_KEY)]
		[DisplayName("Email")]
		[DataType(DATA_TYPE_TITLE)]
        [Email(ErrorMessage = FORMAT_ERROR_MESSAGE)]
        public string EMAIL { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		[DisplayName("備註")]
        [DataType(DATA_TYPE_TEXTAREA)]
        public string MEMO { get; set; }
        /// <summary>
        /// 身分別(0:管理者 1:兌換點)
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("身分別")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT1 { get; set; }
        /// <summary>
        /// 區域
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("區域")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("地址")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT3 { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("聯絡人")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT4 { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("電話")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT5 { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("英文名稱")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT6 { get; set; }
        /// <summary>
        /// 英文地址
        /// </summary>
        //[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("英文地址")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT7 { get; set; }
        [DisplayName("備用")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT8 { get; set; }
        [DisplayName("備用")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT9 { get; set; }
        [DisplayName("備用")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT10 { get; set; }

    }

    /// <summary>
    /// SYSUSER 變更密碼 model
    /// </summary>
    public sealed class SysUserChangePasswordModel : BaseMetadata
    {
        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[Display(Name = "ResetPassword_NewPassword", ResourceType = typeof(Resources.Resource))]
		[DataType(DataType.Password)]
        [RegularExpression(Function.PASSWORD_REGEX, ErrorMessage = "密碼必須 首字英文字母 + 英數混合 8-20 字元")]
		public string Password { get; set; }
        /// <summary>
        /// Confirm Password
        /// </summary>
        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DataType(DataType.Password)]
		[Display(Name = "ResetPassword_ConfirmNewPassword", ResourceType = typeof(Resources.Resource))]
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "新密碼 與 確認新密碼 不同")]
        public string ConfirmPassword { get; set; }
    }

    #endregion

    #region 自訂的群組權限 RoleGroupModel

    /// <summary>
    /// 自訂的群組權限
    /// </summary>
    public sealed class RoleGroupModel : BaseMetadata
    {
        public string ROLE_ID { get; set; }

        public string GROUP_TYPE { get; set; }

        [Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
        [DisplayName("群組名稱")]
        [DataType(DATA_TYPE_TITLE)]
        public string TITLE { get; set; }

        [DisplayName("備註")]
        [DataType(DATA_TYPE_TEXTAREA)]
        public string MEMO { get; set; }

        [Required(ErrorMessage = DEFAULT_AT_LEAST_ONE)]
        [DisplayName("群組權限")]
        public string ROLETREELIST { get; set; }

        [DisplayName("英文標題")]
        [DataType(DATA_TYPE_TITLE)]
        public string CONTENT2 { get; set; }
    }

    #endregion

    #region AuthorityRight

    /// <summary>
    /// 自訂的權限 Class
    /// </summary>
    [Serializable]
    public sealed class AuthorityRight
    {
        /// <summary>
        /// 群組ID
        /// </summary>
        public string ROLE_GROUP_ID { get; set; }
        /// <summary>
        /// 功能NODE_ID
        /// </summary>
        public string NODE_ID { get; set; }
        /// <summary>
        /// 查詢
        /// </summary>
        public bool SEARCH { get; set; }
        /// <summary>
        /// 新增
        /// </summary>
        public bool ADD { get; set; }
        /// <summary>
        /// 修改
        /// </summary>
        public bool UPDATE { get; set; }
        /// <summary>
        /// 刪除
        /// </summary>
        public bool DELETE { get; set; }
    }

    #endregion

    #region JS FullCalendar event Model

    /// <summary>
    /// FullCalendar event source
    /// </summary>
    public sealed class EventModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string content { get; set; }
        public string backgroundColor { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
        public string borderColor { get; set; }
        public string url { get; set; }
        public string className { get; set; }
        public bool allDay { get; set; }
    }

    #endregion

    #region 照片特殊Model PicInfo
    /// <summary>
    /// 照片 exif Model
    /// </summary>
    public sealed class PicInfo : BaseMetadata
    {
        [DisplayName("照片編號")]
        public string PicId { get; set; }

        [DisplayName("照片路徑")]
        public string PicUrl { get; set; }

        [DisplayName("照片名稱")]
        public string Title { get; set; }

        [DisplayName("作者")]
        public string Person { get; set; }

        [DisplayName("最大尺寸")]
        public string MaxSize { get; set; }

        [DisplayName("上傳日期")]
        public DateTime? UploadDate { get; set; }

        [DisplayName("拍攝日期")]
        public DateTime? ShootDate { get; set; }

        [DisplayName("拍攝地點")]
        public string point { get; set; }
    }

    #endregion

    #region 圖表Group Model

    /// <summary>
    /// AmChartsGroupModel Model
    /// </summary>
    public sealed class AmChartsGroupModel
    {
        /// <summary>
        /// 主題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 資料集合
        /// </summary>
        public List<AmChartsModel> Datas { get; set; }
        /// <summary>
        /// JavaScriptSerializer.Serialize 格式的字串
        /// </summary>
        public string JSSerializer { get; set; }
    }
    #endregion

    #region 圖表Model

    /// <summary>
    /// AmChartsModel Model
    /// </summary>
    public sealed class AmChartsModel
    {
        /// <summary>
        /// key
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// value
        /// </summary>
        public double count { get; set; }

    }
    #endregion

    #region 圖表Serial Group Model

    /// <summary>
    /// AmSerialChartsGroupModel Model
    /// </summary>
    public sealed class AmSerialChartsGroupModel
    {
        /// <summary>
        /// 主題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 資料集合
        /// </summary>
        public List<AmSerialChartsModel> Datas { get; set; }
        /// <summary>
        /// 資料集合
        /// </summary>
        public List<AmSerialGraphsChartsModel> GraphsDatas { get; set; }
        /// <summary>
        /// JavaScriptSerializer.Serialize 格式的字串 for Datas
        /// </summary>
        public string JSSerializer { get; set; }
        /// <summary>
        /// JavaScriptSerializer.Serialize 格式的字串 for GraphsDatas
        /// </summary>
        public string JSSerializer_graphs { get; set; }
    }
    #endregion

    #region 圖表Serial Model

    /// <summary>
    /// AmSerialChartsModel Model
    /// </summary>
    public sealed class AmSerialChartsModel
    {
        /// <summary>
        /// categoryField 群組key
        /// </summary>
        public string category { get; set; }
        public double value1 { get; set; }
        public double value2 { get; set; }
        public double value3 { get; set; }
        public double value4 { get; set; }
        public double value5 { get; set; }
        //public double value6 { get; set; }
        //public double value7 { get; set; }
        //public double value8 { get; set; }
        //public double value9 { get; set; }
        //public double value10 { get; set; }
    }
    #endregion

    #region 圖表Serial graphs Model

    /// <summary>
    /// AmSerialGraphsChartsModel Model
    /// </summary>
    public sealed class AmSerialGraphsChartsModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// balloonText
        /// </summary>
        public string balloonText { get; set; }
        /// <summary>
        /// title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// valueField
        /// </summary>
        public string valueField { get; set; }
        /// <summary>
        /// type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// fillAlphas
        /// </summary>
        public double fillAlphas { get; set; }
        /// <summary>
        /// lineAlpha
        /// </summary>
        public double lineAlpha { get; set; }
        /// <summary>
        /// labelText
        /// </summary>
        public string labelText { get; set; }
        /// <summary>
        /// labelPosition  "bottom", "top", "right", "left", "inside", "middle" 
        /// </summary>
        public string labelPosition { get; set; }
    }
    #endregion

    #region AD相關

    /// <summary>
    /// 同步AD用的model
    /// </summary>
    public class AD_Sync_Model
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 部門Id
        /// </summary>
        public decimal DepartmentId { get; set; }
        /// <summary>
        /// 職稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 職稱Id
        /// </summary>
        public string TitleId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 轉入日期(AD建立日期 whencreated)
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否新增
        /// </summary>
        public bool IsAdd { get; set; }

    }

    /// <summary>
    /// AD Server Department
    /// </summary>
    public class AD_Department_Model
    {
        /// <summary>
        /// ou
        /// </summary>
        public string Ou { get; set; }
        /// <summary>
        /// description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否新增
        /// </summary>
        public bool IsAdd { get; set; }
    }

    #endregion
}
