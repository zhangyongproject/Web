using System;
using System.Collections.Generic;
using System.Text;

namespace Krs.Base.Logs
{
    /// <summary>
    /// 计算执行时间差
    /// </summary>
    public class RunTimeLoger
    {
        private string[] runName;
        private DateTime[] startTime;
        private DateTime endTime;
        //private int maxMilliseconds = 100

        private int currStep = -1;
        private int maxStep = 1;

        private LogLevel level = LogLevel.INFO;
        private string userNumber = string.Empty;
        private string _fullMethodName = string.Empty;

        /// <summary>
        /// 开始计算
        /// </summary>
        public void Start()
        { }

        /// <summary>
        /// 停止计算时间，写入日志
        /// </summary>
        public void Stop()
        {
          // LogMessageHelper.Logger(LogOutStyle 
        }
    }
}
