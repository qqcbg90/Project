using KingspModel.DataModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace KingspModel.DB
{
	[MetadataType(typeof(PLUSMetadata))]
	public partial class PLUS
	{
		#region Metadata

		private class PLUSMetadata : BaseMetadata
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
			public string MAIN_ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string PLUS_TYPE { get; set; }
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
		#endregion

		#region function

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
        /// 取得上下線日期 有時分
        /// </summary>
        /// <returns></returns>
        public string GetDateRange()
        {
            return string.Format(Function.DEFAULT_FORMAT_DATE_RANGE, this.DATETIME1.ToDefaultStringWithTime(), this.DATETIME2.ToDefaultStringWithTime());
        }

        /// <summary>
        /// 取得 DateTime1 和 DateTime2 的日期區間字串
        /// </summary>
        /// <param name="type">0:日期 1:時間 2:日期+時間</param>
        /// <param name="delimiter">分隔符號,預設為「/」</param>
        /// <param name="minguo">true:顯示民國年</param>
        /// <returns>string</returns>
        public string getDateTime1AndDateTime2(int type = 0, string delimiter = "/", bool minguo = false, bool week = false)
		{
			string text = string.Empty;
			if (this.DATETIME1.HasValue && this.DATETIME2.HasValue)
			{
				DateTime dt1 = this.DATETIME1.Value;
				DateTime dt2 = this.DATETIME2.Value;
				if (type == 0 || type == 2)
				{
					string DT1 = dt1.ToDateString(delimiter, minguo, week);
					string DT2 = dt2.ToDateString(delimiter, minguo, week);
					text = DT1 + (!DT2.IsNullOrEmpty() && DT1.Equals(DT2) ? "" : "~" + DT2);
				}
				if (type == 1 || type == 2)
				{
					string st = dt1.ToString("HH:mm");
					string et = dt2.ToString("HH:mm");
					text += " " + st + (!et.IsNullOrEmpty() && st.Equals(et) ? "" : "-" + et);
				}
			}
			return text;
		}

        /// <summary>
        /// 產生GoogleCalendar網址-增加日曆
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="details">details</param>
        /// <param name="location">location</param>
        /// <returns></returns>
        public string GetGoogleCalendarShareString(string text, string details, string location)
        {
            //https://calendar.google.com/calendar/r/eventedit
            return $"https://calendar.google.com/calendar/r/eventedit?text={text}&dates={this.DATETIME1.ToDefaultStringGoogleCalendar()}/{this.DATETIME2.ToDefaultStringGoogleCalendar()}&details={details}&location={location}";
        }

        /// <summary>
        /// 取得 STATUS
        /// </summary>
        /// <returns></returns>
        public string GetSTATUS()
        {
            return ((KingspModel.Enum.AuditStatus)this.STATUS.ToInt()).GetDescription();
        }

        /// <summary>
        /// 取得 STATUS for fun4000 OrderStatus
        /// </summary>
        /// <returns></returns>
        public string GetFun4000_STATUS()
        {
            return ((KingspModel.Enum.OrderStatus)this.STATUS.ToInt()).GetDescription();
        }

        /// <summary>
        /// 取得 DECIMAL2 是否1
        /// </summary>
        /// <returns></returns>
        public bool GetDecimal2()
        {
            return this.DECIMAL2 == 1;
        }

        #endregion
    }
}
