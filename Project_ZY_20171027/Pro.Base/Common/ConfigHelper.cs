using System;
using System.Data;
using System.Data.OracleClient;
using System.Xml;
using System.Web;
using System.Reflection;
using System.Data.OleDb;
using System.IO;

namespace Pro.Common
{
    /// <summary>
    ///  配置文件类型
    /// </summary>
    public enum ConfigFileType
    {
        WebConfig = 0,
        AppConfig = 1
    }

    /// <summary>
    /// ConfigHelper
    /// </summary>
    public sealed class ConfigHelper
    {
        #region 变量区

        private static string docName = String.Empty;
        private static string message;
        private static XmlNode node = null;
        private static int _configType;
        private static string _configFilePath;

        #endregion

        #region 属性区

        /// <summary>
        ///  配置类型 0:WebConfig 1:AppConfig
        /// </summary>
        public static int ConfigType
        {
            get { return _configType; }
            set { _configType = value; }
        }

        /// <summary>
        /// 配置程序文档路径
        /// </summary>
        public static string ConfigFilePath
        {
            get { return _configFilePath; }
            set { _configFilePath = value; }
        }

        #endregion

        #region 方法区

        /// <summary>
        ///  构造函数
        /// </summary>
        private ConfigHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        ///  取得结点的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            //读取
            try
            {
                XmlDocument cfgDoc = new XmlDocument();
                LoadConfigDoc(cfgDoc);

                // retrieve the appSettings node  
                node = cfgDoc.SelectSingleNode("//appSettings");
                if (node == null)
                {
                    throw new InvalidOperationException("appSettings section not found");
                }

                // XPath select setting "add" element that contains this key to remove      
                XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + key + "']");

                if (addElem == null)
                {
                    message = "此key不存在.";
                    return message;
                }

                message = System.Configuration.ConfigurationSettings.AppSettings[key];
                return message;
            }
            catch
            {
                message = "操作异常,获取value值失败.";
                return message;
            }
        }

        /// <summary>
        ///  增加节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AddValue(string key, string value)
        {
            //增加
            XmlDocument cfgDoc = new XmlDocument();
            LoadConfigDoc(cfgDoc);

            // retrieve the appSettings node   
            node = cfgDoc.SelectSingleNode("//appSettings");

            if (node == null)
            {
                throw new InvalidOperationException("appSettings section not found");
            }

            try
            {
                // XPath select setting "add" element that contains this key       
                XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + key + "']");

                if (addElem != null)
                {
                    message = "此key已经存在.";
                    return message;
                }
                // not found, so we need to add the element, key and value   
                else
                {
                    XmlElement entry = cfgDoc.CreateElement("add");
                    entry.SetAttribute("key", key);
                    entry.SetAttribute("value", value);
                    node.AppendChild(entry);
                }

                //save it   
                SaveConfigDoc(cfgDoc, docName);
                message = "添加成功.";
                return message;
            }
            catch
            {
                message = "操作异常,添加失败.";
                return message;
            }
        }

        /// <summary>
        ///  保存文件
        /// </summary>
        /// <param name="cfgDoc"></param>
        /// <param name="cfgDocPath"></param>
        private static void SaveConfigDoc(XmlDocument cfgDoc, string cfgDocPath)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(cfgDocPath, null);
                writer.Formatting = Formatting.Indented;
                cfgDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                return;
            }
            catch
            {
                throw new Exception("写入失败，请查看你的配置文件是否已读。");
            }
        }

        /// <summary>
        ///  删除节点
        /// </summary>
        /// <param name="elementKey"></param>
        /// <returns></returns>
        public static string RemoveElement(string elementKey)
        {
            // 删除
            try
            {
                XmlDocument cfgDoc = new XmlDocument();
                LoadConfigDoc(cfgDoc);

                // retrieve the appSettings node  
                node = cfgDoc.SelectSingleNode("//appSettings");

                if (node == null)
                {
                    throw new InvalidOperationException("appSettings section not found");
                }

                XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + elementKey + "']");

                if (addElem == null)
                {
                    message = "此key不存在.";
                    return message;
                }

                // XPath select setting "add" element that contains this key to remove      
                node.RemoveChild(node.SelectSingleNode("//add[@key='" + elementKey + "']"));
                SaveConfigDoc(cfgDoc, docName);
                message = "删除成功.";
                return message;
            }
            catch
            {
                message = "操作异常,删除失败.";
                return message;
            }
        }

        /// <summary>
        ///  修改节点
        /// </summary>
        /// <param name="elementKey"></param>
        /// <param name="elementValue"></param>
        /// <returns></returns>
        public static string ModifyElement(string elementKey, string elementValue)
        {
            //修改
            try
            {
                XmlDocument cfgDoc = new XmlDocument();
                LoadConfigDoc(cfgDoc);

                // retrieve the appSettings node  
                node = cfgDoc.SelectSingleNode("//appSettings");

                if (node == null)
                {
                    throw new InvalidOperationException("appSettings section not found");
                }

                // XPath select setting "add" element that contains this key to remove      
                XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + elementKey + "']");

                if (addElem == null)
                {
                    message = "此key不存在.";
                    return message;
                }

                addElem.SetAttribute("value", elementValue);

                //save it   
                SaveConfigDoc(cfgDoc, docName);
                message = "保存成功。请注销系统，新的配置才能生效。";
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  加载XML文档
        /// </summary>
        /// <param name="cfgDoc"></param>
        /// <returns></returns>
        private static XmlDocument LoadConfigDoc(XmlDocument cfgDoc)
        {
            // load the config file   
            if (Convert.ToInt32(ConfigType) == Convert.ToInt32(ConfigFileType.AppConfig))
            {
                //docName = ((Assembly.GetEntryAssembly()).GetName()).Name;
                //docName += ".exe.config";
                docName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ((Assembly.GetEntryAssembly()).GetName()).Name + ".exe.config");
                //docName += ".config";
                //((Assembly.GetEntryAssembly()).GetName()).Name
            }
            else
            {
                //docName = HttpContext.Current.Server.MapPath(ConfigFilePath);
                docName = AppDomain.CurrentDomain.BaseDirectory;
                docName = Path.Combine(docName, "web.config");
            }

            cfgDoc.Load(docName);
            return cfgDoc;
        }

        /// <summary>
        ///  测试数据库的连接是否正确
        /// </summary>
        /// <param name="strDbCon">数据库连接字串</param>
        /// <returns></returns>
        public static string TestDbCon(string strDbCon)
        {
            try
            {
                OracleConnection Orcon = new OracleConnection(strDbCon);

                if (Orcon != null && Orcon.State == ConnectionState.Closed)
                {
                    Orcon.Open();
                }

                if (Orcon != null && Orcon.State == ConnectionState.Open)
                {
                    Orcon.Close();
                }

                message = "连接成功.";
            }
            catch
            {
                message = "连接失败.";
            }

            return message;
        }

        /// <summary>
        /// 测试上报参数
        /// </summary>
        /// <param name="linkName">参数名</param>
        /// <param name="connStr">本地连接</param>
        /// <returns>true:成功 false 失败</returns>
        public static bool TestDBLink(string linkName, string connStr)
        {
            System.Data.OleDb.OleDbConnection conn = new OleDbConnection(string.Format("Provider=MSDAORA;{0}", connStr));
            OleDbCommand comm = new OleDbCommand();
            comm.CommandText = "pkg_pub.test_dblink";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Connection = conn;
            comm.Parameters.Add("LinkName", OleDbType.VarChar).Value = linkName;
            comm.Parameters.Add("result", OleDbType.Integer).Direction = ParameterDirection.Output;
            //System.Data.OracleClient.OracleConnection conn = new OracleConnection(connStr);
            //OracleCommand comm = new OracleCommand("pro_zxpt.PKG_PUB.Test_DBLink", conn);
            //comm.CommandType = CommandType.StoredProcedure;

            //OracleParameter p1 = new OracleParameter("LinkName", OracleType.VarChar);
            //p1.Direction = System.Data.ParameterDirection.Input;
            //p1.Value = linkName;
            //OracleParameter p2 = new OracleParameter("result", OracleType.Number);
            //p2.Direction = System.Data.ParameterDirection.Output;
            //comm.Parameters.Add(p1);
            //comm.Parameters.Add(p2);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();

                if (Convert.ToInt32(comm.Parameters[1].Value) == 1)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return false;
        }

        #endregion
    }
}
