<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="IE=7.0000" http-equiv="X-UA-Compatible" />

    <title>系统登录</title>
    <script src="include/procommon.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Init() {
            if (1==1) {
                $("Txt_UserCode").value = GetCookie("usercode");
                if ($("Txt_UserCode").value == "")
                    $("Txt_UserCode").focus();
                else $("Txt_Password").focus();
                $("Btn_Login").onclick = function () { SetCookie("usercode", $("Txt_UserCode").value); };

                if (document.documentElement.clientHeight < 500)
                    document.documentElement.scrollTop = 100;
            } else {
                $("div_login").style.display = 'none';
                $("div_msg").style.display = 'block';
            }
        }
    </script>
</head>
<body onload="Init()" background="images/login/bg0.jpg">
    <form id="form1" runat="server" defaultbutton="Btn_Login">
        <div style="text-align: center">
            <div id="div_login" style="width: 985px; height: 876px; background-image: url(images/login/bg2.png);">
                <table border="0" style="position: relative; margin-left: 1px; margin-top: 348px; width: 300px">
                    <tr height="45" align="left" valign="top">
                        <td><span>用户名：</span>
                            <asp:TextBox ID="Txt_UserCode" Style="border-style: none; width: 180px" MaxLength="12" runat="server" Text="admin"></asp:TextBox></td>
                    </tr>
                    <tr height="25" align="left" valign="top">
                        <td><span>密&nbsp;&nbsp;&nbsp;&nbsp;码：</span>
                            <input type="password" style="border-style: none; width: 180px;" maxlength="12" runat="server" id="Txt_Password" value="admin" /></td>
                    </tr>
                    <tr height="20" align="left" valign="top">
                        <td>
                            <asp:Label ID="Lab_Message" Style="font-size: 12px; color: Red" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr height="30" align="center" valign="top">
                        <td>
                            <asp:Button ID="Btn_Login" Style="background-image: url(images/login/btn-01.png); width: 171px; height: 61px; border-style: none" runat="server"
                                OnMouseOver="this.style.backgroundImage='url(images/login/btn-03.png)'"
                                OnMouseOut="this.style.backgroundImage='url(images/login/btn-01.png)'"
                                OnClick="BtnLogin_Click" /></td>
                    </tr>
                </table>
            </div>

            <div id="div_msg" style="display: none">
                本系统要求使用IE9以上版本浏览器。
            </div>
        </div>

    </form>
</body>
</html>
