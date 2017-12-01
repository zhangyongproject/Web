<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Main.master" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="T01User_UserList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        var objDataClient = new DataClient(document.location, "GetList", "div_list", "userlist.xsl");
        objDataClient.OnQuery = function () { ShowConfirm('正在进行数据查询...', '稍候', null, "提示"); document.body.style.cursor = "wait"; };
        //objDataClient.OnData = function(xmlDoc) { ShowDebugInfo(xmlDoc.xml); };
        //objDataClient.OnError = function(strMessage) { alert(strMessage); };
        objDataClient.OnQueryCompleted = function (queryOK) { __UnMaskBG(); document.body.style.cursor = "default"; };
        var hashtable = new Hashtable();
        var userType = "<%=UserType%>";

        window.onload = function () {
            GetList();

        }
        function GetList() {
            hashtable = new Hashtable();
            hashtable.add('username', $j("#txtusername").val());
            hashtable.add('usernick', $j("#txtusernick").val());
            hashtable.add('usertype', $j("#sltusertype").val());
            objDataClient._PageIndex = 1;
            objDataClient.Query(hashtable.toJson());
            
            window.setTimeout(function () {
                if (userType == "0") { return; }
                $j("#table_userlist a").css("display", "none");
                $j("#btnAdd").css("display", "none");
            }, 500);
        }

        function AddUser() {
            var url = "UserDetail.aspx?act=add&from=list";
            ShowPage(url, "添加用户", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function EditUser(userid, username) {
            var url = "UserDetail.aspx?act=edit&from=list&userid=" + userid + "&username=" + username;
            ShowPage(url, "修改用户", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function DelUser(userid) {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                if (!userid) { return; }
                var wm = new WebMethodProxy(document.location, "Delete");
                wm.AddParam("strparam", "{\"userid\":\"" + userid + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        GetList();
                    }
                    else if (code == "-2") {
                        window.alert("不存在该帐号。");
                    }
                    else {
                        window.alert(rootNode.getAttribute("msg"));
                    }
                });
            });
        }
    </script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table class="tb_buttonbar" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <input type="button" class="input_btn" id="btnSreach" value="查询" onclick="GetList()" btnicon="btnIcon01.png" />
                <input type="button" class="input_btn" id="btnAdd" value="添加" onclick="AddUser()" btnicon="btnIcon03.png" />
            </td>
            <td align="right" valign="bottom">
                <img id="IconQueryInfo" style="visibility: hidden;" />
            </td>
        </tr>
    </table>

    <div id="div_querycondition">
        <table class="tb_form" style="" cellspacing="0">
            <tr>
                <td class="tit">用户名
                </td>
                <td>
                    <input type="text" id="txtusername" style="width: 120px" value="" /></td>
                <td class="tit">用户昵称</td>
                <td style="">
                    <input type="text" id="txtusernick" style="width: 120px" value="" /></td>
                <td>用户类型</td>
                <td style="width: 40%">
                    <select id="sltusertype" style="width: 150px">
                        <option value="-1">全部</option>
                        <option value="0">管理员</option>
                        <option value="1">普通用户</option>
                    </select>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        SetExtendIcon("IconQueryInfo", "div_querycondition");
    </script>


    <div class="grid_div" id="div_list">
    </div>

</asp:Content>

