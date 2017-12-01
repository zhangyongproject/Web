<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Main.master" AutoEventWireup="true" CodeFile="EquipmentList.aspx.cs" Inherits="T02Equipment_EquipmentList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        var objDataClient = new DataClient(document.location, "GetList", "div_list", "equipmentlist.xsl");
        objDataClient.OnQuery = function () { ShowConfirm('正在进行数据查询...', '稍候', null, "提示"); document.body.style.cursor = "wait"; };
        //objDataClient.OnData = function(xmlDoc) { ShowDebugInfo(xmlDoc.xml); };
        //objDataClient.OnError = function(strMessage) { alert(strMessage); };
        objDataClient.OnQueryCompleted = function (queryOK) { __UnMaskBG(); document.body.style.cursor = "default"; };
        var hashtable = new Hashtable();
        var userType = "<%=UserType%>";
        var currUserId = "<%=UserId%>";

        window.onload = function () {
            GetList();
        }

        function GetList() {

            hashtable = new Hashtable();
            hashtable.add('einame', $j("#txteiname").val());
            if (userType == "1")
                hashtable.add('userid', currUserId);
            //hashtable.add('iplist', $j("#txtiplist").val());
            objDataClient._PageIndex = 1;
            objDataClient.Query(hashtable.toJson());


            window.setTimeout(function () {
                if (userType == "0") { return; }
                $j("#table_equipmentlist a").css("display", "none");
                $j("#btnAdd").css("display", "none");
                $j("#btnDel").css("display", "none");
            }, 500);
        }


        function AddEquipment() {
            var url = "EquipmentDetail.aspx?act=add&from=list";
            ShowPage(url, "添加设备", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function EditEquipment(eiid, einame) {
            var url = "EquipmentDetail.aspx?act=edit&from=list&eiid=" + eiid + "&einame=" + einame;
            ShowPage(url, "修改设备信息", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function DelEquipment(eiid) {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                if (!eiid) { return; }
                var wm = new WebMethodProxy(document.location, "Delete");
                wm.AddParam("strparam", "{\"eiid\":\"" + eiid + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        GetList();
                    }
                    else if (code == "-2") {
                        window.alert("不存在该设备。");
                    }
                    else {
                        window.alert(rootNode.getAttribute("msg"));
                    }
                });
            });
        }

        //批量删除、批量修改时间，按钮状态判断
        function checkItem() {
            $j("#btnDel").attr("disabled", $j("input[type='checkbox'][name='chk_item']:checked").size() == 0);
        }

        function SltDel() {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                eiids = "";
                $j("input[type='checkbox'][name='chk_item']:checked").each(function (idx) {
                    eiids += this.value + ",";
                });
                var wm = new WebMethodProxy(document.location, "Delete4Ids");
                wm.AddParam("strparam", "{\"eiid\":\"" + eiids.substr(0, eiids.length - 1) + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        ShowAlert("共选择" + eiids.split(",").length + "条记录，已成功删除" + rootNode.getAttribute("value") + "条。");
                        GetList();
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
                <input type="button" class="input_btn" id="btnAdd" value="添加" onclick="AddEquipment()" btnicon="btnIcon03.png" />
                <input type="button" class="input_btn" id="btnDel" value="批量删除" disabled="disabled" onclick="SltDel()" btnicon="item.png" />
            </td>
            <td align="right" valign="bottom">
                <img id="IconQueryInfo" style="visibility: hidden;" />
            </td>
        </tr>
    </table>

    <div id="div_querycondition">
        <table class="tb_form" style="" cellspacing="0">
            <tr>
                <td class="tit">设备名称
                </td>
                <td style="width: 90%">
                    <input type="text" id="txteiname" style="width: 120px" value="" /></td>
                <%-- <td class="tit">IP列表</td>
                <td style="width: 70%">
                    <input type="text" id="txtiplist" style="width: 240px" value="" /></td>--%>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        SetExtendIcon("IconQueryInfo", "div_querycondition");
    </script>


    <div class="grid_div" id="div_list">
    </div>

</asp:Content>

