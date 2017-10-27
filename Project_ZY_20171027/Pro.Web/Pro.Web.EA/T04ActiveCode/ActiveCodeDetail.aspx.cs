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

public partial class T04ActiveCode_ActiveCodeDetail : BasePage
{
    private ActivationCodeLogic acLogic = new ActivationCodeLogic();

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
        ActivationCodeInfo info = new ActivationCodeInfo()
        {

            ACID = Tools.GetInt32((dic.ContainsKey("acid") ? dic["acid"] : "-1"), -1),
            ACCode = dic.ContainsKey("accode") ? dic["accode"] : string.Empty,
            StartDate = Tools.GetDateTime(dic.ContainsKey("begintime") ? dic["begintime"] : string.Empty, DateTime.Now),
            EndDate = Tools.GetDateTime(dic.ContainsKey("endtime") ? dic["endtime"] : string.Empty, new DateTime(2099, 12, 31)),
            Description = dic.ContainsKey("description") ? dic["description"] : string.Empty
        };
        //acid ==-1 添加 否则 修改
        ReturnValue retVal = info.ACID == -1 ? acLogic.Insert(info) : acLogic.Update(info);
        //if (retVal.IsSuccess == false) { return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml; }
        // 
        return MyXml.CreateResultXml(retVal.RetCode, retVal.RetMsg, string.Empty).InnerXml;
    }

}