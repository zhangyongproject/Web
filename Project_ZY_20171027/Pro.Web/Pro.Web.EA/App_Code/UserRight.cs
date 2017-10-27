using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///UserRight 的摘要说明
/// </summary>
public class UserRight
{
    #region 初始化相关

    private char[] RightArray;
    public string RightString
    {
        get
        {
            string str = new string(RightArray);
            str = str.TrimEnd(new char[] { '0' });
            return str;
        }
    }

    public UserRight()
    {
        Init("");
    }

    public UserRight(string strRightString)
    {
        Init(strRightString);
    }

    private void Init(string strRightString)
    {
        RightArray = strRightString.PadRight(100, '0').ToCharArray();
    }

    #endregion

    #region 权限项配置

    /// <summary>
    /// 系统管理权
    /// </summary>
    public bool IsAdmin
    {
        get
        {
            return RightArray[0] != '1';
        }
        set
        {
            RightArray[0] = value ? '0' : '1';
        }
    }


    #endregion
}
