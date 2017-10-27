
using System;
using Pro.CoreModel;

namespace Pro.EABase
{
    /// <summary>
    /// 激活码信息
    /// </summary>
    public class ActivationCodeInfo
    {
        #region 属性

        private int _ACID = -1;

        /// <summary>
        /// 激活码ID
        /// </summary>
        public int ACID
        {
            get { return _ACID; }
            set { _ACID = value; }
        }
        private string _ACCode = string.Empty;

        /// <summary>
        /// 激活码
        /// </summary>
        public string ACCode
        {
            get { return _ACCode; }
            set { _ACCode = value; }
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
