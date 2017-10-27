using System.Data;
using Pro.Common;

namespace Pro.CoreModel
{
    /// <summary>
    /// 执行结果返回信息类
    /// </summary>
    public class ReturnValue
    {
        #region 属性


        private bool _IsSuccess = false;
        private int _RetCode = 0;
        private string _RetMsg = string.Empty;
        private DataTable _RetDt = null;
        private int _OutCount = -1;
        private int _OutId = -1;
        private DataSet _RetDs = null;
        private object _RetObj = null;
        private object _RetObj1 = null;
        private object _RetObj2 = null;
        private object _RetObj3 = null;
        private object _RetObj4 = null;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return _IsSuccess; }
            set { _IsSuccess = value; }
        }

        /// <summary>
        /// 操作返回的代码
        /// </summary>
        public int RetCode
        {
            get { return _RetCode; }
            set { _RetCode = value; }
        }

        /// <summary>
        /// 操作返回的信息
        /// </summary>
        public string RetMsg
        {
            get { return _RetMsg; }
            set { _RetMsg = value; }
        }

        /// <summary>
        /// 操作返回的DataTable
        /// </summary>
        public DataTable RetDt
        {
            get { return _RetDt; }
            set { _RetDt = value; }
        }

        /// <summary>
        /// 操作返回的 记录数
        /// </summary>
        public int OutCount
        {
            get { return _OutCount; }
            set { _OutCount = value; }
        }

        /// <summary>
        /// 操作返回的 记录ID
        /// </summary>
        public int OutId
        {
            get { return _OutId; }
            set { _OutId = value; }
        }

        /// <summary>
        /// 操作返回的 DataSet
        /// </summary>
        public DataSet RetDs
        {
            get { return _RetDs; }
            set { _RetDs = value; }
        }

        /// <summary>
        /// 操作返回的对象
        /// </summary>
        public object RetObj
        {
            get { return _RetObj; }
            set { _RetObj = value; }
        }

        /// <summary>
        /// 操作返回的对象
        /// </summary>
        public object RetObj1
        {
            get { return _RetObj1; }
            set { _RetObj1 = value; }
        }

        /// <summary>
        /// 操作返回的对象
        /// </summary>
        public object RetObj2
        {
            get { return _RetObj2; }
            set { _RetObj2 = value; }
        }

        /// <summary>
        /// 操作返回的对象
        /// </summary>
        public object RetObj3
        {
            get { return _RetObj3; }
            set { _RetObj3 = value; }
        }

        /// <summary>
        /// 操作返回的对象
        /// </summary>
        public object RetObj4
        {
            get { return _RetObj4; }
            set { _RetObj4 = value; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 无参数构造函数(返回失败)
        /// </summary>
        public ReturnValue()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">操作是否成功的标志</param>
        public ReturnValue(bool isSuccess)
        {
            _IsSuccess = isSuccess;
        }

        /// <summary>
        /// 构造函数(当retObj有效时返回成功)
        /// </summary>
        /// <param name="retObj">操作返回的对象</param>
        public ReturnValue(object retObj)
        {
            _IsSuccess = true;
            _RetObj = retObj;

            if (retObj == null)
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(当retDt有效时返回成功)
        /// </summary>
        /// <param name="retDt">操作返回的对象</param>
        public ReturnValue(DataTable retDt)
        {
            _IsSuccess = true;
            _RetDt = retDt;

            if (!Tools.IsValidDt(retDt))
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(操作成功时)
        /// </summary>
        /// <param name="outId">操作成功的记录ID</param>
        public ReturnValue(int outId)
        {
            _IsSuccess = true;
            _OutId = outId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">操作成功的标志</param>
        /// <param name="retCode">操作返回的代码</param>
        public ReturnValue(bool isSuccess, int retCode)
        {
            _IsSuccess = isSuccess;
            _RetCode = retCode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">操作成功的标志</param>
        /// <param name="retMsg">操作返回的信息</param>
        public ReturnValue(bool isSuccess, string retMsg)
        {
            _IsSuccess = isSuccess;
            _RetMsg = retMsg;
        }

        /// <summary>
        /// 构造函数(当retDt有效时返回成功)
        /// </summary>
        /// <param name="retCode">操作返回的代码</param>
        /// <param name="retDt">操作返回的对象</param>
        public ReturnValue(int retCode, DataTable retDt)
        {
            _IsSuccess = true;
            _RetCode = retCode;
            _RetDt = retDt;

            if (!Tools.IsValidDt(retDt))
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(当retDt有效时返回成功)
        /// </summary>
        /// <param name="retDt">操作返回的对象</param>
        /// <param name="outCount">操作返回的对象的记录数</param>
        public ReturnValue(DataTable retDt, int outCount)
        {
            _IsSuccess = outCount >= 0;
            _RetDt = retDt;
            _OutCount = outCount;
            if (!Tools.IsValidDt(retDt))
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(当retObj不为NULL返回成功)
        /// </summary>
        /// <param name="retObj">操作返回的对象</param>
        /// <param name="outCount">操作返回的对象的记录数</param>
        public ReturnValue(object retObj, int outCount)
        {
            _IsSuccess = outCount >= 0;
            _OutCount = outCount;
            if (retObj == null)
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(当retObj有效时返回成功)
        /// </summary>
        /// <param name="retCode">操作返回的代码</param>
        /// <param name="retObj">操作返回的对象</param>
        public ReturnValue(int retCode, object retObj)
        {
            _IsSuccess = true;
            _RetCode = retCode;
            _RetObj = retObj;

            if (retObj == null)
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess">操作成功的标志</param>
        /// <param name="retCode">操作返回的代码</param>
        /// <param name="retMsg">操作返回的信息</param>
        public ReturnValue(bool isSuccess, int retCode, string retMsg)
        {
            _IsSuccess = isSuccess;
            _RetCode = retCode;
            _RetMsg = retMsg;
        }

        /// <summary>
        /// 构造函数(当retDt有效时返回成功)
        /// </summary>
        /// <param name="retCode">操作返回的代码</param>
        /// <param name="retMsg">操作返回的信息</param>
        /// <param name="retDt">操作返回的对象</param>
        public ReturnValue(int retCode, string retMsg, DataTable retDt)
        {
            _IsSuccess = true;
            _RetCode = retCode;
            _RetMsg = retMsg;
            _RetDt = retDt;

            if (!Tools.IsValidDt(retDt))
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        /// <summary>
        /// 构造函数(当retObj有效时返回成功)
        /// </summary>
        /// <param name="retCode">操作返回的代码</param>
        /// <param name="retMsg">操作返回的信息</param>
        /// <param name="retObj">操作返回的对象</param>
        public ReturnValue(int retCode, string retMsg, object retObj)
        {
            _IsSuccess = true;
            _RetCode = retCode;
            _RetMsg = retMsg;
            _RetObj = retObj;

            if (retObj == null)
            {
                _IsSuccess = false;
                _RetMsg = Consts.ERR_InvalidObj;
            }
        }

        #endregion
    }
}
