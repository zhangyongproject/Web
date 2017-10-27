using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class T00Frame_noaccess : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            divMessage.InnerText = Session["ErrMessage"].ToString();
    }

    protected override void InitModuleInfo()
    {
        if (Session["LastModule"] != null)
        {
            this.mModuleInfo = (ModuleInfo)Session["LastModule"];
        }
        else
            base.InitModuleInfo();
    }
}
