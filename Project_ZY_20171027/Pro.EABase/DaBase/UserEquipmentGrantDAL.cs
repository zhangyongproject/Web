using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 用户设备授权
    /// </summary>
    public class UserEquipmentGrantDAL : BaseDAL
    {
        public UserEquipmentGrantDAL()
        { }

        /// <summary>
        /// 添加用户设备授权
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(UserEquipmentGrantInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "insert into userequipmentgrant(userid,username,eiid,einame,startdate,enddate,status,description)values({0},'{1}',{2},'{3}',datetime('{4}'),datetime('{5}'),{6},'{7}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UserID,
            info.UserName, info.EIID, info.EIName, info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"), info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"), info.Status, info.Description));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 修改用户设备授权
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(UserEquipmentGrantInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = string.Format(@"update userequipmentgrant set uegid = uegid ");
            if (info.UserID > -1) { sql += string.Format(" ,userid ={0}", info.UserID); }
            if (info.EIID > -1) { sql += string.Format(" ,eiid ={0}", info.EIID); }
            if (info.UserName.Trim().Length > 0)
            { sql += string.Format(" ,username ='{0}'", info.UserName); }
            if (info.EIName.Trim().Length > 0)
            { sql += string.Format(" ,einame ='{0}'", info.EIName); }
            if (info.StartDate > DateTime.MinValue)
            { sql += string.Format(" ,startdate = datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.EndDate < DateTime.MaxValue)
            { sql += string.Format(" ,enddate = datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.Description.Trim().Length > 0)
            { sql += string.Format(" ,description ='{0}'", info.Description); }
            sql += string.Format(" where uegid={0}", info.UEGID);
            int result = SQLiteHelper.ExecuteNonQuery(sql, null);

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取用户设备授权
        /// </summary>
        /// <param name="info">用户设备授权查询对象</param>
        /// <returns></returns>
        public ReturnValue GetUserEquGrant(UserEquipmentGrantInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select uegid,userid,username,eiid,einame,startdate,enddate,status,description,createtime from userequipmentgrant where 1=1 ";
            if (info.UserName.Trim().Length > 0)
            {
                sql += string.Format(" and username like '%{0}%'", info.UserName);
            }
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and einame like '%{0}%'", info.EIName);
            }
            if (info.StartDate > DateTime.MinValue)
            {
                sql += string.Format(" and enddate >= datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.StartDate < DateTime.MaxValue)
            {
                sql += string.Format(" and startdate <= datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.UEGID > -1)
            {
                sql += string.Format(" and uegid = {0}", info.UEGID);
            }
            if (info.UserID > -1)
            {
                sql += string.Format(" and userid = {0}", info.UserID);
            }
            if (info.EIID > -1)
            {
                sql += string.Format(" and eiid = {0}", info.EIID);
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 从用户设备授权中获取用户及设备信息
        /// </summary>
        /// <param name="info">用户设备授权查询对象</param>
        /// <returns></returns>
        public ReturnValue GetUserAndEquFromUserEquGrant(UserEquipmentGrantInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql =
            "select ueg.[uegid],ueg.[userid],ueg.[eiid]," +
            "u.[usernick],u.[mobilephone]," +
            "e.[einame],e.[einumber],e.[iplist], " +
            "ueg.[startdate] " +
            "from user u,equipmentinfo e,userequipmentgrant ueg " +
            "where u.[userid]= ueg.[userid] and e.[eiid] = ueg.[eiid] ";

            if (info.UserID > -1)
            {
                sql += string.Format(" and ueg.[userid] = {0}", info.UserID);
            }
            if (info.UserName.Trim().Length > 0)
            {
                sql += string.Format(" and username like '%{0}%'", info.UserName);
            }
            if (info.EIName.Trim().Length > 0)
            {
                sql += string.Format(" and einame like '%{0}%'", info.EIName);
            }
            if (info.StartDate > DateTime.MinValue)
            {
                sql += string.Format(" and enddate >= datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.EndDate < DateTime.MaxValue)
            {
                sql += string.Format(" and startdate <= datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (info.UEGID > -1)
            {
                sql += string.Format(" and uegid = {0}", info.UEGID);
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
            }

            DataTable dt = SQLiteHelper.Query(sql);
            retVal.RetDt = dt;
            retVal.IsSuccess = Tools.IsValidDt(dt);
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 删除用户授权记录
        /// </summary>
        /// <param name="info">UserId/EIID/UEGID</param>
        /// <returns></returns>
        public ReturnValue Delete(UserEquipmentGrantInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from userequipmentgrant where userid = {0} or  eiid = {1} or uegid = {2}";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UserID, info.EIID, info.UEGID));
            retVal.OutCount = result;   //由于是多个记录，影响行数将可能大于1。
            retVal.RetCode = result > 0 ? 1 : 0;
            retVal.IsSuccess = result > 0;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 删除用户授权记录
        /// </summary>
        /// <param name="info">记录ID支持逗号分隔多个</param>
        /// <returns></returns>
        public ReturnValue Delete4Ids(string ids)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from userequipmentgrant where uegid in ({0})";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, ids));
            retVal.OutCount = result;   //由于是多个记录，影响行数将可能大于1。
            retVal.RetCode = result > 0 ? 1 : 0;
            retVal.IsSuccess = result > 0;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 批量修改时间
        /// </summary>
        /// <param name="ids">用户设备授权ID列表（逗号分割）</param>
        /// <param name="info">对象【StartDate，EndDate】</param>
        /// <returns></returns>
        public ReturnValue BatchEditDate(string ids, UserEquipmentGrantInfo info)
        {
            if (string.IsNullOrEmpty(ids)) { return new ReturnValue(false, -2, "修改对象ID列表为空。"); }
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = string.Format(@"update userequipmentgrant set uegid = uegid ");
            if (info.StartDate > DateTime.MinValue)
            { sql += string.Format(" ,startdate = datetime('{0}')", info.StartDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.EndDate < DateTime.MaxValue)
            { sql += string.Format(" ,enddate = datetime('{0}')", info.EndDate.ToString("yyyy-MM-dd HH:mm:ss")); }
            if (info.Description.Trim().Length > 0)
                sql += string.Format(" where uegid in ({0})", ids);
            int result = SQLiteHelper.ExecuteNonQuery(sql, null);

            retVal.IsSuccess = result > 0;
            retVal.OutCount = result;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }
    }
}
