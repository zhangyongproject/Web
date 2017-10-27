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
    public class EquipmentInfoLogic : BaseLogic
    {
        /// <summary>
        /// 设备记录缓存
        /// </summary>
        private const string Cache_Equipment = "Cache_Equipment";

        private EquipmentInfoDAL eiDAL = new EquipmentInfoDAL();
        private EquipmentActivationDAL eaDAL = new EquipmentActivationDAL();
        private ActivationCodeDAL acDAL = new ActivationCodeDAL();
        private UserEquipmentGrantDAL uegDAL = new UserEquipmentGrantDAL();

        /// <summary>
        /// 设备激活
        /// </summary>
        /// <param name="strjson"></param>
        /// <returns></returns>
        public ReturnValue Active(EquipmentActivationInfo eainfo)
        {
            if (eainfo.EIName == null || eainfo.EIName.Trim().Length == 0) { return new ReturnValue(false, -1, "设备名称为空"); }
            if (eainfo.ACCode == null || eainfo.ACCode.Trim().Length == 0) { return new ReturnValue(false, -1, "激活码为空"); }
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            //设备信息（目前只有设备名）
            EquipmentInfo eInfo = new EquipmentInfo() { EIName = eainfo.EIName, Status = 0 };

            //（1）是否存在该激活码
            {
                retVal = acDAL.GetActiveCode(new ActivationCodeInfo() { ACCode = eainfo.ACCode });
                if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                DataRow[] drs = retVal.RetDt.Select(string.Format("accode = '{0}'", eainfo.ACCode));
                if (drs.Length == 0) { return new ReturnValue(false, -1, "该激活码不存在"); }
                eainfo.ACID = Tools.GetInt32(drs[0]["acid"], -1);
            }

            //（2）是否存在该设备
            {
                retVal = GetEquipment(eInfo);
                if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                DataRow[] drs = retVal.RetDt.Select(string.Format("einame = '{0}'", eainfo.EIName));
                if (drs.Length == 0)
                { //添加设备记录
                    retVal = eiDAL.Insert(eInfo);
                    if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                }
                else { return new ReturnValue(false, -1, "设备已存在"); }
            }

            //获取已添加的设备ID
            {
                if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                retVal = GetEquipment(eInfo);
                if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                DataRow[] drs = retVal.RetDt.Select(string.Format("einame = '{0}'", eInfo.EIName));
                if (drs.Length == 0) { return new ReturnValue(false, -1, "该设备不存在"); }
                eInfo.EIID = eainfo.EIID = Tools.GetInt32(drs[0]["eiid"], -1);
                //添加设备激活码记录
                ReturnValue retEA = eaDAL.Insert(eainfo);
                if (retEA.IsSuccess == false)
                {
                    //若设备激活添加不成功，需删除该设备,保持一致性
                    eiDAL.Delete(eInfo);
                }
            }
            #region test
            ////（2）是否已经激活设备
            //{
            //    EquipmentActivationInfo eaInfo = new EquipmentActivationInfo() { EIID = info.EIID };
            //    retVal = eaDAL.GetEquipmentActivation(eaInfo);
            //    if (retVal.IsSuccess == false) { return new ReturnValue(false, -1, Consts.EXP_Info); }
            //    DataRow[] drs = retVal.RetDt.Select(string.Format("eiid = '{0}'", eaInfo.EIID));
            //    if (drs.Length == 1)
            //    {
            //        if (Tools.GetInt32(drs[0]["acid"], -1) == eaInfo.EIID)
            //            return new ReturnValue(true, 1, "该设备已激活成功");
            //        else
            //            return new ReturnValue(false, -1, "该设备已被占用");
            //    }
            //    else
            //    {
            //        //添加设备激活码记录
            //        ReturnValue retEA = eaDAL.Insert(info);
            //        if (retEA.IsSuccess == false)
            //        {
            //            //若设备激活添加不成功，需删除该设备,保持一致性
            //            eiDAL.Delete(eInfo);
            //        }
            //    }
            //}
            #endregion test

            retVal.RetCode = 1;
            retVal.IsSuccess = true;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        public ReturnValue GetUserEquipmentGrant(UserEquipmentGrantInfo info)
        {
            return uegDAL.GetUserAndEquFromUserEquGrant(info);;
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <returns></returns>
        public ReturnValue GetEquipment(EquipmentInfo info)
        {
            if (info.EIName == null) { info.EIName = string.Empty; }
            if (CacheHelper.CheckCache(Cache_Equipment))
            {
                return CacheHelper.GetCache(Cache_Equipment) as ReturnValue;
            }
            ReturnValue retVal = eiDAL.GetEquipment(info);
            //CacheHelper.SetCache(Cache_Equipment, retVal);
            return retVal;
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <param name="relationActiveCode">是否关联激活码</param>
        /// <returns></returns>
        public ReturnValue GetEquipment(EquipmentInfo info, bool relationActiveCode, int userID)
        {
            if (relationActiveCode == false && userID < 0) { return GetEquipment(info); }
            ReturnValue retVal = GetEquipment(info);
            if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }

            DataTable dt = new DataTable();
            for (int i = 0; i < retVal.RetDt.Columns.Count; i++)
                dt.Columns.Add(retVal.RetDt.Columns[i].ColumnName);//复制列

            if (userID >= 0)
            {   //查找属于用户的设备
                ReturnValue retValUE = uegDAL.GetUserAndEquFromUserEquGrant(new UserEquipmentGrantInfo() { UserID = userID, });
                if (retValUE.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
                foreach (DataRow row in retValUE.RetDt.Rows)
                {
                    DataRow[] drs = retVal.RetDt.Select(string.Format("eiid = {0}", row["EIID"]));
                    foreach (DataRow rowtemp in drs)
                        dt.ImportRow(rowtemp);
                        //dt.Rows.Add(rowtemp);
                }
            }
            else
                dt = retVal.RetDt;

            dt.Columns.Add("ACCODE", typeof(string));
            //获取激活设备激活信息
            ReturnValue retValAC = eaDAL.GetEquipmentActivation(new EquipmentActivationInfo() { });
            if (retValAC.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    DataRow[] drs = retValAC.RetDt.Select(string.Format("eiid = {0}", row["EIID"]));
                    row["ACCODE"] = drs.Length == 0 ? string.Empty : drs[0]["ACCODE"].ToString();
                }
            }
            catch (Exception)
            {
                return new ReturnValue(false, -9, Consts.EXP_Info);
            }
            return new ReturnValue(1, dt);
        }

           /// <summary>
        /// 获取设备(未授权)
        /// </summary>
        /// <param name="info">设备查询对象</param>
        /// <returns></returns>
        public ReturnValue GetNotGrantEquipment(EquipmentInfo info)
        {
            return eiDAL.GetNotGrantEquipment(info);
        }
        /// <summary>
        /// 更新设备IP列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue UpdateIPList(EquipmentInfo info)
        {
            if (info.IPList == null || info.IPList.Trim().Length == 0) { return new ReturnValue(false, -1, "IP地址列表为空"); }
            ReturnValue retVal = GetEquipment(new EquipmentInfo() { EIName = info.EIName });
            if (retVal.IsSuccess == false) { return new ReturnValue(false, -9, Consts.EXP_Info); }
            DataRow[] drs = retVal.RetDt.Select(string.Format("einame = '{0}'", info.EIName));
            if (drs.Length == 0) { return new ReturnValue(false, -1, "设备已被删除"); }
            //判断IPList是否一致
            string iplist = drs[0]["iplist"].ToString();
            if (info.IPList == iplist) { return new ReturnValue(true, 1, "IPList 无变化"); }
            return eiDAL.Update(info);
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(EquipmentInfo info)
        {
            //是否存在该设备
            ReturnValue retVal = GetEquipment(new EquipmentInfo() { EIName = info.EIName });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("einame='{0}'", info.EIName), "eiid asc");
            if (drs.Length > 0) { return new ReturnValue(false, -2); } //已存在该设备
            return eiDAL.Insert(info);
        }

        /// <summary>
        /// 修改设备（设备名称不支持修改）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(EquipmentInfo info)
        {
            //是否存在该设备
            ReturnValue retVal = GetEquipment(new EquipmentInfo() { EIID = info.EIID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("einame='{0}' or eiid={1}", info.EIName, info.EIID), "eiid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该设备
            return eiDAL.Update(info);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="info">EIID/EINAME</param>
        /// <returns></returns>
        public ReturnValue Delete(EquipmentInfo info)
        {
            //是否存在该设备
            ReturnValue retVal = GetEquipment(new EquipmentInfo() { EIID = info.EIID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("einame='{0}' or eiid={1}", info.EIName, info.EIID), "eiid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该设备
            return eiDAL.Delete(info);
        }

        /// <summary>
        /// 删除激活设备表中设备
        /// </summary>
        /// <param name="info">EIID/EINAME</param>
        /// <returns></returns>
        public ReturnValue DeleteActiveEquipment(EquipmentActivationInfo info)
        {
            //是否存在该设备
            EquipmentActivationInfo eaInfo = new EquipmentActivationInfo() { EIID = info.EIID };
            ReturnValue retVal = eaDAL.GetEquipmentActivation(eaInfo);
            if (retVal.IsSuccess == false) { return new ReturnValue(false, -1, Consts.EXP_Info); }
            DataRow[] drs = retVal.RetDt.Select(string.Format("eiid = '{0}'", eaInfo.EIID));
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该设备
            return eaDAL.Delete(eaInfo);
        }
    }
}
