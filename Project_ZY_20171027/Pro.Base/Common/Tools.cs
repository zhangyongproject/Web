using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Security.Cryptography;
using System.Text;
using Pro.Base.Logs;

namespace Pro.Common
{
    public class Tools
    {
        #region 类型转换
        /// <summary>
        /// 得到一个Int32值
        /// </summary>
        /// <param name="obj">传入的对象</param>
        /// <param name="defaultVal">当对象不能转换为Int32时返回的默认值</param>
        /// <returns>转换后得到的Int32值</returns>
        public static int GetInt32(object obj, int defaultVal)
        {
            if (obj == null)
            {
                return defaultVal;
            }
            int retVal;
            if (int.TryParse(obj.ToString(), out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Int32值
        /// </summary>
        /// <param name="str">传入的字符串</param>
        /// <param name="defaultVal">当对象不能转换为Int32时返回的默认值</param>
        /// <returns>转换后得到的Int32值</returns>
        public static int GetInt32(string str, int defaultVal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultVal;
            }
            int retVal;
            if (int.TryParse(str, out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Int32值(从十六进制数字字符串)
        /// </summary>
        /// <param name="str">十六进制数字字串</param>
        /// <param name="defaultVal">当对象不能转换为Int32时返回的默认值</param>
        /// <returns>十进制数字</returns>
        public static int GetInt32ByHex(string str, int defaultVal)
        {
            int retVal;
            try
            {
                retVal = Convert.ToInt32(str, 16);
            }
            catch
            {
                retVal = defaultVal;
            }
            return retVal;
        }

        /// <summary>
        /// 得到一个Char值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static char GetChar(object obj, char defaultVal)
        {
            char retVal;
            if (char.TryParse(obj.ToString(), out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Decimal值
        /// </summary>
        /// <param name="obj">传入的对象</param>
        /// <param name="defaultVal">当对象不能转换为Decimal时返回的默认值</param>
        /// <returns>转换后得到的Decimal值</returns>
        public static decimal GetDecimal(object obj, decimal defaultVal)
        {
            decimal retVal;
            if (decimal.TryParse(obj.ToString(), out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Decimal值
        /// </summary>
        /// <param name="str">传入的字符串</param>
        /// <param name="defaultVal">当对象不能转换为Decimal时返回的默认值</param>
        /// <returns>转换后得到的Decimal值</returns>
        public static decimal GetDecimal(string str, decimal defaultVal)
        {
            decimal retVal;
            if (decimal.TryParse(str, out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Double值
        /// </summary>
        /// <param name="obj">传入的对象</param>
        /// <param name="defaultVal">当对象不能转换为Double时返回的默认值</param>
        /// <returns>转换后得到的Double值</returns>
        public static double GetDouble(object obj, double defaultVal)
        {
            double retVal;
            if (double.TryParse(obj.ToString(), out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个Double值
        /// </summary>
        /// <param name="str">传入的字符串</param>
        /// <param name="defaultVal">当对象不能转换为Double时返回的默认值</param>
        /// <returns>转换后得到的Double值</returns>
        public static double GetDouble(string str, double defaultVal)
        {
            double retVal;
            if (double.TryParse(str, out retVal))
                return retVal;
            return defaultVal;
        }

        /// <summary>
        /// 得到一个String值
        /// </summary>
        /// <param name="obj">传入的对象</param>
        /// <param name="defaultVal">当字符串为空时返回的默认值</param>
        /// <returns>转换后得到的String值</returns>
        public static string GetString(object obj, string defaultVal)
        {
            if (obj == null)
                return defaultVal;
            return obj.ToString();
        }

        /// <summary>
        /// 得到一个DateTime类型的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string str, DateTime defaultVal)
        {
            DateTime dt;
            if (DateTime.TryParse(str, out dt))
            {
                return dt;
            }
            else
            {
                return defaultVal;
            }
        }
        #endregion

        #region 获取...
        /// <summary>
        /// 得到一个配置的键值
        /// </summary>
        /// <param name="keyName">配置的键名</param>
        /// <returns>配置的键值</returns>
        public static string GetAppSetting(string keyName, string defaultVal)
        {
            string ret = System.Configuration.ConfigurationManager.AppSettings[keyName];
            if (ret == null)
                return defaultVal;
            return ret;
        }

        /// <summary>
        /// MD5 32位加密
        /// </summary>
        /// <param name="src">输入的数据</param>
        /// <returns></returns>
        public static string GetMD5Str(string src)
        {
            //string pwd = "";
            StringBuilder sbPwd = new StringBuilder(32);
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(src));
            for (int i = 0; i < s.Length; i++)
            {
                sbPwd.Append(s[i].ToString("X").PadLeft(2,'0'));    //补齐两位
            }
            return sbPwd.ToString();
        }

        /// <summary>
        /// 对应JS中的Escape方法,在无法使用Server.UrlEncode时使用
        /// </summary>
        /// <param name="s">中文或特殊字符</param>
        /// <returns>编码的字符,可由JS unescape 方法解码</returns>
        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = System.Text.Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < ba.Length; i += 2)
            {
                if (ba[i + 1] == 0)
                {
                    if ((ba[i] >= 48 && ba[i] <= 57)
                        || (ba[i] >= 64 && ba[i] <= 90)
                        || (ba[i] >= 97 && ba[i] <= 122)
                        || (ba[i] == 42 || ba[i] == 43 || ba[i] == 45 || ba[i] == 46 || ba[i] == 47 || ba[i] == 95)
                        )
                    {
                        sb.Append(Encoding.Unicode.GetString(ba, i, 2));

                    }
                    else //%xx形式
                    {
                        sb.Append("%");
                        sb.Append(ba[i].ToString("X2"));
                    }
                }
                else
                {
                    sb.Append("%u");
                    sb.Append(ba[i + 1].ToString("X2"));
                    sb.Append(ba[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 截取字符串的前N位字符
        /// </summary>
        /// <param name="s">指定的字符串</param>
        /// <param name="n">指定的长度</param>
        /// <returns></returns>
        public static string GetLenthStr(string s, int n)
        {
            if (s.Length > n)
                return s.Substring(0, n);
            return s;
        }
        #endregion

        #region System.Data 相关
        /// <summary>
        /// 复制DataRow[]中的指定记录到一个新的DataTable
        /// </summary>
        /// <param name="drs"></param>
        /// <param name="startRecordIndex"></param>
        /// <param name="endRecordIndex"></param>
        /// <returns></returns>
        public static DataTable GetDt4Drs(DataRow[] drs, int startRecord, int endRecord)
        {
            if (startRecord < 0) startRecord = 0;
            if (endRecord < 0 || endRecord >= drs.Length) endRecord = drs.Length - 1;

            DataSet ds = new DataSet("items");
            DataTable dt = new DataTable("item");
            ds.Tables.Add(dt);
            if (drs.Length > 0)
                dt = drs[0].Table.Clone();
            else
                return null;

            for (int i = startRecord; i <= endRecord; i++)
                dt.Rows.Add(drs[i].ItemArray);
            return dt;
        }

        /// <summary>
        /// 复制DataRow[]中的所有记录到一个新的DataTable
        /// </summary>
        /// <param name="drs"></param>
        /// <returns></returns>
        public static DataTable GetDt4Drs(DataRow[] drs)
        {
            return GetDt4Drs(drs, 0, -1);
        }

        /// <summary>
        /// 复制DataTable中的指定记录到一个新的DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startRecord"></param>
        /// <param name="endRecord"></param>
        /// <returns></returns>
        public static DataTable GetDt4Drs(DataTable dt, int startRecord, int endRecord)
        {
            if (dt == null)
            {
                return null;
            }
            DataRow[] drs = dt.Select();
            return GetDt4Drs(drs, startRecord - 1, endRecord - 1);
        }

        /// <summary>
        /// 为一个未知是否存在于DataSet中的DataTable包装DataSet
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataSet GetTableDs(DataTable dt, bool bRemoveOtherTable)
        {
            DataSet ds;
            if (dt == null)
            {
                dt = new DataTable();
            }
            if (dt.DataSet == null)
            {
                ds = new DataSet();
                ds.Tables.Add(dt);
            }
            else
            {
                if (bRemoveOtherTable)
                {
                    DataTable dtTemp = dt.Copy();
                    ds = new DataSet();
                    ds.Tables.Add(dtTemp);
                }
                else
                {
                    ds = dt.DataSet;
                }
            }
            return ds;
        }

        /// <summary>
        /// 判断DataTable是否有效
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsValidDt(DataTable dt)
        {
            if (dt == null || dt.Columns.Count < 1)
                return false;
            return true;
        }

        /// <summary>
        /// 获取交叉表(将原来的行表,转化成交叉表)
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="arrCols">增加多列并指定列名</param>
        /// <param name="srcCol">指定将列名对应到值的字段,如arrCols为{"200901","200902","200903"},那么srcCol指定的列中,应包括对应的值</param>
        /// <param name="valCol">取值字段</param>
        /// <returns></returns>
        public static DataTable GetCrossDt(DataTable dt, string[] arrCols, string keyCol, string srcCol, string valCol)
        {
            if (!Tools.IsValidDt(dt) || dt.Rows.Count < 1 || dt.Rows.Count > 300 || arrCols.Length < 1 || valCol.Length < 1 ||
                !dt.Columns.Contains(srcCol) || !dt.Columns.Contains(valCol))
            {
                return dt;
            }

            DataTable ret = dt.Copy();
            Type valType = dt.Columns[valCol].DataType;
            foreach (string col in arrCols)
            {
                if (ret.Columns.Contains(col))
                    continue;
                if (col.Length > 0)
                    ret.Columns.Add(col, valType);
            }

            IList<string> listCols = arrCols as IList<string>;
            DataRow mainDr = ret.Rows[0];
            int mainIndex = 0;
            string currKeyColValue = ret.Rows[0][keyCol].ToString();
            int nLen = ret.Rows.Count;
            for (int i = 0; i < nLen; i++)
            {
                if (!listCols.Contains(ret.Rows[i][srcCol].ToString()))
                {
                    ret.Rows.Remove(ret.Rows[i]);
                    nLen = ret.Rows.Count;
                    i--;
                    continue;
                }
                if (currKeyColValue == ret.Rows[i][keyCol].ToString())
                {
                    mainDr[ret.Rows[i][srcCol].ToString()] = ret.Rows[i][valCol].ToString();
                    if (mainIndex != i)
                    {
                        ret.Rows.Remove(ret.Rows[i]);
                        nLen = ret.Rows.Count;
                        i--;
                    }
                }
                else
                {
                    currKeyColValue = ret.Rows[i][keyCol].ToString();
                    mainDr = ret.Rows[i];
                    mainIndex = i;
                    i--;
                }
            }
            return ret;
        }

        /// <summary>
        /// 从DataTable的一列中,获取数据组成的字符串数组(该数组经过Distinct)
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="colName">列名</param>
        /// <returns></returns>
        public static DataTable ComboDtCol(DataTable dt, string colName, string format, string[] srcCols)
        {
            dt.Columns.Add(colName);
            Queue<string> q = new Queue<string>();
            foreach (DataRow dr in dt.Rows)
            {
                foreach (string s in srcCols)
                {
                    q.Enqueue(dr[s].ToString());
                }
                dr[colName] = string.Format(format, q.ToArray());
                q.Clear();
            }
            return dt;
        }

        //public static DataTable ComboDtCol(DataTable dt, string colName, string format, string[] srcCols)
        //{
        //    if (!IsValidDt(dt) || dt.Columns.Contains(colName))
        //        return null;
        //    dt.Columns.Add(colName);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        dr[colName] = string.Format(format, srcCols);
        //    }
        //    return dt;
        //}

        /// <summary>
        /// 从DataTable的一列中,获取数据组成的字符串数组(该数组经过Distinct)
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="colName">列名</param>
        /// <returns></returns>
        public static string[] GetDtFieldDistValue(DataTable dt, string colName)
        {
            if (!IsValidDt(dt) || !dt.Columns.Contains(colName))
                return new string[0] { };
            Queue<string> s = new Queue<string>();
            foreach (DataRow dr in dt.Rows)
            {
                if (!s.Contains(dr[colName].ToString()))
                    s.Enqueue(dr[colName].ToString());
            }
            return s.ToArray();
        }
        #endregion

        #region 时间相关
        /// <summary>
        /// 格式化分钟
        /// </summary>
        /// <param name="minute">分钟</param>
        /// <returns></returns>
        public static string GetMinuteFormat(double minute)
        {
            string str = string.Empty;
            int dRes = 0;
            int mRes = 0;
            if (minute > 1440)
            {
                dRes = (int)Math.Truncate(minute / 1440);
                str = dRes + "天";
            }
            if (minute - (dRes * 1440) > 60)
            {
                mRes = (int)Math.Truncate((minute - (dRes * 1440)) / 60);
                str += mRes + "时";
            }
            str += minute - ((dRes * 1440) + (mRes * 60)) + "分";
            return str;
        }

        /// <summary>
        /// 时间格式化(字符串) 如:20090705124243-> 2009 07 05 12:42:43 
        /// </summary>
        /// <param name="strTime">时间字符串</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFmtStr(string strTime)
        {
            int length = strTime.Length;
            if (length == 0)
                throw new ArgumentException("时间字符参数无效。");
            DateTime result = new DateTime(1, 1, 1, 0, 0, 0, 0);
            if (length >= 4)
                result = result.AddYears(int.Parse(strTime.Substring(0, 4)) - 1);    //年

            if (length >= 6)
                result = result.AddMonths(int.Parse(strTime.Substring(4, 2)) - 1);   //月

            if (length >= 8)
                result = result.AddDays(int.Parse(strTime.Substring(6, 2)) - 1);     //日

            if (length >= 10)
                result = result.AddHours(int.Parse(strTime.Substring(8, 2)));        //时

            if (length >= 12)
                result = result.AddMinutes(int.Parse(strTime.Substring(10, 2)));     //分

            if (length >= 14)
                result = result.AddSeconds(int.Parse(strTime.Substring(12, 2)));     //秒

            if (length >= 17)
                result = result.AddMilliseconds(int.Parse(strTime.Substring(14, 3)));//毫秒
            return result;
        }
        #endregion

        #region 获取日期的月天数(格式:yyyy-mm)
        /// <summary>
        /// 获取日期的月天数(格式:yyyy-mm)
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDay4String(string date)
        {
            string result = "31";
            int month = Convert.ToInt32(date.Split('-')[1]);
            if (string.Compare(date.Split('-')[1], "02") == 0)
                result = DateTime.IsLeapYear(Convert.ToInt32(date.Split('-')[0])) ? "29" : "28";
            else if (month == 4 || month == 6 || month == 7 || month == 9 || month == 11)
                result = "30";
            else
                result = "31";
            return "-" + result;
        }
        #endregion

        #region 将有可能为DBNULL或NULL类型转换为String类型
        /// <summary>
        /// 将有可能为DBNULL或NULL类型转换为String类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DbNullToString(object value)
        {
            if (value == DBNull.Value || value == null)
                return "";
            else
                return value.ToString();
        }
        #endregion

        #region 其它方法
        /// <summary>
        /// 获取分页起始索引
        /// </summary>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前是第几页</param>
        /// <returns></returns>
        public static int GetStartRec(ref int pageSize, ref int pageIndex)
        {
            if (pageSize < 1) pageSize = 10;
            if (pageIndex < 1) pageIndex = 1;
            return Tools.GetInt32((pageSize * (pageIndex - 1) + 1), 1);
        }

        /// <summary>
        /// 获取分页终止索引
        /// </summary>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前是第几页</param>
        /// <returns></returns>
        public static int GetEndRec(ref int pageSize, ref int pageIndex)
        {
            if (pageSize < 1) pageSize = 10;
            if (pageIndex < 1) pageIndex = 1;
            return pageSize * pageIndex;
        }

        /// <summary>
        /// 获取分页起始索引
        /// </summary>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前是第几页</param>
        /// <returns></returns>
        public static int GetStartRec(int pageSize, int pageIndex)
        {
            if (pageSize < 1) pageSize = 10;
            if (pageIndex < 1) pageIndex = 1;
            return Tools.GetInt32((pageSize * (pageIndex - 1) + 1), 1);
        }

        /// <summary>
        /// 获取分页终止索引
        /// </summary>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前是第几页</param>
        /// <returns></returns>
        public static int GetEndRec(int pageSize, int pageIndex)
        {
            if (pageSize < 1) pageSize = 10;
            if (pageIndex < 1) pageIndex = 1;
            return pageSize * pageIndex;
        }
        #endregion

        #region Pro 相关方 法字符串数组:[机构代码, 应用程序标识, 使用截止日期]
        /// <summary>
        /// 检查License并返回相关值
        /// </summary>
        /// <param name="licenseFilePath">License文件的路径</param>
        /// <returns>字符串数组:[返回值标识,应用程序标识, 使用截止日期]
        ///   返回值标识: -1:日期过期
        ///               -2:授权码错误
        ///               -9：系统错误
        ///               0：正确返回
        /// </returns>
        public static string[] CheckLicense(string licenseFilePath)
        {
            try
            {
                string text = FileHelper.GetFileText(licenseFilePath);
                string[] texts = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                // 判断授权是否过期
                DateTime dtSrc = Tools.GetDateTime(texts[1], DateTime.Now.AddYears(-100));
                DateTime dtNow = DateTime.Now;
                // 授权过期
                if (dtSrc.AddDays(1) < dtNow)
                    return new string[] { "-1", "-1", "" };// 

                // 判断密钥串是否正确 + texts[2] + "\r\n"
                string keyMust = Tools.GetMD5Str(texts[0] + "\r\n" + texts[1] + "\r\n"  + Consts.KEY_SECRETSTR);
                // 授权码错误
                if (keyMust != texts[2])
                    return new string[] { "-2", "-2", "" };//

                return new string[] { "0",texts[0], texts[1]};// , texts[2] 
            }
            catch(Exception ex)
            {
                Dictionary<string, string> logParms = new Dictionary<string, string>();
                logParms["licenseFilePath"] = licenseFilePath;
                //系统错误[日志]
                LogMessageHelper.LogINFO("检查License出错", logParms, null,ex);
                return new string[] { "-9", "-3", "" };//"-3",
            }
        }

        /// <summary>
        /// 获取项目的版本字符串
        /// 要得到理想的版本字符串,请在AppSettings中配置Pro.ProjName,Pro.ProjVer,Pro.ProjSubVer等值
        /// </summary>
        /// <param name="verType">要求返回值的格式[0=完全组合/1=不含内部版本/2=不含项目名称/3=仅项目名称/4=仅版本号]</param>
        /// <param name="projKey">项目名称:如 XXX,则将取配置文件中的 XXX.ProjName 等配置</param>
        /// <returns></returns>
        public static string GetProjectVer(int verType, string projKey)
        {
            string projName, projVer, projSubVer;
            string ret = "";
            if (projKey.Length > 0) projKey += ".";
            projName = Tools.GetAppSetting(projKey + "ProjName", "Unknown");
            projVer = Tools.GetAppSetting(projKey + "ProjVer", "0.00");
            projSubVer = Tools.GetAppSetting(projKey + "ProjSubVer", "0000");

            if (verType != 2 && verType != 4)
                ret += projName;
            if (verType != 3)
                ret += " " + projVer;
            if (verType != 1 && verType != 3 && verType != 4)
                ret += "." + projSubVer;

            return ret.Trim();
        }

        /// <summary>
        ///  获取项目的版本字符串
        /// </summary>
        /// <param name="projKey">项目名称:如 Pro,则将取配置文件中的 Pro.ProjName 等配置</param>
        /// <returns></returns>
        public static string GetProjectVer(string projKey)
        {
            return GetProjectVer(0, projKey);
        }

        /// <summary>
        /// 获取应用程序标识
        /// 要得到理想的值,需要在web.config中配置AppFlag的值
        /// </summary>
        /// <returns>0=通用 1=MA3 2=MD3 ...</returns>
        public static int GetAppFlag()
        {
            return Tools.GetInt32(Tools.GetAppSetting("AppFlag", "0"), 0);
        }

        /// <summary>
        /// 获取OracleParameterCollection中的out_code返回值
        /// </summary>
        /// <param name="opc">被执行后的OracleParameterCollection</param>
        /// <returns></returns>
        public static int GetOutCode(OracleParameterCollection opc)
        {
            return Tools.GetInt32(opc["out_code"].Value, -9);
        }

        /// <summary>
        /// 写入系统日志(用于Windows系统)
        /// 要得到理想的日志,请在AppSettings中配置Pro.ProjName,Pro.ProjVer,Pro.ProjSubVer等值
        /// </summary>
        /// <param name="source">错误源</param>
        /// <param name="message">错误信息</param>
        /// <param name="type">错误级别</param>
        public static void WriteSysLog(string source, string message, System.Diagnostics.EventLogEntryType type)
        {
            try
            {
                string projectInfo = GetProjectVer(0, "Pro");
                System.Diagnostics.EventLog.WriteEntry(projectInfo, source + "\n" + message, type);
            }
            catch
            {
            }
        }
        #endregion

        #region 恶意字符处理
        /// <summary>
        /// 判断是否包含恶意字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckSpiteChar(string str)
        {
            string[] aryReg = { "'", "%", "*", "\\", "~", "/", "^", "#", "!" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                if (str.Contains(aryReg[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 过滤恶意字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterSpiteChar(string str)
        {
            string[] aryReg = { "'", "%", "*", "\\", "~", "/", "^", "#", "!" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                str = str.Replace(aryReg[i], string.Empty);
            }
            return str;
        }
        #endregion
    }
}