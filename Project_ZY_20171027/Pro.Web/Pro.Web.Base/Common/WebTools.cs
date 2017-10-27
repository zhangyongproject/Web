using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

using Pro.Base.Logs;
using System.Web.UI.WebControls;
using System.IO;
using System;

namespace Pro.Web.Common
{
    public class WebTools
    {
        #region WEB相关方法
        /// <summary>
        /// 得到一个Cookie的值
        /// </summary>
        /// <param name="ckName">要获取的Cookie的名称</param>
        /// <param name="defaultVal">当不存在该Cookie时返回的默认值</param>
        /// <returns>Cookie值</returns>
        public static string GetCookieVal(string ckName, string defaultVal)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpCookie cookie = request.Cookies[ckName];
            if (cookie != null)
                return cookie.Value;
            //else
            //    LogMessageHelper.LogDEBUG(string.Format("获取cookie({0})失败", ckName));
            return defaultVal;
        }

        /// <summary>
        /// 得到Request中指定参数的值
        /// </summary>
        /// <param name="keyName">要获取的参数的名称</param>
        /// <param name="defaultVal">当不存在该参数时返回的默认值</param>
        /// <returns>参数值</returns>
        public static string GetRequestVal(string keyName, string defaultVal)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            string requestStr = request[keyName];
            if (requestStr != null)
                return requestStr;
            //else
            //    LogMessageHelper.LogDEBUG(string.Format("获取参数{0}失败", keyName));
            return defaultVal;
        }

        /// <summary>
        /// 解码URL中的字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string DecodeURL(string val)
        {
            return HttpUtility.UrlDecode(val);
        }

        /// <summary>
        /// 加密URL中的字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EncodeURL(string val)
        {
            return HttpUtility.UrlEncode(val);
        }

        /// <summary>
        /// 获取客户端标识串(适用于WEB系统)
        /// </summary>
        /// <param name="IP">客户端IP</param>
        /// <returns></returns>
        public static string GetClientSecretStr(string IP)
        {
            return System.Web.HttpContext.Current.Request.UserAgent + IP;
        }

        /// <summary>
        /// 获取本机的IP地址
        /// </summary>
        public static string GetIPAddress()
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress SrcAddress = hostInfo.AddressList[0];
            return SrcAddress.ToString();
        }

        /// <summary>
        /// 获取远程客户端的网卡物理地址(MAC)
        /// </summary>
        /// <param name="IP">要获取MAC的IP地址</param>
        /// <returns></returns>
        public static string GetMac(string IP)
        {
            string dirResults = "";
            ProcessStartInfo psi = new ProcessStartInfo();
            Process proc = new Process();
            psi.FileName = "nbtstat";
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = "-A   " + IP;
            psi.UseShellExecute = false;
            proc = Process.Start(psi);
            dirResults = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            dirResults = dirResults.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            Regex reg = new Regex("Mac[   ]{0,}Address[   ]{0,}=[   ]{0,}(?<key>((.)*?))__MAC", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match mc = reg.Match(dirResults + "__MAC");

            if (mc.Success)
            {
                return mc.Groups["key"].Value;
            }
            else
            {
                reg = new Regex("Host not found", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                mc = reg.Match(dirResults);
                if (mc.Success)
                {
                    return "Host not found!";
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region 上传服务器



        public static bool UploadFile(byte[] fs, string fileName)
        {
            try
            {
                ///定义并实例化一个内存流，以存放提交上来的字节数组。 
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。 

                FileStream f = new FileStream(fileName, FileMode.Create);
                ///把内内存里的数据写入物理文件 
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion
    }
}
