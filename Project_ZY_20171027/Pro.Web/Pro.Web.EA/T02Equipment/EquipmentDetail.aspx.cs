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
using Pro.EABase;
using Pro.Web.EALogic;

public partial class T02Equipment_EquipmentDetail : BasePage
{
    private EquipmentInfoLogic equLogic = new EquipmentInfoLogic();

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
        EquipmentInfo info = new EquipmentInfo()
        {
            EIID = Tools.GetInt32((dic.ContainsKey("eiid") ? dic["eiid"] : "-1"), -1),
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty,
            IPList = dic.ContainsKey("iplist") ? dic["iplist"] : string.Empty,
            Status = 0,
            HardWare = string.Empty,
            EINumber = string.Empty,
            Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
        };
        //eiid ==-1 添加 否则 修改
        ReturnValue retVal = info.EIID == -1 ? equLogic.Insert(info) : equLogic.Update(info);
        //if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml; }
        // 
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}