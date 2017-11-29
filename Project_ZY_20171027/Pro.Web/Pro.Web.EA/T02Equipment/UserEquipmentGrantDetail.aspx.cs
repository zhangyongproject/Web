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

public partial class T02Equipment_UserEquipmentGrantDetail : BasePage
{
    private UserEquipmentGrantLogic uegLogic = new UserEquipmentGrantLogic();

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
        UserEquipmentGrantInfo info = new UserEquipmentGrantInfo()
        {
            UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1),
            EIID = Tools.GetInt32((dic.ContainsKey("eiid") ? dic["eiid"] : "-1"), -1),
            UEGID = Tools.GetInt32((dic.ContainsKey("uegid") ? dic["uegid"] : "-1"), -1),
            UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty,
            StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.MinValue),
            EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.MaxValue),
            Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
        };
        info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
        info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);
        //uegid ==-1 添加 否则 修改
        ReturnValue retVal = info.UEGID == -1 ? uegLogic.Insert(info) : uegLogic.Update(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

    /// <summary>
    /// 批量添加（批量授权）
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string EquipmentsGrant(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        string[] eiids = (dic.ContainsKey("eiid") ? dic["eiid"] : string.Empty).Split(',');
        string[] einames = (dic.ContainsKey("einame") ? dic["einame"] : string.Empty).Split(',');
        int errCnt = 0;
        for (int i = 0; i < eiids.Length; i++)
        {
            if (eiids[i].Trim().Length == 0) { continue; }
            try
            {
                UserEquipmentGrantInfo info = new UserEquipmentGrantInfo()
                 {
                     UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1),
                     EIID = Tools.GetInt32(eiids[i], -1),
                     UEGID = Tools.GetInt32((dic.ContainsKey("uegid") ? dic["uegid"] : "-1"), -1),
                     UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
                     EIName = einames[i],
                     StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.MinValue),
                     EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.MaxValue),
                     Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
                 };

                info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
                info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);
                ReturnValue retVal = uegLogic.Insert(info);
            }
            catch (Exception ex)
            {
                errCnt += 1;
                MyLog.WriteExceptionLog("EquipmentsGrant(批量添加)", ex, ex.Source);
            }
        }
        return MyXml.CreateResultXml(1, string.Empty, (einames.Length - errCnt).ToString()).InnerXml;
    }


    /// <summary>
    /// 批量修改时间
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string BatchEditDate(string strparam)
    {
        Dictionary<string, string> dic = MyJson.JsonToDictionary(strparam);
        string uegids = (dic.ContainsKey("uegids") ? dic["uegids"] : string.Empty);
        DateTime begintime = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.Now);
        DateTime endtime = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.Now);
        int errCnt = 0;
        ReturnValue retVal = null;
        try
        {
            retVal = uegLogic.BatchEditDate(uegids, new UserEquipmentGrantInfo() { StartDate = begintime, EndDate = endtime });
        }
        catch (Exception ex)
        {
            errCnt += 1;
            MyLog.WriteExceptionLog("BatchEditDate(批量修改时间)", ex, ex.Source);
        }

        return MyXml.CreateResultXml(1, string.Empty, retVal.OutCount).InnerXml;
    }

}