<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Main.master" AutoEventWireup="true" CodeFile="ActiveCodeList.aspx.cs" Inherits="T04ActiveCode_ActiveCodeList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        var objDataClient = new DataClient(document.location, "GetList", "div_list", "activecode.xsl");
        objDataClient.OnQuery = function () { ShowConfirm('正在进行数据查询...', '稍候', null, "提示"); document.body.style.cursor = "wait"; };
        //objDataClient.OnData = function(xmlDoc) { ShowDebugInfo(xmlDoc.xml); };
        //objDataClient.OnError = function(strMessage) { alert(strMessage); };
        objDataClient.OnQueryCompleted = function (queryOK) { __UnMaskBG(); document.body.style.cursor = "default"; };
        var hashtable = new Hashtable();
        var userType = "<%=UserType%>";

        window.onload = function () {
            //var date = new Date();
            //$('txtbegintime').value = date.format("yyyy-MM-dd");
            //$('txtendtime').value = date.addMonth(12).format("yyyy-MM-dd");
           
            GetList();
        }

        function GetList() {
            hashtable = new Hashtable();
            hashtable.add('accode', $j("#txtaccode").val());
            //hashtable.add('begintime', $j("#txtbegintime").val());
            //hashtable.add('endtime', $j("#txtendtime").val());
            objDataClient._PageIndex = 1;
            objDataClient.Query(hashtable.toJson());


            window.setTimeout(function () {
                if (userType == "0") { return; }
                $j("#table_activecode a").css("display", "none");
                $j("#btnAdd").css("display", "none");
            }, 500);
        }


        function Add() {
            var url = "ActiveCodeDetail.aspx?act=add&from=list";
            ShowPage(url, "添加激活码", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function Edit(acid, accode) {
            var url = "ActiveCodeDetail.aspx?act=edit&from=list&acid=" + acid + "&accode=" + accode;
            ShowPage(url, "修改激活码信息", function (retCode, retValue) {
                if (retCode == 0) {
                    GetList();
                }
            }, document.documentElement.clientWidth > 600 ? (document.documentElement.clientWidth - 100) : 600, document.documentElement.clientHeight > 400 ? (document.documentElement.clientHeight - 100) : 400);
        }

        function Del(acid) {
            ShowConfirm("是否要删除该记录？", "", function (code, value) {
                if (code != 0) return;
                if (!acid) { return; }
                var wm = new WebMethodProxy(document.location, "Delete");
                wm.AddParam("strparam", "{\"acid\":\"" + acid + "\"}");
                wm.Call(function (ret) {
                    if (ret == undefined) { return; }
                    var xmlDoc = CreateXmlFromString(ret);
                    var rootNode = xmlDoc.selectSingleNode("xml");
                    var code = rootNode.getAttribute("code");
                    if (code == "1") {
                        GetList();
                    }
                    else if (code == "-2") {
                        window.alert("不存在该激活码。");
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
                <input type="button" class="input_btn" id="btnAdd" value="添加" onclick="Add()" btnicon="btnIcon03.png" />
            </td>
            <td align="right" valign="bottom">
                <img id="IconQueryInfo" style="visibility: hidden;" />
            </td>
        </tr>
    </table>

    <div id="div_querycondition">
        <table class="tb_form" style="" cellspacing="0">
            <tr>
                <td class="tit">激活码
                </td>
                <td style="width: 90%">
                    <input type="text" id="txtaccode" style="width: 120px" value="" /></td>
               <%-- <td class="tit">有效时间</td>
                <td style="width: 70%">
                    <input type="text" id="txtbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtendtime\')}' })" class="Wdate" style="width: 130px;" />
                    -
                    <input type="text" id="txtendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtbegintime\')}' })" class="Wdate" style="width: 130px;" />
                </td>--%>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        SetExtendIcon("IconQueryInfo", "div_querycondition");
    </script>


    <div class="grid_div" id="div_list">
    </div>

</asp:Content>

