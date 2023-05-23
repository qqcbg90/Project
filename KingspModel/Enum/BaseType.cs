
using System.ComponentModel;
using System.Linq;

namespace KingspModel.Enum
{
	#region Sample 示範用

	/// <summary>
	/// SampleType
	/// </summary>
	public enum SampleType
	{
		/// <summary>
		/// Type0 0
		/// </summary>
		[Description("Type0")]
		Type0,
		/// <summary>
		/// Type1 1
		/// </summary>
		[Description("Type1")]
		Type1,
		/// <summary>
		/// Type2 2
		/// </summary>
		[Description("Type2")]
		Type2
	}

	/// <summary>
	/// SampleType 延伸類別
	/// </summary>
	public static class SampleTypeExtensions
	{
		/// <summary>
		/// 轉換Description
		/// </summary>
		/// <returns></returns>
		public static string ToDescription(this SampleType type, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			return System.Enum.GetValues(typeof(SampleType)).Cast<SampleType>()
			.Where(p => p.ToIntValue().Equals(key)).FirstOrDefault().GetDescription();
		}
		/// <summary>
		/// 在語系檔中的Value
		/// </summary>
		/// <param name="roomType"></param>
		/// <returns></returns>
		public static string ToResourceValue(this SampleType type)
		{
			return "";
			//switch (type)
			//{
			//    default:
			//    case SampleType.Type0:
			//        return Resource.Ecology;
			//    case SampleType.Type1:
			//        return Resource.Science;
			//    case SampleType.Type2:
			//        return Resource.CultureR;
			//}
		}
	}

	#endregion

	#region 一般用

	/// <summary>
	/// 登入狀態
	/// </summary>
	public enum LogOnStatus
	{
		/// <summary>
		/// 鎖住
		/// </summary>
		NotActivated,
		/// <summary>
		/// 登入成功
		/// </summary>
		Successful,
		/// <summary>
		/// 登入失敗
		/// </summary>
		Failure
	}

	/// <summary>
	/// 登入驗證模式
	/// </summary>
	public enum LogOnAuthentication
	{
		/// <summary>
		/// 無敵帳號
		/// </summary>
		KINGSP,
		/// <summary>
		/// DataBase
		/// </summary>
		DB,
		/// <summary>
		/// Active Directory
		/// </summary>
		AD
	}

	/// <summary>
	/// Enable 類型
	/// </summary>
	public enum EnableType : byte
	{
		/// <summary>
		/// 取消 0
		/// </summary>
		[Description("取消")]
		Disable,
		/// <summary>
		/// 顯示 1
		/// </summary>
		[Description("顯示")]
		Enable
	}

	/// <summary>
	/// 操作權限
	/// </summary>
	public enum Authority_Right
	{
		/// <summary>
		/// 查詢 0
		/// </summary>
		[Description("查詢")]
		Search,
		/// <summary>
		/// 新增 1
		/// </summary>
		[Description("新增")]
		Add,
		/// <summary>
		/// 修改 2
		/// </summary>
		[Description("修改")]
		Update,
		/// <summary>
		/// 刪除 3
		/// </summary>
		[Description("刪除")]
		Delete
	}

	/// <summary>
	/// ROLE_GROUP TYPE
	/// </summary>
	public enum RoleGroupType
	{
		/// <summary>
		/// 權限管理>群組管理 0
		/// </summary>
		SYSUSER,
		/// <summary>
		/// 電子報管理>訂閱者 1
		/// </summary>
		NEWSLETTER,
		/// <summary>
		/// 電子報管理>電子報發送 2
		/// </summary>
		NEWSLETTER_SEND
	}

    /// <summary>
    /// 語言 先只留英文
    /// </summary>
    public enum LanguageType
    {
        /// <summary>
        /// 英文 1
        /// </summary>
        [Description("英文")]
        English = 1
        ///// <summary>
        ///// 其它 0
        ///// </summary>
        //[Description("其它")]
        //Else,
        ///// <summary>
        ///// 中文 1
        ///// </summary>
        //[Description("中文")]
        //Chinese,
        ///// <summary>
        ///// 英文 2
        ///// </summary>
        //[Description("英文")]
        //English,
        ///// <summary>
        ///// 日文 3
        ///// </summary>
        //[Description("日文")]
        //Japanese,
        ///// <summary>
        ///// 韓文 4
        ///// </summary>
        //[Description("韓文")]
        //Korean
    }

    /// <summary>
    /// 性別
    /// </summary>
    public enum SexType
	{
		/// <summary>
		/// 男 0
		/// </summary>
		[Description("男")]
		Male,
		/// <summary>
		/// 女 1
		/// </summary>
		[Description("女")]
		Female
		///// <summary>
		///// 其他 3
		///// </summary>
		//[Description("其他")]
		//Other = 3

	}

	/// <summary>
	/// SexType 延伸類別
	/// </summary>
	public static class SexTypeExtensions
	{
		/// <summary>
		/// 轉換Description
		/// </summary>
		/// <returns></returns>
		public static string ToDescription(this SexType type, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			return System.Enum.GetValues(typeof(SexType)).Cast<SexType>()
			.Where(p => p.ToIntValue().Equals(key)).FirstOrDefault().GetDescription();
		}

		/// <summary>
		/// 在語系檔中的Value
		/// </summary>
		/// <param name="roomType"></param>
		/// <returns></returns>
		public static string ToResourceValue(this SexType type)
		{
			return "";
			//switch (type)
			//{
			//    default:
			//    case SampleType.Type0:
			//        return Resource.Ecology;
			//    case SampleType.Type1:
			//        return Resource.Science;
			//    case SampleType.Type2:
			//        return Resource.CultureR;
			//}
		}
	}

	/// <summary>
	/// 附件類型
	/// </summary>
	public enum AttachmentType
	{
		/// <summary>
		/// 圖片 0
		/// </summary>
		[Description("圖片")]
		Image,
		/// <summary>
		/// 檔案 1
		/// </summary>
		[Description("檔案")]
		File,
		/// <summary>
		/// 影片 2
		/// </summary>
		[Description("影片")]
		Video
	}

	/// <summary>
	/// 圖片尺寸
	/// </summary>
	public enum PictureType
	{
		/// <summary>
		/// 原始Size 0
		/// </summary>
		Original,
		/// <summary>
		/// 中型Size 1
		/// </summary>
		Medium,
		/// <summary>
		/// 小型Size 2
		/// </summary>
		Small
	}

	/// <summary>
	/// 訊息類型 email 0、簡訊 1、純文字 2
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// email 0
		/// </summary>
		Type0,
		/// <summary>
		/// 簡訊 1
		/// </summary>
		Type1,
		/// <summary>
		/// 純文字 2
		/// </summary>
		Type2
	}

	/// <summary>
	/// 訊息類別
	/// </summary>
	public enum AlertMsgType
	{
		/// <summary>
		/// Notice
		/// </summary>
		Notice,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Success
		/// </summary>
		Success
	}

	/// <summary>
	/// Position
	/// </summary>
	public enum Position
	{
		/// <summary>
		/// Top_left
		/// </summary>
		Top_left,
		/// <summary>
		/// Top_center
		/// </summary>
		Top_center,
		/// <summary>
		/// Top_right
		/// </summary>
		Top_right,
		/// <summary>
		/// Middle_left
		/// </summary>
		Middle_left,
		/// <summary>
		/// Middle_center
		/// </summary>
		Middle_center,
		/// <summary>
		/// Middle_right
		/// </summary>
		Middle_right
	}

	/// <summary>
	/// YesNo
	/// </summary>
	public enum YesNo
	{
		/// <summary>
		/// Yes
		/// </summary>
		[Description("是")]
		Yes = 1,
		/// <summary>
		/// No
		/// </summary>
		[Description("否")]
		No = 0
	}

    /// <summary>
    /// SelectListValueType
    /// </summary>
    public enum SelectListValueType
    {
        Int,
        String
    }

    #endregion



    #region 學歷 Education

    /// <summary>
    /// EducationType
    /// </summary>
    public enum EducationType
	{
		/// <summary>
		/// 學齡前 0
		/// </summary>
		[Description("學齡前")]
		Type0,
		/// <summary>
		/// 國小 1
		/// </summary>
		[Description("國小")]
		Type1,
		/// <summary>
		/// 國中 2
		/// </summary>
		[Description("國中")]
		Type2,
		/// <summary>
		/// 高中職 3
		/// </summary>
		[Description("高中職")]
		Type3,
		/// <summary>
		/// 大專院校 4
		/// </summary>
		[Description("大專院校")]
		Type4,
		/// <summary>
		/// 碩士 5
		/// </summary>
		[Description("碩士")]
		Type5,
		/// <summary>
		/// 博士 6
		/// </summary>
		[Description("博士")]
		Type6
	}

	/// <summary>
	/// EducationType 延伸類別
	/// </summary>
	public static class EducationTypeExtensions
	{
		/// <summary>
		/// 轉換Description
		/// </summary>
		/// <returns></returns>
		public static string ToDescription(this EducationType type, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			return System.Enum.GetValues(typeof(EducationType)).Cast<EducationType>()
			.Where(p => p.ToIntValue().Equals(key)).FirstOrDefault().GetDescription();
		}

		/// <summary>
		/// 在語系檔中的Value
		/// </summary>
		/// <param name="roomType"></param>
		/// <returns></returns>
		public static string ToResourceValue(this EducationType type)
		{
			return "";
			//switch (type)
			//{
			//    default:
			//    case SampleType.Type0:
			//        return Resource.Ecology;
			//    case SampleType.Type1:
			//        return Resource.Science;
			//    case SampleType.Type2:
			//        return Resource.CultureR;
			//}
		}
	}

	#endregion

	#region 職業 Job

	/// <summary>
	/// JobType
	/// </summary>
	public enum JobType
	{
		/// <summary>
		/// 農林漁牧礦 0
		/// </summary>
		[Description("農林漁牧礦")]
		Type0,
		/// <summary>
		/// 公家單位 1
		/// </summary>
		[Description("公家單位")]
		Type1,
		/// <summary>
		/// 工/製造業 2
		/// </summary>
		[Description("工/製造業")]
		Type2,
		/// <summary>
		/// 專業/技術人員 3
		/// </summary>
		[Description("專業/技術人員")]
		Type3,
		/// <summary>
		/// 商 4
		/// </summary>
		[Description("商")]
		Type4,
		/// <summary>
		/// 服務業 5
		/// </summary>
		[Description("服務業")]
		Type5,
		/// <summary>
		/// 學生 6
		/// </summary>
		[Description("學生")]
		Type6,
		/// <summary>
		/// 其他 7
		/// </summary>
		[Description("其他")]
		Type7
	}

	/// <summary>
	/// JobType 延伸類別
	/// </summary>
	public static class JobTypeExtensions
	{
		/// <summary>
		/// 轉換Description
		/// </summary>
		/// <returns></returns>
		public static string ToDescription(this JobType type, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			return System.Enum.GetValues(typeof(JobType)).Cast<JobType>()
			.Where(p => p.ToIntValue().Equals(key)).FirstOrDefault().GetDescription();
		}

		/// <summary>
		/// 在語系檔中的Value
		/// </summary>
		/// <returns></returns>
		public static string ToResourceValue(this JobType type)
		{
			return "";
			//switch (type)
			//{
			//    default:
			//    case SampleType.Type0:
			//        return Resource.Ecology;
			//    case SampleType.Type1:
			//        return Resource.Science;
			//    case SampleType.Type2:
			//        return Resource.CultureR;
			//}
		}
	}

	#endregion

	#region 婚姻狀況 Marriage

	/// <summary>
	/// MarriageType
	/// </summary>
	public enum MarriageType
	{
		/// <summary>
		/// 已婚 0
		/// </summary>
		[Description("已婚")]
		Type0,
		/// <summary>
		/// 未婚 1
		/// </summary>
		[Description("未婚")]
		Type1,
		/// <summary>
		/// 離婚 2
		/// </summary>
		[Description("離婚")]
		Type2,
		/// <summary>
		/// 其他 3
		/// </summary>
		[Description("其他")]
		Type3
	}

	/// <summary>
	/// MarriageType 延伸類別
	/// </summary>
	public static class MarriageTypeExtensions
	{
		/// <summary>
		/// 轉換Description
		/// </summary>
		/// <returns></returns>
		public static string ToDescription(this MarriageType type, string key)
		{
			if (key.IsNullOrEmpty()) return string.Empty;
			return System.Enum.GetValues(typeof(MarriageType)).Cast<MarriageType>()
			.Where(p => p.ToIntValue().Equals(key)).FirstOrDefault().GetDescription();
		}

		/// <summary>
		/// 在語系檔中的Value
		/// </summary>
		/// <returns></returns>
		public static string ToResourceValue(this MarriageType type)
		{
			return "";
			//switch (type)
			//{
			//    default:
			//    case SampleType.Type0:
			//        return Resource.Ecology;
			//    case SampleType.Type1:
			//        return Resource.Science;
			//    case SampleType.Type2:
			//        return Resource.CultureR;
			//}
		}
	}

	#endregion
}
