using System;
using System.IO;

namespace KingspModel
{
    /// <summary>
    /// 產生檔案將會以時間日期為主要的檔名。格式為yyyy-MM-dd-hhmmss.log 檔案路徑則是在confing (LogFilePath)中設定<br/>
    /// 的目錄下 ./log/xxxx-xx-xx-xxxxxx.log   
    /// 欲啟動LogSystem 請先執行 initLogSystem() 函式
    /// </summary>
    public sealed class LogSystem
    {
        /// <summary>
        /// LogSystem的實體物件.呼叫initLogSystem時由內部產生;
        /// </summary>
        static LogSystem s_log = null;
        /// <summary>
        /// log file 的 output stream
        /// </summary>
        static FileStream s_fileStream = null;
        /// <summary>
        /// Stream writer for log message
        /// </summary>
        static StreamWriter s_writer = null;
        /// <summary>
        /// log存放的目錄檔案
        /// </summary>
        static readonly string FileFolder = System.Web.Hosting.HostingEnvironment.MapPath(Function.GetConfigSetting("LogFilePath"));
        /// <summary>
        /// 初始化時可能產生的Error Message
        /// </summary>
        static string s_errorMsg = string.Empty;

        LogSystem()
        {
            DirectoryInfo dirInfo = null;
            //check directoryinfo first
            try
            {
                if (!Directory.Exists(FileFolder))
                {
                    dirInfo = Directory.CreateDirectory(FileFolder);
                }
                else
                    dirInfo = new DirectoryInfo(FileFolder);

                String FileName = String.Format("{0}/{1}.log", dirInfo.FullName, DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
                s_fileStream = new FileStream(FileName, FileMode.Create);
            }
            catch (Exception ex)
            {
                s_errorMsg = string.Format("{0}\r\n堆疊:{1}", ex.Message, ex.StackTrace);
                //if error use default directory
                if (!Directory.Exists(FileFolder))
                {
                    dirInfo = Directory.CreateDirectory(FileFolder);
                }
                else
                    dirInfo = new DirectoryInfo(FileFolder);
                String fileName = String.Format("{0}/{1}.log", dirInfo.FullName, DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
                s_fileStream = new FileStream(fileName, FileMode.Create);
            }

            if (s_fileStream != null)
            {
                s_writer = new StreamWriter(s_fileStream);  //create stream writer
            }
        }

        /// <summary>
        /// initial LogSystem environment.
        /// </summary>
        public static void InitLogSystem()
        {
            s_log = new LogSystem();
            if (!string.IsNullOrEmpty(s_errorMsg))
            {
                WriteLine(s_errorMsg);//write error first
                s_errorMsg = string.Empty;//clean error
            }
        }

        /// <summary>
        /// 執行寫入的底層函式
        /// </summary>
        /// <param name="msg">log message</param>
        public static void WriteLine(String msg)
        {
            if (s_log == null || s_writer == null || msg == null)
                return;
            s_writer.WriteLine(msg);
            s_writer.Flush();
        }

        /// <summary>
        /// Close stream and file stream
        /// </summary>
        public static void CloseUnderlayingStream()
        {
            s_writer.Flush();
            s_fileStream.Flush();
            s_writer.Close();
            s_fileStream.Close();
        }
    }
}
