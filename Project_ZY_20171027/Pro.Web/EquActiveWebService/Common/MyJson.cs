using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Pro.Common
{
    public class MyJson
    {
        /// <summary>
        /// 转换成Dictionary 
        /// </summary>
        public static JavaScriptSerializer js = new JavaScriptSerializer();

        public static Dictionary<string, string> JsonToDictionary(string json)
        {
            try
            {
                return js.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 转换成Dictionary，值为object类型
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDictionaryObjValue(string json)
        {
            try
            {
                return js.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
            catch (Exception e)
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// 转换成object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object JsonToObject(string json)
        {
            try
            {
                return js.Deserialize<object>(json) ?? new object();
            }
            catch (Exception e)
            {
                return new object();
            }
        }
    }


}
