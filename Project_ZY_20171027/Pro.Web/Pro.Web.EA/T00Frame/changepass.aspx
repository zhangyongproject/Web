<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="changepass.aspx.cs" Inherits="T00Frame_changepass" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
        window.onload = function () { SetAutoClose("txt_oldpass"); }

        function doKeyDown(keyEvent) {
            if (event.keyCode == 13)
                keyEvent();
        }

        function OnChangePass() {
            if (!g_re_pwd.test($j("#txt_newpass1").val()) || !g_re_pwd.test($j("#txt_newpass2").val())) { alert("密码必须由数字和26个英文字母组成。"); return; }
            var txtOldPass = $("txt_oldpass").value;
            var txtNewPass1 = $("txt_newpass1").value;
            var txtNewPass2 = $("txt_newpass2").value;

            if (txtNewPass1 != txtNewPass2) {
                $("td_passinfo").innerHTML = "新密码输入不一致。";
                $("txt_newpass1").focus();
                return;
            }

            var objWebmethod = new WebMethodProxy(document.location, "ChangePass");
            objWebmethod.AddParam("oldPass", txtOldPass);
            objWebmethod.AddParam("newPass", txtNewPass1);
            objWebmethod.Call(OnChangePassCallback);
        }

        function OnChangePassCallback(strxml) {
            if (strxml == undefined) { return; }
            var xmlDoc = CreateXmlFromString(strxml);
            var rootNode = xmlDoc.selectSingleNode("xml");
            var code = rootNode.getAttribute("code");
            if (code == "1") {
                setTimeout(function () {
                    callback(0, "密码修改成功。");
                }, 1000);
            }
            else if (code == "-8") {
                window.alert("原密码错误。");
            }
            else {
                window.alert(rootNode.getAttribute("msg"));
            }
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table width="100%">
        <tr>
            <td width="80" align="center">
                <img src="../images/frame/key.gif" />
            </td>
            <td>
                <table align="center">
                    <tr>
                        <td>原密码</td>
                        <td>
                            <input type="password" id="txt_oldpass" style="width: 120px" onkeydown="doKeyDown(OnChangePass)" /></td>
                    </tr>
                    <tr>
                        <td>新密码</td>
                        <td>
                            <input type="password" id="txt_newpass1" style="width: 120px" onkeydown="doKeyDown(OnChangePass)" /></td>
                    </tr>
                    <tr>
                        <td>确认密码</td>
                        <td>
                            <input type="password" id="txt_newpass2" style="width: 120px" onkeydown="doKeyDown(OnChangePass)" /></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td id="td_passinfo" style="color: red">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <input type="button" onclick="OnChangePass()" value="更改密码" class="input_btn" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:Content>

