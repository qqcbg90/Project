
namespace KingspModel.DataModel
{
    /// <summary>
    /// JQuery.zTree v3.5 用的 Model
    /// </summary>
    public sealed class zNode
    {
        /// <summary>
        /// 預設指定 target="_self"
        /// </summary>
        public zNode()
        {
            target = "_self";
        }
        //以下為對照zTree v3.5
        public string id { get; set; }
        public string pId { get; set; }
        public string name { get; set; }
        public bool open { get; set; }
        public bool isParent { get; set; }
        public bool nocheck { get; set; }
        public string url { get; set; }
        public string target { get; set; }
        /// <summary>
        /// 可設定標準Javascript語法 ex: alert("test");
        /// </summary>
        public string click { get; set; }
        public string icon { get; set; }
        public string iconOpen { get; set; }
        public string iconClose { get; set; }

		/// <summary>
		/// 標題後附加圖片
		/// </summary>
		public bool IsNew { get; set; }
		/// <summary>
		/// 筆數
		/// </summary>
		public int NewCount { get; set; }

	}
}
