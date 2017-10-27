using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;

using Pro.Common;
using Pro.CoreModel;
using Pro.Web.CoreLogic;
using Pro.Base.Logs;

namespace Pro.Web.Common
{
    public abstract class BaseFrame : System.Web.UI.Page
    {
        #region 私有变量
        // for global
        private int _AppLine = 1;
        private int _AppFlag = -1;

        // for page state
        private bool _IsAjax = false;
        private string _Act = string.Empty;
        private int _PageIndex = -1;
        private int _PageSize = -1;
        private string _OrderStr = string.Empty;
        private string _PageJson = string.Empty;
        private string _topHtml = string.Empty;
        private string _g_HeadHtml = string.Empty;
        private string _btmHtml = string.Empty;
        private string _PageXml = string.Empty;

        // for user rights
        private UserInfo _UCInfo = null;
        private UserRequestInfo _URInfo;               // * 用户请求信息,为空代表用户没有当前页面的访问权限

        // for user configs
        private Dictionary<string, string> _SysCfg = null;
        private Dictionary<string, string> _UserCfg = null;
        private string _Skin = "default";

        /// <summary>
        /// 当前页面权限值
        /// </summary>
        private int _PermValue = 0;
        // objects
        RightSys rightSys = new RightSys();
        BaseSys baseSys = new BaseSys();
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
            set
            {
                Application["AppFlag"] = value;
                _AppFlag = value;
            }
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
        public string g_HeadHtml
        {
            get { return _g_HeadHtml; }
            set { _g_HeadHtml = value; }
        }
        public string g_BtmHtml
        {
            get { return _btmHtml; }
            set { _btmHtml = value; }
        }
        private bool _IsUnAuth;

        /// <summary>
        /// //屏蔽用户，自动认证
        /// </summary>
        public bool g_IsUnAuth
        {
            get { return _IsUnAuth; }
            set { _IsUnAuth = value; }
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
        

        // for user rights
        public UserInfo g_UCInfo
        {
            get { return _UCInfo; }
        }

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


        /// <summary>
        /// 当前页面权限值
        /// </summary>
        protected int PermValue
        {
            get
            {
                return _PermValue;
            }

            set
            {
                _PermValue = value;
            }

        }
        #endregion

        #region OnInit
        protected override void OnInit(EventArgs e)
        {
            //屏蔽用户，自动认证
            g_IsUnAuth = WebTools.GetRequestVal("from", string.Empty) == "unauth";

            //自动登录
            g_IsAutoLogin = WebTools.GetRequestVal("from1", string.Empty) == "autologin";
            if (g_IsAutoLogin)
            {
                RightSys rs = new RightSys();
                ReturnValue retVal = rs.Login("admin", "admin", 2628000, DateTime.Now);
            }

            ReadyPageState();

            if (g_IsUnAuth == false)
                ReadyUCInfo();

            ReadySysCfg();
            if (g_IsUnAuth == false)
                ReadyUserCfg();
            //ReadyURInfo();
            baseSys.GetCurrMng();

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

            Random rnd = new Random();
            Int32 rndNum = rnd.Next(100000000, 999999999);      //得1000000~9999999的随机数
            Session["SeqNo"] = rndNum;

            g_HeadHtml = "<meta http-equiv='X-UA-Compatible' content='IE=EmulateIE7' />";
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

        /// <summary>
        /// 确认UserRequestInfo
        /// </summary>
        //private void ReadyURInfo()
        //{
        //    ReturnValue ret_URInfo = rightSys.CheckUserRequestInfo(g_UCInfo.UserId, g_AppFlag, g_IsAjax);
        //    if (!ret_URInfo.IsSuccess)
        //        Err("系统错误: 用户请求信息获取失败. ");
        //    g_URInfo = (UserRequestInfo)ret_URInfo.RetObj;
        //    if (g_URInfo.Perms == -1)
        //        Err("您无权查看当前请求的页面. 请向管理员咨询. ");

        //    this._PermValue = g_URInfo.Perms;
        //}

        /// <summary>
        /// 确认UserCookieInfo
        /// </summary>
        private bool ReadyUCInfo()
        {
            bool result = true;
            ReturnValue ret_UCInfo = rightSys.GetUserCookieInfo();
            if (!ret_UCInfo.IsSuccess)
            {
                NoLogin();
                return result;
            }
            _UCInfo = ret_UCInfo.RetObj as UserInfo;
            return result;
        }

        /// <summary>
        /// 确认系统配置信息
        /// </summary>
        private void ReadySysCfg()
        {
            SysCfgInfo info = new SysCfgInfo();
            info.AppFlag = g_AppLine;

            ReturnValue ret_SysCfg = baseSys.GetSysCfgs(info);
            if (!ret_SysCfg.IsSuccess)
                Err(ret_SysCfg.RetMsg);
            _SysCfg = (Dictionary<string, string>)ret_SysCfg.RetObj;
        }

        /// <summary>
        /// 确认用户配置信息
        /// </summary>
        private void ReadyUserCfg()
        {
            UserCfgInfo info = new UserCfgInfo();
            info.AppFlag = g_AppFlag;
            info.UserId = g_UCInfo.UserId;

            ReturnValue ret_UserCfg = baseSys.GetUserCfgs(info);
            if (ret_UserCfg.IsSuccess)
                return;
            _UserCfg = (Dictionary<string, string>)ret_UserCfg.RetObj;
        }
        /// <summary>
        /// 给该模块的TopHtml赋值
        /// </summary>
        protected void ReadyTopHtml()
        {
            string strDesc = "首页";
            ReturnValue ret_parentModule = rightSys.GetParentModules(g_URInfo.ModuleId, g_AppFlag);
            if (!ret_parentModule.IsSuccess)
                Err("系统错误: 权限树获取失败! ");
            DataTable dtModule = ret_parentModule.RetDt;
            for (int i = dtModule.Rows.Count - 1; i > 0; i--)
                strDesc += string.Format(" > {0}", dtModule.Rows[i]["MODULENAME"].ToString());
            g_TopHtml = GetTopHtml(g_URInfo.ModuleName, strDesc);
        }

        /// <summary>
        /// 给该模块的BtmHtml赋值
        /// </summary>
        protected void ReadyBtmHtml()
        {
            g_BtmHtml = "<!--<script language='javascript' type='text/javascript'></script><br/>-->";
        }
        /// <summary>
        /// 组合TopHtml
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static string GetTopHtml(string title, string desc)
        {
            title = Tools.GetString(title, "");
            desc = Tools.GetString(desc, "");
            return "<script>SetNav('" + title + "','" + desc + "');</script>";
        }
        #endregion

        #region 获取当前post数据
        /// <summary>
        /// 获取当前post数据
        /// </summary>
        /// <returns></returns>
        protected string GetPostInfo()
        {
            string ret = string.Empty;
            if (Request.InputStream == null || Request.InputStream.Length == 0)
            {
                return ret;
            }
            StreamReader sr = new StreamReader(Request.InputStream);
            ret = sr.ReadToEnd();
            sr.Close();
            return ret;
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
                if (retCode == -3)
                {
                    string login = "nologin";
                    Response.Write(login);
                    //Response.Close();
                    Response.End();
                    return;
                }
                //  Response.Redirect("/prowebui/login.aspx?type=-3&msg=" + retMsg, true);
            }
            else if (Request.RawUrl.ToLower() == "/prowebui/default.aspx" && retCode == -3)
                Response.Redirect("/prowebui/login.aspx?type=-3&msg=" + retMsg, true);
            else
                Response.Redirect("/prowebui/errpage.aspx?type=" + retCode + "&msg=" + retMsg, true);
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


    }
}
