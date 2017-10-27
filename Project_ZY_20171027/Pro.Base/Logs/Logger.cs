using System;
using System.Collections.Generic;
using System.Text;
using log4net;
namespace Krs.Base.Logs
{
    public class Logger
    {
        private static readonly ILog log = log4net.LogManager.Exists("");

        /// <summary>
        /// 获取字典字符表示
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string GetMessage(Dictionary<string, string> dir)
        {
            if (dir == null
                || dir.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> value in dir)
            {
                sb.AppendFormat("{0}:{1}|", value.Key, value.Value);
            }
            int len = sb.ToString().Length;
            return sb.ToString().Substring(0, len - 1);
        }


        //private static LogInfo GetLogInfo()
        //{
        //    LogInfo info = new LogInfo();
        //    log.Debug(
        //}

        //public static void Error(string message)
        //{ 
        
        //}

        //public static void Error(string message, Exception ex)
        //{
 
        //}

    }
}
