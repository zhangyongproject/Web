using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 设备激活
    /// </summary>
    public class EquipmentActivationDAL : BaseDAL
    {
        public EquipmentActivationDAL()
        { }

        /// <summary>
        /// 添加设备激活
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(EquipmentActivationInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "insert into equipmentactivation(acid,eiid,accode,einame)values({0},{1},'{2}','{3}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.ACID, info.EIID, info.ACCode, info.EIName));

            retVal.IsSuccess    = result > 0;
            retVal.RetCode      = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg       = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取设备激活记录
        /// </summary>
        /// <param name="info">设备激活查询对象</param>
        /// <returns></returns>
        public ReturnValue GetEquipmentActivation(EquipmentActivationInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select uaid,acid,eiid,accode,einame,createtime from equipmentactivation where 1=1 ";
            if (info.ACCode.Trim().Length > 0)
            {
                sql += string.Format(" and accode like '%{0}%'", info.ACCode);
            }
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and einame  like '%{0}%'", info.EIName);
            }
            if (info.ACID > 0)
            {
                sql += string.Format(" and acid = {0}", info.ACID);
            }
            if (info.EIID > 0)
            {
                sql += string.Format(" and eiid = {0}", info.EIID);
            }
            if (info.UAID > 0)
            {
                sql += string.Format(" and uaid = {0}", info.UAID);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess    = Tools.IsValidDt(dt);
            retVal.RetCode      = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg       = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info">UAID/ACID/ACCODE</param>
        /// <returns></returns>
        public ReturnValue Delete(EquipmentActivationInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from equipmentactivation where uaid = {0} or  acid = {1} or accode='{2}' or eiid = {3}";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UAID, info.ACID,info.ACCode, info.EIID), info.UAID, info.ACID, info.ACCode);

            retVal.IsSuccess    = result > 0;
            retVal.RetCode      = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg       = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 删除激活记录按照EIID
        /// </summary>
        /// <param name="info">记录ID支持逗号分隔多个</param>
        /// <returns></returns>
        public ReturnValue Delete4IdsByEiid(string ids)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from equipmentactivation where eiid in ({0})";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, ids));
            retVal.OutCount = result;   //由于是多个记录，影响行数将可能大于1。
            retVal.RetCode = result > 0 ? 1 : 0;
            retVal.IsSuccess = result > 0;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }
    }
}
