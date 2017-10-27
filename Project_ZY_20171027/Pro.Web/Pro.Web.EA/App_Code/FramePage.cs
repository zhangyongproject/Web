using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;
using Pro.Common;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Pro.CoreModel;
using Pro.Web.EALogic;
using System.Collections.Generic;

/// <summary>
/// TJ项目基础框架页面，实现登录检查/模块初始化/提供客户端用平台的操作函数接口
/// </summary>
public class FramePage : AjaxPage
{
    protected SessionInfo IPApi;
    protected ModuleInfo mModuleInfo;

    protected string UserType = string.Empty;
    protected string UserId = string.Empty;

    private const string pageLOGIN = "../login.aspx";
    private const string pageNOACCESS = "../T00Frame/noaccess.aspx";

    protected override void OnInit(EventArgs e)
    {
        #region 登录检查

        Session["LastPage"] = this.Request.Url.ToString();
        IPApi = AppInfo.GetSessionInfo(Session);
        if (IPApi == null)
        {
            MyLog.WriteLogInfo("no session!");
            Response.Redirect(pageLOGIN); //todo
        }
        //add by qiubaosheng  注销时情况默认连接
        if (this.Request.Url.ToString().IndexOf("Logout") > -1)
        {
            Session["LastPage"] = this.Request.Url.ToString().Substring(0, this.Request.Url.ToString().IndexOf('?'));
        }

        if (!IPApi.HasLogin)
        {
            Response.Redirect(pageLOGIN);
        }

        this.InitModuleInfo();
        //
        UserType = IPApi.UserRight.IsAdmin ? "0" : "1";
        UserId = IPApi.UserID;

        //检查当前页面权限
        string errMessage = "您无权访问该页面模块！";
        if (!CheckPageRight(ref errMessage))
        {
            this.InitModuleInfo();
            Session["LastModule"] = this.mModuleInfo;

            //todo:显示样式需要再丰富一些一点
            Session["ErrMessage"] = errMessage;
            Response.Redirect(pageNOACCESS); //todo
        }

        #endregion

        #region  响应Ajax请求

        //这个放在权限检查之后
        //如果有Ajax请求，则不会执行后面的写日志操作
        base.OnInit(e);

        #endregion

        #region 记录页面登录日志

        //设置页名与写日志
        //InitModuleInfo();
        WriteLogA("进入页面", mModuleInfo.ModuleName + " " + Request.Url.ToString(), "");
        this.Title = mModuleInfo.ModuleName;

        #endregion
    }

    #region 菜单初始化

    /// <summary>
    /// 设置页面模块信息
    /// 本函数提供通用计算方式
    /// 若有特殊情况，需要更改菜单项索信息，可在相关页面中进行重载
    /// </summary>
    protected virtual void InitModuleInfo()
    {
        string typeName = this.GetType().FullName;
        if (!AppInfo.htModuleInfo.Contains(typeName))
        {
            #region 检查对应的节点

            string strUrl = Request.Url.ToString();
            string urlflag0 = AppInfo.ParsePageFlag(strUrl);
            XmlNode rootNode = AppInfo.xmlSiteMap.SelectSingleNode("siteMap");
            string urlflag1 = "";
            int idx1 = 0;
            int idx2 = -1;
            int idx3 = -1;
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                for (int j = 0; j < rootNode.ChildNodes[i].ChildNodes.Count; j++)
                {
                    XmlNode node1 = rootNode.ChildNodes[i].ChildNodes[j];
                    urlflag1 = MyXml.GetAttributeValue(node1, "pageflag");
                    if (urlflag1 == urlflag0)
                    {
                        idx1 = i;
                        idx2 = j;
                        break;
                    }
                    for (int k = 0; k < rootNode.ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++)
                    {
                        XmlNode node2 = node1.ChildNodes[k];
                        urlflag1 = MyXml.GetAttributeValue(node2, "pageflag");
                        if (urlflag1 == urlflag0)
                        {
                            idx1 = i;
                            idx2 = j;
                            idx3 = k;
                            break;
                        }
                    }
                }
            }

            XmlNode mNode = rootNode.ChildNodes[idx1];
            if (idx2 > -1)
                mNode = mNode.ChildNodes[idx2];
            if (idx3 > -1)
                mNode = mNode.ChildNodes[idx3];

            #endregion

            #region 从结点中解析模块信息

            ModuleInfo m = new ModuleInfo();
            m.PageFlag = urlflag0;
            m.ModuleName = MyXml.GetAttributeValue(mNode, "title");
            m.ModuleDesc = MyXml.GetAttributeValue(mNode, "description");
            m.idxMenu1 = idx1;
            m.idxMenu2 = idx2;
            m.idxMenu3 = (idx3 > -1) ? 0 : -1;

            AppInfo.htModuleInfo.Add(typeName, m);

            #endregion
        }

        this.mModuleInfo = (ModuleInfo)AppInfo.htModuleInfo[typeName];
    }

    #endregion

    #region 管理权限

    protected bool IsAdmin
    {
        get
        {
            if (IPApi != null && IPApi.HasLogin)
                return IPApi.IsAdmin;
            else
                return false;
        }
    }

    public virtual string RightStr
    {
        get { return IsAdmin ? "true" : "false"; }
    }

    #endregion

    #region 页面权限检查，具体页面根据情况进行重载

    protected virtual bool CheckPageRight(ref string errMessage)
    {
        return true;
    }

    #endregion

    #region 公用函数(供AJAX调用)

    /// <summary>
    /// 系统登出
    /// </summary>
    /// <returns>系统登出是否成功 0/-1 成功/失败</returns>
    public int Logout()
    {
        Session["LastPage"] = null;
        return IPApi.Logout() ? 0 : -1;
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="oldPass">原密码</param>
    /// <param name="newPass">新密码</param>
    /// <returns>修改密码是否成功 0/-1 成功/失败</returns>
    public string ChangePass(string oldPass, string newPass)
    {
        UserLogic userLogin = new UserLogic();
        ReturnValue retVal = userLogin.GetUserById(new UserInfo() { UserName = IPApi.UserCode });
        if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(-9, Consts.EXP_Info, string.Empty).InnerXml; }
        UserInfo info = retVal.RetObj as UserInfo;
        if (oldPass != info.UserPwd) { return MyXml.CreateResultXml(-8, "原密码错误", string.Empty).InnerXml; }
        retVal = userLogin.UpdatePassWord(new UserInfo() { UserPwd = newPass, UserID = info.UserID });
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

    public string GetMenuInfo()
    {
        InitModuleInfo();
        XmlNode rootNode = AppInfo.xmlMenu.SelectSingleNode("xml");
        MyXml.AddAttribute(rootNode, "idxMenu1", (mModuleInfo != null) ? mModuleInfo.idxMenu1 : -1);
        MyXml.AddAttribute(rootNode, "idxMenu2", (mModuleInfo != null) ? mModuleInfo.idxMenu2 : -1);
        MyXml.AddAttribute(rootNode, "idxMenu3", (mModuleInfo != null) ? mModuleInfo.idxMenu3 : -1);
        MyXml.AddAttribute(rootNode, "name", (mModuleInfo != null) ? mModuleInfo.ModuleName : "nonamed");
        MyXml.AddAttribute(rootNode, "username", IPApi.UserName);
        MyXml.AddAttribute(rootNode, "usercall", IPApi.UserCall);
        //一个副本
        XmlNode xmlDoc = AppInfo.xmlMenu.Clone();
        //权限控制
        if (IsAdmin == false)
        {
            rootNode = xmlDoc.SelectSingleNode("xml");
            List<XmlNode> lstFirstSiteMap = new List<XmlNode>();
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                int nodeCnt = node.ChildNodes.Count;
                List<XmlNode> lstSiteMap = new List<XmlNode>(nodeCnt);
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    if (subNode.Attributes["right"].Value == "1") { continue; }
                    lstSiteMap.Add(subNode);
                }
                foreach (XmlNode delNode in lstSiteMap)
                {
                    node.RemoveChild(delNode);
                }
                //删除的个数跟该节点子节数量一致时，一级菜单也删除
                if (lstSiteMap.Count != nodeCnt) { continue; }
                lstFirstSiteMap.Add(node);
            }
            foreach (XmlNode node in lstFirstSiteMap)
            {
                rootNode.RemoveChild(node);
            }

            //
            XmlNodeList list = rootNode.SelectNodes("siteMapNode/siteMapNode");

            switch (mModuleInfo.ModuleName)
            {
                case "定时启动记录":
                    rootNode.Attributes["idxMenu1"].Value = "1";
                    break;
                default:
                    break;
            }
        }

        return xmlDoc.InnerXml;
    }

    /// <summary>
    /// 心跳包处理(ipapi定时自动向webservice向送心跳，此处只处理webclient向本webserver发送的心跳)
    /// todo:保留返回消息
    /// </summary>
    /// <returns></returns>
    public string HeartBeat()
    {
        XmlDocument xmlDoc = MyXml.CreateResultXml(mTestFlag++, "保留", string.Empty);
        return xmlDoc.InnerXml; // IPApi.HeartBeat(5) ? 1 : 0;
    }
    private int mTestFlag = 0;

    #endregion

    #region 公用函数

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="opType"></param>
    /// <param name="strInfo"></param>
    /// <param name="dataId"></param>
    protected void WriteLogA(string opType, string strInfo, string dataId)
    {
        IPApi.WriteLog(this.mModuleInfo.ModuleName, opType, strInfo, dataId);
    }


    public string GetVerifyCode(string blobid)
    {
        return AppInfo.CreateVerifyCode(blobid, IPApi);
    }

    #endregion
}
