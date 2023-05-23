using KingspModel.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace KingspModel.DB
{
	[MetadataType(typeof(ARTICLEMetadata))]
    public partial class ARTICLE
    {
        private class ARTICLEMetadata : BaseMetadata
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
			public string ARTICLE_TYPE { get; set; }
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
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public PARAGRAPH GetParagraphByOrder(int order = 1)
        {
            return this.PARAGRAPH.FirstOrDefault(p => p.ORDER == order);
        }

        /// <summary>
        /// 取得 PARAGRAPH by CONTENT2
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public PARAGRAPH GetParagraphByC2(string c2="0")
        {
            return this.PARAGRAPH.FirstOrDefault(p => c2.Equals(p.CONTENT2));
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
        /// 取得上下線日期
        /// </summary>
        /// <returns></returns>
        public string GetDateRange()
        {
            return string.Format(Function.DEFAULT_FORMAT_DATE_RANGE, this.DATETIME1.ToDefaultString(), this.DATETIME2.ToDefaultString());
        }

        /// <summary>
        /// 取得 DECIMAL1 是否1
        /// </summary>
        /// <returns></returns>
        public bool GetDecimal1()
        {
            return this.DECIMAL1 == 1;
        }

        #endregion

        #region 根據語系取值
        /// <summary>
        /// 根據語系取值
        /// </summary>
        /// <param name="t">1=標題；2=網址；3=影片連結；4=內容</param>
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
                            _value = this.CONTENT2;
                            break;
                        case 3:
                            _value = this.CONTENT3;
                            break;
                        case 4:
                            _value = this.PARAGRAPH.FirstOrDefault(p => p.ORDER == 0).CONTENT;
                            break;
                    }
                    break;
                case CultureHelper.EN_US:
                    switch (t)
                    {
                        default:
                        case 1:
                            _value = this.CONTENT11;
                            break;
                        case 2:
                            _value = this.CONTENT12;
                            break;
                        case 3:
                            _value = this.CONTENT13;
                            break;
                        case 4:
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
