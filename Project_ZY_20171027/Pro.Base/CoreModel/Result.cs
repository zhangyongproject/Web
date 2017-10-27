using System;
using System.Collections.Generic;
using System.Text;

namespace Pro.CoreModel
{
    /// <summary>
    /// 操作返回信息定义
    /// </summary>
    [Serializable]
    public  class Result<T>
    {
        #region 构造函数
        public Result()
        { 
        }
        public Result(bool isSuccess,string code,string msg)
        {
            _IsSuccess = isSuccess;
            _Code = code;
            _Message = msg; 
        }
        public Result(bool isSuccess, string code, string msg, T detail)
        {
            _IsSuccess = isSuccess;
            _Code = code;
            _Message = msg;
            _detail = detail;
        }

        public Result(bool isSuccess, string code, string msg, T detail, int outCount)
        {
            _IsSuccess = isSuccess;
            _Code = code;
            _Message = msg;
            _detail = detail;
            _OutCount = outCount;
        }
        #endregion

        #region 返回的描述信息
        private bool _IsSuccess = false;
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return _IsSuccess; }
            set { _IsSuccess = value; }
        }

        private string _Code = string.Empty;
        /// <summary>
        /// 操作返回的代码
        /// (000:代表正确执行并返回数据,对应Consts.ResultCode_Succeed)
        /// (999:代表程序程序出错,对应Consts.ResultCode_Error)
        /// </summary>
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private string _Message = string.Empty;
        /// <summary>
        /// 操作返回的描述信息
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        #endregion

        #region 返回的具体数据
        private int _OutCount = 0;
        /// <summary>
        /// 操作返回的总记录数（保留）
        /// </summary>
        public int OutCount
        {
            get { return _OutCount; }
            set { _OutCount = value; }
        }

        private T _detail;
        /// <summary>
        /// 返回信息的详情信息
        /// </summary>
        public T Detail
        {
            get { return _detail; }
            set { _detail = value; }
        }
        #endregion
    }
}
