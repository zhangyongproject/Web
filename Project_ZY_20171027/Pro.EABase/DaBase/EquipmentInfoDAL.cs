using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 设备
    /// </summary>
    public class EquipmentInfoDAL : BaseDAL
    {
        public EquipmentInfoDAL() { }
        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(EquipmentInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            //添加设备记录
            string sql = "insert into equipmentinfo(einame,einumber,iplist,status,hardware,description)values('{0}','{1}','{2}',{3},'{4}','{5}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.EIName, info.EINumber, info.IPList, info.Status, info.HardWare, info.Description));
            retVal.RetCode = result;
            retVal.IsSuccess = result > 0;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 修改设备（设备名称不支持修改）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(EquipmentInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = string.Format(@"update equipmentinfo set eiid = eiid ");
            if (info.IPList.Trim().Length > 0)
            { sql += string.Format(" ,iplist ='{0}'", info.IPList); }
            if (info.Status > -1)
            { sql += string.Format(" ,status = {0}", info.Status); }
            if (info.HardWare.Trim().Length > 0)
            { sql += string.Format(" ,hardware ='{0}'", info.HardWare); }
            if (info.Description.Trim().Length > 0)
            { sql += string.Format(" ,description ='{0}'", info.Description); }
            sql += string.Format(" where eiid={0} or einame ='{1}'", info.EIID, info.EIName);
            int result = SQLiteHelper.ExecuteNonQuery(sql, null);

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取设备
        /// </summary>
        /// <param name="info">设备查询对象</param>
        /// <returns></returns>
        public ReturnValue GetEquipment(EquipmentInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select eiid,einame,einumber,iplist,status,hardware,description,createtime from equipmentinfo where 1=1 ";
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and einame like '%{0}%'", info.EIName);
            }
            if (info.EINumber.Trim().Length > 0)
            {
                sql += string.Format(" and einumber like '%{0}%'", info.EINumber);
            }
            if (info.IPList.Trim().Length > 0)
            {
                sql += string.Format(" and iplist like '%{0}%'", info.IPList);
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
            }
            if (info.EIID > 0)
            {
                sql += string.Format(" and eiid = {0}", info.EIID);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取设备(未授权)
        /// </summary>
        /// <param name="info">设备查询对象</param>
        /// <returns></returns>
        public ReturnValue GetNotGrantEquipment(EquipmentInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select equ.[eiid],equ.[einame],equ.[iplist] from equipmentinfo equ where not exists (select * from userequipmentgrant uqg where equ.[eiid] = uqg.[eiid])";
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and equ.[einame] like '%{0}%'", info.EIName);
            }
            if (info.EIID > 0)
            {
                sql += string.Format(" and equ.[eiid] = {0}", info.EIID);
            }
            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="info">EIID/EINAME</param>
        /// <returns></returns>
        public ReturnValue Delete(EquipmentInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from equipmentinfo where eiid = {0} or  einame = '{1}'";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.EIID, info.EIName));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }
    }
}
