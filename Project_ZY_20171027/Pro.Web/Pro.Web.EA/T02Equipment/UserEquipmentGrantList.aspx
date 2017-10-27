<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Main.master" AutoEventWireup="true" CodeFile="UserEquipmentGrantList.aspx.cs" Inherits="T02Equipment_UserEquipmentGrantList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        var objDataClient = new DataClient(document.location, "GetList", "div_list", "userequipmentgrantlist.xsl");
        objDataClient.OnQuery = function () { ShowConfirm('正在进行数据查询...', '稍候', null, "提示"); document.body.style.cursor = "wait"; };
        //objDataClient.OnData = function(xmlDoc) { ShowDebugInfo(xmlDoc.xml); };
        //objDataClient.OnError = function(strMessage) { alert(strMessage); };
        objDataClient.OnQueryCompleted = function (queryOK) { __UnMaskBG(); document.body.style.cursor = "default"; };
        var hashtable = new Hashtable();
        var userType = "<%=UserType%>";
        var mXml = null;
        window.onload = function () {
            var wm = new WebMethodProxy("../T03Timing/TimingList.aspx", "GetInitInfo");
            wm.Call(Init);
        };

        function Init(ret) {
            mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            SetSelOption($("sltuser"), mXml.selectSingleNode("xml/users"));
            SetSelOption($("sltequipment"), mXml.selectSingleNode("xml/equipments"));

            var date = new Date();
            $('txtbegintime').value = date.format("yyyy-MM-dd");
            $('txtendtime').value = date.addMonth(12).format("yyyy-MM-dd");

            GetList();

        }

        function GetList() {
            hashtable = new Hashtable();
            hashtable.add('userid', $j("#sltuser").val());
            hashtable.add('eiid', $j("#sltequipment").val());
            hashtable.add('begintime', $j("#txtbegintime").val());
            hashtable.add('endtime', $j("#txtendtime").val());
            objDataClient._PageIndex = 1;
            objDataClient.Query(hashtable.toJson());


            window.setTimeout(function () {
                checkItem();
                if (userType == "0") { return; }
                $j("#table_userequipmentgrantlist a").css("display", "none");
                $j("#btnAdd").css("display", "none");
            }, 300);
        }


        function Add() {
            var url = "UserEquipmentGrantDetail.aspx?act=add&from=list";
            ShowPage(url, "添加用户设备授权", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, 600, 450);
        }

        function Edit(uegid) {
            var url = "UserEquipmentGrantDetail.aspx?act=edit&from=list&uegid=" + uegid;
            ShowPage(url, "修改用户设备授权", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, 420, 330);
        }

        function Del(uegid) {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                if (!uegid) { return; }
                var wm = new WebMethodProxy(document.location, "Delete");
                wm.AddParam("strparam", "{\"uegid\":\"" + uegid + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        GetList();
                    }
                    else {
                        window.alert(rootNode.getAttribute("msg"));
                    }
                });
            });
        }

        //批量修改授权时间
        function SltEditDate() {
            uegids = "";
            $j("input[type='checkbox'][name='chk_item']:checked").each(function (idx) {
                uegids += this.value + ",";
            });
            var url = "UserEquipmentGrantDetail.aspx?act=editdate&from=list&uegids=" + uegids.substr(0, uegids.length - 1);
            ShowPage(url, "批量修改授权时间", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, 350, 100);
        }

        //选择需授权用户
        function SltGrant() {
            var xmlDocUser = CreateXmlFromString(mXml.selectSingleNode("xml/users").xml.replaceAll("users", "xml").replaceAll("item", "data").replaceAll("key", "value").replaceAll("name", "data"));
            ShowSingleSelectWindow("请选择用户", xmlDocUser, $("btnGrant").tag, function (text, value) {
                var wm = new WebMethodProxy("EquipmentList.aspx", "GetNotGrantEquipment");
                wm.AddParam("strparam", "{\"status\":\"0\"}");
                wm.AddParam("pageIndex", "1");
                wm.AddParam("pageSize", "10000");
                wm.AddParam("sort", "1");
                wm.Call(function (xml) {
                    var xmlDoc = CreateXmlFromString(xml);
                    ShowMultiSelectWindow("请选择未授权设备", xmlDoc, $("btnGrant").tag, function (text, value) {
                        var wm = new WebMethodProxy("EquipmentDetail.aspx", "Edit");
                        wm.AddParam("strparam", "{\"stids\":\"" + text + "\",\"ltid\":\"" + ltid + "\"}");
                        wm.Call(function (ret) {
                            var xmlDoc = CreateXmlFromString(ret);
                            var rootNode = xmlDoc.selectSingleNode("xml");
                            GetAreaList();
                        });
                    }, 240);
                });
            }, 240);

            window.setTimeout(function () {
                $j("#btnComfirm0").attr("disabled", true);
            }, 100);
        }

        //批量删除、批量修改时间，按钮状态判断
        function checkItem() {
            $j("#btnDel").attr("disabled", $j("input[type='checkbox'][name='chk_item']:checked").size() == 0);
            $j("#btnEditDate").attr("disabled", $j("input[type='checkbox'][name='chk_item']:checked").size() == 0);
        }

        function SltDel() {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                uegids = "";
                $j("input[type='checkbox'][name='chk_item']:checked").each(function (idx) {
                    uegids += this.value + ",";
                });
                var wm = new WebMethodProxy(document.location, "Delete4Ids");
                wm.AddParam("strparam", "{\"uegid\":\"" + uegids.substr(0, uegids.length - 1) + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        ShowAlert("共选择" + uegids.split(",").length + "条记录，已成功删除" + rootNode.getAttribute("value") + "条。");
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
                <input type="button" class="input_btn" id="btnAdd" value="添加(批量)" onclick="Add()" btnicon="btnIcon03.png" />
                <%--<input type="button" class="input_btn" id="btnGrant" value="批量授权" onclick="SltGrant()" btnicon="import.png" />--%>
                <input type="button" class="input_btn" id="btnDel" value="批量删除" disabled="disabled" onclick="SltDel()" btnicon="item.png" />
                <input type="button" class="input_btn" id="btnEditDate" value="修改时间" disabled="disabled" onclick="SltEditDate()" btnicon="mark.png" />
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
                    <select id="sltuser" style="width: 150px">
                        <option value=""></option>
                    </select></td>
                <td class="tit">设备名称
                </td>
                <td>
                    <select id="sltequipment" style="width: 150px">
                        <option value=""></option>
                    </select></td>
                <td class="tit">有效时间</td>
                <td style="width: 40%">
                    <input type="text" id="txtbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtendtime\')}' })" class="Wdate" style="width: 130px;" />
                    -
                    <input type="text" id="txtendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtbegintime\')}' })" class="Wdate" style="width: 130px;" />
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


