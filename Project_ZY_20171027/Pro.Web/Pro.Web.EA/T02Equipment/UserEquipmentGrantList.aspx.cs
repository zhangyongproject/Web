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
using Pro.EABase;


public partial class T02Equipment_UserEquipmentGrantList : BasePage
{
    private UserEquipmentGrantLogic uegLogic = new UserEquipmentGrantLogic();

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
        ReturnValue retVal = uegLogic.GetUserEquGrant(new UserEquipmentGrantInfo()
        {
            StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.MinValue),
            EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.MaxValue),
            EIID = dic.ContainsKey("eiid") ? Tools.GetInt32(dic["eiid"], -1) : -1,
            UserID = dic.ContainsKey("userid") ? Tools.GetInt32(dic["userid"], -1) : -1,
            UEGID = dic.ContainsKey("uegid") ? Tools.GetInt32(dic["uegid"], -1) : -1,
            UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty
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
        UserEquipmentGrantInfo info = new UserEquipmentGrantInfo()
        {
            UEGID = Tools.GetInt32((dic.ContainsKey("uegid") ? dic["uegid"] : "-1"), -1)
        };
        ReturnValue retVal = uegLogic.Delete(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, retVal.OutCount.ToString()).InnerXml;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string Delete4Ids(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        ReturnValue retVal = uegLogic.Delete4Ids((dic.ContainsKey("uegid") ? dic["uegid"] : "-1"));
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, retVal.OutCount.ToString()).InnerXml;
    }


}
