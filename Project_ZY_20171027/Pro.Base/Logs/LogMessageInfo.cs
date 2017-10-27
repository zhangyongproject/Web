using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

using Pro.Common;
using System.Net;

namespace Pro.Base.Logs
{
    /// <summary>
    /// 日志信息(Web)
    /// </summary>
    [Serializable]
    public class LogMessageInfo
    {

        public LogMessageInfo()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null
                     && HttpContext.Current.Session["ModuleName"] != null)
                {
                    _LogClass = HttpContext.Current.Session["ModuleName"].ToString();
                }

                if (HttpContext.Current.Session != null
                    && HttpContext.Current.Session["SeqNo"] != null)
                {
                    _BusinessSeqNo = HttpContext.Current.Session["SeqNo"].ToString();
                }
                _URL = HttpContext.Current.Request.Url.ToString();

                _Ip = HttpContext.Current.Request.UserHostAddress;


                if (HttpRuntime.Cache[Consts.CacheKey_CurMngCode] != null)
                    _MNGCode = HttpRuntime.Cache[Consts.CacheKey_CurMngCode].ToString();


                if (HttpContext.Current.Application["AppFlag"] != null)
                {
                    _appflag = HttpContext.Current.Application["AppFlag"].ToString();
                }

                if (HttpContext.Current.Request.Cookies["UserName"] != null
                    && HttpContext.Current.Request.Cookies["UserId"] != null)
                {
                    //_UserName = string.Format("{0}({1})",
                    //    HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["UserName"].Value),
                    //     HttpContext.Current.Request.Cookies["UserId"].Value);

                    _UserName = string.Format("{0}/{1}/{2}",
    HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["UserName"].Value),
     HttpContext.Current.Request.Cookies["UserId"].Value, _Ip);
                }
            }

            _logfrom = Tools.GetProjectVer("Pro");
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


        #region 自动获取属性
        private string _BusinessSeqNo = string.Empty;
        /// <summary>
        /// 获取业务流水号
        /// </summary>
        public string SeqNo
        {
            get
            {
                return _BusinessSeqNo;
            }
            //set { _BusinessSeqNo = value; }
        }

        private string _URL = string.Empty;
        /// <summary>
        /// 获取请求的URL
        /// </summary>
        public string URL
        {
            get
            {
                return _URL;
            }
        }

        private string _Ip = string.Empty;
        /// <summary>
        /// 获取客户端ＩＰ地址
        /// </summary>
        public string IP
        {
            get
            {
                return _Ip;
            }
        }
        private string _MNGCode = string.Empty;
        /// <summary>
        /// 获取或设置机构代码
        /// </summary>
        public string MNGCode
        {
            get { return _MNGCode; }
        }
        private string _appflag = string.Empty;
        /// <summary>
        /// 应用程序标识
        /// </summary>
        public string AppFlag
        {
            get { return _appflag; }
        }

        private string _UserName = string.Empty;
        /// <summary>
        /// 获取或设置系统登陆用户(id/name)
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
        }

        private string _logfrom = string.Empty;
        /// <summary>
        /// 获取或设置系统登陆用户(id/name)
        /// </summary>
        public string LogFrom
        {
            get { return _logfrom; }
        }
        #endregion



        private LogLevel _Level = LogLevel.INFO;
        /// <summary>
        /// 获取或设置日志级别(默认为info)
        /// </summary>
        public LogLevel Level
        {

            set
            {
                _Level = value;
            }
        }

        public string LevelLog
        {
            get
            {
                string ret = string.Empty;
                switch (_Level)
                {
                    case LogLevel.ERROR:
                        ret = "1";
                        break;
                    case LogLevel.INFO:
                        ret = "3";
                        break;
                    case LogLevel.DEBUG:
                        ret = "2";
                        break;
                    default:
                        ret = "0";
                        break;
                }
                return ret;

            }
        }



        //private string _Source = string.Empty;
        ///// <summary>
        ///// 获取或设置日志信息来源(业务系统名称)
        ///// </summary>
        //public string Source
        //{
        //    get { return _Source; }
        //    set { _Source = value; }
        //}
        private string _LogClass = string.Empty;
        /// <summary>
        /// 获取或设置日志分类(具体的模块名)
        /// </summary>
        public string LogClass
        {
            get { return _LogClass; }
            set { _LogClass = value; }
        }

        private string _subClass = string.Empty;
        /// <summary>
        /// 获取或设置日志小类(全路径类或页面地址或方法名)
        /// </summary>
        public string SubClass
        {
            get { return _subClass; }
            set { _subClass = value; }
        }

        public string _InvokeDesc = string.Empty;
        /// <summary>
        /// 日志调用方方法的描述
        /// </summary>
        public string CallDesc
        {
            get { return _InvokeDesc; }
            set { _InvokeDesc = value; }
        }
        private string _ParamDesc = string.Empty;
        /// <summary>
        /// 获取或设置参数信息描述
        /// </summary>
        public string ParamDesc
        {
            get { return _ParamDesc; }
            set { _ParamDesc = value; }
        }

        public string _ResultDesc = string.Empty;
        /// <summary>
        /// 获取或设置返回值信息描述
        /// </summary>
        public string ResultDesc
        {
            get { return _ResultDesc; }
            set { _ResultDesc = value; }
        }


        private string _Details = string.Empty;
        /// <summary>
        ///  获取或设置日志详细信息描述
        /// </summary>
        public string Details
        {
            get { return _Details; }
            set { _Details = value; }
        }

        private string _errorMsg = string.Empty;
        /// <summary>
        /// 异常信息描述
        /// </summary>
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { _errorMsg = value; }
        }
    }
}
