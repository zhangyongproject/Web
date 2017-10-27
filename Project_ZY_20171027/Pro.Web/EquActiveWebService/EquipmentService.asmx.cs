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
    /// EquipmentService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://yixiubox.com:5000/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class EquipmentService : BaseService
    {
        private EquipmentInfoLogic equLogic = new EquipmentInfoLogic();


        /// <summary>
        /// 获取设备
        /// </summary>
        /// <param name="strjson">{"equipmentname ": "设备名称"}</param>
        /// <returns></returns>
        [WebMethod]
        public string GetEquipmentInfo(string strjson)
        {
            try
            {
                Dictionary<string, string> dic = MyJson.JsonToDictionary(strjson);
                if (dic.Count == 0) { return Json.Write(-1, "参数JSON格式错误"); }
                string equipmentname = string.Empty;
                if (dic.TryGetValue("equipmentname", out equipmentname) == false) { return Json.Write(-1, "设备名称无法识别"); }
                //获取设备信息
                EquipmentInfo info = new EquipmentInfo() { EIName = equipmentname };
                ReturnValue retVal = equLogic.GetEquipment(info);
                return Json.Write(retVal.RetCode, "成功", retVal.RetDt);

            }
            catch (Exception ex)
            {
                return Json.Write(-1, Consts.EXP_Info);
            }
        }


        /// <summary>
        /// 设备激活
        /// </summary>
        /// <param name="strjson">{"code":"激活码","equipmentname":"设备名称"}</param>
        /// <returns></returns>
        [WebMethod]
        public string Active(string strjson)
        {
            try
            {
                if (string.IsNullOrEmpty(strjson)) { return Json.Write(-1, "参数JSON格式错误"); }
                Dictionary<string, string> dic = MyJson.JsonToDictionary(strjson);
                if (dic.Count == 0) { return Json.Write(-1, "参数JSON格式错误"); }
                string equipmentname = string.Empty;
                string accode = string.Empty;
                if (dic.TryGetValue("equipmentname", out equipmentname) == false) { return Json.Write(-1, "设备名称无法识别"); }
                if (dic.TryGetValue("code", out accode) == false) { return Json.Write(-1, "激活码无法识别"); }
                ReturnValue retVal = equLogic.Active(new EquipmentActivationInfo() { ACCode = accode, EIName = equipmentname });
                return Json.Write(retVal.RetCode, retVal.RetMsg);
            }
            catch (Exception ex)
            {
                return Json.Write(-1, Consts.EXP_Info);
            }

        }

        /// <summary>
        /// 更新IP列表
        /// </summary>
        /// <param name="strjson">{"equipmentname": "设备名称","iplist":"172.16.16.123,192.16.15.86"}</param>
        /// <returns></returns>
        [WebMethod]
        public string UpdateIPList(string strjson)
        {
            try
            {
                if (string.IsNullOrEmpty(strjson)) { return Json.Write(-1, "参数JSON格式错误"); }
                Dictionary<string, string> dic = MyJson.JsonToDictionary(strjson);
                if (dic.Count == 0) { return Json.Write(-1, "参数JSON格式错误"); }
                string equipmentname = string.Empty;
                string iplist = string.Empty;
                if (dic.TryGetValue("equipmentname", out equipmentname) == false) { return Json.Write(-1, "设备名称无法识别"); }
                if (dic.TryGetValue("iplist", out iplist) == false) { return Json.Write(-1, "IP列表无法识别"); }
                ReturnValue retVal = equLogic.UpdateIPList(new EquipmentInfo() { IPList = iplist, EIName = equipmentname });
                return Json.Write(retVal.RetCode, retVal.RetMsg);
            }
            catch (Exception ex)
            {
                return Json.Write(-1, Consts.EXP_Info);
            }
        }
    }
}
