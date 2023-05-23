using KingspModel.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace KingspModel.DB
{
	[MetadataType(typeof(DATA8Metadata))]
    public partial class DATA8
    {
        #region Metadata

        private class DATA8Metadata : BaseMetadata
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
			public string CONTENT31 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT32 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT33 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT34 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT35 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT36 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT37 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT38 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT39 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT40 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT41 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT42 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT43 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT44 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT45 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT46 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT47 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT48 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT49 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT50 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT51 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT52 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT53 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT54 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT55 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT56 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT57 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT58 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT59 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT60 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT61 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT62 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT63 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT64 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT65 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT66 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT67 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT68 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT69 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT70 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT71 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT72 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT73 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT74 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT75 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT76 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT77 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT78 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT79 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT80 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT81 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT82 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT83 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT84 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT85 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT86 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT87 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT88 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT89 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT90 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT91 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT92 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT93 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT94 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT95 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT96 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT97 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT98 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT99 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT100 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT101 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT102 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT103 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT104 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT105 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT106 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT107 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT108 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT109 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public string CONTENT110 { get; set; }
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
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL11 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL12 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL13 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL14 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL15 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL16 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL17 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL18 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL19 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL20 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL21 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL22 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL23 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL24 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL25 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL26 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL27 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL28 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL29 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL30 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL31 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL32 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL33 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL34 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL35 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL36 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL37 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL38 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL39 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL40 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL41 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL42 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL43 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL44 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL45 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL46 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL47 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL48 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL49 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL50 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL51 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL52 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL53 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL54 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL55 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL56 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL57 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL58 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL59 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL60 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL61 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL62 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL63 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL64 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL65 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL66 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL67 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL68 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL69 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL70 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL71 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL72 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL73 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL74 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL75 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL76 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL77 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL78 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL79 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL80 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL81 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL82 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL83 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL84 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL85 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL86 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL87 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL88 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL89 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL90 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL91 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL92 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL93 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL94 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL95 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL96 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL97 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL98 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL99 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL100 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL101 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL102 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL103 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL104 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL105 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL106 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL107 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL108 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL109 { get; set; }
			/// <summary>
			/// 
			/// </summary>
			//[DisplayName("")]
			[DataType(DATA_TYPE_TITLE)]
			public decimal? DECIMAL110 { get; set; }
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

		#region Function 取子Table

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
        /// 根據plus plus_type 取得1筆資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PLUS GetPlusByType(string type = "0")
        {
            return this.PLUS.FirstOrDefault(p => type.Equals(p.PLUS_TYPE));
        }

        /// <summary>
        /// 根據plus plus_type 取得列表資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PLUS> GetPlusListByType(string type)
        {
            return this.PLUS.Where(p => type.Equals(p.PLUS_TYPE) && p.ENABLE.IsEnable()).OrderByDescending(p => p.DATETIME1).ToList();
        }
		
        #endregion
    }
}
