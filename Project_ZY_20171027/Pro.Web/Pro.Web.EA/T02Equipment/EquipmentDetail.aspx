<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="EquipmentDetail.aspx.cs" Inherits="T02Equipment_EquipmentDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">

        var act = getUrlParam("act");
        var from = getUrlParam("from");
        var eiid = getUrlParam("eiid");
        var einame = getUrlParam("einame");

        window.onload = function () {
            $j("#txtdesc").val("");
            $j("#txtiplist").val("");
            if (!eiid) { $j("#txteiname").css("display", "block"); $j("#lbleiname").css("display", "none"); return; }
            else { $j("#txteiname").css("display", "none"); $j("#lbleiname").css("display", "block"); }
            var wm = new WebMethodProxy("EquipmentList.aspx", "GetList");
            wm.AddParam("strparam", "{\"eiid\":\"" + eiid + "\"}")
            wm.Call(Init);
        };

        var mModuleName = "";
        function Init(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            mModuleName = GetAttributeValue(rootNode, "value");
            //$j("#txteiname").attr("disabled", "disabled");
            $j("#lbleiname").text(rootNode.selectSingleNode("items/item/EINAME").text);
            $j("#txtiplist").val(rootNode.selectSingleNode("items/item/IPLIST").text);
            $j("#txtdesc").val(rootNode.selectSingleNode("items/item/DESCRIPTION").text);
        }

        function AddorEdit() {
            if (act == "add" && $j("#txteiname").val().trim().length == 0) { alert("设备名称必须输入。"); return; }
            else if ($j("#txteiname").val().length2() > 30) { alert("设备输入内容超出限制。"); return; }
            else if ($j("#txtiplist").val().length2() > 200) { alert("IP列表输入内容超出限制。"); return; }
            else if ($j("#txtdesc").val().length2() > 200) { alert("描述输入内容超出限制。"); return; }
            else { }
            var hashtable = new Hashtable();
            hashtable.add("eiid", (eiid) ? eiid : "-1");
            hashtable.add("einame", ($j("#txteiname").val().trim()));
            hashtable.add("iplist", ($j("#txtiplist").val().trim()));
            hashtable.add("description", ($j("#txtdesc").val().trim()));

            var wm = new WebMethodProxy("EquipmentDetail.aspx", "AddorEdit");
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
                    window.alert("已存在相同的设备。");
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
            <td class="tit" style="text-align: center">设备名称
            </td>
            <td>
                <input type="text" id="txteiname" style="width: 200px" maxlength="30" />
                <label id="lbleiname" style="width: 200px"></label>
            </td>
            <td>
                <label style="color: gray">(1～30，中文占2个)</label></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">IP列表
            </td>
            <td>
                <textarea rows="5" cols="25" id="txtiplist">
                </textarea>
            </td>
            <td>
                <label style="color: gray">(逗号分割多个IP)</label></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">描述
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

