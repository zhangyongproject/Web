
using System;
using Pro.CoreModel;

namespace Pro.EABase
{
    /// <summary>
    /// 用户设备授权信息
    /// </summary>
    public class EquipmentInfo
    {

        #region 属性
        private int _EIID=-1;

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
        /// 设备名称（唯一）
        /// </summary>
        public string EIName
        {
            get { return _EIName; }
            set { _EIName = value; }
        }
        private string _EINumber = string.Empty;

        /// <summary>
        /// 设备编码
        /// </summary>
        public string EINumber
        {
            get { return _EINumber; }
            set { _EINumber = value; }
        }
        private string _IPList = string.Empty;

        /// <summary>
        /// IP地址列表（多个用逗号分割）
        /// </summary>
        public string IPList
        {
            get { return _IPList; }
            set { _IPList = value; }
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
        private string _HardWare = string.Empty;

        /// <summary>
        /// 硬件信息
        /// </summary>
        public string HardWare
        {
            get { return _HardWare; }
            set { _HardWare = value; }
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
