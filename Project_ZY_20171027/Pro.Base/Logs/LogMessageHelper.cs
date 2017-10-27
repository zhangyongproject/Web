using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using log4net;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace Pro.Base.Logs
{
    public sealed class LogMessageHelper
    {
        private static  object curLock = new object();
        ///// <summary>
        ///// 写文件日志ILog
        ///// </summary>
        //private static readonly ILog logFile = log4net.LogManager.GetLogger("FileLog");

        ///// <summary>
        ///// 写数据库日志ILog
        ///// </summary>
        //private static readonly ILog logDB = log4net.LogManager.GetLogger("DataBaseLog");


        /// <summary>
        /// info日志的Log
        /// </summary>
        private static readonly ILog logInfo = log4net.LogManager.GetLogger("info");

        /// <summary>
        /// Error日志的Log
        /// </summary>
        private static readonly ILog logError = log4net.LogManager.GetLogger("error");

        /// <summary>
        /// Debug日志的Log
        /// </summary>
        private static readonly ILog logDebug = log4net.LogManager.GetLogger("debug");


        private delegate void WriteLogDelegate( LogLevel level, LogMessageInfo info);//, Exception ex);
        /// <summary>
        /// 当前实例对象所用的Ilog
        /// </summary>
        private readonly ILog log;
        private LogMessageHelper(LogLevel level) //(LogOutStyle outStyle)
        {
            switch (level)
            {
                case LogLevel.DEBUG://   LogOutStyle.FileLog:  //文本日志
                    log = logDebug; // LogManager.GetLogger("FileLog");
                    break;
                case LogLevel.ERROR:
                    log = logError;// logDB;// LogManager.GetLogger("DataBaseLog");
                    break;
                case LogLevel.INFO:
                    log = logInfo;
                    break;
            }
        }

        private void WriteLog(LogLevel level,LogMessageInfo info)//,Exception ex)
        {
            lock (curLock)
            {
                switch (level)
                {
                    //case LogLevel.FATAL:
                    //    if (log.IsFatalEnabled)
                    //    {
                    //        log.Fatal(info);
                    //    }
                    //    break;
                    case LogLevel.ERROR:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(info);
                        }
                        break;
                    //case LogLevel.WARN:
                    //    if (log.IsWarnEnabled)
                    //    {
                    //        log.Warn(info);
                    //    }
                    //    break;
                    case LogLevel.INFO:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(info);
                        }
                        break;
                    case LogLevel.DEBUG:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(info);
                        }
                        break;
                }
            }
        }


        //private void WriteLog(RunTimeLoger rtl)
        //{
        //    //if (log.IsInfoEnabled)
        //    //{
        //    //    log.Info(rtl.loginfo,
        //    //}
        
        //}
//LogOutStyle outStyle,
        /// <summary>
        /// 写日志统一入口
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="level"></param>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        private static void AsyWriteLog(LogLevel level, LogMessageInfo info)//, Exception ex)
        {   
            //lock (curLock)
            //{
            LogMessageHelper logHelper = new LogMessageHelper(level);//LogOutStyle.FileLog);
                WriteLogDelegate logDeletegate = logHelper.WriteLog;
                logDeletegate.BeginInvoke(level, info, null, null);// OnMethodWriteLog, null);
            //}
        }
        private static void OnMethodWriteLog(IAsyncResult asynResult)
        {
            WriteLogDelegate caller = (WriteLogDelegate)((AsyncResult)asynResult).AsyncDelegate;
             caller.EndInvoke(asynResult);
        }

        #region GetInvokeDesc
        /// <summary>
        /// 调用写日志方法的当前类与方法信息(GetMessageInfo中调用)
        /// </summary>
        /// <remarks>默认调用堆栈为
        /// 0: GetInvokeDesc();
        /// 1:LogWrite
        /// 2: LogINFO或LogDebug或LogError 
        /// 3:调用日志记录方法的外部方法描述
        /// </remarks>
        /// <returns></returns>
        private static string GetInvokeDesc()
        {
            //stack: 0: GetInvokeDesc(); 
            //1: LogWrite
            //2: LogINFO或LogDebug或LogError 
            //3:调用日志记录方法的外部方法描述
            int stack = 3;
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(stack);
            if (sf == null || sf.GetFileName() == null)
            {
                sf = st.GetFrame(1);
            }
            //E:\\VSSWork\\2_SPDT\\02_源代码提交区\\MA3_V2.X\\DataAnalysis_BS\\Pro.MngBase\\MngBase\\RightMng.cs

            //调用日志方法的文件名
            string callFileName = sf.GetFileName();
          
            //调用日志方法的方法名
            string callMethod = sf.GetMethod().ToString();
            //调用日志方法的行号
            int callLine = sf.GetFileLineNumber();

            string callDesc = string.Format("fill:{0},method:{1},line:{2}",
                callFileName, callMethod, callLine);
            return callDesc;//.GetMethod().Name.ToString();
        }
        #endregion

        private static void LogWrite( LogLevel level,
            string msg,string logClass,string subClass, string pars,
            string results, Exception ex)
        {
            LogMessageInfo info = new LogMessageInfo();
            if (pars != null) // && pars.Count > 0)
            {
                info.ParamDesc = pars;// GetMessage(pars);
            }
            if (results != null)//&& results.Count > 0)
            {
                info.ResultDesc = results;// GetMessage(results);
            }
            info.Details = msg;

            info.CallDesc = GetInvokeDesc();
            info.Level = level;
            if (ex != null)
            {
                info.ErrorMsg = string.Format("msg:{0};stack:{1}", ex.Message, ex.StackTrace);
            }
            if (!string.IsNullOrEmpty(logClass))
            {
                info.LogClass = logClass;
            }
            if (!string.IsNullOrEmpty(subClass))
            {
                info.SubClass = subClass;               
            }
            AsyWriteLog(level, info);           
        }

          //private static LogMessageInfo GetLogMessageInfo(string msg,Dictionary<string, string> pars, 
        //    Dictionary<string, string> results)
        //{
        //    LogMessageInfo info = new LogMessageInfo();

        //    info.ParamDesc = GetMessage(pars);
        //    info.ResultDesc = GetMessage(results);
        //    info.Details = msg;

        //    info.CallDesc = GetInvokeDesc();
        //    return info;
        //}

        /// <summary>
        /// 获取字典字符表示
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string GetMessage(Dictionary<string, string> dir)
        {
            if (dir == null
                || dir.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> value in dir)
            {
                sb.AppendFormat("{0}:{1}|", value.Key, value.Value);
            }
            int len = sb.ToString().Length;
            return sb.ToString().Substring(0, len - 1);
        }

        /// <summary>
        /// 获取字典字符表示
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string GetMessage(Dictionary<string, object> dir)
        {
            if (dir == null
                || dir.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> value in dir)
            {
                sb.AppendFormat("{0}:{1}|", value.Key, value.Value);
            }
            int len = sb.ToString().Length;
            return sb.ToString().Substring(0, len - 1);
        }
        #region 公共调用接口

        #region 完整形式调用
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogDEBUG(string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.DEBUG, msg, logClass, subClass, GetMessage(pars), GetMessage(results), ex);
        }

        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogINFO(string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.INFO, msg, logClass, subClass,  GetMessage(pars), GetMessage(results), ex);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogERROR(string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.ERROR, msg, logClass, subClass,  GetMessage(pars), GetMessage(results), ex);
        }
        #endregion

        #region 完整形式调用(不同的参数与返回值参数)
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogDEBUG(  string msg, string logClass, string subClass,
            Dictionary<string, object> pars, Dictionary<string, object> results, Exception ex)
        {
            LogWrite(LogLevel.DEBUG, msg, logClass, subClass, GetMessage(pars), GetMessage(results), ex);
        }

        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogINFO(  string msg, string logClass, string subClass,
            Dictionary<string, object> pars, Dictionary<string, object> results, Exception ex)
        {
            LogWrite(LogLevel.INFO, msg, logClass, subClass, GetMessage(pars), GetMessage(results), ex);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogERROR(string msg, string logClass, string subClass,
            Dictionary<string, object> pars, Dictionary<string, object> results, Exception ex)
        {
            LogWrite(LogLevel.ERROR, msg, logClass, subClass, GetMessage(pars), GetMessage(results), ex);
        }
        #endregion

        #region 没有异常信息
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="logClass">日志大类描述</param>
        /// <param name="subClass">日志小类描述</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        public static void LogDEBUG( string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.DEBUG, msg, logClass, subClass, GetMessage(pars), GetMessage(results), null);
        }

        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="logClass">日志大类描述</param>
        /// <param name="subClass">日志小类描述</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        public static void LogINFO(   string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.INFO, msg, logClass, subClass, GetMessage(pars), GetMessage(results), null);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="logClass">日志大类描述</param>
        /// <param name="subClass">日志小类描述</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        public static void LogERROR(  string msg, string logClass, string subClass, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.ERROR, msg, logClass, subClass, GetMessage(pars), GetMessage(results), null);
        }
        #endregion

        #region 没有logclass与subClss调用
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogDEBUG(  string msg,  Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.DEBUG, msg, null, null, GetMessage(pars), GetMessage(results), ex);
        }
        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogINFO(  string msg,  Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.INFO, msg, null, null, GetMessage(pars), GetMessage(results), ex);
        }

        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogERROR(  string msg,  Dictionary<string, string> pars, Dictionary<string, string> results, Exception ex)
        {
            LogWrite(LogLevel.ERROR, msg, null, null, GetMessage(pars), GetMessage(results), ex);
        }
        #endregion


        #region 只有msg日志
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="msg">日志的描述信息</param>
        public static void LogDEBUG(string msg)
        {
            LogWrite( LogLevel.DEBUG, msg, null, null, null, null, null);
        }
        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogINFO(string msg)
        {
            LogWrite( LogLevel.INFO, msg, null, null, null, null, null);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogERROR(string msg)
        {
            LogWrite( LogLevel.ERROR, msg, null, null, null, null, null);
        }
        #endregion

        #region msg、Exception
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="msg">日志的描述信息</param>
        public static void LogDEBUG(string msg,Exception ex)
        {
            LogWrite(LogLevel.DEBUG, msg, null, null, null, null,ex);
        }
        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogINFO(string msg, Exception ex)
        {
            LogWrite(LogLevel.INFO, msg, null, null, null, null, ex);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息</param>
        /// <param name="pars">参数信息列表</param>
        /// <param name="results">返回值信息列表</param>
        /// <param name="ex">异常信息</param>
        public static void LogERROR(string msg, Exception ex)
        {
            LogWrite(LogLevel.ERROR, msg, null, null, null, null,ex);
        }
        #endregion

        #region  msg,pars，results
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogDEBUG(string msg, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.DEBUG, msg, null, null, GetMessage(pars), GetMessage(results), null);
        }

        /// <summary>
        /// 写入INFO日志
        /// </summary> 
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogINFO(string msg, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.INFO, msg, null, null, GetMessage(pars), GetMessage(results), null);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogERROR(string msg, Dictionary<string, string> pars, Dictionary<string, string> results)
        {
            LogWrite(LogLevel.ERROR, msg, null, null, GetMessage(pars), GetMessage(results), null);
        }
        #endregion

        #region  无pars与results
        /// <summary>
        /// 写入DEBUG日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogDEBUG(string msg, string logClass, string subClass,  Exception ex)
        {
            LogWrite(LogLevel.DEBUG, msg, logClass, subClass, null, null, ex);
        }

        /// <summary>
        /// 写入INFO日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="pars">参数信息列表(可以为null)</param>
        /// <param name="results">返回值信息列表(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogINFO(string msg, string logClass, string subClass,  Exception ex)
        {
            LogWrite(LogLevel.INFO, msg, logClass, subClass, null, null, ex);
        }
        /// <summary>
        /// 写入ERROR日志
        /// </summary>
        /// <param name="outStyle">日志输出类型 FileLog或DataBaseLog</param>
        /// <param name="msg">日志的描述信息(不能为null）</param>
        /// <param name="logClass">日志大类描述(可以为null)</param>
        /// <param name="subClass">日志小类描述(可以为null)</param>
        /// <param name="ex">异常信息(可以为null)</param>
        public static void LogERROR(string msg, string logClass, string subClass,  Exception ex)
        {
            LogWrite(LogLevel.ERROR, msg, logClass, subClass, null, null, ex);
        }
        #endregion

        #endregion
    }
}
