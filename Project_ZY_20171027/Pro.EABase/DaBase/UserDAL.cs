using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using System.Data;
using Pro.Common;

namespace Pro.EABase
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserDAL : BaseDAL
    {
        public UserDAL()
        { }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="info">用户对象</param>
        /// <returns></returns>
        public ReturnValue Insert(UserInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "insert into user(username,userpwd,usernick,status,usertype,mobilephone,description)values('{0}','{1}','{2}',{3},{4},'{5}','{6}')";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql,
                 info.UserName, info.UserPwd, info.UserNick, info.Status, info.UserType, info.MobilePhone, info.Description));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 修改用户信息（用户帐号不支持修改）
        /// </summary>
        /// <param name="info">用户对象</param>
        /// <returns></returns>
        public ReturnValue Update(UserInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "update user set usernick ='{1}',usertype={2},status={3},mobilephone='{4}',description='{5}',userpwd='{6}' where userid={0}";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql,
                info.UserID, info.UserNick, info.UserType, info.Status, info.MobilePhone, info.Description, info.UserPwd));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue UpdatePassWord(UserInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "update user set userpwd ='{1}' where userid={0}";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UserID, info.UserPwd));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="info">用户查询对象</param>
        /// <returns></returns>
        public ReturnValue GetUser(UserInfo info)
        {
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "select userid, username,userpwd,usernick,status,usertype,mobilephone,description,createtime from user where 1=1 ";
            if (info.UserName.Trim().Length > 0)
            {
                sql += string.Format(" and username like '%{0}%'", info.UserName);
            }
            if (info.UserNick.Trim().Length > 0)
            {
                sql += string.Format(" and usernick like '%{0}%'", info.UserNick);
            }
            if (info.MobilePhone.Trim().Length > 0)
            {
                sql += string.Format(" and mobilephone like '%{0}%'", info.MobilePhone);
            }
            if (info.UserType > -1)
            {
                sql += string.Format(" and usertype = {0}", info.UserType);
            }
            if (info.Status > -1)
            {
                sql += string.Format(" and status = {0}", info.Status);
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
        /// 删除用户
        /// </summary>
        /// <param name="info">UserId/UserName</param>
        /// <returns></returns>
        public ReturnValue Delete(UserInfo info)
        {
            if (info.UserID == -1 && info.UserName.Trim().Length == 0) { return new ReturnValue(false, -1); }
            ReturnValue retVal = new ReturnValue(false, 0, string.Empty);
            string sql = "delete from user where userid = {0} or  username = '{1}'";
            int result = SQLiteHelper.ExecuteNonQuery(string.Format(sql, info.UserID, info.UserName));

            retVal.IsSuccess = result > 0;
            retVal.RetCode = retVal.IsSuccess ? 1 : -1;
            retVal.RetMsg = retVal.IsSuccess ? "成功" : "失败";
            return retVal;
        }

    }
}
