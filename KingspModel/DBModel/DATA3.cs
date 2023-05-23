using KingspModel.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace KingspModel.DB
{
	[MetadataType(typeof(DATA3Metadata))]
	public partial class DATA3
	{
		private class DATA3Metadata : BaseMetadata
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
			public string NODE_ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
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
			[DataType(DATA_TYPE_DATE)]
			public DateTime? UPDATE_DATE { get; set; }
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
			[DataType(DATA_TYPE_TITLE)]
			public string DATA_TYPE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string STATUS { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public byte ENABLE { get; set; }
			/// <summary>
			/// 
			/// </summary>
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
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT11 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT12 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT13 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT14 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT15 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT16 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT17 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT18 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT19 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT20 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT21 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT22 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT23 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT24 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT25 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT26 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT27 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT28 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT29 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT30 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL1 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL2 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL3 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL4 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL5 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL6 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL7 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL8 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL9 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL10 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME1 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME2 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME3 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME4 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME5 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME6 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME7 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME8 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME9 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? DATETIME10 { get; set; }
		}

		/// <summary>
		/// 資料庫無此欄位 額外新增
		/// </summary>
		[AllowHtml]
		public string CONTENT { get; set; }

        #region Function

        /// <summary>
        /// 取得 PARAGRAPH by ORDER
        /// <para>null的話會new一個回傳</para>
        /// </summary>
        /// <param name="order"></param>
        /// <returns>null的話會new一個回傳</returns>
        public PARAGRAPH GetParagraphByOrder(int order = 1)
        {
            return this.PARAGRAPH.FirstOrDefault(p => p.ORDER == order) ?? new DB.PARAGRAPH();
        }

        public PLUS GetPlusByOrder(int order = 1)
		{
			return PLUS.FirstOrDefault(p => p.ORDER == order);
		}

		public PLUS GetPlusType(string type)
		{
			return PLUS.FirstOrDefault(p => p.PLUS_TYPE == type);
		}

        /// <summary>
		/// 取得ATTACHMENT
		/// </summary>
		/// <param name="a">0:圖片 1:檔案</param>
		/// <returns></returns>
		public List<ATTACHMENT> GetAttachments(string a = "0")
        {
            return this.ATTACHMENT.Where(p => a.Equals(p.ATT_TYPE))
                .OrderBy(p => p.ORDER).ThenBy(p => p.CREATE_DATE).ToList();
        }

        /// <summary>
		/// 取得ATTACHMENT 第1個
		/// </summary>
		/// <param name="a">0:圖片 1:檔案</param>
		/// <returns></returns>
		public ATTACHMENT GetFirstAttachment(string a = "0")
        {
            return this.ATTACHMENT.Where(p => a.Equals(p.ATT_TYPE))
                .OrderBy(p => p.ORDER).ThenBy(p => p.CREATE_DATE).FirstOrDefault() ?? new ATTACHMENT();
        }

        /// <summary>
        /// 取得上下線日期
        /// </summary>
        /// <returns></returns>
        public string GetDateRange()
        {
            return string.Format(Function.DEFAULT_FORMAT_DATE_RANGE, this.DATETIME1.ToDefaultString(), this.DATETIME2.ToDefaultString());
        }

        /// <summary>
        /// 取得 STATUS 是否1
        /// </summary>
        /// <returns></returns>
        public bool GetSTATUS()
        {
            return "1".Equals(this.STATUS);
        }

        /// <summary>
		/// 取得PLUS
		/// <para>預設取得 兌換紀錄</para>
		/// </summary>
		/// <param name="pt">fun5003 = 兌換紀錄</param>
		/// <returns></returns>
		public List<PLUS> GetPlusList(string pt = "fun5003")
        {
            return this.PLUS.Where(p => p.ENABLE == 1 && pt.Equals(p.PLUS_TYPE)).OrderByDescending(p => p.CREATE_DATE).ToList();
        }

        /// <summary>
		/// 有無 PLUS
		/// <para>預設取得 兌換紀錄</para>
		/// </summary>
		/// <param name="pt">fun5003 = 訂單管理</param>
		/// <returns></returns>
		public bool HasPlusList(string pt = "fun5003")
        {
            return this.PLUS.Any(p => p.ENABLE == 1 && pt.Equals(p.PLUS_TYPE));
        }

        /// <summary>
        /// 取得QRCode圖片
        /// </summary>
        /// <returns></returns>
        public string GetQRCode(bool TrueUrl = false)
        {
            return $"{Path.Combine(Function.GetUploadPath(TrueUrl), "QRCode", this.ID)}.png";
        }

        #endregion

        #region 根據語系取值
        /// <summary>
        /// 根據語系取值
        /// </summary>
        /// <param name="t">1=標題；2=地址；3=網址；4=影片連結；5=營業時間；6=內容</param>
        /// <returns></returns>
        public string GetValueOnLang(int t = 1)
        {
            string _value = string.Empty;
            switch (Function.CultureName())
            {
                default:
                case CultureHelper.ZH_TW:
                    switch (t)
                    {
                        default:
                        case 1:
                            _value = this.CONTENT1;
                            break;
                        case 2:
                            _value = $"{Function.GetNodeTitle(this.CONTENT3)}{Function.GetNodeTitle(this.CONTENT2)}{this.CONTENT5}";
                            break;
                        case 3:
                            _value = this.CONTENT8;
                            break;
                        case 4:
                            _value = this.CONTENT11;
                            break;
                        case 5:
                            _value = this.CONTENT7;
                            break;
                        case 6:
                            _value = this.PARAGRAPH.FirstOrDefault(p => p.ORDER == 0).CONTENT;
                            break;
                    }
                    break;
                case CultureHelper.EN_US:
                    switch (t)
                    {
                        default:
                        case 1:
                            _value = this.CONTENT2;
                            break;
                        case 2:
                            //_value = $"{Function.GetNodeValueByLang(this.CONTENT3)}{Function.GetNodeValueByLang(this.CONTENT2)}{this.CONTENT17}";
                            _value = this.CONTENT17;
                            break;
                        case 3:
                            _value = this.CONTENT18;
                            break;
                        case 4:
                            _value = this.CONTENT19;
                            break;
                        case 5:
                            _value = this.CONTENT20;
                            break;
                        case 6:
                            _value = this.PARAGRAPH.FirstOrDefault(p => p.ORDER == 1).CONTENT;
                            break;
                    }
                    break;
            }

            return _value;
        }


        #endregion
    }
}
