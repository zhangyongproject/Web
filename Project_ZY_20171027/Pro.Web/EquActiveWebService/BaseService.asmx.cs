using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Pro.CoreModel;

namespace Pro.Web.EquActive.WebService
{
    /// <summary>
    /// BaseService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://yixiubox.com:5000/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class BaseService : System.Web.Services.WebService
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        List<string> lstRequestObject = new List<string>();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

    }
}
