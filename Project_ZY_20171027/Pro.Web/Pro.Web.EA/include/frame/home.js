function ChangePass() {
    ShowPage("../T00Frame/changepass.aspx", "更改密码", function(code, val) { if (code >= 0) ShowAlert(val); }, 320, 180);
    //ShowAlert("功能保留", "更改密码");
}

function ShowHelp() {
    window.open("../help/index.htm", "ipuihelp");
}

function ShowAbout() {
    ShowPage("../T00Frame/about.aspx", "关于系统", null, 400, 320);
    //ShowAlert("功能保留", "关于系统");
}

var LastLogoutSetting = "LastLogoutSetting";
function Logout() { 
    str = "您是否确定要退出系统？<br /><input id=check_logout type=checkbox style='height:12px;' /><label for=check_logout>退出时自动关闭窗口</label>";
    ShowConfirm(str, null, OnLogout);
    $("check_logout").checked = (GetCookie(LastLogoutSetting) == "1");
}

function OnLogout(code, val) {
    if (code != 0) return;
    var objWebmethod = new WebMethodProxy(document.location, "Logout");
    objWebmethod.Call(OnLogoutCallBack, OnLogoutCallBack);
}

function OnLogoutCallBack(ret){
    var mCheck = $("check_logout").checked ? 1 : 0;
    SetCookie(LastLogoutSetting, mCheck);

    if (mCheck == 0) {
        //刷新页面，正常情况下自动跳转到登录页面
        //document.location = document.location+"?rnd="+Math.random ();
        window.open("../index.htm", "_self", '');
    } else {
       //关闭窗口
       window.opener = null;
       window.open('', '_self', '');
       close();
    }
}

//---------------自动发送心跳包，保持在线状态------------------

var objWebmethod = new WebMethodProxy(document.location, "HeartBeat");
function HeartBeat(){
  objWebmethod.Call(DoHeartBeat);   
}

function DoHeartBeat(xmlstr) {
    if (xmlstr != null) {
        //todo:保留，返回即时信息
        var mXml = CreateXmlFromString(xmlstr);
        //ShowMessageBox(xmlstr, "保留功能");
    }
    setTimeout(HeartBeat,1000*20); //20秒更新一次状态
}

//DoHeartBeat(null);
//AutoMoveMessageBox(); //主框架，自动移动消息窗口