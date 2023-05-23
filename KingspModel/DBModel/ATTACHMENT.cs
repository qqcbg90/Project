using KingspModel.DataModel;
using KingspModel.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace KingspModel.DB
{
	[MetadataType(typeof(ATTACHMENTMetadata))]
    public partial class ATTACHMENT
    {
        #region const property
        /// <summary>
        /// 路徑格式 {0}{1}{2}
        /// </summary>
        const string PATH_FORMAT = "{0}{1}{2}";

        #endregion

        #region 建構子
        /// <summary>
        /// 初始化
        /// </summary>
        public ATTACHMENT()
        {
            SetUpValue(true, null);
        }
        /// <summary>
        /// 初始化 ID,CREATE_DATE,ORIGINAL_FILE_NAME,EXTENSION
        /// </summary>
        /// <param name="fileName">原始檔案名稱</param>
        /// <param name="isNew">是否新建立</param>
        public ATTACHMENT(string fileName = null, bool isNew = true)
        {
            SetUpValue(isNew, fileName);
        }
        /// <summary>
        /// 設定值 ID,CREATE_DATE,ORIGINAL_FILE_NAME,EXTENSION
        /// </summary>
        /// <param name="isNew">是否新建立</param>
        /// <param name="fileName">原始檔案名稱</param>
        public void SetUpValue(bool isNew = true, string fileName = null)
        {
            if (isNew)
            {
                this.ID = Function.GetGuid();
            }
            if (!fileName.IsNullOrEmpty())
            {
                this.CREATE_DATE = DateTime.Now;
                this.ORIGINAL_FILE_NAME = fileName;

				//20200619 統一改小寫，因為大寫在無障礙掃瞄會判定不符合
                this.EXTENSION = fileName.GetExtension().ToLower();
				if (this.EXTENSION.CheckStringValue(".jpeg"))
				{
					this.EXTENSION = ".jpg";
				}
            }
        }

        #endregion

        private class ATTACHMENTMetadata : BaseMetadata
        {
			//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string ATT_TYPE { get; set; }
			/// <summary>
			/// 建立日期
			/// </summary>
			[DisplayName("建立日期")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime CREATE_DATE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CREATER { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string UPDATER { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? UPDATE_DATE { get; set; }
			/// <summary>
			/// 檔案說明
			/// </summary>
			[DisplayName("檔案說明")]
            [DataType(DATA_TYPE_TEXTAREA)]
            public string DESCRIPTION { get; set; }
            /// <summary>
            /// 檔案名稱
            /// </summary>
            [DisplayName("檔案名稱")]
			[DataType(DATA_TYPE_DATE)]
			public string FILE_NAME { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string ORIGINAL_FILE_NAME { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string EXTENSION { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string MAIN_ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public int ORDER { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT1 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT2 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT3 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT4 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT5 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT6 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT7 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT8 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT9 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT10 { get; set; }
		}

		/// <summary>
		/// 資料庫無此欄位 額外新增
		/// </summary>
		public HttpPostedFileBase HPF { get; set; }

        /// <summary>
        /// Medium PicName
        /// </summary>
        public string Medium_PicName
        {
            set
            {
                Medium_PicName = value;
            }
            get
            {
                return string.Format(PATH_FORMAT, this.FILE_NAME.Replace(this.EXTENSION, string.Empty), Function.PIC_MEDIUM, this.EXTENSION);
            }
        }

        /// <summary>
        /// Small PicName
        /// </summary>
        public string Small_PicName
        {
            set
            {
                Small_PicName = value;
            }
            get
            {
                return string.Format(PATH_FORMAT, this.FILE_NAME.Replace(this.EXTENSION, string.Empty), Function.PIC_SMALL, this.EXTENSION);
            }
        }

        /// <summary>
        /// 設定 FILE_NAME
        /// </summary>
        public void SetUpFileName()
        {
			Random r = new Random();
			string sRandNum = (1000 + r.Next(0, 1000)).ToString().Substring(1);
			string sRandChar = Function.GetGuid().Substring(r.Next(0, 25), 5);
			this.FILE_NAME = string.Concat(this.CREATE_DATE.ToString("yyyyMMddHHmm"), sRandNum, sRandChar, this.ORDER, this.EXTENSION);
		}

        /// <summary>
        /// 取得檔案路徑
        /// </summary>
        /// <returns></returns>
        public string GetFile(bool isTrueUrl = false)
        {
            return Path.Combine(Function.GetUploadPath(isTrueUrl), FILE_NAME);
        }

        /// <summary>
        /// 取得圖片路徑
        /// </summary>
        /// <param name="type">不指定預設為原始圖片</param>
        /// <returns></returns>
        public string GetPic(PictureType type = PictureType.Original, bool TrueUrl = false)
        {
            string _picPath = string.Empty;
            switch (type)
            {
                case PictureType.Original:
                    _picPath = Path.Combine(Function.GetUploadPath(TrueUrl), FILE_NAME);
                    break;
                case PictureType.Medium:
                    _picPath = Path.Combine(Function.GetUploadPath(TrueUrl), Medium_PicName);
                    break;
                case PictureType.Small:
                    _picPath = Path.Combine(Function.GetUploadPath(TrueUrl), Small_PicName);
                    break;
            }
            return _picPath;
        }

        /// <summary>
        /// 取得圖片本身的副檔名
        /// </summary>
        /// <param name="image"></param>
        public void GetRealExtension(Image image)
        {
            if (image != null)
            {
                string _extension = string.Empty;//預設值
                //equals by guid
                if (image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    _extension = ImageFormat.Jpeg.ToString();
                }
                else if (image.RawFormat.Equals(ImageFormat.Bmp))
                {
                    _extension = ImageFormat.Bmp.ToString();
                }
                else if (image.RawFormat.Equals(ImageFormat.Gif))
                {
                    _extension = ImageFormat.Gif.ToString();
                }
                else if (image.RawFormat.Equals(ImageFormat.Png))
                {
                    _extension = ImageFormat.Png.ToString();
                }
                else if (image.RawFormat.Equals(ImageFormat.Tiff))
                {
                    _extension = ImageFormat.Tiff.ToString();
                }
                else
                {
                    //預設值
                    _extension = "jpg";
                }
                this.EXTENSION = "." + _extension;
            }
        }

        /// <summary>
        /// 刪除實體檔案
        /// </summary>
        /// <param name="filePath">如果指定路徑則刪除特定檔案</param>
        /// <returns></returns>
        public bool DeleteFile(string filePath = "")
        {
            try
            {
                if (filePath.IsNullOrEmpty())
                {
                    switch (((AttachmentType)this.ATT_TYPE.ToInt()))
                    {
                        case AttachmentType.File:
                            File.Delete(GetFile(true));
                            break;
                        case AttachmentType.Image:
                            foreach (PictureType type in System.Enum.GetValues(typeof(PictureType)))
                            {
                                string _path = GetPic(type, true);
                                if (File.Exists(_path))
                                    File.Delete(_path);
                            }
                            break;
                    }
                }
                else
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                LogSystem.InitLogSystem();
                LogSystem.WriteLine(ex.Message);
                LogSystem.CloseUnderlayingStream();
                return false;
            }
            return true;
        }


        /// <summary>
        /// 根據副檔名轉換名稱
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            string _extension = string.Empty;
            try
            {
                _extension = this.EXTENSION.Replace(".", "").ToLower();
                switch (_extension)
                {
                    case "doc":
                    case "docx":
                        _extension = "word";
                        break;
                    case "xls":
                    case "xlsx":
                        _extension = "xls";
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.InitLogSystem();
                LogSystem.WriteLine(ex.Message);
                LogSystem.CloseUnderlayingStream();
            }
            return _extension;
        }

        #region 根據語系取值
        /// <summary>
        /// 根據語系取值
        /// </summary>
        /// <param name="t">0=文字；1=連結</param>
        /// <returns></returns>
        public string GetValueOnLang(int t = 0)
        {
            string _value = string.Empty;
            switch (Function.CultureName())
            {
                default:
                case CultureHelper.ZH_TW:
                    switch (t)
                    {
                        default:
                        case 0:
                            _value = this.DESCRIPTION;
                            break;
                        case 1:
                            _value = this.CONTENT1;
                            break;
                    }
                    break;
                case CultureHelper.EN_US:
                    switch (t)
                    {
                        default:
                        case 0:
                            _value = this.CONTENT6;
                            break;
                        case 1:
                            _value = this.CONTENT2;
                            break;
                    }
                    break;
            }
            
            return _value;
        }


        #endregion
    }
}
