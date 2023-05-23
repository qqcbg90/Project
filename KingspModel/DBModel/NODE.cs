using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KingspModel.DataModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace KingspModel.DB
{
    [MetadataType(typeof(NODEMetadata))]
    public partial class NODE
    {
        private class NODEMetadata : BaseMetadata
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
			/// 標題
			/// </summary>
			[Required(ErrorMessage = REQUIRED_ERROR_MESSAGE)]
            [DisplayName("標題")]
			[DataType(DATA_TYPE_TITLE)]
			public string TITLE { get; set; }
			/// <summary>
			/// 
			/// </summary>
			[DisplayName("連結")]
			[DataType(DATA_TYPE_TITLE)]
			public string URL { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string PARENT_ID { get; set; }
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
			public byte ENABLE { get; set; }
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
        /// 根據語系取值
        /// </summary>
        /// <param name="t">1=證照類別 2=title</param>
        /// <returns></returns>
        public string GetValueOnLang(int t = 1)
        {
            string _value = string.Empty;
            switch (t)
            {
                default:
                case 1:
                    string[] arr = this.CONTENT1.Split(Function.DELIMITER);
                    List<string> _arr = new List<string>();
                    foreach (var ar in arr)
                    {
                        _arr.Add(Function.GetNodeTitle(ar));
                    }
                    _value = string.Join(",", _arr);
                    break;
                case 2:
                    if (CultureHelper.ZH_TW.Equals(Function.CultureName()))
                    {
                        _value = TITLE;
                    }
                    else
                    {
                        _value = CONTENT2;
                    }
                    break;
            }

            return _value;
        }


        #endregion
    }
}
