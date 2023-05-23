using System;
using KingspModel.DB;

namespace KingspModel.DataModel
{
    /// <summary>
    /// Metadata Base Class
    /// </summary>
    public class BaseMetadata : IDisposable
    {
        bool isDisposed = false;

        #region const property base

        /// <summary>
        /// DataType("Title")
        /// </summary>
        protected const string DATA_TYPE_TITLE = "Title";
        /// <summary>
        /// DataType("Date") 僅日期選擇器
        /// </summary>
        protected const string DATA_TYPE_DATE = "Date";
        /// <summary>
        /// DataType("DateTaiwan") 僅日期選擇器 (呈現方式為民國年)
        /// </summary>
        protected const string DATA_TYPE_DATE_TAIWAN = "DateTaiwan";
        /// <summary>
        /// DataType("DateTime") 日期和時分選擇
        /// </summary>
        protected const string DATA_TYPE_DATETIME = "DateTime";
        /// <summary>
        /// DataType("TextArea")
        /// </summary>
        protected const string DATA_TYPE_TEXTAREA = "TextArea";
        /// <summary>
        /// DataType("TextAreaRequired") 必填
        /// </summary>
        protected const string DATA_TYPE_TEXTAREA_REQUIRED = "TextAreaRequired";
        /// <summary>
        /// DataType("TextAreaMaxLength") 字數限制預設500 or 自訂 length=???
        /// </summary>
        protected const string DATA_TYPE_TEXTAREA_MAXLENGTH = "TextAreaMaxLength";
        /// <summary>
        /// DataType("TextAreaRequiredMaxLength") 必填 and 字數限制預設500 or 自訂 length=???
        /// </summary>
        protected const string DATA_TYPE_TEXTAREA_REQUIRED_MAXLENGTH = "TextAreaRequiredMaxLength";
        /// <summary>
        /// DataType("CKeditor")
        /// </summary>
        protected const string DATA_TYPE_CKEDITOR = "CKeditor";
		/// <summary>
		/// DataType("CKeditorRequired")
		/// </summary>
		protected const string DATA_TYPE_CKEDITOR_REQUIRED = "CKeditorRequired";
		/// <summary>
		/// DataType("SexType") 性別RadioButton
		/// </summary>
		protected const string DATA_TYPE_SEXTYPE = "SexType";
        /// <summary>
        /// DataType("FileUpload") jquery file upload 上傳元件
        /// </summary>
        protected const string DATA_TYPE_FILEUPLOAD = "FileUpload";
        /// <summary>
        /// DataType("CityType") 縣市下拉選單
        /// </summary>
        protected const string DATA_TYPE_CITYTYPE = "CityType";
        /// <summary>
        /// DataType("CountryTypeNoAll") 鄉鎮下拉選單，沒有「全部」選項
        /// <para>頁面要另外配合jquery才能縣市連動</para>
        /// </summary>
        protected const string DATA_TYPE_COUNTRYTYPE_NO_ALL = "CountryTypeNoAll";
        /// <summary>
        /// DataType("AreaType") 北中南東離島下拉選單
        /// </summary>
        protected const string DATA_TYPE_AREATYPE = "AreaType";
        /// <summary>
        /// Required(ErrorMessage = "*")
        /// </summary>
        protected const string REQUIRED_ERROR_MESSAGE = "*";
		/// <summary>
		/// 請輸入正確的格式
		/// </summary>
		protected const string FORMAT_ERROR_MESSAGE = "請輸入正確的格式";
        /// <summary>
        /// 已重覆，請重新設定
        /// </summary>
        protected const string DEFAULT_REPEAT_KEY = "已重覆，請重新設定";
        /// <summary>
        /// 至少選擇一項
        /// </summary>
        protected const string DEFAULT_AT_LEAST_ONE = "至少選擇一項";

		/// <summary>
		/// DataType("YesNo")
		/// </summary>
		protected const string DATA_YESNO = "YesNo";
		#endregion

		#region const property for project
		/// <summary>
		/// DataType("FileUpload2") jquery file upload 上傳元件
		/// </summary>
		protected const string DATA_TYPE_FILEUPLOAD2 = "FileUpload2";
		/// <summary>
		/// DataType("FileUpload3") jquery file upload 上傳元件
		/// </summary>
		protected const string DATA_TYPE_FILEUPLOAD3 = "FileUpload3";
		/// <summary>
		/// DataType("FileUpload4") jquery file upload 上傳元件
		/// </summary>
		protected const string DATA_TYPE_FILEUPLOAD4 = "FileUpload4";
		#endregion

		#region IDisposable 成員

		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        ~BaseMetadata()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (this != null)
                {
                    this.Dispose();
                }
                isDisposed = true;
            }
        }
    }
}
