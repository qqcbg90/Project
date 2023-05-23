
using KingspModel.Attributes;
using System.ComponentModel;

namespace KingspModel.Enum
{
	#region 臺灣北中南東離島
	/// <summary>
	/// 北、中、南、東、離島
	/// </summary>
	public enum AreaType
	{
		/// <summary>
		/// 北 0
		/// </summary>
		[Description("北")]
		North,
		/// <summary>
		/// 中 1
		/// </summary>
		[Description("中")]
		Central,
		/// <summary>
		/// 南 2
		/// </summary>
		[Description("南")]
		South,
		/// <summary>
		/// 東 3
		/// </summary>
		[Description("東")]
		East,
		/// <summary>
		/// 離島 4
		/// </summary>
		[Description("離島")]
		Islands
	}
    #endregion

    #region 兌換狀態
    /// <summary>
    /// 0:待確認 1:確認 2:退回
    /// </summary>
    public enum AuditStatus
	{
        /// <summary>
        /// 待確認 0
        /// </summary>
        [Description("待確認")]
		Type0,
        /// <summary>
        /// 確認 1
        /// </summary>
        [Description("確認")]
		Type1,
        /// <summary>
        /// 退回 2
        /// </summary>
        [Description("退回")]
		Type2
	}
	#endregion

	#region 公告類別
	public enum NewsType
	{
		/// <summary>
		/// 最新公告 0
		/// </summary>
		[Description("最新公告")]
		Type0,
		/// <summary>
		/// 展演公告 1
		/// </summary>
		[Description("展演新聞")]
		Type1,
		/// <summary>
		/// 標案公告 2
		/// </summary>
		[Description("標案公告")]
		Type2,
		/// <summary>
		/// 徵才資訊 3
		/// </summary>
		[Description("徵才資訊")]
		Type3,
		/// <summary>
		/// 政府公告 4
		/// </summary>
		[Description("政府公告")]
		Type4

	}
	#endregion

	#region 藝文講堂-分類
	public enum LectureType
	{
		/// <summary>
		/// 電影講座 0
		/// </summary>
		[Description("電影講座")]
		Type0,
		/// <summary>
		/// 影視推廣 1
		/// </summary>
		[Description("影視推廣")]
		Type1,
		/// <summary>
		/// 親子活動 2
		/// </summary>
		[Description("親子活動")]
		Type2,
		/// <summary>
		/// 影片播放 3
		/// </summary>
		//[Description("影片播放")]
		//Type3
	}
	#endregion

	#region 影片管理
	/// <summary>
	/// 發音
	/// </summary>
	public enum FilmPronunciation
	{
		/// <summary>
		/// 中文發音 0
		/// </summary>
		[Description("中文發音")]
		TW,
		/// <summary>
		/// 英文發音 1
		/// </summary>
		[Description("英文發音")]
		EN,
	}

	/// <summary>
	/// 字幕
	/// </summary>
	public enum FilmSubtitle
	{
		/// <summary>
		/// 中文字幕 0
		/// </summary>
		[Description("中文字幕")]
		TW,
		/// <summary>
		/// 英文字幕 1
		/// </summary>
		[Description("英文字幕")]
		EN,
	}

	/// <summary>
	/// 放映規格
	/// </summary>
	public enum FilmScreeningSpecifications
	{
		/// <summary>
		/// BD 0
		/// </summary>
		[Description("BD")]
		BD,
		/// <summary>
		/// DVD 1
		/// </summary>
		[Description("DVD")]
		DVD,
		/// <summary>
		/// HD DVD 2
		/// </summary>
		[Description("HD DVD")]
		HD_DVD,
	}

	/// <summary>
	/// 色彩
	/// </summary>
	public enum FilmColor
	{
		/// <summary>
		/// 黑白
		/// </summary>
		[Description("黑白")]
		Type0,
		/// <summary>
		/// 彩色
		/// </summary>
		[Description("彩色")]
		Type1
	}
	#endregion

	#region 主辦類型
	public enum HostingType
	{
		/// <summary>
		/// 自辦 1
		/// </summary>
		[Priority(1)]
		[Description("自辦")]
		Type1 = 1,
		/// <summary>
		/// 補助 5
		/// </summary>
		[Priority(2)]
		[Description("補助")]
		Type5 = 5,
		/// <summary>
		/// 協辦(含本府他單位協辦) 5
		/// </summary>
		[Priority(3)]
		[Description("協辦(含本府他單位協辦)")]
		Type2 = 2,
		/// <summary>
		/// 租借 3
		/// </summary>
		[Priority(4)]
		[Description("租借")]
		Type3 = 3,
		/// <summary>
		/// 其他 4
		/// </summary>
		[Priority(5)]
		[Description("其他")]
		Type4 = 4,

	}
	#endregion

	#region 入場方式
	public enum AdmissionType
	{
		/// <summary>
		/// 售票 1
		/// </summary>
		[Priority(1)]
		[Description("售票")]
		sellTicket = 1,
		/// <summary>
		/// 報名或索票 2
		/// </summary>
		[Priority(2)]
		[Description("報名或索票")]
		ropeTicket = 2,
		/// <summary>
		/// 自由入場 0
		/// </summary>
		[Priority(3)]
		[Description("自由入場")]
		freeTicket = 0,
		/// <summary>
		/// 其他 3
		/// </summary>
		[Priority(4)]
		[Description("其他")]
		otherTicket = 3,
	}
	#endregion

	#region 檔期時間類別
	public enum ExhibitionTimeType
	{
		/// <summary>
		/// 拆／裝台／佈展 0
		/// </summary>
		[Description("拆／裝台／佈展")]
		dress,
		/// <summary>
		/// 正式展演 1
		/// </summary>
		[Description("正式展演")]
		performance,
		/// <summary>
		/// 卸展 2
		/// </summary>
		[Description("卸展")]
		unload
	}
	#endregion

	#region 20211018 身分別
    /// <summary>
    /// 0管理者、1兌換點
    /// </summary>
	public enum IdentityType
	{
		/// <summary>
		/// 0:管理者
		/// </summary>
		[Description("管理者")]
		Type0,
		/// <summary>
		/// 1:兌換點(商家)
		/// </summary>
		[Description("兌換點")]
		Type1
	}
    #endregion

    #region 是否解說
    /// <summary>
    /// 0:否 1:是
    /// </summary>
    public enum Decimal2Status
    {
        /// <summary>
        /// 否 0
        /// </summary>
        [Description("否")]
        Type0,
        /// <summary>
        /// 是 1
        /// </summary>
        [Description("是")]
        Type1
    }
    #endregion

    #region 訂單狀態
    /// <summary>
    /// 0:新訂單 1:已使用 2:待請款 3:已請款 4:待核銷 5:已核銷 6:處理中 7:已處理 99:已取消
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 新訂單 0
        /// </summary>
        [Description("新訂單")]
        Type0,
        /// <summary>
        /// 已使用 1
        /// </summary>
        [Description("已使用")]
        Type1,
        /// <summary>
        /// 待請款 2
        /// </summary>
        [Description("待請款")]
        Type2,
        /// <summary>
        /// 已請款 3
        /// </summary>
        [Description("已請款")]
        Type3,
        /// <summary>
        /// 待核銷 4
        /// </summary>
        [Description("待核銷")]
        Type4,
        /// <summary>
        /// 已核銷 5
        /// </summary>
        [Description("已核銷")]
        Type5,
        /// <summary>
        /// 處理中 6
        /// </summary>
        [Description("處理中")]
        Type6,
        /// <summary>
        /// 已處理 7
        /// </summary>
        [Description("已處理")]
        Type7,
        /// <summary>
        /// 已取消 99
        /// </summary>
        [Description("已取消")]
        Type99 = 99
    }
    #endregion
    
}
