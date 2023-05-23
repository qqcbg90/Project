using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingspModel.Enum;

namespace KingspModel
{
    public class Msgbox
    {
        /// <summary>
        /// 提示對話框
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="type">訊息類別：1.notice  2.warning 3.error 4.success</param>
        /// <param name="position">訊息位置：1.top-left  2.top-center 3.top-right 4.middle-left 5.middle-center 6.middle-right</param>
        /// <param name="nEffectDuration">動態效果持續時間  deafult:600</param>
        /// <param name="stayTime">停留時間  deafult:3000</param>
        /// <param name="sticky">是否要停留</param>
        /// <param name="closeText">關閉的訊息 預設為""</param>
        /// <returns></returns>
        public static string Toast(string message, AlertMsgType type, Position position, int nEffectDuration, int stayTime, bool sticky, string closeText)
        {
            // top-left, top-center, top-right, middle-left, middle-center, middle-right
            // notice, warning, error, success

            string postStr = string.Empty;


            switch (position)
            {
                case Position.Top_left:
                    postStr = "top-left";
                    break;
                case Position.Top_center:
                    postStr = "top-center";
                    break;
                case Position.Top_right:
                    postStr = "top-right";
                    break;
                case Position.Middle_left:
                    postStr = "middle-left";
                    break;
                case Position.Middle_center:
                    postStr = "middle-center";
                    break;
                case Position.Middle_right:
                    postStr = "middle-right";
                    break;
            }

            string msg = "<script type='text/javascript'>jQuery(function() { $().toastmessage('showToast',{"
                            + "nEffectDuration:" + nEffectDuration + ","//600
                            + "stayTime:" + stayTime + "," //3000
                            + "text     : '" + message + "',"
                            + "sticky   : " + sticky.ToString().ToLower() + "," //false為自動消失
                            + "position : '" + postStr + "'," //top-right
                            + "type     : '" + type.ToString().ToLower() + "',"
                            + "closeText: '" + closeText + "'"
                //+ ",close: function () {console.log('toast is closed ...');"
                            + "}); });</script>";


            return msg;
        }


        public static string Toast(string message, AlertMsgType type, Position position)
        {
            return Toast(message, type, position, 600, 3000, false, "");
        }

        public static string Toast(string message, AlertMsgType type)
        {
            return Toast(message, type, Position.Middle_center);
        }

        public static string ToastNoJs(string message, AlertMsgType type, Position position, string id)
        {
            return ToastNoJs(message, type, position, 600, 3000, false, "", id);
        }

        public static string ToastNoJs(string message, AlertMsgType type, Position position, int nEffectDuration, int stayTime, bool sticky, string closeText, string id)
        {
            // top-left, top-center, top-right, middle-left, middle-center, middle-right
            // notice, warning, error, success

            string postStr = string.Empty;


            switch (position)
            {
                case Position.Top_left:
                    postStr = "top-left";
                    break;
                case Position.Top_center:
                    postStr = "top-center";
                    break;
                case Position.Top_right:
                    postStr = "top-right";
                    break;
                case Position.Middle_left:
                    postStr = "middle-left";
                    break;
                case Position.Middle_center:
                    postStr = "middle-center";
                    break;
                case Position.Middle_right:
                    postStr = "middle-right";
                    break;
            }

            string msg = "jQuery(function() { $().toastmessage('showToast',{"
                            + "nEffectDuration:" + nEffectDuration + ","//600
                            + "stayTime:" + stayTime + "," //3000
                            + "text     : '" + message + "',"
                            + "sticky   : " + sticky.ToString().ToLower() + "," //false為自動消失
                            + "position : '" + postStr + "'," //top-right
                            + "type     : '" + type.ToString().ToLower() + "',"
                            + "closeText: '" + closeText + "'"
                //+ ",close: function () {console.log('toast is closed ...');"
                            + "}); Remove('" + id + "'); });";


            return msg;
        }
    }
}
