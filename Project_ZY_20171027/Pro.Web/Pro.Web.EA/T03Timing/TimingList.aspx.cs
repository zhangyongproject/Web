using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Pro.Common;
using Pro.CoreModel;
using Pro.EABase;
using Pro.Web.EALogic;
using System.Xml;

public partial class T03Timing_TimingList : BasePage
{
    private TimingStartRecordLogic tsrLogic = new TimingStartRecordLogic();
    private UserLogic userLogic = new UserLogic();
    private EquipmentInfoLogic equLogic = new EquipmentInfoLogic();
    private UserEquipmentGrantLogic uegLogic = new UserEquipmentGrantLogic();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override XmlDocument CreateInitInfo()
    {
        XmlDocument xmlDoc = MyXml.CreateResultXml(1, string.Empty, string.Empty);
        XmlNode rootNode = xmlDoc.SelectSingleNode("xml");

        XmlNode userNode = MyXml.AddXmlNode(rootNode, "users");
        XmlNode equipmentsNode = MyXml.AddXmlNode(rootNode, "equipments");
        XmlNode grantequipmentsNode = MyXml.AddXmlNode(rootNode, "grantequipments");
        XmlNode notgrantequipmentsNode = MyXml.AddXmlNode(rootNode, "notgrantequipments");
        //用户对象
        ReturnValue retVal = userLogic.GetUser(new UserInfo() { UserID = -1 });
        if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(-1, "加载用户时异常", string.Empty); }
        foreach (DataRow dr in retVal.RetDt.Rows)
        {
            XmlNode itemNode = MyXml.AddXmlNode(userNode, "item");
            MyXml.AddAttribute(itemNode, "key", dr["userid"]);
            MyXml.AddAttribute(itemNode, "name", string.Format("{0}【{1}】", dr["username"], dr["usernick"]));
        }
        //设备对象
        retVal = equLogic.GetEquipment(new EquipmentInfo() { EIID = -1 });
        if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(-1, "加载设备时异常", string.Empty); }
        foreach (DataRow dr in retVal.RetDt.Rows)
        {
            XmlNode itemNode = MyXml.AddXmlNode(equipmentsNode, "item");
            MyXml.AddAttribute(itemNode, "key", dr["eiid"]);
            MyXml.AddAttribute(itemNode, "name", dr["einame"]);
        }
        //未授权设备对象 
        retVal = equLogic.GetNotGrantEquipment(new EquipmentInfo() { EIID = -1 });
        if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(-1, "加载未授权设备时异常", string.Empty); }
        foreach (DataRow dr in retVal.RetDt.Rows)
        {
            XmlNode itemNode = MyXml.AddXmlNode(notgrantequipmentsNode, "item");
            MyXml.AddAttribute(itemNode, "key", dr["eiid"]);
            MyXml.AddAttribute(itemNode, "name", dr["einame"]);
        }

        //昂前登录用户授权设备对象
        retVal = uegLogic.GetUserEquGrant(new UserEquipmentGrantInfo() { UserID = Tools.GetInt32(IPApi.UserID, int.MaxValue) });
        if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(-1, "加载未授权设备时异常", string.Empty); }
        foreach (DataRow dr in retVal.RetDt.Rows)
        {
            XmlNode itemNode = MyXml.AddXmlNode(grantequipmentsNode, "item");
            MyXml.AddAttribute(itemNode, "key", dr["eiid"]);
            MyXml.AddAttribute(itemNode, "name", dr["einame"]);
        }

        return xmlDoc;
    }

    public string GetList(string strparam, int pageIndex, int pageSize, string sort)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        ReturnValue retVal = tsrLogic.GetTimingStartRecord(new TiminGstartRecordInfo()
        {
            PackName = dic.ContainsKey("packname") ? dic["packname"] : string.Empty,
            ExpStartDate = Tools.GetDateTime(dic.ContainsKey("expbegintime") ? dic["expbegintime"] : string.Empty, DateTime.MinValue),
            ExpEndDate = Tools.GetDateTime(dic.ContainsKey("expendtime") ? dic["expendtime"] : string.Empty, DateTime.MaxValue),
            TSRID = dic.ContainsKey("tsrid") ? Tools.GetInt32(dic["tsrid"], -1) : -1,
            UserID = dic.ContainsKey("userid") ? Tools.GetInt32(dic["userid"], -1) : -1,
            EIID = dic.ContainsKey("eiid") ? Tools.GetInt32(dic["eiid"], -1) : -1,
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
        TiminGstartRecordInfo info = new TiminGstartRecordInfo()
        {
            TSRID = dic.ContainsKey("tsrid") ? Tools.GetInt32(dic["tsrid"], -1) : -1
        };
        ReturnValue retVal = tsrLogic.Delete(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

    public string ReleaseIds(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        //tsrid ==-1 添加 否则 修改
        ReturnValue retVal = tsrLogic.ReleaseIds((dic.ContainsKey("tsrid") ? dic["tsrid"] : "-1"));
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}
