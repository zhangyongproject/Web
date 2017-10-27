
using System;
using Pro.CoreModel;

namespace Pro.EABase
{
    /// <summary>
    /// 设备激活码授权信息
    /// </summary>
    public class UserEquipmentGrantInfo
    {
        #region 属性
        private int _UEGID = -1;

        /// <summary>
        /// 用户设备授权ID
        /// </summary>
        public int UEGID
        {
            get { return _UEGID; }
            set { _UEGID = value; }
        }
        private int _UserID = -1;

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        private string _UserName = string.Empty;

        /// <summary>
        /// 用户名（账号）
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private int _EIID = -1;

        /// <summary>
        /// 设备ID
        /// </summary>
        public int EIID
        {
            get { return _EIID; }
            set { _EIID = value; }
        }

        private string _EIName = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EIName
        {
            get { return _EIName; }
            set { _EIName = value; }
        }

        private DateTime _StartDate = DateTime.MinValue;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private DateTime _EndDate = DateTime.MaxValue;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        private int _Status = -1;

        /// <summary>
        /// 禁用 0：正常 1：禁用
        /// </summary>
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
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
