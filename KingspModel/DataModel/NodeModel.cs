using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace KingspModel.DataModel
{
	/// <summary>
	/// NODE model
	/// </summary>
	public sealed class NodeModel : BaseMetadata
	{
		/// <summary>
		/// ID
		/// </summary>
		[DisplayName("ID")]
		[DataType(DATA_TYPE_TITLE)]
		[Remote("NodeExist", "Json", ErrorMessage = DEFAULT_REPEAT_KEY)]
		public string ID { get; set; }
		/// <summary>
		/// TITLE
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("TITLE")]
		[DataType(DATA_TYPE_TITLE)]
		public string TITLE { get; set; }
		/// <summary>
		/// URL
		/// </summary>
		[DisplayName("URL")]
		[DataType(DATA_TYPE_TITLE)]
		public string URL { get; set; }
		/// <summary>
		/// PARENT_ID
		/// </summary>
		[DisplayName("PARENT_ID")]
		[DataType(DATA_TYPE_TITLE)]
		public string PARENT_ID { get; set; }
		/// <summary>
		/// ORDER
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("ORDER")]
		[DataType(DATA_TYPE_TITLE)]
		public int ORDER { get; set; }
		/// <summary>
		/// ENABLE
		/// </summary>
		[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
		[DisplayName("ENABLE")]
		[DataType(DATA_TYPE_TITLE)]
		public byte ENABLE { get; set; }
		/// <summary>
		/// CONTENT1
		/// </summary>
		[DisplayName("CONTENT1")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT1 { get; set; }
		/// <summary>
		/// CONTENT2
		/// </summary>
		[DisplayName("CONTENT2")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT2 { get; set; }
		/// <summary>
		/// CONTENT3
		/// </summary>
		[DisplayName("CONTENT3")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT3 { get; set; }
		/// <summary>
		/// CONTENT4
		/// </summary>
		[DisplayName("CONTENT4")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT4 { get; set; }
		/// <summary>
		/// CONTENT5
		/// </summary>
		[DisplayName("CONTENT5")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT5 { get; set; }
		/// <summary>
		/// CONTENT6
		/// </summary>
		[DisplayName("CONTENT6")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT6 { get; set; }
		/// <summary>
		/// CONTENT7
		/// </summary>
		[DisplayName("CONTENT7")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT7 { get; set; }
		/// <summary>
		/// CONTENT8
		/// </summary>
		[DisplayName("CONTENT8")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT8 { get; set; }
		/// <summary>
		/// CONTENT9
		/// </summary>
		[DisplayName("CONTENT9")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT9 { get; set; }
		/// <summary>
		/// CONTENT10
		/// </summary>
		[DisplayName("CONTENT10")]
		[DataType(DATA_TYPE_TITLE)]
		public string CONTENT10 { get; set; }

		public DateTime CREATE_DATE { get; set; }
		public string CREATER { get; set; }
		public DateTime? UPDATE_DATE { get; set; }
		public string UPDATER { get; set; }
	}
}

