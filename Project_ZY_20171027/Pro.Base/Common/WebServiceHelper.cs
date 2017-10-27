using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace Pro.Common
{
    public class WebServiceHelper
    {
        #region InvokeWebService 动态调用web服务

        //调用例子：
        /*
        string url = "http://localhost/IPwebservice/sysservice.asmx";
        string methodname = "Login";
        Hashtable htParam = new Hashtable();
        htParam.Add("userCode", "150043");
        htParam.Add("userPassword", "");
        htParam.Add("appCode", "ip");
        string ret = HTTPS.WebServiceHelper.InvokeWebService(url, methodname, htParam).ToString();
        */
        
        /// < summary> 
        /// 动态调用web服务 
        /// < /summary> 
        /// < param name="url">WSDL服务地址< /param> 
        /// < param name="methodname">方法名< /param> 
        /// < param name="htParam">参数< /param> 
        /// < returns>< /returns> 
        public static object InvokeWebService(string url, string methodname, Hashtable htParam)
        {
            return WebServiceHelper.InvokeWebService(url, null, methodname, htParam);
        }

        /// < summary> 
        /// 动态调用web服务 
        /// < /summary> 
        /// < param name="url">WSDL服务地址< /param> 
        /// < param name="classname">类名< /param> 
        /// < param name="methodname">方法名< /param> 
        /// < param name="htParam">参数< /param> 
        /// < returns>< /returns> 
        public static object InvokeWebService(string url, string classname, string methodname, Hashtable htParam)
        {
            Type t=GetWebServiceType(url, classname);
            return InvokeMethod(t, methodname, htParam);
        }

        #endregion

        #region GetWebServiceType 通过webservice接口，自动创建代码类

        public static Type GetWebServiceType(string url)
        {
            return GetWebServiceType(url, null);
        }

        /// <summary>
        /// 通过webservice接口，自动创建代码类
        /// </summary>
        /// <param name="url"></param>
        /// <param name="classname"></param>
        /// <param name="methodname"></param>
        /// <returns></returns>
        public static Type GetWebServiceType(string url, string classname)
        {
            #region 实现过程

            if ((classname == null) || (classname == ""))
            {
                classname = WebServiceHelper.GetWsClassName(url);
            }

            try
            {
                //获取WSDL ，得到sdi
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");

                //得到ccu 
                string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
                CodeNamespace cn = new CodeNamespace(@namespace);
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);

                //设定编译参数 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译
                CSharpCodeProvider icc = new CSharpCodeProvider();
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);

                //出错信息
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //自动创建类信息
                Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);

                return t;
            }
            catch (Exception e)
            {
                MyLog.WriteExceptionLog("WebServiceHelper.GetWebServiceType", e,
                    string.Format("\r\n\turl:{0}", url));
                return null;
            }

            #endregion
        }

        #endregion

        #region InvokeMethod 通过反射机制，动态调用类方法，传类名、类方法

        /// <summary>
        /// 动态调用类方法，传类名、类方法
        /// </summary>
        /// <param name="t"></param>
        /// <param name="methodname"></param>
        /// <param name="htParam"></param>
        /// <returns></returns>
        public static object InvokeMethod(Type t,string methodname, Hashtable htParam)
        {
            try
            {
                object obj = Activator.CreateInstance(t);
                MethodInfo mi = t.GetMethod(methodname);
                return InvokeMethod(obj, mi, htParam);
            }
            catch (Exception e)
            {
                MyLog.WriteExceptionLog("WebServiceHelper.InvokeMethod", e,
                    string.Format("\r\n\tmethod:{0}", methodname));
                return null;
            }
        }

        /// <summary>
        /// 动态调用类方法，传类实例、类方法信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mi"></param>
        /// <param name="htParam"></param>
        /// <returns></returns>
        public static object InvokeMethod(object obj, MethodInfo mi, Hashtable htParam)
        {
            try
            {
                //参数
                ParameterInfo[] paramList = mi.GetParameters();
                object[] objParams = new object[paramList.Length];
                for (int i = 0; i < paramList.Length; i++)
                {
                    string paramValue = "";
                    foreach (DictionaryEntry de in htParam)
                    {
                        if (de.Key.ToString().Equals(paramList[i].Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            paramValue = de.Value.ToString();
                            break;
                        }
                    }
                    objParams[i] = CreateParamObj(paramValue, paramList[i].ParameterType);
                }

                return mi.Invoke(obj, objParams);
            }
            catch (Exception e)
            {
                MyLog.WriteExceptionLog("WebServiceHelper.InvokeMethod", e,
                    string.Format("\r\n\tmethod:{0}", mi == null ? "method为空" : mi.Name));
                return null;
            }
        }

        #endregion

        #region 私有过程

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }

        /// <summary>
        /// 将字符串转换为不同的对象类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object CreateParamObj(string str, Type type)
        {
            object obj = new object();
            switch (type.Name.ToLower())
            {
                case "int":
                case "int32":
                case "int64":
                    return MyType.ToInt(str, 0);
                //return GetInt32(str, 0);
                case "double":
                case "float":
                    return MyType.ToDouble(str, 0);
                //return GetDouble(str, 0);
                case "string":
                    return str;
                case "datetime":
                    return MyType.ToDateTime(str, DateTime.Now);
                //return GetDateTime(str, DateTime.Now);
                case "byte[]":
                    return MyType.ToBytes(str);
                //return Encoding.Unicode.GetBytes(str);
                default:
                    return null;
            }
        }

        #endregion
    }
}
