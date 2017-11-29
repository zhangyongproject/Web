using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 激活码
    /// </summary>
    public class ActivationCodeDAL : BaseDAL
    {

        /// <summary>
        /// 添加激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(ActivationCodeInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "insert into activationcode(accode,startdate,enddate,status,description)values('{0}',datetime('{1}'),datetime('{2}'),{3},'{4}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql,
                 info.ACCode, info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"), info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"), info.Status, info.Description));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 修改激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(ActivationCodeInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = string.Format(@"update activationcode set acid = acid ");
            if (info.ACCode.Trim().Length > 0)
            { sql += string.Format(" ,accode ='{0}'", info.ACCode); }
            if (info.Status > -1)
            { sql += string.Format(" ,status = {0}", info.Status); }
            if (info.StartDate > DateTime.MinValue)
            { sql += string.Format(" ,startdate = datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.EndDate < DateTime.MaxValue)
            { sql += string.Format(" ,enddate = datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.Description.Trim().Length > 0)
            { sql += string.Format(" ,description ='{0}'", info.Description); }
            sql += string.Format(" where acid={0} ", info.ACID);
            int result = SQLiteHelper.ExecuteNonQuery(sql, null);

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取激活码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue GetActiveCode(ActivationCodeInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select acid,accode,startdate,enddate,status,description,createtime from activationcode where 1=1 ";
            if (info.ACCode.Trim().Length > 0)
            {
                sql += string.Format(" and accode like '%{0}%'", info.ACCode);
            }
            if (info.StartDate > DateTime.MinValue)
            {
                sql += string.Format(" and startdate >= datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.EndDate < DateTime.MaxValue)
            {
                sql += string.Format(" and enddate <= datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
            }
            if (info.ACID > -1)
            {
                sql += string.Format(" and acid = {0}", info.ACID);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info">ACID/ACCODE</param>
        /// <returns></returns>
        public ReturnValue Delete(ActivationCodeInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from activationcode where acid = {0} or accode='{1}'";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.ACID, info.ACCode));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }
    }
}
