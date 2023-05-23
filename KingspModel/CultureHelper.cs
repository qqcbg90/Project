using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KingspModel
{
    /// <summary>
    /// 多語系 Class
    /// </summary>
    public sealed class CultureHelper
    {
        /// <summary>
        /// 繁體中文 zh-TW
        /// </summary>
        public const string ZH_TW = "zh-TW";
        /// <summary>
        /// 英文 en-US
        /// </summary>
        public const string EN_US = "en-US";
        /// <summary>
        /// 日文 ja-JP
        /// </summary>
        public const string JA_JP = "ja-JP";
        /// <summary>
        /// 韓文 ko-KR
        /// </summary>
        public const string KO_KR = "ko-KR";
        /// <summary>
        /// 泰文 th-TH
        /// </summary>
        public const string TH_TH = "th-TH";
        /// <summary>
        /// 越南語 vi-VN
        /// </summary>
        public const string VI_VN = "vi-VN";
        /// <summary>
        /// 馬來西亞文 ms-MY
        /// </summary>
        public const string MS_MY = "ms-MY";

        /// <summary>
        /// 實作語系
        /// </summary>
        static readonly IList<string> _cultures = new List<string>
        {
            ZH_TW,
            EN_US,
            JA_JP,
            KO_KR,
            TH_TH,
            VI_VN,
            MS_MY
        };

        /// <summary>
        /// 依照「name」參數回傳有效並已實作之語系名稱。
        /// 若無合適語系名稱，則回傳預設語系名稱。
        /// 本專案的預設語系名稱為「zh-TW」
        /// </summary>
        /// <param name="name">語系名稱</param>
        public static string GetImplementedCulture(string name)
        {
            // 確認是否為空字串
            if (name.IsNullOrEmpty())
                return GetDefaultCulture();  // 若是空字串則回傳預設語系

            // 如果該語系名稱已被實作，則接受使用該語系名稱
            if (_cultures.Where(c =>
                                c.Equals(name,
                                StringComparison.InvariantCultureIgnoreCase))
                               .Count() > 0)
                return name; // 接受這個語系


            // 取得最接近之語系名稱。例如，如果已經實作了「en-US」而使用者的請求是「en-GB」， 
            // 則回傳最接近的「en-US」因為這樣至少是相同的語言（例如：英文）  
            var n = GetNeutralCulture(name);
            foreach (var c in _cultures)
                if (c.StartsWith(n))
                    return c;

            // 如果沒有合適的，就回傳預設語系名稱
            return GetDefaultCulture();
        }


        /// <summary>
        /// 回傳預設的語系名稱        
        /// </summary>    
        public static string GetDefaultCulture()
        {
            return _cultures[0];
        }



        /// <summary>
        /// 取得目前語系名稱
        /// </summary>    
        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// 取得目前的中性語系名稱
        /// </summary>    
        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }

        /// <summary>
        /// 取得中性語系名稱
        /// </summary>        
        public static string GetNeutralCulture(string name)
        {
            if (name.Length < 2)
                return name;

            return name.Substring(0, 2); // 回傳前兩個字元，例如："en", "es"
        }
    }

}
