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


public partial class T04ActiveCode_ActiveCodeList : BasePage
{
    private ActivationCodeLogic acLogic = new ActivationCodeLogic();
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
        ReturnValue retVal = acLogic.GetActiveCode(new ActivationCodeInfo()
        {
            ACCode = dic.ContainsKey("accode") ? dic["accode"] : string.Empty,
            StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.MinValue),
            EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.MaxValue),
            ACID = dic.ContainsKey("acid") ? Tools.GetInt32(dic["acid"], -1) : -1
        }, true);
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
        ActivationCodeInfo info = new ActivationCodeInfo()
        {
            ACID = dic.ContainsKey("acid") ? Tools.GetInt32(dic["acid"], -1) : -1
        };
        ReturnValue retVal = acLogic.Delete(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}
