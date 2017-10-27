using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Pro.Web.Common;
using Pro.CoreModel;
using Pro.Web.EALogic;
using Pro.Base.Common;
using Pro.Common;
using Pro.Common;
using System.Data;

namespace Pro.Web.EquActive.WebService
{
    /// <summary>
    /// AccessService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://yixiubox.com:5000/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class AccessService : BaseService
    {
        private UserLogic userLogic = new UserLogic();

        [WebMethod]
        public new string HelloWorld()
        {
            return "Hello World";
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="strjson">{"username":"admin","password":"admin"}</param>
        /// <returns></returns>
        [WebMethod]
        public string Login(string strjson)
        {
            try
            {
                if (string.IsNullOrEmpty(strjson)) { return Json.Write(-1, "参数JSON格式错误"); }
                Dictionary<string, string> dic = MyJson.JsonToDictionary(strjson);
                if (dic.Count == 0) { return Json.Write(-1, "参数JSON格式错误"); }
                string username = string.Empty;
                string userpwd = string.Empty;
                if (dic.TryGetValue("username", out username) == false) { return Json.Write(-1, "用户帐号无法识别"); }
                if (dic.TryGetValue("password", out userpwd) == false) { return Json.Write(-1, "用户密码无法识别"); }
                UserInfo info = new UserInfo() { UserName = username, UserPwd = userpwd };
                ReturnValue retVal = userLogic.Login(info);
                return Json.Write(retVal.RetCode, retVal.RetMsg, retVal.RetDt ?? new DataTable());
            }
            catch (Exception ex)
            {
                return Json.Write(-1, Consts.EXP_Info);
            }

        }
    }
}
