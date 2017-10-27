using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using Pro.Web.Common;
using Pro.EABase;
using System.Data;
using Pro.Common;


namespace Pro.Web.EALogic
{
    /// <summary>
    /// 激活码
    /// </summary>
    public class ActivationCodeLogic
    {
        private EquipmentInfoDAL eiDAL = new EquipmentInfoDAL();
        private EquipmentActivationDAL eaDAL = new EquipmentActivationDAL();
        private ActivationCodeDAL acDAL = new ActivationCodeDAL();

        /// <summary>
        /// 添加激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(ActivationCodeInfo info)
        {
            //是否存在该激活码
            ReturnValue retVal = GetActiveCode(new ActivationCodeInfo() { ACCode = info.ACCode });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("accode='{0}'", info.ACCode), "acid asc");
            if (drs.Length > 0) { return new ReturnValue(false, -2); } //已存在该设备
            return acDAL.Insert(info);
        }


        /// <summary>
        /// 修改激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(ActivationCodeInfo info)
        {
            //是否存在该激活码
            ReturnValue retVal = GetActiveCode(new ActivationCodeInfo() { ACID = info.ACID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("accode='{0}' or acid={1}", info.ACCode, info.ACID), "acid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该设备
            return acDAL.Update(info);
        }


        /// <summary>
        /// 获取激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue GetActiveCode(ActivationCodeInfo info)
        {
            if (info.ACCode == null) { info.ACCode = string.Empty; }
            ReturnValue retVal = acDAL.GetActiveCode(info);
            //CacheHelper.SetCache(Cache_Equipment, retVal);
            return retVal;
        }


        /// <summary>
        /// 获取激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue GetActiveCode(ActivationCodeInfo info, bool relationEquipment)
        {
            if (relationEquipment == false) { return GetActiveCode(info); }
            ReturnValue retVal = GetActiveCode(info);
            if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
            DataTable dt = retVal.RetDt;
            dt.Columns.Add("EQUIPMENTS", typeof(string));
            //获取激活设备激活信息
            ReturnValue retValAC = eaDAL.GetEquipmentActivation(new EquipmentActivationInfo() { });
            if (retValAC.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string equipments = string.Empty;
                    DataRow[] drs = retValAC.RetDt.Select(string.Format("acid = {0}", row["ACID"]));
                    foreach (DataRow rowequ in drs)
                    {
                        equipments += rowequ["EINAME"].ToString() + ",";
                    }
                    row["EQUIPMENTS"] = equipments.Length > 0 ? equipments.Substring(0, equipments.Length - 1) : string.Empty;
                }
            }
            catch (Exception)
            {
                return new ReturnValue(false, -9, Consts.EXP_Info);
            }
            return new ReturnValue(1, dt);
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info">ACID/ACCODE</param>
        /// <returns></returns>
        public ReturnValue Delete(ActivationCodeInfo info)
        {
            //是否存在该设备
            ReturnValue retVal = GetActiveCode(new ActivationCodeInfo() { ACID = info.ACID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("accode='{0}' or acid={1}", info.ACCode, info.ACID), "acid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该设备
            return acDAL.Delete(info);
        }
    }
}
