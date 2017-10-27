using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using Pro.Common;
using Pro.Common;

/// <summary>
/// 支持Ajax请求的响应处理
/// </summary>
public class AjaxPage : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        if (this.Request["ajaxflag"] != null) //&& (this.Request.InputStream.Length > 0))
        {
            /* 当客户端以post方法提交soap请求，认为是客户端提交了AJAX请求
            * 具体的处理过程由虚方法responseAJAX实现
            * 子类页面可以重载其实现过程
            * 若不能在相应的子类页面中找到实现过程，系统将按原请求的流程进行处理
            */

            //0/1 json(jquery)/soap
            int ajaxtype = MyType.ToInt(this.Request["ajaxflag"]);

            if (ajaxtype == 0)
            {
                #region AJAX请求处理（JQuery调用，JSON格式）

                string methodName = this.Request["webmethodname"];

                Hashtable htParam = new Hashtable();
                foreach (string strkey in this.Request.Form.AllKeys)
                {
                    //htParam.Add(strkey.ToLower(), this.Request.Form[strkey]);
                    string value = this.Request.Form[strkey].Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
                    htParam.Add(strkey.ToLower(), value);
                }

                object objRet = null;
                string strXml = "";
                if (responseAJAX(methodName, htParam, ref objRet))
                {
                    if (objRet.GetType() == typeof(XmlDocument))
                        strXml = (objRet as XmlDocument).InnerXml;
                    else
                    {
                        if (MyXml.LoadXml(objRet.ToString()) == null)
                        {
                            MyLog.WriteLogInfo(string.Format("WebService:{0} 返回结果非xml格式", methodName));
                            strXml = MyXml.CreateResultXml(0, "", objRet).InnerXml;
                        }
                        else
                            strXml = objRet.ToString();
                    }
                }
                else
                {
                    string strErr = "responseAJAX error:"
                        + "\r\n  url:" + this.Request.Url.LocalPath
                        + "\r\n  method:" + methodName
                        + "\r\n  message:" + objRet;
                    Pro.Common.MyLog.WriteLogInfo(strErr);

                    strXml = MyXml.CreateResultXml(-1, strErr, "").InnerXml;
                }
                Response.ContentType = "text/xml";
                Response.Charset = "UTF-8";
                Response.Write(strXml);
                Response.End();

                #endregion
            }
            else
            {
                #region AJAX请求处理(SOAP方式）

                XmlDocument xmlRequest = MyXml.CreateXml();
                try
                {
                    xmlRequest.Load(this.Request.InputStream);
                    if (xmlRequest == null) return;
                }
                catch
                {
                    return;
                }
                XmlNamespaceManager man = new XmlNamespaceManager(xmlRequest.NameTable);
                man.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                man.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                man.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

                XmlNode soapNode = xmlRequest.SelectSingleNode("soap:Envelope/soap:Body", man);
                if (soapNode == null) return;

                XmlNode methodNode = soapNode.ChildNodes[0];
                if (methodNode == null) return;
                string methodName = methodNode.Name;

                Hashtable htParam = new Hashtable();
                foreach (XmlNode paramNode in methodNode.ChildNodes)
                {
                    if (htParam[paramNode.Name.ToLower()] == null)
                        htParam.Add(paramNode.Name.ToLower(), paramNode.InnerText);
                }

                object objRet = null;
                string strXml = "";
                if (responseAJAX(methodName, htParam, ref objRet))
                {
                    string strResult = "";
                    if (objRet.GetType() == typeof(XmlDocument))
                        strResult = (objRet as XmlDocument).InnerXml;
                    else
                        strResult = objRet.ToString();

                    strXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                        + "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\""
                        + " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
                        + " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"
                        + "<soap:Body>"
                        + "<" + methodName + "Response xmlns=\"http://tempuri.org/\">"
                        + "<" + methodName + "Result>"
                        + strResult.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                        + "</" + methodName + "Result></" + methodName + "Response></soap:Body></soap:Envelope>";
                }
                else
                {
                    string strErr = "responseAJAX error:"
                        + "\r\n  url:" + this.Request.Url.LocalPath
                        + "\r\n  method:" + methodName
                        + "\r\n  message:" + objRet;
                    MyLog.WriteLogInfo(strErr);

                    strXml = MyXml.CreateResultXml(-1, strErr, "").InnerXml;
                }
                Response.ContentType = "text/xml";
                Response.Charset = "UTF-8";
                Response.Write(strXml);
                Response.End();

                #endregion
            }
        }
        else
            base.OnInit(e);
    }

    #region 处理AJAX请求相关函数

    /// <summary>
    /// 处理AJAX请求
    /// </summary>
    /// <param name="methoname">函数名</param>
    /// <param name="htParam">参数列表</param>
    /// <param name="strResult">返回的处理结果，以字符串形式返回</param>
    /// <returns>是否响应请求</returns>
    private bool responseAJAX(string methodName, Hashtable htParam, ref object retObject)
    {
        Type type = this.GetType();
        MethodInfo method = type.GetMethod(methodName);
        if (method == null)
        {
            retObject = "webmethod(" + methodName + ") not exist.";
            return false;
        }

        try
        {
            retObject = WebServiceHelper.InvokeMethod(this, method, htParam);
            return true;
        }
        catch (Exception e)
        {
            retObject = e.Message;
            return false;
        }
    }

    /// <summary>
    /// 将字符串转换为不同的对象类型
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private object CreateParamObj(string str, Type type)
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