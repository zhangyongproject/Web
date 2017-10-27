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

public partial class T03Timing_TimingDetail : BasePage
{
    private TimingStartRecordLogic tsrLogic = new TimingStartRecordLogic();

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
        TiminGstartRecordInfo info = new TiminGstartRecordInfo()
        {
            UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1),
            EIID = Tools.GetInt32((dic.ContainsKey("eiid") ? dic["eiid"] : "-1"), -1),
            TSRID = Tools.GetInt32((dic.ContainsKey("tsrid") ? dic["tsrid"] : "-1"), -1),
            UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
            EIName = dic.ContainsKey("einame") ? dic["einame"] : string.Empty,
            PackName = dic.ContainsKey("packname") ? dic["packname"] : string.Empty,
            StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.MinValue),
            EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.MaxValue),
            ExpStartDate = Tools.GetDateTime(dic.ContainsKey("expbegintime") ? dic["expbegintime"] : string.Empty, DateTime.MinValue),
            ExpEndDate = Tools.GetDateTime(dic.ContainsKey("expendtime") ? dic["expendtime"] : string.Empty, DateTime.MaxValue),
            Release = Tools.GetInt32((dic.ContainsKey("release") ? dic["release"] : "-1"), -1),
            Status = 0,
            Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
        };
        //tsrid ==-1 添加 否则 修改
        ReturnValue retVal = info.TSRID == -1 ? tsrLogic.Insert(info) : tsrLogic.Update(info);
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

    /// <summary>
    /// 批量添加（批量关联）
    /// </summary>
    /// <param name="strparam"></param>
    /// <returns></returns>
    public string UserEquipmentsGrant(string strparam)
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
                ReturnValue retVal = tsrLogic.Insert(new TiminGstartRecordInfo()
                    {
                        UserID = Tools.GetInt32((dic.ContainsKey("userid") ? dic["userid"] : "-1"), -1),
                        EIID = Tools.GetInt32(eiids[i], -1),
                        TSRID = Tools.GetInt32((dic.ContainsKey("tsrid") ? dic["tsrid"] : "-1"), -1),
                        UserName = dic.ContainsKey("username") ? dic["username"] : string.Empty,
                        EIName = einames[i],
                        PackName = dic.ContainsKey("packname") ? dic["packname"] : string.Empty,
                        StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.Now),
                        EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, DateTime.Now),
                        ExpStartDate = Tools.GetDateTime(dic.ContainsKey("expbegintime") ? dic["expbegintime"] : string.Empty, DateTime.Now),
                        ExpEndDate = Tools.GetDateTime(dic.ContainsKey("expendtime") ? dic["expendtime"] : string.Empty, DateTime.Now),
                        Release = Tools.GetInt32((dic.ContainsKey("release") ? dic["release"] : "-1"), -1),
                        Status = 0,
                        Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
                    }
               );
            }
            catch (Exception ex)
            {
                errCnt += 1;
                MyLog.WriteExceptionLog("UserEquipmentsGrant(定时记录-批量添加)", ex, ex.Source);
            }
        }
        return MyXml.CreateResultXml(1, string.Empty, (einames.Length - errCnt).ToString()).InnerXml;
    }


}