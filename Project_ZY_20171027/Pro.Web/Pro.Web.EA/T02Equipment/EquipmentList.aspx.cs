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

public partial class T02Equipment_EquipmentList : BasePage
{
    private EquipmentInfoLogic equLogic = new EquipmentInfoLogic();
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
        ReturnValue retVal = equLogic.GetEquipment(new EquipmentInfo()
        {
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty,
            IPList = dic.ContainsKey("iplist") ? dic["iplist"] : string.Empty,
            EIID = dic.ContainsKey("eiid") ? Tools.GetInt32(dic["eiid"], -1) : -1
        }, true, dic.ContainsKey("userid") ? Tools.GetInt32(dic["userid"], -1) : -1);
        if (retVal.IsSuccess == false) { return MyXml.CreateTabledResultXml(new DataTable(), 0, 10, 0).InnerXml; }
        //
        //DataTable dt = Tools.GetDt4Drs(retVal.RetDt, Tools.GetStartRec(pageSize, pageIndex), Tools.GetEndRec(pageSize, pageIndex)) ?? new DataTable();
        return MyXml.CreateTabledResultXml(retVal.RetDt, pageIndex, pageSize, retVal.RetDt.Rows.Count).InnerXml;
    }

    /// <summary>
    /// 获取设备(未授权)
    /// </summary>
    /// <param name="strparam"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    public string GetNotGrantEquipment(string strparam, int pageIndex, int pageSize, string sort)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        ReturnValue retVal = equLogic.GetNotGrantEquipment(new EquipmentInfo()
        {
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty,
            EIID = dic.ContainsKey("eiid") ? Tools.GetInt32(dic["eiid"], -1) : -1
        });
        if (retVal.IsSuccess == false) { return MyXml.CreateTabledResultXml(new DataTable(), 0, 10, 0).InnerXml; }
        //
        DataTable dt = Tools.GetDt4Drs(retVal.RetDt, Tools.GetStartRec(pageSize, pageIndex), Tools.GetEndRec(pageSize, pageIndex)) ?? new DataTable();
        return MyXml.CreateTabledResultXml(dt, pageIndex, pageSize, dt.Rows.Count).InnerXml;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string Delete(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        EquipmentInfo info = new EquipmentInfo()
        {
            EIID = Tools.GetInt32((dic.ContainsKey("eiid") ? dic["eiid"] : "-1"), -1)
        };
        ReturnValue retVal = equLogic.Delete(info);
        if (retVal.IsSuccess == false) { return MyXml.CreateTabledResultXml(new DataTable(), 0, 10, 0).InnerXml; }

        EquipmentActivationInfo infoea = new EquipmentActivationInfo()
        {
            EIID = Tools.GetInt32((dic.ContainsKey("eiid") ? dic["eiid"] : "-1"), -1)
        };
        ReturnValue retVal1 = equLogic.DeleteActiveEquipment(infoea);
        if (retVal1.IsSuccess == false) { return MyXml.CreateTabledResultXml(new DataTable(), 0, 10, 0).InnerXml; }

        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }


}
