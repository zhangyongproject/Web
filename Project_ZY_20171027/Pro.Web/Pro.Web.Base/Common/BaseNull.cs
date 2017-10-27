using System;
using System.Collections.Generic;
using System.Text;


using Pro.Common;
using Pro.CoreModel;
using Pro.Web.CoreLogic;
using Pro.Base.Logs;
using System.Web;
using System.IO;

namespace Pro.Web.Common
{
    public class BaseNull : System.Web.UI.Page
    {
        #region 私有变量
        // for global
        private int _AppLine = 1;
        private int _AppFlag = 10;

        // for page state
        private bool _IsAjax = false;
        private string _Act = string.Empty;
        private int _PageIndex = -1;
        private int _PageSize = -1;
        private string _OrderStr = string.Empty;
        private string _PageJson = string.Empty;
        private string _topHtml = string.Empty;
        private string _btmHtml = string.Empty;
        private string _PageXml = string.Empty;

        // for user rights
        //private UserInfo _UCInfo = null;

        // for user configs
        private Dictionary<string, string> _SysCfg = null;
        private Dictionary<string, string> _UserCfg = null;
        private string _Skin = "default";
        private UserRequestInfo _URInfo;               // * 用户请求信息,为空代表用户没有当前页面的访问权限
        // objects
        //RightSys rightSys = new RightSys();
        //BaseSys baseSys = new BaseSys();
        #endregion

        #region 公共变量
        // for global
        public int g_AppLine
        {
            get { return _AppLine; }
        }
        protected int g_AppFlag
        {
            get { return _AppFlag; }
        }
        public string g_MngCode
        {
            get { return string.Empty; }
        }
        public string g_PageXml
        {
            get { return _PageXml; }
            set { _PageXml = value; }
        }

        // for page state
        public bool g_IsAjax
        {
            get { return _IsAjax; }
        }
        public string g_Act
        {
            get { return _Act; }
        }
        public int g_PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }
        public int g_PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
        public string g_OrderStr
        {
            get { return _OrderStr; }
        }
        public string g_PageJson
        {
            get { return _PageJson; }
            set { _PageJson = value; }
        }
        public string g_TopHtml
        {
            get { return _topHtml; }
            set { _topHtml = value; }
        }
        public string g_BtmHtml
        {
            get { return _btmHtml; }
            set { _btmHtml = value; }
        }

        private bool _IsAutoLogin;
        /// <summary>
        /// //自动登录
        /// </summary>
        public bool g_IsAutoLogin
        {
            get { return _IsAutoLogin; }
            set { _IsAutoLogin = value; }
        }

        private string _UserType;

        /// <summary>
        /// 用户类型
        /// </summary>
        public string g_UserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        //// for user rights
        //public UserInfo g_UCInfo
        //{
        //    get { return _UCInfo; }
        //}


        public UserRequestInfo g_URInfo
        {
            get { return _URInfo; }
            set { _URInfo = value; }
        }

        // for user configs
        public Dictionary<string, string> g_SysCfg
        {
            get { return _SysCfg; }
        }
        public Dictionary<string, string> g_UserCfg
        {
            get { return _UserCfg; }
        }
        public string g_Skin
        {
            get { return _Skin; }
        }
        #endregion

        #region OnInit
        protected override void OnInit(EventArgs e)
        {
            ReadyPageState();
            //自动登录
            g_IsAutoLogin = WebTools.GetRequestVal("from1", string.Empty) == "autologin";
            g_UserType = WebTools.GetRequestVal("usertype", "admin");
            if (g_IsAutoLogin)
            {
                //RightSys rs = new RightSys();
                //ReturnValue retVal = rs.Login(g_UserType, "admin", 2628000, DateTime.Now);
            }
            

            if (!g_IsAjax)
            {
                if (WebTools.GetCookieVal("testSkin", string.Empty) != string.Empty)                   // 用于测试的皮肤
                    _Skin = WebTools.GetCookieVal("testSkin", string.Empty);
                else if (WebTools.GetCookieVal("tempSkin", string.Empty) != string.Empty)              // 用于修改数据库后即时修改页面
                    _Skin = WebTools.GetCookieVal("tempSkin", string.Empty);
                else if (g_UserCfg != null && g_UserCfg.ContainsKey("skin"))    // 用户自定义的皮肤
                    _UserCfg.TryGetValue("skin", out _Skin);
                else if (g_SysCfg != null && g_SysCfg.ContainsKey("skin"))      // 系统默认的皮肤
                    _SysCfg.TryGetValue("skin", out _Skin);
            }
        }

        /// <summary>
        /// 准备页面与请求状态
        /// </summary>
        private void ReadyPageState()
        {
            _Act = WebTools.GetRequestVal("act", string.Empty).ToLower();
            if (_Act != string.Empty)
                _IsAjax = true;

            _PageIndex = Tools.GetInt32(WebTools.GetRequestVal("pi", "-1"), -1);
            _PageSize = Tools.GetInt32(WebTools.GetRequestVal("ps", "-1"), -1);
            _OrderStr = Tools.GetString(WebTools.GetRequestVal("os", string.Empty), string.Empty);
        }
   
        #endregion

        #region 页面请求处理
        /// <summary>
        /// 请求的返回函数
        /// </summary>
        /// <param name="retCode">返回代码:-9:异常,-5:没有权限,-3:未登陆,-1:没有数据,0:执行成功</param>
        /// <param name="retMsg">返回信息:字符串(如果是Json字符串自动解析)</param>
        public void Ret(int retCode, string retMsg)
        {
            if (g_IsAjax)
            {
                // Response.Write(Json.WriteRaw(retCode, retMsg));
                if (retCode == -3)
                {
                    string login = "nologin";
                    Response.Write(login);
                    Response.End();
                    return;
                }
            }
            else if (Request.RawUrl.ToLower() == "/prowebequactive/default.aspx" && retCode == -3)
            {
                Response.Redirect("/prowebequactive/login.aspx?type=-3&msg=" + retMsg, true);
                LogMessageHelper.LogINFO("尝试访问未登录系统。");
            }
            else
            {
                Response.Redirect("/prowebequactive/errpage.aspx?type=" + retCode + "&msg=" + retMsg, true);
            }
            Response.Flush();
            Response.Clear();
        }

        /// <summary>
        /// 请求的返回函数
        /// </summary>
        /// <param name="retVal">返回内容</param>
        public void Ret(string retVal)
        {
            Ret(0, retVal);
        }

        /// <summary>
        /// 错误处理：未登录
        /// </summary>
        public void NoLogin()
        {
            Ret(-3, "未登录或登录己超时。");
        }

        /// <summary>
        /// 错误处理：权限不足
        /// </summary>
        public void NoRight()
        {
            Ret(-5, "没有足够的权限完成当前操作。");
        }

        /// <summary>
        /// 错误处理：其它错误
        /// </summary>
        /// <param name="errMsg"></param>
        public void Err(string errMsg)
        {
            Ret(-1, errMsg);
        }

        /// <summary>
        /// 获取AJAX请求字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRequestStr(HttpRequest request)
        {
            string ret = string.Empty;
            if (request.InputStream == null || request.InputStream.Length == 0)//|| request.InputStream.Length == 0
            {
                return ret;
            }
            StreamReader sr = new StreamReader(request.InputStream);
            ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }
        /// <summary>
        /// 获取AJAX请求字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRequestStr()
        {
            HttpRequest request = HttpContext.Current.Request;
            return GetRequestStr(request);
        }
        #endregion

        #region 执行脚本
        /// <summary>
        /// 基于Page对象执行一段JS脚本(ClientScript.RegisterStartupScript)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="script"></param>
        public void ExecScript(string script)
        {
            string rdnum = new Random().Next().ToString();
            Page.ClientScript.RegisterStartupScript(this.Page.GetType(), rdnum, script, true);
        }
        #endregion

        #region 添加日志
        /// <summary>
        /// 添加日志(日志分类,后续将由程序自动获取)
        /// </summary>
        /// <param name="logType">日志类型（LogType.Error=错误，LogType.Warn=警告，LogType.Info=信息）</param>
        /// <param name="logClass">日志分类（可以写入具体的模块名）</param>
        /// <param name="subClass">日志小类 (可以写入具体的方法名)</param>
        /// <param name="logDesc">日志描述</param>
        protected void AddLog(LogType logType, string logClass, string subClass, string logDesc)
        {
            
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logType">日志类型（LogType.Error=错误，LogType.Warn=警告，LogType.Info=信息）</param>
        /// <param name="subClass">日志小类 (可以写入具体的方法名)</param>
        /// <param name="logDesc">日志描述</param>
        protected void AddLog(LogType logType, string subClass, string logDesc)
        {
            AddLog(logType, "WebEquActive", subClass, logDesc);
        }
        #endregion
    }

}
