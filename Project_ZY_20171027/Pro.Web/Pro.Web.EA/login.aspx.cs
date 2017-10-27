using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    String ConstDefaultPage = "T03Timing/TimingList.aspx";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppInfo.GetSessionInfo(Session).HasLogin)
        {
            ////TODO：Auto Login，（临时处理方案，直接登录）
            //string str = "";
            //AppInfo.GetSessionInfo(Session).Login("xjp", "", this.Context.Request.UserHostAddress, ref str);
            //if (Session["LastPage"] != null)
            //    Response.Redirect(Session["LastPage"].ToString());
            //else
            //    Response.Redirect(ConstDefaultPage);
        }
        else
            Response.Redirect(ConstDefaultPage);

        //if (AppInfo.GetSessionInfo(Session).HasLogin)
        //    Response.Redirect(ConstDefaultPage);
    }

    protected void BtnLogin_Click(object sender, EventArgs e)
    {
        string str = "";
        if (AppInfo.GetSessionInfo(Session).Login(this.Txt_UserCode.Text, this.Txt_Password.Value, this.Context.Request.UserHostAddress, ref str))
        {
            if (Session["LastPage"] != null)
                Response.Redirect(Session["LastPage"].ToString());
            else
                Response.Redirect(ConstDefaultPage);
        }
        else
            this.Lab_Message.Text = str;
    }
}
