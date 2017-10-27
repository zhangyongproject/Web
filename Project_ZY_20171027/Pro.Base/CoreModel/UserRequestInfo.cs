
namespace Pro.CoreModel
{
    /// <summary>
    /// 用户请求信息
    /// </summary>
    public class UserRequestInfo
    {
        #region 属性

        private int _UserId;
        private int _ModuleId;
        private string _ModuleName;
        private string _ModulePath;
        private int _Perms;
        private CommProcInfo _CommProcInfo = new CommProcInfo();

        /// <summary>
        /// 通用存储过程信息
        /// </summary>
        public CommProcInfo CommProcInfo
        {
            get { return _CommProcInfo; }
            set { if (value != null) _CommProcInfo = value; }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        /// <summary>
        /// 模块ID
        /// </summary>
        public int ModuleId
        {
            get { return _ModuleId; }
            set { _ModuleId = value; }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }

        /// <summary>
        /// 模块路径
        /// </summary>
        public string ModulePath
        {
            get { return _ModulePath; }
            set { _ModulePath = value; }
        }

        /// <summary>
        /// 模块权限
        /// </summary>
        public int Perms
        {
            get { return _Perms; }
            set { _Perms = value; }
        }
        #endregion
    }
}
