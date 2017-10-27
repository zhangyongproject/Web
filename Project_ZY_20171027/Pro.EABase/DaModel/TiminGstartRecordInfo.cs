﻿
using System;
using Pro.CoreModel;

namespace Pro.EABase
{
    /// <summary>
    /// 设备激活码授权信息
    /// </summary>
    public class TiminGstartRecordInfo
    {
        #region 属性
        private int _TSRID = -1;

        /// <summary>
        /// 记录ID
        /// </summary>
        public int TSRID
        {
            get { return _TSRID; }
            set { _TSRID = value; }
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
        private string _PackName = string.Empty;

        /// <summary>
        /// 包名称
        /// </summary>
        public string PackName
        {
            get { return _PackName; }
            set { _PackName = value; }
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

        private DateTime _ExpStartDate = DateTime.MinValue;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime ExpStartDate
        {
            get { return _ExpStartDate; }
            set { _ExpStartDate = value; }
        }
        private DateTime _ExpEndDate = DateTime.MaxValue;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime ExpEndDate
        {
            get { return _ExpEndDate; }
            set { _ExpEndDate = value; }
        }

        private int _Status = -1;

        /// <summary>
        /// 状态 0：正常 1：禁用
        /// </summary>
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private int _Release = -1;

        /// <summary>
        /// 发布状态 0：发布 1：未发布
        /// </summary>
        public int Release
        {
            get { return _Release; }
            set { _Release = value; }
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

        private DateTime _CreateTime = DateTime.Now;

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
