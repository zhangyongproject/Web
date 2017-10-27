<%@ WebHandler Language="C#" Class="UpFileHandler" %>

using System;
using System.Web;

public class UpFileHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        HttpPostedFile _upfile = context.Request.Files["File1"];
        if (_upfile == null)
        {
            ResponseWriteEnd(context, "4");//请选择要上传的文件
        }
        else
        {
            string fileName = _upfile.FileName;/*获取文件名： C:\Documents and Settings\Administrator\桌面\123.jpg*/
            string suffix = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();/*获取后缀名并转为小写： jpg*/
            int bytes = _upfile.ContentLength;//获取文件的字节大小

            if (suffix != "xls" || suffix != "csv")
                ResponseWriteEnd(context, "2"); //只能上传xls/csv格式图片
            if (bytes > 1024 * 201024)
                ResponseWriteEnd(context, "3"); //图片不能大于20M

            _upfile.SaveAs(HttpContext.Current.Server.MapPath("~/Temp/Import/station.xls"));//保存文件
            ResponseWriteEnd(context, "1"); //上传成功
        }
    }

    private void ResponseWriteEnd(HttpContext context, string msg)
    {
        context.Response.Write(msg);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}


