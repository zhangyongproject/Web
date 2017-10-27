<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="ActiveCodeDetail.aspx.cs" Inherits="T04ActiveCode_ActiveCodeDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">

        var act = getUrlParam("act");
        var from = getUrlParam("from");
        var acid = getUrlParam("acid");
        var accode = getUrlParam("accode");

        window.onload = function () {
            $j("#txtdesc").val("");
            //var date = new Date();
            //$('txtbegintime').value = date.format("yyyy-MM-dd");
            //$('txtendtime').value = date.addMonth(12).format("yyyy-MM-dd");
            if (!acid) { $j("#txtaccode").css("display", "block"); $j("#lblaccode").css("display", "none"); return; }
            else { $j("#txtaccode").css("display", "none"); $j("#lblaccode").css("display", "block"); }
            var wm = new WebMethodProxy("ActiveCodeList.aspx", "GetList");
            wm.AddParam("strparam", "{\"acid\":\"" + acid + "\"}")
            wm.Call(Init);
        };

        var mModuleName = "";
        function Init(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            mModuleName = GetAttributeValue(rootNode, "value");
            //$j("#txtaccode").attr("disabled", "disabled");
            $j("#lblaccode").text(rootNode.selectSingleNode("items/item/ACCODE").text);
            //$j("#txtbegintime").val(rootNode.selectSingleNode("items/item/STARTDATE").text.split(' ')[0]);
            //$j("#txtendtime").val(rootNode.selectSingleNode("items/item/ENDDATE").text.split(' ')[0]);
            $j("#txtdesc").val(rootNode.selectSingleNode("items/item/DESCRIPTION").text);
        }

        function AddorEdit() {
            if (act == "add" && $j("#txtaccode").val().trim().length == 0) { alert("激活码必须输入。"); return; }
            else if ($j("#txtaccode").val().length2() > 30) { alert("激活码输入内容超出限制。"); return; }
            else if ($j("#txtdesc").val().length2() > 200) { alert("描述输入内容超出限制。"); return; }
            var hashtable = new Hashtable();
            hashtable.add("acid", (acid) ? acid : "-1");
            hashtable.add("accode", ($j("#txtaccode").val().trim()));
            //hashtable.add("begintime", ($j("#txtbegintime").val().trim()));
            //hashtable.add("endtime", ($j("#txtendtime").val().trim()));
            hashtable.add("description", ($j("#txtdesc").val().trim()));

            var wm = new WebMethodProxy("ActiveCodeDetail.aspx", "AddorEdit");
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
                    window.alert("已存在相同的激活码。");
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
            <td class="tit" style="text-align: center">激活码
            </td>
            <td>
                <input type="text" id="txtaccode" style="width: 180px" maxlength="30" />
                <label id="lblaccode" style="width: 200px"></label>
            </td>
            <td>
                <label style="color: gray">(1～30，数字或英文字母)</label></td>
        </tr>
        <%--<tr>
            <td class="tit" style="text-align: center">有效期限
            </td>
            <td>
                <input type="text" id="txtbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtendtime\')}' })" class="Wdate" style="width: 85px;" />
                -
                <input type="text" id="txtendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtbegintime\')}' })" class="Wdate" style="width: 85px;" />
            </td>
        </tr>--%>
        <tr>
            <td class="tit" style="text-align: center">描述
            </td>
            <td>
                <textarea rows="10" cols="22" id="txtdesc">

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

