using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KingspModel.DataModel;
using System;

namespace KingspModel.DB
{
    [MetadataType(typeof(ROLE_GROUPMetadata))]
    public partial class ROLE_GROUP
    {
        private class ROLE_GROUPMetadata : BaseMetadata
        {
			//[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("ID")]
			[DataType(DATA_TYPE_TITLE)]
			public string ID { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CREATE_DATE")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime CREATE_DATE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CREATER")]
			[DataType(DATA_TYPE_TITLE)]
			public string CREATER { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("UPDATE_DATE")]
			[DataType(DATA_TYPE_DATE)]
			public DateTime? UPDATE_DATE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("UPDATER")]
			[DataType(DATA_TYPE_TITLE)]
			public string UPDATER { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("ENABLE")]
			[DataType(DATA_TYPE_TITLE)]
			public byte ENABLE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("GROUP_TYPE")]
			[DataType(DATA_TYPE_TITLE)]
			public string GROUP_TYPE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
			[DisplayName("TITLE")]
			[DataType(DATA_TYPE_TITLE)]
			public string TITLE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("MEMO")]
			[DataType(DATA_TYPE_TITLE)]
			public string MEMO { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CONTENT1")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT1 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CONTENT2")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT2 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CONTENT3")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT3 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CONTENT4")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT4 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("CONTENT5")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT5 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("ORDER")]
			[DataType(DATA_TYPE_TITLE)]
			public int? ORDER { get; set; }
		}
    }
}
