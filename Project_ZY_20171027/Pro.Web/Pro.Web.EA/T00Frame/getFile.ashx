<%@ WebHandler Language="C#" Class="getFile" %>

using System;
using System.Web;
using System.Web.SessionState;
using Pro.Common; 

public class getFile : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        #region 参数存在性检查

        SessionInfo IPApi = AppInfo.GetSessionInfo(context.Session);
        if (IPApi == null)
        {
            AppInfo.ReturnMessage(context, -1, "系统尚未登录");
            return;
        }

        if (context.Request["blobid"] == null)
        {
            AppInfo.ReturnMessage(context, -2, "未指定blobid参数");
            return;
        }
        if (context.Request["verify"] == null)
        {
            AppInfo.ReturnMessage(context, -3, "未指定verify参数");
            return;
        }

        #endregion

        #region 参数合法性检查

        string strBlobID = context.Request["blobid"].ToString();
        string strVerify = context.Request["verify"].ToString();

        if (AppInfo.CreateVerifyCode(strBlobID, IPApi) != strVerify)
            if (MySecurity.EnCodeByMD5(strBlobID) != strVerify) //补丁，用于非登录状态下进行文件下载
            {
                AppInfo.ReturnMessage(context, -5, "verify参数不合法");
                return;
            }

        #endregion

        #region 内容检查与输出

        //Sys_blob blob = new Sys_blob(AppInfo.dbEmis, strBlobID);
        //if (blob.Sb_id != strBlobID)
        //{
        //    AppInfo.ReturnMessage(context, -7, "指定的文件不存在");
        //    return;
        //}

        context.Response.Clear();
        bool isPic = (context.Request["pic"] != null) ? (context.Request["pic"] == "1") : false; //是否图片（直接显示），否则下载
        //if (isPic)
        //{
        //    HttpContext.Current.Response.ContentType = "image/jpeg";
        //}
        //else
        //{
        //    //context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        //    string strFileName = blob.Sb_title + (blob.Sb_fileext.StartsWith (".")?"":".")+ blob.Sb_fileext;
        //    strFileName = context.Server.UrlEncode(strFileName);
        //    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + strFileName);//设置文件名
        //    context.Response.AddHeader("Content-Length", blob.Sb_blob.Length.ToString()); //设置下载文件大小
        //    context.Response.ContentType = "application/octet-stream";
        //}
        //context.Response.BinaryWrite(blob.Sb_blob);
        //如果需要分几次发送，可以考虑用这个语句做循环
        //context.Response.OutputStream.Write(blob.Sb_blob, 0, blob.Sb_blob.Length);
        //context.Response.Flush();

        context.Response.End();

        #endregion
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}