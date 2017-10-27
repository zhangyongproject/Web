using System.Xml;
using Pro.Common;
using System.Data;
using Newtonsoft.Json;
using System;
using Pro.Web.Common;
using Pro.Common;

/// <summary>
/// 本系统基础页面类，提供一些与UI操作相关的公共函数
/// </summary>
public class BasePage : FramePage
{
    protected string ConstDefaultLoginPage = "../login.aspx";

    #region 网页传入参数解析

    protected string GetRequest(string key)
    {
        return Request[key] != null ? Request[key] : "";
    }

    protected int GetRequestInt(string key)
    {
        return MyType.ToInt(GetRequest(key));
    }

    protected int GetRequestInt(string key, int min, int max)
    {
        int value = MyType.ToInt(GetRequest(key));
        if (value < min) value = min;
        if (value > max) value = max;
        return value;
    }

    #endregion

    #region 客户端初始化数据

    /// <summary>
    /// 客户端页面初始化时用到的初始化数据，缓存处理。
    /// 具体初始化内容由子页面通过重写CreateInitInfo函数提供
    /// </summary>
    /// <returns></returns>
    public string GetInitInfo()
    {
        XmlDocument xmlInitInfo = null;
        string type = this.GetRequest("type"); //这个参数有时也有用
        string strCacheName = string.Format("{0}_Init_{1}{2}", this.GetType().Name, this.IPApi.SMCode, type);
        if (!CheckInitCacheEnabled() || !CacheHelper.CheckCache(strCacheName))
        {
            xmlInitInfo = CreateInitInfo();
            if (xmlInitInfo == null)
                xmlInitInfo = MyXml.CreateResultXml(-1, "no init info", "");
            CacheHelper.SetCache(strCacheName, CreateInitInfo());
        }

        xmlInitInfo = (XmlDocument)(CacheHelper.GetCache(strCacheName));
        return xmlInitInfo.InnerXml;
    }

    /// <summary>
    /// 检查是否支持初始化数据缓存，默认支持
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckInitCacheEnabled()
    {
        return false;
    }

    //public bool _CheckInitCacheEnabled = true;

    /// <summary>
    /// 生成初始化数据函数
    /// </summary>
    /// <returns></returns>
    protected virtual XmlDocument CreateInitInfo()
    {
        return MyXml.CreateResultXml(-1, "no init info", "");
    }

    #endregion

    #region 响应自动完成控件的数据请求

    ///// <summary>
    ///// 取数据字典，将系统数据字典内容以自动完成控制要求格式返回。页面可重写本函数，自定义选项内容
    ///// </summary>
    ///// <param name="strparam"></param>
    ///// <param name="tag"></param>
    ///// <returns></returns>
    //public virtual string GetDictList(string strparam, string tag)
    //{
    //    Sys_diclist sys_diclist = new Sys_diclist(AppInfo.dbEmis);
    //    DataTable dt = sys_diclist.GetDataTable(",sdl_text data,sdl_value value", " and sdl_keycode='" + tag + "'", "sdl_index");
    //    XmlDocument xmlDoc = MyXml.CreateAutocompleteResultXml(dt, tag, dt.Rows.Count);
    //    XmlNode rootNode = xmlDoc.SelectSingleNode("xml");
    //    MyXml.AddAttribute(rootNode, "value", strparam);
    //    return xmlDoc.InnerXml;
    //}

    #endregion
        
}