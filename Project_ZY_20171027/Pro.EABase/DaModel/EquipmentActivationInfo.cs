
using System;
using Pro.CoreModel;

namespace Pro.EABase
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class EquipmentActivationInfo
    {
        #region 属性
        private int _UAID = -1;

        /// <summary>
        /// 设备授权ID
        /// </summary>
        public int UAID
        {
            get { return _UAID; }
            set { _UAID = value; }
        }
        private int _ACID = -1;

        /// <summary>
        /// 激活码ID
        /// </summary>
        public int ACID
        {
            get { return _ACID; }
            set { _ACID = value; }
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
        private string _ACCode = string.Empty;

        /// <summary>
        /// 激活码
        /// </summary>
        public string ACCode
        {
            get { return _ACCode; }
            set { _ACCode = value; }
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
