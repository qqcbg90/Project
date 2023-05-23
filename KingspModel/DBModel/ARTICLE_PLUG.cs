using System;
using System.ComponentModel.DataAnnotations;
using KingspModel.DataModel;
using System.Linq;
using System.ComponentModel;

namespace KingspModel.DB
{
    [MetadataType(typeof(ARTICLE_PLUGMetadata))]
    public partial class ARTICLE_PLUG
    {
        private class ARTICLE_PLUGMetadata : BaseMetadata
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
			public string ARTICLE_ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string ARTICLE_PLUG_TYPE { get; set; }
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
		}

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

        #endregion

    }
}
