using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Pro.Web.EALogic;
using Pro.Web.Common;
using Pro.Common;
using Pro.EABase;
using Pro.CoreModel;
using Pro.Common;

namespace Pro.Web.EquActive.WebService
{
    /// <summary>
    /// TimingService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://yixiubox.com:5000/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class TimingService : BaseService
    {
        private TimingStartRecordLogic tsrLogic = new TimingStartRecordLogic();

        /// <summary>
        /// 获取设备启动记录
        /// </summary>
        /// <param name="strjson">{"equipmentname ": "设备名称"}/{"equipmentname":"设备名称","username":"admin","startdate":"2017-09-14 10:00:00","enddate":"2017-09-25 10:00:00"}</param>
        /// <returns></returns>
        [WebMethod]
        public string GetTimingList(string strjson)
        {
            try
            {
                Dictionary<string, string> dic = MyJson.JsonToDictionary(strjson);
                if (dic.Count == 0) { return Json.Write(-1, "参数JSON格式错误"); }
                string equipmentname = string.Empty;
                string username = string.Empty;
                string expstartdate = string.Empty;
                string expenddate = string.Empty;
                if (dic.TryGetValue("equipmentname", out equipmentname) == false) { return Json.Write(-1, "设备名称无法识别"); }
                if (equipmentname.Trim().Length == 0) { return Json.Write(-1, "设备名称为空"); }
                //以下参数为可选，先判断是否存在
                if (dic.ContainsKey("username") && dic.TryGetValue("username", out username) == false) { return Json.Write(-1, "用户账号无法识别"); }
                if (dic.ContainsKey("expstartdate") && dic.TryGetValue("expstartdate", out expstartdate) == false) { return Json.Write(-1, "有效期开始时间无法识别"); }
                if (dic.ContainsKey("expenddate") && dic.TryGetValue("expenddate", out expenddate) == false) { return Json.Write(-1, "有效期结束时间无法识别"); }
                //获取设备信息
                TiminGstartRecordInfo info = new TiminGstartRecordInfo() { EIName = equipmentname };
                if (dic.ContainsKey("username")) { info.UserName = username; }
                if (dic.ContainsKey("expstartdate"))
                {
                    DateTime starttime = Tools.GetDateTime(expstartdate, DateTime.MinValue);
                    if (starttime == DateTime.MinValue) { return Json.Write(-1, "有效期开始时间格式输入不正确"); }
                    info.ExpStartDate = starttime;
                }
                if (dic.ContainsKey("expenddate"))
                {
                    DateTime endtime = Tools.GetDateTime(expenddate, DateTime.MaxValue);
                    if (endtime == DateTime.MaxValue) { return Json.Write(-1, "有效期结束时间格式输入不正确"); }
                    info.ExpEndDate = endtime;
                }
                ReturnValue retVal = tsrLogic.GetTimingStartRecord(info);
                return Json.Write(retVal.RetCode, retVal.RetMsg, retVal.RetDt);
            }
            catch (Exception ex)
            {
                return Json.Write(-1, Consts.EXP_Info);
            }
        }
    }
}
