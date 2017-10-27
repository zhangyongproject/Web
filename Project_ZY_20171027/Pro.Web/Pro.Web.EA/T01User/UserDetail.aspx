<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="UserDetail.aspx.cs" Inherits="T01User_UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">

        var act = getUrlParam("act");
        var from = getUrlParam("from");
        var userid = getUrlParam("userid");
        var username = getUrlParam("username");

        window.onload = function () {
            $j("#txtdesc").val("");
            $j("#txtusername").focus();
            if (!userid) { $j("#txtusername").css("display", "block"); $j("#lblusername").css("display", "none"); return; }
            else { $j("#txtusername").css("display", "none"); $j("#lblusername").css("display", "block"); }
            var wm = new WebMethodProxy("UserList.aspx", "GetList");
            wm.AddParam("strparam", "{\"userid\":\"" + userid + "\"}")
            wm.Call(Init);
        };

        var mModuleName = "";
        function Init(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            //$j("#txtusername").attr("disabled", "disabled");
            $j("#lblusername").text(rootNode.selectSingleNode("items/item/USERNAME").text);
            $j("#txtusernick").val(rootNode.selectSingleNode("items/item/USERNICK").text);
            $j("#sltusertype").val(rootNode.selectSingleNode("items/item/USERTYPE").text);
            $j("#txtmobile").val(rootNode.selectSingleNode("items/item/MOBILEPHONE").text);
            $j("#txtdesc").val(rootNode.selectSingleNode("items/item/DESCRIPTION").text);
            $j("#txtuserpwd").val(rootNode.selectSingleNode("items/item/USERPWD").text);

        }

        function AddorEdit() {
            if (act == "add" && $j("#txtusername").val().trim().length == 0) { alert("用户名必须输入。"); return; }
            else if (!userid && !g_re_pwd.test($j("#txtusername").val())) { alert("用户名必须由数字和26个英文字母组成。"); return; }
            else if (!g_re_pwd.test($j("#txtuserpwd").val())) { alert("密码必须由数字和26个英文字母组成。"); return; }
            else if ($j("#txtusernick").val().trim().length == 0) { alert("用户昵称必须输入。"); return; }
            else if ($j("#txtmobile").val().trim().length > 0 && !g_re_mobile.test($j("#txtmobile").val())) { alert("手机号码输入不正确。"); return; }
            else if ($j("#txtusername").val().length2() > 30) { alert("用户名输入内容超出限制。"); return; }
            else if ($j("#txtusernick").val().length2() > 30) { alert("用户昵称输入内容超出限制。"); return; }
            else if ($j("#txtdesc").val().length2() > 200) { alert("描述输入内容超出限制。"); return; }
            else { }
            var hashtable = new Hashtable();
            hashtable.add("userid", (userid) ? userid : "-1");
            hashtable.add("username", ($j("#txtusername").val().trim()));
            hashtable.add("usernick", ($j("#txtusernick").val().trim()));
            hashtable.add("usertype", ($j("#sltusertype").val()));
            hashtable.add("mobilephone", ($j("#txtmobile").val().trim()));
            hashtable.add("description", ($j("#txtdesc").val().trim()));
            hashtable.add("userpwd", ($j("#txtuserpwd").val().trim()));

            var wm = new WebMethodProxy("UserDetail.aspx", "AddorEdit");
            wm.AddParam("strparam", hashtable.toJson2());
            wm.Call(function (ret) {
                if (ret == undefined) { alert("输入项存在特殊字符（如：&，%，\等）。"); return; }
                var xmlDoc = CreateXmlFromString(ret);
                var rootNode = xmlDoc.selectSingleNode("xml");
                var code = rootNode.getAttribute("code");
                if (code == "1") {
                    setTimeout(function () {
                        callback(0, code);
                    }, 1000);
                }
                else if (code == "-2") {
                    window.alert("已存在相同的用户名。");
                }
                else {
                    window.alert(rootNode.getAttribute("msg"));
                }
            });
        }

        document.onkeydown = function () {
            if (window.event && window.event.keyCode == 13) {
                window.event.returnValue = false;
            }
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">


    <table class="tb_form" cellspacing="1">
        <tr>
            <td class="tit" style="text-align: center">用户名
            </td>
            <td>
                <input type="text" id="txtusername" style="width: 200px" maxlength="30" />
                <label id="lblusername" style="width: 200px"></label>
            </td>
            <td>
                <label style="color: gray">(数字和英文字母组成)</label></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">用户密码
            </td>
            <td>
                <input type="password" id="txtuserpwd" style="width: 200px" maxlength="25" />
            </td>
            <td>
                <label style="color: gray">(数字和英文字母组成)</label></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">用户昵称
            </td>
            <td>
                <input type="text" id="txtusernick" style="width: 200px" maxlength="30" />
            </td>
            <td>
                <label style="color: gray">(1～30，中文占2个)</label></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">用户类型
            </td>
            <td>
                <select id="sltusertype" style="width: 150px">
                    <option value="0">管理员</option>
                    <option value="1" selected="selected">普通用户</option>
                </select>
            </td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">手机号码
            </td>
            <td>
                <input type="text" id="txtmobile" style="width: 200px" maxlength="11" />
            </td>
        </tr>
        <tr>
            <td class="tit" align="center">描述
            </td>
            <td>
                <textarea rows="5" cols="25" id="txtdesc">

                </textarea>
            </td>
            <td>
                <label style="color: gray">(0～200，中文占2个)</label></td>
        </tr>

        <tr style="height: 40px; vertical-align: middle;">
            <td align="center" colspan="4" style="margin-top: 10px;">
                <input type="button" id="Button1" class="input_btn" value=" 确定 " onclick="AddorEdit()" />
                <input type="button" id="Button2" class="input_btn" value=" 取消 " onclick="callback(1, null);" />
            </td>
        </tr>
    </table>
</asp:Content>

