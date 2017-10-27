using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 定时启动记录
    /// </summary>
    public class TimingStartRecordDAL : BaseDAL
    {
        public TimingStartRecordDAL()
        { }

        /// <summary>
        /// 添加定时启动记录
        /// </summary>
        /// <param name="info">定时启动信息</param>
        /// <returns></returns>
        public ReturnValue Insert(TiminGstartRecordInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "insert into timingstartrecord(userid,username,eiid,einame,packname,startdate,enddate,expstartdate,expenddate,status,release,description)values({0},'{1}',{2},'{3}','{4}',datetime('{5}'),datetime('{6}'),datetime('{7}'),datetime('{8}'),{9},{10},'{11}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql,
                 info.UserID, info.UserName, info.EIID, info.EIName, info.PackName, info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"), info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"), info.ExpStartDate.ToString("yyyy-MM-dd HH:mm:ss"), info.ExpEndDate.ToString("yyyy-MM-dd HH:mm:ss"), info.Status, info.Release, info.Description));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 修改定时启动记录
        /// </summary>
        /// <param name="info">定时启动信息</param>
        /// <returns></returns>
        public ReturnValue Update(TiminGstartRecordInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = string.Format(@"update timingstartrecord set tsrid = tsrid ");
            if (info.UserID > -1)
            { sql += string.Format(" ,userid ={0},username='{1}'", info.UserID, info.UserName); }
            if (info.EIID > -1)
            { sql += string.Format(" ,eiid ={0},einame='{1}'", info.EIID, info.EIName); }
            if (info.PackName.Trim().Length > 0)
            { sql += string.Format(" ,packname ='{0}'", info.PackName); }
            if (info.Status > -1)
            { sql += string.Format(" ,status = {0}", info.Status); }
            if (info.Release > -1)
            { sql += string.Format(" ,release = {0}", info.Release); }
            if (info.StartDate > DateTime.MinValue)
            { sql += string.Format(" ,startdate = datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.EndDate < DateTime.MaxValue)
            { sql += string.Format(" ,enddate = datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.ExpStartDate > DateTime.MinValue)
            { sql += string.Format(" ,expstartdate = datetime('{0}')", info.ExpStartDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.ExpEndDate < DateTime.MaxValue)
            { sql += string.Format(" ,expenddate = datetime('{0}')", info.ExpEndDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.Description.Trim().Length > 0)
            { sql += string.Format(" ,description ='{0}'", info.Description); }
            sql += string.Format(" where tsrid={0}", info.TSRID);
            int result = SQLiteHelper.ExecuteNonQuery(sql, null);

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取定时启动记录
        /// </summary>
        /// <param name="info">查询对象</param>
        /// <returns></returns>
        public ReturnValue GetTimingStartRecord(TiminGstartRecordInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select tsrid,userid,username,eiid,einame,packname,startdate,enddate,expstartdate,expenddate,status,release,description,createtime from timingstartrecord where 1=1 ";
            if (info.UserName.Trim().Length > 0)
            {
                sql += string.Format(" and username like '%{0}%'", info.UserName);
            }
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and einame like '%{0}%'", info.EIName);
            }
            if (info.PackName.Trim().Length > 0)
            {
                sql += string.Format(" and packname like '%{0}%'", info.PackName);
            }
            if (info.ExpStartDate > DateTime.MinValue)
            {
                sql += string.Format(" and expstartdate >= datetime('{0}')", info.ExpStartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.ExpEndDate < DateTime.MaxValue)
            {
                sql += string.Format(" and expenddate <= datetime('{0}')", info.ExpEndDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
            }
            if (info.Release > -1)
            {
                sql += string.Format(" and release = {0}", info.Release);
            }
            if (info.EIID > -1)
            {
                sql += string.Format(" and eiid = {0}", info.EIID);
            }
            if (info.TSRID > -1)
            {
                sql += string.Format(" and tsrid = {0}", info.TSRID);
            }
            if (info.UserID > -1)
            {
                sql += string.Format(" and userid = {0}", info.UserID);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 删除定时启动记录
        /// </summary>
        /// <param name="info">UserId/EIID/TSRID</param>
        /// <returns></returns>
        public ReturnValue Delete(TiminGstartRecordInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from timingstartrecord where userid = {0} or  eiid = {1} or tsrid={2}";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UserID, info.EIID, info.TSRID));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

    }
}
