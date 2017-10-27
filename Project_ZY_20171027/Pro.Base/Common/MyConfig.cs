using System.Collections;
using System.Configuration;

namespace Pro.Common
{
    public class MyConfig
    {

        #region 取WebConfig

        /// <summary>
        /// 得到一个配置的键值
        /// </summary>
        /// <param name="keyName">配置的键名</param>
        /// <returns>配置的键值</returns>
        public static string GetWebConfig(string keyName)
        {
            return GetWebConfig(keyName, string.Empty);
        }

        /// <summary>
        /// 得到一个配置的键值
        /// </summary>
        /// <param name="keyName">配置的键名</param>
        /// <returns>配置的键值</returns>
        public static string GetWebConfig(string keyName, string defaultVal)
        {
            string ret = System.Configuration.ConfigurationManager.AppSettings[keyName];
            if (ret == null)
                return defaultVal;
            return ret;
        }

        #endregion

        #region 读写配置内容

        //static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        static Hashtable htConfig = new Hashtable();

        public static string GetConfig(string key)
        {
            return GetConfig(key, string.Empty);
        }

        public static string GetConfig(string key, string defaultvalue)
        {
            if (htConfig[key] == null)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string value = defaultvalue;
                if (config.AppSettings.Settings[key] != null)
                    value = config.AppSettings.Settings[key].Value;
                htConfig.Add(key, value);
            }
            return htConfig[key].ToString();
        }

        public static void SetConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);

            if (htConfig[key] == null)
                htConfig.Add(key, value);
            else
                htConfig[key] = value;
        }

        public static void RemoveConfig(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
            }
        }

        #endregion
    }
}
