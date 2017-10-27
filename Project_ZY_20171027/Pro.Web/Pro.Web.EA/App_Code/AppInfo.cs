using System.Web;
using System.Collections;
using Pro.Common;
using System.Web.SessionState;
using System;
using System.Xml;
using Pro.Common;

/// <summary>
///AppInfo 的摘要说明
/// </summary>
public class AppInfo
{
    #region 版本信息

    /// <summary>
    /// 系统名称
    /// </summary>
    static public string AppTitle = Tools.GetAppSetting("AppTitle", "（系统标题）");

    #region 版本记录

    public const string AppVersion = "V1.2017.1005.1629";
    #endregion

    #endregion

    #region 私有变量

    private static HttpApplicationState Application;

    #endregion

    #region 全局变量

    //数据连接串可以通过web.config中的配置生成
    //...其他库...

    /// <summary>
    /// 模块配置文件
    /// </summary>
    static public XmlDocument xmlSiteMap;
    static public XmlDocument xmlMenu;

    /// <summary>
    /// 模块信息
    /// </summary>
    static public Hashtable htModuleInfo;

    #endregion

    #region 初始化

    static public void InitApplication(HttpApplicationState CurApplication)
    {
        Application = CurApplication;
        HttpServerUtility Server = HttpContext.Current.Server;

        xmlSiteMap = new XmlDocument();
        string mapfile = Server.MapPath("~/app_data/sitemap.xml");
        xmlSiteMap.Load(mapfile);

        htModuleInfo = new Hashtable();

        xmlMenu = MyXml.CreateXml();
        InitSiteMap();
    }

    static private void InitSiteMap()
    {
        XmlNode rootNode = xmlSiteMap.SelectSingleNode("siteMap");
        XmlNode rootNode1 = xmlMenu.SelectSingleNode("xml");
        InitSiteNode(rootNode);
        CopyNode(rootNode, rootNode1);
    }

    private static void CopyNode(XmlNode srcNode, XmlNode tgtNode)
    {
        string DefaultWebServer = MyConfig.GetWebConfig("webipaddr", "localhost");
        foreach (XmlNode subNode in srcNode.ChildNodes)
        {
            if (subNode.Name != "siteMapNode") continue; //只处理siteMapNode结点

            XmlNode newNode = MyXml.AddXmlNode(tgtNode, subNode.Name);
            foreach (XmlAttribute attr in subNode.Attributes)
            {
                switch (attr.Name.ToLower())
                { 
                    case "url":
                    case "title":
                    case "right":
                    case "icon":
                    case "style": //菜单样式 1，2，3
                        //以上结点可以被传向客户端，可以扩充

                        //url内容可以进行字符串替换
                        string value = attr.Value;
                        if (attr.Name.ToLower().Equals("url"))
                        {
                            value = value.Replace("[webipaddr]", DefaultWebServer);
                        }
                        MyXml.AddAttribute(newNode, attr.Name, value);
                        break;
                }
            }
            CopyNode(subNode, newNode);
        }
    }

    static private void InitSiteNode(XmlNode siteNode)
    {
        foreach (XmlNode subNode in siteNode.ChildNodes)
        {
            if (subNode.Name != "siteMapNode") continue; //只处理siteMapNode结点

            string url = MyXml.GetAttributeValue(subNode, "url");
            string urlflag = ParsePageFlag(url);
            MyXml.AddAttribute(subNode, "pageflag", urlflag);
            InitSiteNode(subNode);

            //不同子菜单项进行特殊处理
            string title = MyXml.GetAttributeValue(subNode, "title");
            switch (title)
            {
                default: break;
            }
        }
    }

    static public string ParsePageFlag(string url)
    {
        string[] sList = url.Split(new char[] { '?' })[0].Split(new char[] { '/', '.' });
        if (sList.Length >= 3)
            return string.Format("{0}/{1}", sList[sList.Length - 3], sList[sList.Length - 2]).ToLower();
        else
            return url.ToLower();
    }

    #endregion

    #region  SessionInfo

    public static void OnSessionStart(HttpSessionState Session)
    {
        //SessionInfo mSessionInfo = mIPProxy.CreateSessionInfo();
        SessionInfo mSessionInfo = new SessionInfo();
        Session["id"] = mSessionInfo;
        //MyLog.WriteLogInfo("OnSessionStart:" + mSessionInfo.LogID);
    }

    public static SessionInfo GetSessionInfo(HttpSessionState Session)
    {
        if ((Session != null) && (Session["id"] != null))
        {
            SessionInfo mSessionInfo = (SessionInfo)Session["id"];
            return mSessionInfo;
        }
        else
            return null;
    }

    public static void OnSessionEnd(HttpSessionState Session)
    {
        SessionInfo mSessionInfo = GetSessionInfo(Session);
        if (mSessionInfo != null)
        {
            //mIPProxy.RemoveSessionInfo(mSessionInfo.SessionID);
        }
    }

    #endregion

    #region 公共函数

    public static void ReturnMessage(HttpContext context, int mCode, string strMessage)
    {
        ReturnMessage(context, mCode, strMessage, "");
    }

    public static void ReturnMessage(HttpContext context, int mCode, string strMessage, string strValue)
    {
        context.Response.ContentType = "text/plain";
        string strResult = MyXml.CreateResultXml(mCode, strMessage, strValue).InnerXml;
        context.Response.Write(strResult);
    }

    public static string CreateVerifyCode(string strSource, SessionInfo IPApi)
    {
        return MySecurity.EnCodeByMD5(strSource + "-" + IPApi.LogID);
    }

    #endregion
}