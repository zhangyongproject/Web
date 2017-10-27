using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Pro.Common;
using Pro.CoreModel;
using Pro.Web.EALogic;

public partial class T01User_UserDetail : BasePage
{
    private UserLogic userLogic = new UserLogic();

    protected void Page_Load(object sender, EventArgs e)
    {

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string AddorEdit(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        UserInfo info = new UserInfo()
        {
            UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1),
            UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
            UserNick = dic.ContainsKey("usernick") ? dic["usernick"] : string.Empty,
            MobilePhone = dic.ContainsKey("mobilephone") ? dic["mobilephone"] : string.Empty,
            UserType = Tools.GetInt32(dic["usertype"], -1),
            Status = 0,
            UserPwd = dic.ContainsKey("userpwd") ? dic["userpwd"] : string.Empty,
            Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
        };
        //if (info.UserID == -1) { info.UserPwd = dic.ContainsKey("userpwd") ? dic["userpwd"] : string.Empty; }
        //userid ==-1 添加 否则 修改
        ReturnValue retVal = info.UserID == -1 ? userLogic.Insert(info) : userLogic.Update(info);
        //if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml; }
        // 
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}