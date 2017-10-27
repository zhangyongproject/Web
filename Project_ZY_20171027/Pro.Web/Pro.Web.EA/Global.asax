<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 在应用程序启动时运行的代码
         Pro.Common.MyLog.WriteLogInfo("Application start......");
        AppInfo.InitApplication(Application);

    }

    void Application_End(object sender, EventArgs e)
    {
        //在应用程序关闭时运行的代码
        Pro.Common.MyLog.WriteLogInfo("Application end.");
    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在出现未处理的错误时运行的代码
        Exception objErr = Server.GetLastError().GetBaseException();
        string err = "Error Caught in Application_Error event"
            + "\r\nError Message:" + objErr.Message.ToString()
            + "\r\nStack Trace:" + objErr.StackTrace.ToString();
        Pro.Common.MyLog.WriteLogInfo(err);
        Server.ClearError();
    }

    void Session_Start(object sender, EventArgs e)
    {
        // 在新会话启动时运行的代码
        AppInfo.OnSessionStart(Session);
    }

    void Session_End(object sender, EventArgs e)
    {
        // 在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer
        // 或 SQLServer，则不引发该事件。
        AppInfo.OnSessionEnd(Session);
    }
       
</script>
