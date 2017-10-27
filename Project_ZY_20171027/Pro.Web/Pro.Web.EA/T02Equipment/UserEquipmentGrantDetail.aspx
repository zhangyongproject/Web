<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="UserEquipmentGrantDetail.aspx.cs" Inherits="T02Equipment_UserEquipmentGrantDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../include/frame/message.js"></script>
    <script type="text/javascript">

        var act = getUrlParam("act");
        var from = getUrlParam("from");
        var uegid = getUrlParam("uegid");
        var uegids = getUrlParam("uegids");

        window.onload = function () {
            var wm = new WebMethodProxy("../T03Timing/TimingList.aspx", "GetInitInfo");
            wm.Call(Init);
        };
        var mXml = null;
        function Init(ret) {
            mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            SetSelOption($("sltuser"), mXml.selectSingleNode("xml/users"));
            //SetSelOption($("sltequipment"), mXml.selectSingleNode("xml/equipments"));

            $j("#txtdesc").val("");
            var date = new Date();
            $('txtbegintime').value = date.format("yyyy-MM-dd");
            $('txtendtime').value = date.addMonth(12).format("yyyy-MM-dd");
            if (act == "editdate") {
                $j("#trUser").css("display", "none");
                $j("#trEquipment").css("display", "none");
                $j("#trDesc").css("display", "none");
                return;
            }
            if (!uegid) { return; }
            var wm = new WebMethodProxy("UserEquipmentGrantList.aspx", "GetList");
            wm.AddParam("strparam", "{\"uegid\":\"" + uegid + "\"}")
            wm.Call(LoadData);
        }

        var mModuleName = "";
        function LoadData(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            mModuleName = GetAttributeValue(rootNode, "value");
            $j("#sltuser").val(rootNode.selectSingleNode("items/item/USERID").text);
            $j("#txtequipments").val(rootNode.selectSingleNode("items/item/EINAME").text);
            $j("#txtequipments").data("eiid", rootNode.selectSingleNode("items/item/EIID").text);
            $j("#txtbegintime").val(rootNode.selectSingleNode("items/item/STARTDATE").text.split(' ')[0]);
            $j("#txtendtime").val(rootNode.selectSingleNode("items/item/ENDDATE").text.split(' ')[0]);
            $j("#txtdesc").val(rootNode.selectSingleNode("items/item/DESCRIPTION").text);
        }

        var eiids, einames;
        function AddorEdit() {
            if (act != "editdate" && $j("#sltuser").val().trim().length == 0) { alert("请选择用户。"); return; }
            else if ($j("#txtbegintime").val().trim().length == 0) { alert("授权有效开始时间必须输入。"); return; }
            else if ($j("#txtendtime").val().trim().length == 0) { alert("授权有效结束时间必须输入。"); return; }
                //else if ($j("#txtendtime").val().toDate("yyyy-MM-dd") < $j("#txtendtime").val().toDate("yyyy-MM-dd")) { alert("授权有效结束时间必须大于有效开始时间。"); return; }
            else if ($j("#txtdesc").val().length2() > 200) { alert("描述输入内容超出限制。"); return; }
            else { }
            var hashtable = new Hashtable();
            hashtable.add("uegid", (uegid) ? uegid : "-1");
            hashtable.add("uegids", (uegids) ? uegids : "-1");
            hashtable.add("userid", ($j("#sltuser").val().trim()));
            hashtable.add("username", ($j("#sltuser").find("option:selected").text().trim().split("【")[0]));
            hashtable.add("eiid", ($j("#txtequipments").data("eiid")));
            hashtable.add("einame", ($j("#txtequipments").val().trim()));
            hashtable.add("begintime", ($j("#txtbegintime").val().trim()));
            hashtable.add("endtime", ($j("#txtendtime").val().trim()));
            hashtable.add("description", ($j("#txtdesc").val().trim()));
            // act= add 批量添加（单个添加） act=editdate 批量修改时间 
            var wm = new WebMethodProxy("UserEquipmentGrantDetail.aspx", (act == "add") ? "EquipmentsGrant" : ((act == "editdate") ? "BatchEditDate" : "AddorEdit"));
            if (act == "editdate") { hashtable.remove("eiid"); hashtable.remove("einame"); hashtable.remove("description"); }
            else if (act == "edit" && ($j("#txtequipments").data("eiid")).split(',').length > 1) { alert("修改时授权设备只允许选择一个，请重新选择。"); return; }
            else if (act == "add") {
                if ($j("#txtequipments").data("eiid") == undefined || $j("#txtequipments").data("eiid").length == 0) { alert("至少选择一个授权设备。"); return; }
            }
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

        function SelectExamEquipment() {
            var xmlDoc = CreateXmlFromString(mXml.selectSingleNode("xml/notgrantequipments").xml.replaceAll("notgrantequipments", "xml").replaceAll("item", "data").replaceAll("key", "value").replaceAll("name", "data"));
            ShowMultiSelectWindow("请选择未授权设备", xmlDoc, $("txtequipments").tag, function (value, text) {
                $j("#txtequipments").val(text);
                $j("#txtequipments").data("eiid", value);
            }, 400);
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">


    <table class="tb_form" cellspacing="1">
        <tr id="trUser">
            <td class="tit" style="text-align: center">用户名
            </td>
            <td>
                <select id="sltuser" style="width: 200px">
                    <option value=""></option>
                </select>
            </td>
            <td></td>
        </tr>
        <tr id="trEquipment">
            <td class="tit" style="text-align: center">授权设备</td>
            <td>
                <input type="text" id="txtequipments" style="width: 160px" value="" />
                <input type="button" id="btnequipments" value="..." onclick="SelectExamEquipment()" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">授权期限
            </td>
            <td>
                <input type="text" id="txtbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtendtime\')}' })" class="Wdate" style="width: 85px;" />
                -
                <input type="text" id="txtendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtbegintime\')}' })" class="Wdate" style="width: 85px;" />
            </td>
            <td></td>
        </tr>
        <tr id="trDesc">
            <td class="tit" style="text-align: center">描述
            </td>
            <td>
                <textarea rows="5" cols="25" id="txtdesc">

                </textarea>
            </td>
            <td>
                <label style="color: gray">(0～200，中文占2个)</label></td>
        </tr>

        <tr style="height: 60px; vertical-align: middle;">
            <td align="center" colspan="4" style="margin-top: 10px;">
                <input type="button" id="Button1" class="input_btn" value=" 确定 " onclick="AddorEdit()" />
                <input type="button" id="Button2" class="input_btn" value=" 取消 " onclick="callback(1, null);" />
            </td>
        </tr>
    </table>
</asp:Content>

