using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using KingspModel.Enum;
using System.Text;
using System.Globalization;
using KingspModel.Attributes;
using System.Web.Mvc;

namespace KingspModel
{

	#region String

	/// <summary>
	/// String 自訂的擴展方法
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Html Tag regex
		/// </summary>
		public const string HTML_REGEX = @"<[^>]*>";
		/// <summary>
		/// 截字(字尾預設加...)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string CutString(this string str, int maxLength)
		{
			return CutString(str, maxLength, "...");
		}
		/// <summary>
		/// 截字
		/// </summary>
		/// <param name="str"></param>
		/// <param name="maxLength"></param>
		/// <param name="lastStr">加在字尾的字串</param>
		/// <returns></returns>
		public static string CutString(this string str, int maxLength, string lastStr)
		{
			if (str.IsNullOrEmpty())
				return str;
			if (str.Length > maxLength)
				return string.Format("{0}{1}", str.Substring(0, maxLength), lastStr);
			else
				return str;
		}

		/// <summary>
		/// 將換行符號轉成Br
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ReplaceToBr(this string str)
		{
			return str.IsNullOrEmpty() ? string.Empty : str.Replace("\r\n", "<br />").Replace("\n", "<br />"); ;
		}

		/// <summary>
		/// 取得副檔名 有.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string GetExtension(this string str)
		{
			return Path.GetExtension(str);
		}

		/// <summary>
		/// 瀘掉html tag
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static string RegexHtmlTag(this string str)
		{
			return Regex.Replace(str, HTML_REGEX, string.Empty);
		}

		/// <summary>
		/// 比較字串相不相同(不管大小寫)(預設忽略)
		/// </summary>
		/// <param name="source">來源</param>
		/// <param name="target">目標</param>
		/// <param name="target">忽略大小寫 / 不忽略</param>
		/// <returns></returns>
		public static bool CheckStringValue(this string source, string target, bool ignore = true)
		{
			if (null == source || null == target) return false;
			return string.Compare(source.Trim(), target.Trim(), ignore) == 0;
		}

		/// <summary>
		/// 比較字串相不相同(不管大小寫)
		/// </summary>
		/// <param name="source">來源</param>
		/// <param name="targets">目標s</param>
		/// <returns></returns>
		public static bool CheckStringValue(this string source, string[] targets)
		{
			if (null == targets || targets.Length < 1)
				return false;
			else
			{
				foreach (string target in targets)
				{
					if (source.CheckStringValue(target))
						return true;
				}
				return false;
			}
		}
		/// <summary>
		/// 字串是否等於True(不分大小寫)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsTrue(this string str)
		{
			return string.Compare(str, bool.TrueString, true) == 0;
		}
		/// <summary>
		/// 字串是否等於False(不分大小寫)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsFalse(this string str)
		{
			return string.Compare(str, bool.FalseString, true) == 0;
		}
		/// <summary>
		/// 字串是否為 Null or Empty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

        /// <summary>
		/// 字串是否Email格式
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsEmail(this string str)
        {
            return string.IsNullOrEmpty(str) ? false : Regex.IsMatch(str, Function.EMAIL_REGEX);
        }

        /// <summary>
        /// 轉成 Int (非正常格式轉換則回傳0)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
		{
			int m_result;
			int.TryParse(str, out m_result);
			return m_result;
		}

		/// <summary>
		/// 字串是否 Int
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsInt(this string str)
		{
			int m_result;
			return int.TryParse(str, out m_result);
		}

		/// <summary>
		/// 轉成 Double (非正常格式轉換則回傳0)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static double ToDouble(this string str)
		{
			double m_result;
			double.TryParse(str, out m_result);
			return m_result;
		}

        /// <summary>
        /// 轉成 Decimal
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str)
        {
            decimal m_result;
            decimal.TryParse(str, out m_result);
            return m_result;
        }

        /// <summary>
        /// 轉成 Byte (非正常格式轉換則回傳0)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToByte(this string str)
		{
			byte m_result;
			byte.TryParse(str, out m_result);
			return m_result;
		}

		/// <summary>
		/// 轉成 MvcPaging 的 Index
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int ToMvcPaging(this string str)
		{
			if (str.IsNullOrEmpty()) return 0;
			return str.ToInt() > 0 ? (str.ToInt() - 1) : 0;
		}

		/// <summary>
		/// Contains 加上規則參數
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool Contains(this string str, string target, StringComparison comp)
		{
			if (target.IsNullOrEmpty()) return false;
			return str.IndexOf(target, comp) > -1;
		}

		/// <summary>
		/// Contains 加上規則參數
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool Contains(this string str, string[] targets, StringComparison comp)
		{
			if (targets == null || targets.Length < 1) return false;
			foreach (string target in targets)
			{
				if (str.Contains(target, comp))
					return true;
			}
			return false;
		}

		/// <summary>
		/// 轉成有加 http:// or https:// 字首的Url
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToHttpUrl(this string str)
		{
			if (str.IsNullOrEmpty()) return string.Empty;
			str = str.ToMyString();
			string[] contains = new string[] { "http://", "https://" };
			return str.Contains(contains, StringComparison.OrdinalIgnoreCase) ? str : string.Format("http://{0}", str);
		}
		/// <summary>
		/// 字串日期轉民國
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="format">轉換格式</param>
		/// <returns>非日期格式字串會回傳 string.Empty</returns>
		public static string ToTaiwanTypeDateString(this string datetime, string format = "yyyMMdd")
		{
			System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("zh-TW");
			System.Globalization.TaiwanCalendar calendar = new System.Globalization.TaiwanCalendar();
			info.DateTimeFormat.Calendar = calendar;
			DateTime tmpDate;
			if (DateTime.TryParse(datetime, out tmpDate))
			{
				return tmpDate.ToString(format, info);
			}
			return string.Empty;
		}

		/// <summary>
		/// 將文字補空白 補在右側
		/// </summary>
		/// <param name="str">
		/// <param name="emptyCount">不足要補string.Empty的位數</param></param>
		/// <returns></returns>
		public static string ToStringAddEmpty(this string str, int emptyCount)
		{
			return str.PadRight(emptyCount, ' ');
		}

		/// <summary>
		/// 字串轉日期格式(預設1753/01/01)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this string str)
		{
			return str.ToDateTime(Function.DEFAULT_TIME);
		}

		/// <summary>
		/// 字串轉日期格式
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultDate">非正確日期字串回傳指定預設值</param>
		/// <returns></returns>
		public static DateTime ToDateTime(this string str, DateTime defaultDate)
		{
			DateTime tmpDate;
			if (!DateTime.TryParse(str, out tmpDate) || tmpDate < Function.DEFAULT_TIME || tmpDate > DateTime.MaxValue)
				return defaultDate;
			return tmpDate;
		}
		/// <summary>
		/// 回傳 System.Web.Helpers.Crypto.SHA1 編碼後的字串
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToSHA1(this string str)
		{
			return Crypto.SHA1(str);
		}

		/// <summary>
		/// 轉換成string[]
		/// </summary>
		/// <param name="str"></param>
		/// <param name="split">預設 , 分隔</param>
		/// <returns></returns>
		public static string[] ToSplit(this string str, char split = ',')
		{
			return str.IsNullOrEmpty() ? new string[1] : str.Split(split);
		}
		/// <summary>
		/// 轉換 0 to false
		/// <para>1 to true</para>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToBoolString(this string str)
		{
			return ((EnableType)str.ToInt()) == EnableType.Enable ? bool.TrueString : bool.FalseString;
		}

		/// <summary>
		/// 轉換 false to 0
		/// <para>true to 1</para>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToEnableString(this string str)
		{
			return str.IsTrue() ? EnableType.Enable.ToIntValue() : EnableType.Disable.ToIntValue();
		}

		/// <summary>
		/// 取得變數名稱 c#6.0 can use nameof
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <param name="memberExpression"></param>
		/// <returns></returns>
		public static string GetMemberName<T>(this string str, Expression<Func<T>> memberExpression)
		{
			MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
			return expressionBody.Member.Name;
		}

		/// <summary>
		/// 取得youtube嵌入式網址縮圖
		/// </summary>
		/// <param name="str"></param>
		/// <param name="t">0~4</param>
		/// <returns></returns>
		public static string GetYoutubePic(this string str, string t = "0")
		{
			if (str.IsNullOrEmpty()) return string.Empty;
			return $"https://img.youtube.com/vi/{str.ToSplit('/')[str.ToSplit('/').Length - 1]}/{t}.jpg";
		}

		/// <summary>
		/// 回傳以半形計算的長度
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int GetBytesLength(this string str)
		{
			return str.IsNullOrEmpty() ? 0 : Encoding.Default.GetBytes(str).Length;
		}

		/// <summary>
		/// 變更部分字串顯示字元
		/// </summary>
		/// <param name="str"></param>
		/// <param name="start"></param>
		/// <param name="replace_len"></param>
		/// <param name="replace_sign">預設 X</param>
		/// <returns></returns>
		public static string ChangeDisplay(this string str, int start, int replace_len, string replace_sign = "X")
		{
			if (str.IsNullOrEmpty())
				return string.Empty;

			str = str.Trim();

			string replace_str = string.Empty;
			for (int i = 0; i < str.Length; i++)
			{
				if (i >= start && i < (start + replace_len))
				{
					replace_str += replace_sign;
				}
				else
				{
					replace_str += str.Substring(i, 1);
				}
			}
			return replace_str;
		}

		/// <summary>
		/// 取得右邊字串
		/// </summary>
		/// <param name="str"></param>
		/// <param name="maxLength">位數</param>
		/// <returns></returns>
		public static string Right(this string str, int maxLength)
		{
			if (str.IsNullOrEmpty())
				return "";

			return str.Substring(str.Length - maxLength);
		}

		/// <summary>
		/// 將字串中的民國替代掉 ex 109.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="baseDate">計算的基準日期</param>
		/// <returns></returns>
		public static string DeleteMinguo(this string str, DateTime? baseDate)
		{
			if (str.IsNullOrEmpty())
				return "";
			if (!baseDate.HasValue)
			{
				baseDate = DateTime.Now;
			}
			return str.Replace((baseDate.Value.Year - 1911).ToString() + ".", "");
		}

        /// <summary>
		/// 將字串轉換為NODE中的TITLE
		/// </summary>
		/// <param name="str"></param>
		/// <param name="baseDate">計算的基準日期</param>
		/// <returns></returns>
		public static string ToNodeTitle(this string str)
        {
            return Function.GetNodeTitle(str);
        }

    }
	#endregion

	#region object
	/// <summary>
	/// object 自訂的擴展方法
	/// </summary>
	public static class objectExtensions
	{
		/// <summary>
		/// 不用先判斷null，去除頭尾空白
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToMyString(this object obj)
		{
			return null == obj ? string.Empty : obj.ToString().Trim();
		}
		/// <summary>
		/// 不用先判斷null
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int ToMvcPaging(this object obj)
		{
			return null == obj ? 0 : obj.ToString().ToMvcPaging();
		}
		/// <summary>
		/// 轉換 From datetime
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static DateTime ToFromDate(this object obj)
		{
			if (obj == null) return Function.DEFAULT_TIME;
			DateTime tmpDate;
			if (!DateTime.TryParse(obj.ToString(), out tmpDate))
			{
				tmpDate = Function.DEFAULT_TIME;
			}
			return tmpDate;
		}

		/// <summary>
		/// 轉換 End datetime 給予 xxxx/xx/xx 23.59.59
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static DateTime ToEndDate(this object obj)
		{
			if (obj == null) return DateTime.MaxValue;

			DateTime tmpDate;
			if (DateTime.TryParse(obj.ToString(), out tmpDate))
			{
				tmpDate = tmpDate >= DateTime.MaxValue.AddDays(-1) ? DateTime.MaxValue :
					new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 23, 59, 59);
			}
			else
			{
				tmpDate = DateTime.MaxValue;
			}
			return tmpDate;
		}

		/// <summary>
		/// 轉成 預設列表顯示筆數 (object)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int ToDefaultPaging(this object obj, int defaultPage)
		{
			if (null == obj) return defaultPage;
			return obj.ToString().ToInt() > 0 ? obj.ToString().ToInt() : defaultPage;
		}

		/// <summary>
		/// 20200717 ting add
		/// 轉成日期字串(不含時間)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="delimiter">分隔符號,預設為「/」</param>
		/// <param name="minguo">true:顯示民國年</param>
		/// <param name="week">true:顯示星期幾</param>
		/// <returns>string</returns>
		public static string ToDateString(this object obj, string delimiter = "/", bool minguo = false, bool week = false)
		{
			if (obj == null) return string.Empty;
			string[] WEEKs = new string[] { "日", "一", "二", "三", "四", "五", "六" };
			DateTime dt = Convert.ToDateTime(obj);
			//dt.ToString("dddd", new CultureInfo("zh-TW")).Substring(2); //取得星期幾的另一個寫法
			int year = minguo ? (dt.Year - 1911) : dt.Year;
			return string.Format("{1}{0}{2}{0}{3}{4}", delimiter, year, dt.Month.ToString("00"), dt.Day.ToString("00"), (week ? " (" + WEEKs[dt.DayOfWeek.ToInt()] + ")" : ""));
		}

		/// <summary>
		/// 20200717 ting add 2022年02月14日(星期二) 23:59:59(sinyi改)
		/// 轉成日期字串(含時間)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="delimiter">分隔符號,預設為「/」</param>
		/// <param name="minguo">true:顯示民國年</param>
		/// <param name="week">true:顯示星期幾</param>
		/// <returns>string</returns>
		public static string ToDateTimeString(this object obj, string delimiter = "/", bool minguo = false, bool week = false)
		{
			if (obj == null) return string.Empty;
			string[] WEEKs = new string[] { "日", "一", "二", "三", "四", "五", "六" };
			DateTime dt = Convert.ToDateTime(obj);
			int year = minguo ? (dt.Year - 1911) : dt.Year;
			return string.Format("{1}年{2}月{3}日{4} {5}", delimiter, year, dt.Month.ToString("00"), dt.Day.ToString("00")
				, (week ? " (" + WEEKs[dt.DayOfWeek.ToInt()] + ")" : ""), dt.ToString("HH:mm:ss"));
		}
	}
	#endregion

	#region Int
	/// <summary>
	/// Int 自訂的擴展方法
	/// </summary>
	public static class intExtensions
	{
		/// <summary>
		/// 補0的位數，8位
		/// </summary>
		const int ZERO_COUNT = 8;
		/// <summary>
		/// 將數字轉成文字並且補0(排序用，8位數)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string IntToStringAddZero(this int num)
		{
			return num.IntToStringAddZero(ZERO_COUNT);
		}
		/// <summary>
		/// 將數字轉成文字並且補0(排序用，8位數) (補右邊)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string IntToStringAddZeroRight(this int num)
		{
			return num.IntToStringAddZeroRight(ZERO_COUNT);
		}
		/// <summary>
		/// 將數字轉成文字並且補0
		/// </summary>
		/// <param name="num">
		/// <param name="zeroCount">不足要補0的位數</param></param>
		/// <returns></returns>
		public static string IntToStringAddZero(this int num, int zeroCount)
		{
			return num.ToString().PadLeft(zeroCount, '0');
		}
		/// <summary>
		/// 將數字轉成文字並且補0 (補右邊)
		/// </summary>
		/// <param name="num">
		/// <param name="zeroCount">不足要補0的位數</param></param>
		/// <returns></returns>
		public static string IntToStringAddZeroRight(this int num, int zeroCount)
		{
			return num.ToString().PadRight(zeroCount, '0');
		}
		/// <summary>
		/// 將數字轉為千位加逗號的顯示方式
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string ReplaceNumToThousand(this int num)
		{
			return num.ToString("N0");
		}
		/// <summary>
		/// 除法 四捨五入
		/// </summary>
		/// <param name="numerator"></param>
		/// <param name="denominator">0的話會回傳0</param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static string Division(this int numerator, int denominator, int point = 2)
		{
			if (denominator == 0)
			{
				return "0";
			}
			return Math.Round(((double)numerator / denominator), point, MidpointRounding.AwayFromZero).ToMyDoubleString(point: point);
		}
	}
	#endregion

	#region Long

	/// <summary>
	/// Long 自訂的擴展方法
	/// </summary>
	public static class longExtensions
	{
		/// <summary>
		/// 補0的位數，8位
		/// </summary>
		const int ZERO_COUNT = 8;
		/// <summary>
		/// 將數字轉成文字並且補0(排序用，8位數)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string LongToStringAddZero(this long num)
		{
			return num.ToString().PadLeft(ZERO_COUNT, '0');
		}

		/// <summary>
		/// 轉換為 Int32
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static int ToInt(this long num)
		{
			return Convert.ToInt32(num);
		}
	}

	#endregion

	#region Double

	/// <summary>
	/// Double 自訂的擴展方法
	/// </summary>
	public static class doubleExtensions
	{
		/// <summary>
		/// 轉換自訂的double 格式 四捨五入
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isPercent">是否百分比</param>
		/// <param name="point">小數點幾位</param>
		/// <returns></returns>
		public static double ToMyDouble(this double value, bool isPercent = false, int point = 2)
		{
			return value.ToMyDoubleString(isPercent, point).ToDouble();
		}

		/// <summary>
		/// 轉換自訂的double 格式 四捨五入
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isPercent">是否百分比</param>
		/// <param name="point">小數點幾位</param>
		/// <returns></returns>
		public static string ToMyDoubleString(this double value, bool isPercent = false, int point = 2)
		{
			//ToString("P4", CultureInfo.InvariantCulture);
			string _tmpPoint = string.Empty.PadLeft(point, '#');
			return (isPercent ? value * 100 : value).ToString(string.Format("0.{0}", _tmpPoint));
		}
	}

	#endregion

	#region DateTime

	/// <summary>
	/// DateTime 自訂的擴展方法
	/// </summary>
	public static class DateTimeExtensions
	{

		/// <summary>
		/// To the full taiwan date. ex. 106/01/01
		/// </summary>
		/// <param name="datetime">The datetime.</param>
		/// <returns></returns>
		public static string ToFullTaiwanDate(this DateTime datetime)
		{
			TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

			return string.Format("{0}/{1}/{2}",
								 taiwanCalendar.GetYear(datetime),
								 datetime.Month,
								 datetime.Day);
		}

		/// <summary>
		/// 民國xx年xx月xx日
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToTaiwanTypeDateString(this DateTime datetime, string format = "yyy年MM月dd日")
		{
			//string _format = "yyy年MM月dd日";
			System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("zh-TW");
			System.Globalization.TaiwanCalendar calendar = new System.Globalization.TaiwanCalendar();
			info.DateTimeFormat.Calendar = calendar;
			string tmpString;
			if (datetime.Year < 1912)
			{
				int offsetYear = 1912 - datetime.Year;
				datetime = datetime.AddYears(offsetYear * 2 - 1);
				tmpString = datetime.ToString(format, info);
				tmpString = "民國前" + tmpString;
			}
			else
			{
				tmpString = datetime.ToString(format, info);
			}
			return tmpString;
		}

		/// <summary>
		/// 製造20位數的時間字串
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToString20s(this DateTime datetime)
		{
			return datetime.ToString("yyyyMMddHHmmssffffff");
		}

		/// <summary>
		/// check datetime 要在資料庫容許範圍內 1753/01/01~9999/12/31
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime CheckValue(this DateTime datetime)
		{
			if (datetime > DateTime.MaxValue || datetime < Function.DEFAULT_TIME)
				return DateTime.Now;
			return datetime;
		}
		/// <summary>
		/// 大於等於 Today
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static bool IsBiggerToday(this DateTime datetime)
		{
			return datetime >= DateTime.Today;
		}
		/// <summary>
		/// 小於等於 Today
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static bool IsSmallerToday(this DateTime datetime)
		{
			return datetime <= DateTime.Today;
		}

		/// <summary>
		/// 是否為上班日
		/// </summary>
		/// <param name="date"></param>
		/// <param name="notInclude"></param>
		/// <returns></returns>
		public static bool IsWorkDay(this DateTime datetime, DateTime[] notInclude = null)
		{
			if (datetime.DayOfWeek == DayOfWeek.Saturday || datetime.DayOfWeek == DayOfWeek.Sunday)
				return false;
			if (notInclude != null && notInclude.Contains(datetime.Date))
				return false;
			return true;
		}

		/// <summary>
		/// 回傳預設日期字串 (ex:2012/01/01)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultString(this DateTime datetime)
		{
			return datetime.ToString("yyyy/MM/dd");
		}

		/// <summary>
		/// 回傳日期字串 (ex:20120101)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultString2(this DateTime datetime)
		{
			return datetime.ToString("yyyyMMdd");
		}

		/// <summary>
		/// 回傳日期字串 (ex:2012.01.01)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultString3(this DateTime datetime)
		{
			return datetime.ToString("yyyy.MM.dd");
		}

		/// <summary>
		/// 回傳日期字串+時分 (ex:201201011506)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultString4(this DateTime datetime)
		{
			return datetime.ToString("yyyyMMddhhmm");
		}

		/// <summary>
		/// 回傳預設日期時間字串 (ex:2012/01/01 08:00)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultStringWithTime(this DateTime datetime)
		{
			return datetime.ToString("yyyy/MM/dd HH:mm");
		}

		/// <summary>
		/// 回傳預設日期時間字串 (ex:2012/01/01 08:00:00)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultStringWithTime1(this DateTime datetime)
		{
			return datetime.ToString("yyyy/MM/dd HH:mm:ss");
		}

		/// <summary>
		/// 回傳預設日期時間字串 (ex:2012/01/01 12:00:00)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultTimdString(this DateTime datetime)
		{
			return datetime.ToString("yyyy/MM/dd HH:mm");
		}

		/// <summary>
		/// 日期轉民國 1020101
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToTaiwanTypeDateString2(this DateTime datetime, string format = "yyyMMdd")
		{
			System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("zh-TW");
			System.Globalization.TaiwanCalendar calendar = new System.Globalization.TaiwanCalendar();
			info.DateTimeFormat.Calendar = calendar;
			return datetime.ToString(format, info);
		}

		/// <summary>
		/// 取得與xx日期的間隔天數
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static int GetBetweenDays(this DateTime datetime, DateTime end)
		{
			return Function.GetDaysBetweenTwoDates(datetime, end);
		}

		/// <summary>
		/// 計算兩個日期時間差
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static Tuple<int, int, int> TimespanToDate(this DateTime self, DateTime target)
		{
			int years, months, days;
			// 因為只需取量，不決定誰大誰小，所以如果self < target時要交換將大的擺前面
			if (self < target)
			{
				DateTime tmp = target;
				target = self;
				self = tmp;
			}

			// 將年轉換成月份以便用來計算
			months = 12 * (self.Year - target.Year) + (self.Month - target.Month);

			// 如果天數要相減的量不夠時要向月份借天數補滿該月再來相減
			if (self.Day < target.Day)
			{
				months--;
				days = DateTime.DaysInMonth(target.Year, target.Month) - target.Day + self.Day;
			}
			else
			{
				days = self.Day - target.Day;
			}

			// 天數計算完成後將月份轉成年
			years = months / 12;
			months = months % 12;

			return Tuple.Create(years, months, days);
		}

		/// <summary>
		/// 日期轉填報時期 (ex:2010年03月)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToReportDateString(this DateTime datetime)
		{
			return datetime.ToString("yyyy年MM月");
		}

		/// <summary>
		/// 回傳該日期當週的第一天 (星期一)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime ToWeekFirstDay(this DateTime datetime)
		{
			int w = (int)datetime.DayOfWeek;
			return w == 0 ? datetime.AddDays(w - 6) : datetime.AddDays(-(w - 1));
		}

		/// <summary>
		/// 回傳 1號 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime ToMonthStart(this DateTime datetime)
		{
			return new DateTime(datetime.Year, datetime.Month, 1);
		}
		/// <summary>
		/// 回傳 最後一天 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime ToMonthEnd(this DateTime datetime)
		{
			return new DateTime(datetime.AddMonths(1).Year, datetime.AddMonths(1).Month, 1).AddDays(-1);
		}

		/// <summary>
		/// 計算年齡
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="reference">計算基準 不輸入為今日</param>
		/// <returns></returns>
		public static int GetAge(this DateTime datetime, DateTime? reference = null)
		{
			if (!reference.HasValue)
			{
				reference = DateTime.Today;
			}
			int age = reference.Value.Year - datetime.Year;
			//if (reference < datetime.AddYears(age)) age--;//不用算足月

			return age;
		}

		/// <summary>
		/// 取得變數名稱 c#6.0 can use nameof
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <param name="memberExpression"></param>
		/// <returns></returns>
		public static string GetMemberName<T>(this DateTime datetime, Expression<Func<T>> memberExpression)
		{
			MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
			return expressionBody.Member.Name;
		}

		/// <summary>
		/// 回傳日期字串 for Google Calendar (ex:20160101T120000Z) GMT -8 因為台灣
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultStringGoogleCalendar(this DateTime datetime)
		{
			//"yyyyMMddTHHmmssZ"
			return string.Format("{0}T{1}{2}Z", datetime.ToString("yyyyMMdd"), (datetime.Hour - 8).IntToStringAddZero(2), datetime.ToString("mmss"));
		}

		/// <summary>
		/// 回傳該月的天數集合
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static IEnumerable<DateTime> AllDatesInMonth(this DateTime datetime)
		{
			int year = datetime.Year;
			int month = datetime.Month;
			int days = DateTime.DaysInMonth(year, month);
			for (int day = 1; day <= days; day++)
			{
				yield return new DateTime(year, month, day);
			}
		}
	}



	#endregion

	#region Nullable<T>
	/// <summary>
	/// Nullable 自訂的擴展方法
	/// </summary>
	public static class NullableExtensions
	{
		/// <summary>
		/// check datetime 要在資料庫容許範圍內 1753/01/01~9999/12/31
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime CheckValue(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.CheckValue() : DateTime.Now;
		}
		/// <summary>
		/// 大於等於 Today (Datetime?)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static bool IsBiggerToday(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.IsBiggerToday() : false;
		}
		/// <summary>
		/// 小於等於 Today (DateTime?)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static bool IsSmallerToday(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.IsSmallerToday() : false;
		}

		/// <summary>
		/// 是否為上班日
		/// </summary>
		/// <param name="date"></param>
		/// <param name="notInclude"></param>
		/// <returns></returns>
		public static bool IsWorkDay(this DateTime? datetime, DateTime[] notInclude = null)
		{
			return datetime.HasValue ? datetime.Value.IsWorkDay(notInclude) : false;
		}

		/// <summary>
		/// 回傳預設日期字串 (ex:2012/01/01)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultString(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultString() : string.Empty;
		}

		/// <summary>
		/// 回傳日期字串 (ex:20120101)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultString2(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultString2() : string.Empty;
		}

		/// <summary>
		/// 回傳預設日期字串 (ex:2012.01.01)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultString3(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultString3() : string.Empty;
		}

		/// <summary>
		/// 回傳預設日期字串 (ex:2012 or 01.01)
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="t">0:年份 其它:日期的部分 01.01</param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultString4(this DateTime? datetime, int t = 0)
		{
			if (t == 0)
			{
				return datetime.HasValue ? datetime.Value.Year.ToString() : string.Empty;
			}
			else
			{
				return datetime.HasValue ? datetime.Value.ToString("MM.dd") : string.Empty;
			}
		}

		/// <summary>
		/// 回傳預設日期時間字串 (ex:2012/01/01 08:00)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultStringWithTime(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultStringWithTime() : string.Empty;
		}

		/// <summary>
		/// 回傳預設日期時間字串 (ex:2012/01/01 08:00:00)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 string.Empty</returns>
		public static string ToDefaultStringWithTime1(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultStringWithTime1() : string.Empty;
		}

		/// <summary>
		/// 回傳 From 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 1753/01/01</returns>
		public static DateTime ToFromTime(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value : Function.DEFAULT_TIME;
		}

		/// <summary>
		/// 回傳 End 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 9999/12/31</returns>
		public static DateTime ToEndTime(this DateTime? datetime)
		{
			if (datetime.HasValue)
				return datetime.Value >= DateTime.MaxValue.AddDays(-1) ? DateTime.MaxValue : datetime.Value.AddDays(1);
			else
			{
				return DateTime.MaxValue;
			}
		}
		/// <summary>
		/// 回傳 前月一號 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 前月一號</returns>
		public static DateTime ToLastMonthStart(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value : Function.LAST_MONTH_FIRST_DATE;
		}
		/// <summary>
		/// 回傳 前月最後一天 日期
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>null 則回傳 前月最後一天</returns>
		public static DateTime ToLastMonthEnd(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value : Function.LAST_MONTHR_LAST_DATE;
		}
		/// <summary>
		/// 轉成 MvcPaging 的 Index (Int?)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int ToMvcPaging(this int? number)
		{
			return number.HasValue ? (number.Value > 0 ? number.Value - 1 : 0) : 0;
		}

		/// <summary>
		/// 轉成 預設列表顯示筆數 (Int?)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int ToDefaultPaging(this int? number, int defaultPage)
		{
			return number.HasValue ? (number.Value > 0 ? number.Value : defaultPage) : defaultPage;
		}

		/// <summary>
		/// 日期轉填報時期 (ex:2010年03月)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToReportDateString(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToReportDateString() : string.Empty;
		}

		/// <summary>
		/// 回傳該日期當週的第一天 (星期一)
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static DateTime ToWeekFirstDay(this DateTime? datetime)
		{
			DateTime _tmp = datetime.HasValue ? datetime.Value : DateTime.Today;
			int w = (int)_tmp.DayOfWeek;
			return w == 0 ? _tmp.AddDays(w - 6) : _tmp.AddDays(-(w - 1));
		}

		/// <summary>
		/// Decimal 轉 Int
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static int ToInt(this decimal? value)
		{
			if (value.HasValue)
			{
				return Convert.ToInt32(value.Value);
			}
			return 0;
		}

		/// <summary>
		/// 字串日期轉民國
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="format">轉換格式</param>
		/// <returns>非日期格式字串會回傳 string.Empty</returns>
		public static string ToTaiwanTypeDateString(this DateTime? datetime, string format = "yyy/MM/dd")
		{
			System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("zh-TW");
			System.Globalization.TaiwanCalendar calendar = new System.Globalization.TaiwanCalendar();
			info.DateTimeFormat.Calendar = calendar;
			DateTime tmpDate;
			if (DateTime.TryParse(datetime.ToString(), out tmpDate))
			{
				return tmpDate.ToString(format, info);
			}
			return string.Empty;
		}

		/// <summary>
		/// 計算年齡
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="reference">計算基準 不輸入為今日</param>
		/// <returns></returns>
		public static int GetAge(this DateTime? datetime, DateTime? reference = null)
		{
			if (datetime.HasValue)
			{
				return datetime.Value.GetAge(reference);
			}
			return 0;
		}

		/// <summary>
		/// 將民國日期互轉西元日期 特別for JqueryDatePickerTW.js 的邏輯
		/// </summary>
		/// <param name="datetime"></param>
		/// <param name="isTaiwan">true=轉民國 預設轉西元</param>
		/// <returns>無值回傳null</returns>
		public static DateTime? ConvertTaiwanToWorld(this DateTime? datetime, bool isTaiwan = false)
		{
			if (datetime.HasValue)
			{
				if (isTaiwan)//轉台灣
				{
					if (datetime.Value.Year < 2000)//1961/04/22 介面上要呈現民國50年的話 西元要變 1950/04/22
					{
						return new DateTime(datetime.Value.Year - 11, datetime.Value.Month, datetime.Value.Day);
					}
					else//介面上要呈現民國106年的話 西元要變 0106/04/22
					{
						return new DateTime(datetime.Value.Year - 1911, datetime.Value.Month, datetime.Value.Day);
					}
					//return new DateTime(datetime.Value.Year - 1911, datetime.Value.Month, datetime.Value.Day);
				}
				else//轉西元
				{
					if (datetime.Value.Year >= 1911)//介面上如果是 50/04/22 會直接回傳 1950/04/22
					{
						return new DateTime(datetime.Value.Year + 11, datetime.Value.Month, datetime.Value.Day);
					}
					else//介面上如果是 106/04/22 會直接回傳 0106/04/22
					{
						return new DateTime(datetime.Value.Year + 1911, datetime.Value.Month, datetime.Value.Day);
					}
					//return new DateTime(datetime.Value.Year + 1911, datetime.Value.Month, datetime.Value.Day);
				}
			}
			return null;
		}

		/// <summary>
		/// 回傳日期字串 for Google Calendar (ex:20160101T120000Z) GMT -8 因為台灣
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDefaultStringGoogleCalendar(this DateTime? datetime)
		{
			return datetime.HasValue ? datetime.Value.ToDefaultStringGoogleCalendar() : string.Empty;
		}

		/// <summary>
		/// 回傳該月的天數集合
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static IEnumerable<DateTime> AllDatesInMonth(this DateTime? datetime)
		{
			if (!datetime.HasValue)
			{
				datetime = DateTime.Now;
			}
			return datetime.Value.AllDatesInMonth();
		}

	}
	#endregion

	#region byte
	/// <summary>
	/// byte 自訂的擴展方法
	/// </summary>
	public static class byteExtensions
	{
		/// <summary>
		/// 是否 Enable 1
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsEnable(this byte value)
		{
			return value == EnableType.Enable.ToByteValue();
		}
		/// <summary>
		/// 是否 Disable 0
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDisable(this byte value)
		{
			return value == EnableType.Disable.ToByteValue();
		}
	}
	#endregion

	#region Enum
	/// <summary>
	/// Enum 自訂的擴展方法
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// 取得 [Description("xxx")]
		/// </summary>
		/// <param name="value">value of an Enum</param>
		/// <returns></returns>
		public static string GetDescription(this System.Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			if (fi == null)
			{
				return " ";
			}
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}

		/// <summary>
		/// 將Enum value ConverTo Int String
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToIntValue(this System.Enum value)
		{
			return value == null ? string.Empty : Convert.ToInt32(value).ToString();
		}

		/// <summary>
		/// 將Enum value ConverTo Byte 0~255
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte ToByteValue(this System.Enum value)
		{
			return Convert.ToByte(value);
		}

		/// <summary>
		/// 將Enum value ConverTo int 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ToInt(this System.Enum value)
		{
			return Convert.ToInt32(value);
		}

		/// <summary>
		/// 取得 enum 列舉
		/// 用法：ExhibitionTimeType.dress.GetEnumForeach()
		/// </summary>
		/// <param name="value">typeof(enum..)</param>
		/// <returns></returns>
		public static Array GetEnumForeach(this System.Enum value)
		{
			return System.Enum.GetValues(value.GetType());
		}

		/// <summary>
		/// 取得根據 priority 排序的 enum 列舉，
		/// 用法：typeof(HostingType).GetEnumForeachOrderByPriority<HostingType>()
		/// </summary>
		/// <param name="value">value of an Type</param>
		/// <returns></returns>
		public static Array GetEnumForeachOrderByPriority<T>(this System.Type type)
		{
			Dictionary<string, int> priorityTable = new Dictionary<string, int>();
			var values = System.Enum.GetValues(typeof(T)).Cast<T>();
			MemberInfo[] members = typeof(T).GetMembers();
			foreach (MemberInfo member in members)
			{
				object[] attrs = member.GetCustomAttributes(typeof(PriorityAttribute), false);
				foreach (object attr in attrs)
				{
					PriorityAttribute orderAttr = attr as PriorityAttribute;
					if (orderAttr != null)
					{
						string propName = member.Name;
						int priority = orderAttr.priority;
						priorityTable.Add(propName, priority);
					}
				}
			}
			return values.OrderBy(n => priorityTable[n.ToString()]).ToArray();
		}

		/// <summary>
		/// 設定顯示的順序，用法：[Priority(0)]
		/// </summary>
		/// <param name="value">value of an Enum</param>
		/// <returns></returns>
		public static int GetPriority(this System.Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			if (fi == null)
			{
				return -1;
			}
			PriorityAttribute[] attributes = (PriorityAttribute[])fi.GetCustomAttributes(typeof(PriorityAttribute), false);
			return (attributes.Length > 0) ? attributes[0].priority : value.ToInt();
		}
	}
	#endregion

	#region IQueryable
	/// <summary>
	/// IQueryable 自訂的擴展方法
	/// </summary>
	public static class IQueryableExtensions
	{
		public static IQueryable<T> OrderByRandom<T>(this IQueryable<T> query)
		{
			return (from q in query
					orderby Guid.NewGuid()
					select q);
		}
	}

	#endregion

	#region IEnumerable
	/// <summary>
	/// IEnumerable 自訂的擴展方法
	/// </summary>
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> OrderByRandom<T>(this IEnumerable<T> query)
		{
			return (from q in query
					orderby Guid.NewGuid()
					select q);
		}
	}

    #endregion

    #region Generic

    public static class Generic
    {
        public static SelectList ToSelectList<T>(this T value, SelectListValueType type = SelectListValueType.Int)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            Type t = typeof(T);

            if (t.IsEnum)
            {
                foreach (T obj in System.Enum.GetValues(t))
                {
                    System.Enum _enum = System.Enum.Parse(typeof(T), obj.ToString()) as System.Enum;
                    SelectListItem item = new SelectListItem();

                    item.Text = _enum.GetDescription();

                    switch (type)
                    {
                        case SelectListValueType.Int:
                            item.Value = _enum.ToIntValue();
                            break;
                        default:
                            item.Value = _enum.ToString();
                            break;
                    }

                    list.Add(item);
                }
            }

            return new SelectList(list, "Value", "Text");
        }
    }

    #endregion
}
