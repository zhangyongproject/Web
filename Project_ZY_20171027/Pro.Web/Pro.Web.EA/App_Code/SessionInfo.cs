using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Pro.Common;
using System.Collections;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using AccessService;
using Pro.Web.Common;
using Pro.CoreModel;

public class SessionInfo
{
    public SessionInfo()
    {
        //mProxy = _Proxy;
        //ID = Guid.NewGuid().ToString("N");
        this.mLogID = ""; //尚未登录
        this.mUserRight = new UserRight();
    }

    #region 公开变量

    //private string ID;
    //public string SessionID
    //{
    //    get
    //    {
    //        return ID;
    //    }
    //}

    /// <summary>
    /// 当前客户端是否已登录
    /// </summary>
    public bool HasLogin
    {
        get
        {
            return !LogID.Equals(string.Empty);
        }
    }

    public string LogID { get { return mLogID; } }
    public string UserID { get { return mUserID; } }
    public string UserCode { get { return mUserCode; } }
    public string UserName { get { return mUserName; } }
    public string UserCall { get { return mUserCall; } }
    //public string SMID { get { return mSMID; } }
    public string SMCode { get { return mSMCode; } }
    public UserRight UserRight { get { return mUserRight; } }

    /// <summary>
    /// 是否管理员
    /// </summary>
    public bool IsAdmin { get { return mUserRight.IsAdmin; } }

    #endregion

    #region 私有变量

    //private IPProxy mProxy;
    private string mLogID = string.Empty;
    private string mUserID = "";
    private string mUserCode = "";
    private string mUserName = "";
    private string mUserCall = "";
    private string mSMCode = "";
    private UserRight mUserRight;

    #endregion

    //#region 调用WebService的通用参数准备

    ////这几个参数是用于解释webService返回结果的
    ////没有实际意义 
    //private int __mCode;
    //private string __strMessage;
    //private string __strResult;
    //private string __strReserved;

    //private void InitWebServiceParam()
    //{
    //    __mCode = 0;
    //    __strMessage = "";
    //    __strResult = "";
    //    __strReserved = "";
    //}

    //#endregion

    #region (sysService)方法封装

    //public bool InitSystemData()
    //{
    //    //if (!this.HasLogin)
    //    //    return false;

    //    //InitWebServiceParam();
    //    //string strResultXml = mProxy.sysService.InitSystemData(this.LogID, __strReserved);
    //    //return MyXml.ParseResultOK(strResultXml, ref __mCode, ref __strMessage, ref __strResult);
    //}

    /// <summary>
    /// 系统登录
    /// </summary>
    /// <param name="userCode">用户代码，也可以是用户名，即用户可见的标识用户身份的字符串（但系统内部不用此字段作标识）</param>
    /// <param name="userPassword">用户密码</param>
    /// <param name="clientIP">客户端IP</param>
    /// <param name="strMessage">如登录不成功，返回出错信息</param>
    /// <returns>登录是否成功</returns>
    public bool Login(string userCode, string userPassword, string clientIP, ref string strMessage)
    {
        if (this.HasLogin)
        {
            strMessage = "current connecttion has logged in.";
            return false;
        }
        Dictionary<string, string> dicParam = new Dictionary<string, string>(2);
        dicParam["username"] = userCode;
        dicParam["password"] = userPassword;
        AccessService.AccessServiceSoapClient access = new AccessServiceSoapClient();
        string resultjson = access.Login(Json.Dictionary2Json(dicParam));
        Dictionary<string, object> dicResult = MyJson.JsonToDictionaryObjValue(resultjson);
        bool checkpass = false;
        if (Tools.GetInt32(dicResult["result"], -9) == 1)
        {
            checkpass = true;
        }
        if (!checkpass)
        {
            strMessage = dicResult["resulttext"].ToString();
            return false;
        }
        //获取用户信息及权限
        Pro.CoreModel.ReturnValue retVal = new Pro.Web.EALogic.UserLogic().GetUserById(new Pro.CoreModel.UserInfo() { UserName = userCode });
        if (retVal.IsSuccess == false)
        {
            strMessage = dicResult["resulttext"].ToString();
            return false;
        }
        UserInfo info = retVal.RetObj as UserInfo;

        this.mLogID = Guid.NewGuid().ToString("N");
        this.mUserCode = userCode;// 
        this.mUserName = userCode;// 
        this.mUserCall = mUserCall.Equals(string.Empty) ? mUserName : info.UserNick;
        this.mUserID = info.UserID.ToString();//
        this.mUserRight = new UserRight(info.UserType.ToString());
        this.mUserRight.IsAdmin = info.UserType == 0;

        return true;
    }

    //public XmlDocument GetUserInfo()
    //{
    //    //InitWebServiceParam();
    //    //string strResultXml = mProxy.sysService.GetUserInfo(LogID, __strReserved);

    //    //XmlDocument xmlDoc = MyXml.LoadXml(strResultXml);
    //    //if (MyXml.ParseResultOK(xmlDoc))
    //    //{
    //    //    XmlNode xmlRoot = xmlDoc.SelectSingleNode("xml");
    //    //    mUserID = xmlRoot.SelectSingleNode("id").InnerText;
    //    //    mUserCode = xmlRoot.SelectSingleNode("code").InnerText;
    //    //    mUserName = xmlRoot.SelectSingleNode("name").InnerText;
    //    //    mSMID = xmlRoot.SelectSingleNode("smid").InnerText;
    //    //    mSMCode = xmlRoot.SelectSingleNode("smcode").InnerText;

    //    //    mIsAdmin = (xmlRoot.SelectSingleNode("IsAdmin").InnerText == "1");
    //    //}

    //    //return xmlDoc;

    //    //todo:
    //    return null;
    //}

    ///// <summary>
    ///// 生成授权码（单点登录使用）
    ///// </summary>
    ///// <param name="appCode">新打开的子系统Code</param>   
    ///// <returns>value=GrantKey</returns>
    //public string CreateGrantKey(string appCode)
    //{
    //    if (!this.HasLogin)
    //        return "";

    //    string strResultXml = mProxy.sysService.CreateGrantKey(this.LogID, appCode, __strReserved);
    //    if (!MyXml.ParseResultOK(strResultXml, ref __mCode, ref __strMessage, ref __strResult))
    //    {
    //        MyLog.WriteLogInfo(string.Format("CreateGrantKey error:{0} {1}", __mCode, __strMessage));
    //    }
    //    return __strResult;
    //}

    ///// <summary>
    ///// 授权方式登录
    ///// </summary>
    ///// <param name="grantKey">授权码</param>
    ///// <param name="appCode">子系统Code</param>
    ///// <returns>登录是否成功</returns>
    //public bool LoignByGrantKey(string grantKey, ref string strMessage)
    //{
    //    if (this.HasLogin)
    //    {
    //        strMessage = "current connecttion has logged in.";
    //        return false;
    //    }

    //    InitWebServiceParam();
    //    string strResultXml = mProxy.sysService.LoignByGrantKey(grantKey, mProxy.AppCode, __strReserved);
    //    if (MyXml.ParseResultOK(strResultXml, ref __mCode, ref strMessage, ref __strResult))
    //    {
    //        this.mLogID = __strResult;
    //        GetUserInfo();
    //        return true;
    //    }
    //    else
    //    {
    //        MyLog.WriteLogInfo(string.Format("LoignByGrantKey error:{0} {1}", __mCode, __strMessage));
    //        return false;
    //    }
    //}

    /// <summary>
    /// 注销
    /// </summary>
    /// <returns>注销是否成功</returns>
    public bool Logout()
    {
        if (!this.HasLogin)
            return false;

        //string strResultXml = mProxy.sysService.Logout(this.LogID, __strReserved);
        //if (MyXml.ParseResultOK(strResultXml))
        //{
        //    //说明，登录注销后，application中的loglist并未清除本loginfo类，
        //    //只有session_end时，才会触发该清除操作
        //    //而用户logout后，有可能继续login，
        //    this.mLogID = string.Empty;
        //    return true;
        //}
        //else
        //    return false;

        //todo
        mLogID = string.Empty;
        return true;
    }

    /// <summary>
    /// 保持在线(代理类调用，不公开)
    /// </summary>
    /// <param name="interval">心跳有效期（5-120）分钟之间，表示本次登录在线状态可保持多长时间。执行任何服务端调用均产生心跳效果</param>
    //internal bool HeartBeat(int interval)
    //{
    //    if (!this.HasLogin)
    //        return false;

    //    string strResultXml = mProxy.sysService.HeartBeat(this.LogID, interval, __strReserved);
    //    return MyXml.ParseResultOK(strResultXml);
    //}

    /// <summary>
    /// 获取登录用户所拥有的本系统权限
    /// </summary>
    /// <returns>xmlDocument</returns>
    //public XmlDocument GetUserRight()
    //{
    //    //if (!this.HasLogin)
    //    //    return MyXml.CreateResultXml(1, "has not logged in.", "");

    //    //string strResultXml = mProxy.sysService.GetUserRight(this.LogID, __strReserved);
    //    //XmlDocument xmlDoc = MyXml.LoadXml(strResultXml);

    //    //htRight = new Dictionary<string, int>(); //重置权限缓存
    //    //if (MyXml.ParseResultOK(xmlDoc))
    //    //{
    //    //    XmlNode rootNode = xmlDoc.SelectSingleNode("xml");
    //    //    foreach (XmlNode itemNode in rootNode.ChildNodes)
    //    //    {
    //    //        string strKey = MyXml.GetAttributeValue(itemNode, "key");
    //    //        string mValue = MyXml.GetAttributeValue(itemNode, "value");
    //    //        htRight.Add(strKey, MyType.ToInt(mValue));
    //    //    }
    //    //}

    //    //return xmlDoc;

    //    //todo
    //}

    //private Dictionary<string, int> htRight = null; //权限缓存信息

    ///// <summary>
    ///// 直接按权限项取权限值（说明：用了缓存，可能会出现权限结果更新不及时的情况）
    ///// 直接调用一次GetUserRight（）不带参数，即可更新缓存
    ///// </summary>
    ///// <param name="strRightCode"></param>
    ///// <returns></returns>
    //public int GetUserRight(string strRightCode)
    //{
    //    if (htRight == null)
    //    {
    //        //GetUserRight();
    //        //todo
    //        htRight = new Dictionary<string, int>();
    //    }

    //    if (!htRight.ContainsKey(strRightCode))
    //        return 0;
    //    else
    //        return htRight[strRightCode];
    //}


    /// <summary>
    /// 添加操作日志信息(只记录操作日志，不建议把系统异常信息记录到这里)
    /// </summary>
    /// <param name="strModule">模块名（用户管理...)</param>
    /// <param name="opType">操作类型（增、删、改、中频测量....）</param>
    /// <param name="strInfo">操作内容</param>
    /// <param name="dataId">相关记录表id</param>
    /// <returns>当前日志ID,空字串表示插入记录失败</returns>
    public string WriteLog(string strModule, string opType, string strInfo, string dataId)
    {
        if (!this.HasLogin)
            return "";

        //string strResultXml = mProxy.sysService.WriteLog(this.LogID, strModule, 0, opType, strInfo, dataId, __strReserved);
        //if (!MyXml.ParseResultOK(strResultXml, ref __mCode, ref __strMessage, ref __strResult))
        //{
        //    MyLog.WriteLogInfo(string.Format("WriteLog error:{0} {1}", __mCode, __strMessage));
        //}
        //return __strResult;

        //todo:
        return "";
    }

    ///// <summary>
    ///// 更新日志（结束时间），这个主要用于一段持续时间比较长的操作
    ///// 例如：进行一段测量，在开始测量时，生成一条日志记录，结束时，更新此记录，补记上结束时间
    ///// </summary>
    ///// <param name="sllID">日志记录ID</param>
    ///// <param name="strInfo">内容更新，如果此内容为空，则为更新此字段</param>
    ///// <returns>当前日志ID</returns>
    //public bool UpdateLog(string sllID, string strInfo)
    //{
    //    if (!this.HasLogin)
    //        return false;

    //    string strResultXml = mProxy.sysService.UpdateLog(this.LogID, sllID, strInfo, __strReserved);
    //    return MyXml.ParseResultOK(strResultXml);
    //}

    /// <summary>
    /// 保存用户配置记录
    /// </summary>
    /// <param name="strKey">配置字段名</param>
    /// <param name="strValue">配置字段值</param>
    /// <returns></returns>
    public bool SetConfig(string strKey, string strValue)
    {
        if (!this.HasLogin)
            return false;

        //string strResultXml = mProxy.sysService.SetConfig(this.LogID, strKey, strValue, __strReserved);
        //return MyXml.ParseResultOK(strResultXml);

        //todo
        return true;
    }

    /// <summary>
    /// 读取用户配置记录
    /// </summary>
    /// <param name="appCode">子系统Code（可以取其他子系统内容，为空则表示当前系统）</param>
    /// <param name="strKey">配置字段名</param>
    /// <returns>value=strValue</returns>
    public string GetConfig(string appCode, string strKey)
    {
        //if (!this.HasLogin)
        //    return "";

        //string strResultXml = mProxy.sysService.GetConfig(this.LogID, appCode, strKey, __strReserved);
        //if (!MyXml.ParseResultOK(strResultXml, ref __mCode, ref __strMessage, ref __strResult))
        //{
        //    MyLog.WriteLogInfo(string.Format("GetConfig error:{0} {1}", __mCode, __strMessage));
        //}
        //return __strResult;

        //todo
        return "";
    }

    ///// <summary>
    ///// 获取数据字典
    ///// </summary>
    ///// <param name="appCode">子系统Code（可以取其他子系统内容，为空则表示当前系统）</param>
    ///// <param name="strKey"></param>
    ///// <returns></returns>
    //public XmlDocument GetDict(string appCode, string strKey)
    //{
    //    if (!this.HasLogin)
    //        return MyXml.CreateResultXml(1, "has not logged in.", "");

    //    string strResultXml = mProxy.sysService.GetDict(this.LogID, appCode, strKey, __strReserved);
    //    return MyXml.LoadXml(strResultXml);
    //}

    #endregion

    #region 公共函数

    public string CreateVerifyCode(string strSource)
    {
        return MySecurity.EnCodeByMD5(strSource + "-" + this.LogID);
    }

    #endregion
}