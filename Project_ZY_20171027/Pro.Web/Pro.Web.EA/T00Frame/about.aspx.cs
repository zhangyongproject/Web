﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class T00Frame_about : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 向客户端传送的全局变量

    protected string GetAppTitle()
    {
        return AppInfo.AppTitle;
    }
    protected string GetAppVersion()
    {
        return AppInfo.AppVersion;
    }

    #endregion
}
