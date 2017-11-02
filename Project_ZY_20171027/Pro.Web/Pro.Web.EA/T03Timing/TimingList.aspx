<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Main.master" AutoEventWireup="true" CodeFile="TimingList.aspx.cs" Inherits="T03Timing_TimingList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        var objDataClient = new DataClient(document.location, "GetList", "div_list", "timinglist.xsl");
        objDataClient.OnQuery = function () { ShowConfirm('正在进行数据查询...', '稍候', null, "提示"); document.body.style.cursor = "wait"; };
        //objDataClient.OnData = function(xmlDoc) { ShowDebugInfo(xmlDoc.xml); };
        //objDataClient.OnError = function(strMessage) { alert(strMessage); };
        objDataClient.OnQueryCompleted = function (queryOK) { __UnMaskBG(); document.body.style.cursor = "default"; };
        var hashtable = new Hashtable();
        var userType = "<%=UserType%>";
        var currUserId = "<%=UserId%>";

        window.onload = function () {
            var wm = new WebMethodProxy(document.location, "GetInitInfo");
            wm.Call(Init);
        };

        function Init(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            SetSelOption($("sltuser"), mXml.selectSingleNode("xml/users"));
            SetSelOption($("sltequipment"), mXml.selectSingleNode("xml/equipments"));

            var date = new Date();
            $('txtexpbegintime').value = date.addMonth(-12).format("yyyy-MM-dd");
            $('txtexpendtime').value = date.addMonth(12).format("yyyy-MM-dd");

            GetList();
        }

        function GetList() {
            hashtable = new Hashtable();
            hashtable.add('userid', (userType == "0") ? $j("#sltuser").val() : currUserId);
            hashtable.add('eiid', $j("#sltequipment").val());
            hashtable.add('packname', $j("#txtpackname").val());
            hashtable.add('expbegintime', $j("#txtexpbegintime").val());
            hashtable.add('expendtime', $j("#txtexpendtime").val());
            objDataClient._PageIndex = 1;
            objDataClient.Query(hashtable.toJson());


            window.setTimeout(function () {
                if (userType == "0") { return; }
                //$j("#table_timinglist a").css("display", "none");
                $j("#sltuser").css("display", "none");
                $j("#tdUserTitle").css("display", "none");
                $j("#sltequipment").css("display", "none");
                $j("#tdEquTitle").css("display", "none");
            }, 500);
        }


        function Add() {
            var url = "TimingDetail.aspx?act=add&from=list";
            ShowPage(url, "添加定时启动记录", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, 600, 350);
        }

        function Edit(tsrid) {
            var url = "TimingDetail.aspx?act=edit&from=list&tsrid=" + tsrid;
            ShowPage(url, "修改定时启动记录", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, 450, 350);
        }

        // 发布
        function Release(tsrid, release) {
            var wm = new WebMethodProxy("TimingDetail.aspx", "AddorEdit");
            wm.AddParam("strparam", "{\"tsrid\":\"" + tsrid + "\",\"release\":\"" + (release == 1 ? 0 : 1) + "\"}");
            wm.Call(function (ret) {
                if (ret == undefined) { return; }
                var xmlDoc = CreateXmlFromString(ret);
                var rootNode = xmlDoc.selectSingleNode("xml");
                var code = rootNode.getAttribute("code");
                if (code == "1") {
                    GetList();
                    //$j("#aRelease").text(release == 0 ? "取消发布" : "发布");
                    //$j("#tdRelease").text(release == 0 ? "发布" : "未发布");
                    //$j("#tdRelease").css("color", release == 0 ? "lime" : "red");
                }
                else {
                    window.alert(rootNode.getAttribute("msg"));
                }
            });
        }

        function Del(tsrid) {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                if (!tsrid) { return; }
                var wm = new WebMethodProxy(document.location, "Delete");
                wm.AddParam("strparam", "{\"tsrid\":\"" + tsrid + "\"}");
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

        //批量发布，按钮状态判断
        function checkItem() {
            $j("#btnRelease").attr("disabled", $j("input[type='checkbox'][name='chk_item']:checked").size() == 0);
        }

        function SltRelease() {
            ShowConfirm("是否发布该记录？", "", function (code, value) {
                if (code != 0) return;
                tsrids = "";
                $j("input[type='checkbox'][name='chk_item']:checked").each(function (idx) {
                    tsrids += this.value + ",";
                });
                var wm = new WebMethodProxy(document.location, "ReleaseIds");
                wm.AddParam("strparam", "{\"tsrid\":\"" + tsrids.substr(0, tsrids.length - 1) + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        ShowAlert("共选择" + tsrids.split(",").length + "条记录，已成功发布" + rootNode.getAttribute("value") + "条。");
                        GetList();
                    }
                    else {
                        window.alert(rootNode.getAttribute("msg"));
                    }
                });
            });
        }
        function Relation() {

        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table class="tb_buttonbar" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <input type="button" class="input_btn" id="btnSreach" value="查询" onclick="GetList()" btnicon="btnIcon01.png" />
                <input type="button" class="input_btn" id="btnAdd" value="添加" onclick="Add()" btnicon="btnIcon03.png" /> 
                <input type="button" class="input_btn" id="btnRelease" value="批量发布" disabled="disabled" onclick="SltRelease()" btnicon="item.png" />
            </td>
            <td align="right" valign="bottom">
                <img id="IconQueryInfo" style="visibility: hidden;" />
            </td>
        </tr>
    </table>

    <div id="div_querycondition">
        <table class="tb_form" style="" cellspacing="0">
            <tr>
                <td class="tit">包名称
                </td>
                <td>
                    <input type="text" id="txtpackname" style="width: 120px" value="" /></td>
                <td class="tit">有效期限</td>
                <td>
                    <input type="text" id="txtexpbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtexpendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtexpbegintime\')}' })" class="Wdate" style="width: 130px;" />
                    -
                    <input type="text" id="txtexpendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtexpbegintime\')}' })" class="Wdate" style="width: 130px;" />
                </td>
                <td id="tdUserTitle" class="tit">用户
                </td>
                <td>
                    <select id="sltuser" style="width: 150px">
                        <option value=""></option>
                    </select></td>
                <td id="tdEquTitle" class="tit">设备名称
                </td>
                <td style="width: 5%">
                    <select id="sltequipment" style="width: 150px">
                        <option value=""></option>
                    </select></td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        SetExtendIcon("IconQueryInfo", "div_querycondition");
    </script>


    <div class="grid_div" id="div_list">
    </div>

</asp:Content>


