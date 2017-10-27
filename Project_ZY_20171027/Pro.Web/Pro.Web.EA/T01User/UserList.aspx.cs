using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Pro.Common;
using Pro.Web.EALogic;
using Pro.CoreModel;

public partial class T01User_UserList : BasePage
{
    private UserLogic userLogic = new UserLogic();
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected override void InitModuleInfo()
    {
        base.InitModuleInfo();
    }

    public string GetList(string strparam, int pageIndex, int pageSize, string sort)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        ReturnValue retVal = userLogic.GetUser(new UserInfo()
        {
            UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
            UserNick = dic.ContainsKey("usernick") ? dic["usernick"] : string.Empty,
            UserType = dic.ContainsKey("usertype") ? Tools.GetInt32(dic["usertype"], -1) : -1,
            UserID = dic.ContainsKey("userid") ? Tools.GetInt32(dic["userid"], -1) : -1
        });
        if (retVal.IsSuccess == false) { return MyXml.CreateTabledResultXml(new DataTable(), 0, 10, 0).InnerXml; }
        //
        //DataTable dt = Tools.GetDt4Drs(retVal.RetDt, Tools.GetStartRec(pageSize, pageIndex), Tools.GetEndRec(pageSize, pageIndex)) ?? new DataTable();
        return MyXml.CreateTabledResultXml(retVal.RetDt, pageIndex, pageSize, retVal.RetDt.Rows.Count).InnerXml;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string Delete(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        UserInfo info = new UserInfo()
        {
            UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1)
        };
        ReturnValue retVal = userLogic.Delete(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}
