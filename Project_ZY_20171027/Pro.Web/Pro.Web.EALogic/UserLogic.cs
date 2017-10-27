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
    /// 用户相关业务处理类
    /// </summary>
    public class UserLogic : BaseLogic
    {
        private UserDAL userDAL = new UserDAL();
        private UserEquipmentGrantDAL uegDAL = new UserEquipmentGrantDAL();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="strjson">登录信息（JSON）</param>
        /// <returns></returns>
        public ReturnValue Login(UserInfo info)
        {
            if (info.UserName == null || info.UserName.Trim().Length == 0) { return new ReturnValue(false, -1, "账号为空"); }
            if (info.UserPwd == null || info.UserPwd.Trim().Length == 0) { return new ReturnValue(false, -1, "账号密码为空"); }
            //获取用户 
            ReturnValue retVal = GetUser(info);
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("username='{0}' and userpwd='{1}'", info.UserName, info.UserPwd), "userid asc");
            if (drs.Length == 1)
            {
                int status = Tools.GetInt32(drs[0]["status"], 0);
                if (status == 1) { return new ReturnValue(false, -8, "用户已被禁用"); } //用户状态 1：禁用
                //获取用户当前拥有的设备
                int userid = Tools.GetInt32(drs[0]["userid"], -1);
                retVal = uegDAL.GetUserAndEquFromUserEquGrant(new UserEquipmentGrantInfo() { UserID = userid });
                retVal.OutId = userid;
                return retVal;
            }
            else
            {
                drs = dt.Select(string.Format("username='{0}'", info.UserName), "userid asc");
                if (drs.Length == 1) { return new ReturnValue(false, -1, "密码错误"); } //密码错误
                else { return new ReturnValue(false, -1, "用户名不存在"); } //用户名不存在
            }
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="info">查询对象</param>
        /// <returns></returns>
        public ReturnValue GetUser(UserInfo info)
        {
            return userDAL.GetUser(info);
        }


        /// <summary>
        /// 根据用户ID、用户帐号获取用户
        /// </summary>
        /// <param name="info">用户ID/用户帐号</param>
        /// <returns></returns>
        public ReturnValue GetUserById(UserInfo info)
        {
            ReturnValue retVal = GetUser(new UserInfo() { UserID = info.UserID, UserName = info.UserName });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format(" userid={0} or username='{1}'", info.UserID, info.UserName), "userid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该用户
            UserInfo uInfo = new UserInfo()
            {
                UserID = Tools.GetInt32(drs[0]["userid"], -1),
                UserName = drs[0]["username"].ToString(),
                UserNick = drs[0]["usernick"].ToString(),
                UserType = Tools.GetInt32(drs[0]["usertype"], -1),
                Status = Tools.GetInt32(drs[0]["status"], -1),
                CreateTime = Tools.GetDateTime(drs[0]["createtime"].ToString(), DateTime.MinValue),
                MobilePhone = drs[0]["mobilephone"].ToString(),
                Description = drs[0]["description"].ToString(),
                UserPwd = drs[0]["userpwd"].ToString()
            };
            retVal.IsSuccess = true;
            retVal.RetCode = 1;
            retVal.RetObj = uInfo;  //用户实体对象

            return retVal;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(UserInfo info)
        {
            ReturnValue retVal = GetUser(new UserInfo() { UserName=info.UserName });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("username='{0}'", info.UserName), "userid asc");
            if (drs.Length > 0) { return new ReturnValue(false, -2); } //已存在该用户
            return userDAL.Insert(info);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(UserInfo info)
        {
            ReturnValue retVal = GetUser(new UserInfo() { UserID = info.UserID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("username='{0}' or userid={1}", info.UserName, info.UserID), "userid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该用户
            return userDAL.Update(info);
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue UpdatePassWord(UserInfo info)
        {
            ReturnValue retVal = GetUser(new UserInfo() { UserID = info.UserID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("username='{0}' or userid={1}", info.UserName, info.UserID), "userid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该用户
            return userDAL.UpdatePassWord(info);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="info">UserId/UserName</param>
        /// <returns></returns>
        public ReturnValue Delete(UserInfo info)
        {
            //是否存在该用户
            ReturnValue retVal = GetUser(new UserInfo() { UserID = info.UserID });
            if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            DataTable dt = retVal.RetDt;
            DataRow[] drs = dt.Select(string.Format("username='{0}' or userid={1}", info.UserName, info.UserID), "userid asc");
            if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该用户
            return userDAL.Delete(info);
        }
    }
}
