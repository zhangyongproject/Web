
using System;
namespace Pro.CoreModel
{
    /// <summary>
    /// 用户请求信息
    /// </summary>
    public class UserInfo
    {
        #region 属性

        private int _UserId = -1;


        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private string _UserName = string.Empty;

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _UserPwd = string.Empty;


        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd
        {
            get { return _UserPwd; }
            set { _UserPwd = value; }
        }

        private string _UserNick = string.Empty;

        /// <summary>
        /// 用户呢称
        /// </summary>
        public string UserNick
        {
            get { return _UserNick; }
            set { _UserNick = value; }
        }

        private int _Status = -1;

        /// <summary>
        /// 禁用标志 0：正常  1：禁用
        /// </summary>
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private int _UserType = -1;

        /// <summary>
        /// 用户类型 0:管理员 1:普通用户
        /// </summary>
        public int UserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        private string _MobilePhone = string.Empty;

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone
        {
            get { return _MobilePhone; }
            set { _MobilePhone = value; }
        }

        private string _Description = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private DateTime _CreateTime;

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }

        private CommProcInfo _CommProcInfo = new CommProcInfo();

        /// <summary>
        /// 通用存储过程信息
        /// </summary>
        public CommProcInfo CommProcInfo
        {
            get { return _CommProcInfo; }
            set { if (value != null) _CommProcInfo = value; }
        }

        #endregion
    }
}
