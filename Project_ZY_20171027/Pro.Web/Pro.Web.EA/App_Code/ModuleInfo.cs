using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///ModuleInfo 的摘要说明
/// </summary>
public class ModuleInfo
{
    public string PageFlag { get; set; }    //模块标识，唯一，通过网页url解析
    public string ModuleName { get; set; }  //模块名      （通过pageFlag从配置文件中解释）
    public string ModuleDesc { get; set; }  //模块描述    （通过pageFlag从配置文件中解释）
    public int idxMenu1 { get; set; }       //主菜单序号  （通过pageFlag从配置文件中解释）
    public int idxMenu2 { get; set; }       //二级菜单序号（通过pageFlag从配置文件中解释）
    public int idxMenu3 { get; set; }       //三级菜单序号（-1/0）

    public ModuleInfo()
    {
        PageFlag = string.Empty;
        ModuleName = string.Empty;
        ModuleDesc = string.Empty;
        idxMenu1 = -1;
        idxMenu2 = -1;
        idxMenu3 = -1;
    }

    /// <summary>
    /// 模块信息是否初始化成功
    /// </summary>
    /// <returns></returns>
    public bool IsMouduleInited()
    {
        return !PageFlag.Equals(string.Empty);
    }
}
