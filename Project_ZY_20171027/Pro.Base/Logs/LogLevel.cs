using System;
using System.Collections.Generic;
using System.Text;

namespace Pro.Base.Logs
{
    public enum LogLevel
    {
        /// <summary>
        /// 调试日志
        /// </summary>
        DEBUG = 0,

        /// <summary>
        /// 信息日志 
        /// </summary>
        INFO = 1,

        ///// <summary>
        ///// 警告日志
        ///// </summary>
        //WARN = 2,

        /// <summary>
        /// 错误日志
        /// </summary>
        ERROR = 3,

        ///// <summary>
        ///// 致命错误日志
        ///// </summary>
        //FATAL = 4
    }
    
    /// <summary>
    ///  日志输出类型(文件日志与数据库日志)
    /// </summary>
    //public enum LogOutStyle
    //{
    //    ///// <summary>
    //    ///// 文件日志
    //    ///// </summary>
    //    //FileLog = 0,
    //    ///// <summary>
    //    ///// 数据库日志
    //    ///// </summary>
    //    //DataBaseLog = 1
    //}
}
