using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Pro.Common
{
    /// <summary>
    /// 提供向 URI 标识的资源发送数据和从 URI 标识的资源接收数据的公共方法。
    /// </summary>
    public class WebClientHelper
    {
        private static WebClient webClient = new WebClient();
        private static string strResult = string.Empty;

        /// <summary>
        /// 指定 URI 的资源下载的数据打开一个可读的流
        /// </summary>
        /// <param name="Uri">指定 URI</param>
        /// <returns></returns>
        public static string OpenRead(string Uri)
        {
            webClient.Credentials = CredentialCache.DefaultCredentials;
            using (Stream stream = webClient.OpenRead(Uri))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (reader != null)
                    {
                        strResult = reader.ReadToEnd();
                    }
                    reader.Close();
                    stream.Close();
                }
                webClient.Dispose();
            }
            return strResult;
        }

        /// <summary>
        /// 指定 URI 的资源下载的数据打开一个可读的流(异步)
        /// </summary>
        /// <param name="Uri">指定 URI</param>
        /// <returns></returns>
        public static string OpenReadAsync(string Uri)
        {

            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCallback2);
            webClient.OpenReadAsync(new Uri(Uri));
            return strResult;

        }

        private static void OpenReadCallback2(Object sender, OpenReadCompletedEventArgs e)
        {
            Stream reply = null;
            StreamReader s = null;
            try
            {
                reply = (Stream)e.Result;
                s = new StreamReader(reply);
                strResult = s.ReadToEnd();
                //AppLog.Write(strResult, AppLog.LogMessageType.Error);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }

                if (reply != null)
                {
                    reply.Close();
                }
            }
        }
    }
}
