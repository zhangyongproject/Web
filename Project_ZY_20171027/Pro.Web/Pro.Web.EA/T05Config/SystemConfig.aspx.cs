using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Pro.Common;
using Newtonsoft.Json;
public partial class T05Config_SystemConfig : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected override void InitModuleInfo()
    {
        base.InitModuleInfo();
    }

    protected override XmlDocument CreateInitInfo()
    {
        XmlDocument xmlDoc = MyXml.CreateResultXml(0, "", string.Empty);
        return xmlDoc;
    }
}