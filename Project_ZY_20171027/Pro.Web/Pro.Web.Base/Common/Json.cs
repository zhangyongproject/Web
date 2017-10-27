using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Serialization;

namespace Pro.Web.Common
{
    public class Json
    {
        #region 创建一个与前台交互的JSON
        /// <summary>
        /// 创建一个与前台交互的JSON
        /// </summary>
        /// <param name="code">返回代码:-9:异常,-5:没有权限,-3:未登陆,-1:没有数据,0:执行成功</param>
        /// <param name="info">返回信息:字符串(如果是Json字符串不会解析)</param>
        /// <returns></returns>
        public static string Write(int code, string info)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("result");
                jtw.WriteValue(code);
                jtw.WritePropertyName("resulttext");
                jtw.WriteValue(info);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        /// <summary>
        /// 创建一个与前台交互的JSON
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="info">返回信息:字符串(如果是Json字符串自动解析)</param>
        /// <returns></returns>
        public static string WriteRaw(int code, string info)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("result");
                jtw.WriteValue(code);
                jtw.WritePropertyName("resulttext");
                jtw.WriteRawValue(info);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }

        /// <summary>
        /// {"result":1,"resulttext":"success","nums":2,"data":[]}
        /// </summary>
        /// <param name="result"></param>
        /// <param name="resulttext"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Write(int result, string resulttext, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("result");
                jtw.WriteValue(result);
                jtw.WritePropertyName("resulttext");
                jtw.WriteValue(resulttext);
                jtw.WritePropertyName("nums");
                jtw.WriteValue(dt.Rows.Count);
                jtw.WritePropertyName("data");
                jtw.WriteRawValue(DataTable2JsonNoName(dt));
                jtw.WriteEndObject();
                return sb.ToString();
            }
        }
        #endregion

        #region 连接字符串
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Connect(string key, string val)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName(key);
                jtw.WriteRawValue(val);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        #endregion

        #region DataTable转换成Json格式(无表名)
        /// <summary>
        /// DataTable转换成Json格式(无表名)
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2JsonNoName(DataTable dt)
        {
            return SerializeObject<DataTable>(dt);
        }
        #endregion

        #region DataTable转换成Json格式
        /// <summary>
        /// DataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(DataTable dt)
        {
            return DataTable2Json(dt, dt.TableName);
        }

        /// <summary>
        /// DataTable转换成Json格式  
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataTable dt, string tableName)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"").Append(tableName).Append("\":");
            jsonBuilder.Append(DataTable2JsonNoName(dt));
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        #endregion

        #region DataTable转换为Grid所需的Json格式
        /// <summary>
        /// DataTable转换为Grid所需的Json格式
        /// </summary>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2GridJson(int pageSum, int pageIndex, DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"").Append(dt.TableName == "" ? "tb" : dt.TableName).Append("\":{\"rows\":");
            jsonBuilder.Append(DataTable2JsonNoName(dt));
            jsonBuilder.Append(",\"pageSum\":").Append(pageSum).Append(",\"pageIndex\":").Append(pageIndex).Append("}}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// DataTable转换为Grid所需的Json格式(不需要翻页)
        /// </summary>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2GridJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"").Append(dt.TableName == "" ? "tb" : dt.TableName).Append("\":{\"rows\":");
            jsonBuilder.Append(DataTable2JsonNoName(dt));
            jsonBuilder.Append("}}");
            return jsonBuilder.ToString();
        }
        #endregion

        #region DataSet转换成Json格式
        /// <summary>  
        /// DataSet转换成Json格式  
        /// </summary>  
        /// <param name="ds">DataSet</param>
        /// <returns></returns>  
        public static string Dataset2Json(DataSet ds)
        {
            return SerializeObject<DataSet>(ds);
        }
        #endregion

        #region Dictionary转换成Json格式
        /// <summary>
        /// Dictionary转换成Json格式  
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string Dictionary2Json<Tk, Tv>(Dictionary<Tk, Tv> dic)
        {
            return SerializeObject<Dictionary<Tk, Tv>>(dic);
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T t)
        {
            switch (t.GetType().Name)
            {
                case "DataTable":
                    return JsonConvert.SerializeObject(t, new DataTableConverter());
                case "DataSet":
                    return JsonConvert.SerializeObject(t, new DataSetConverter());
                default:
                    return JsonConvert.SerializeObject(t, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
        }
        /// <summary>
        /// 反序列化一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor });
        }

        #endregion
    }

    #region Json扩展
    public class DataSetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataSet).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DataSet ds = (DataSet)value;
            writer.WriteStartObject();
            foreach (DataTable dt in ds.Tables)
            {
                writer.WritePropertyName(dt.TableName);
                writer.WriteStartArray();
                foreach (DataRow dr in dt.Rows)
                {
                    writer.WriteStartObject();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        writer.WritePropertyName(dc.ColumnName.ToUpper());
                        writer.WriteValue(dr[dc].ToString());
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }

    public class DataTableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataTable).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DataTable dt = (DataTable)value;

            writer.WriteStartArray();
            foreach (DataRow dr in dt.Rows)
            {
                writer.WriteStartObject();
                foreach (DataColumn dc in dt.Columns)
                {
                    writer.WritePropertyName(dc.ColumnName.ToUpper());
                    writer.WriteValue(dr[dc].ToString());
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }

    #endregion
}
