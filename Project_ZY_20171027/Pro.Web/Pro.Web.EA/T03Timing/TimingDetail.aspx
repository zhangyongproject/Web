<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="TimingDetail.aspx.cs" Inherits="T03Timing_TimingDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../include/frame/message.js"></script>
    <script type="text/javascript">

        var act = getUrlParam("act");
        var from = getUrlParam("from");
        var tsrid = getUrlParam("tsrid");
        var userType = "<%=UserType%>";
        var currUserId = "<%=UserId%>";
        var mXml = '';
        window.onload = function () {
            var wm = new WebMethodProxy("TimingList.aspx", "GetInitInfo");
            wm.Call(Init);
        };

        function Init(ret) {
            mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            SetSelOption2($("sltuser"),
                CreateXmlFromString("<users>" + mXml.selectSingleNode("xml/users/item[@key='" + currUserId + "']").xml + "</users>").selectSingleNode("users"));
            //SetSelOption($("sltuser"), mXml.selectSingleNode("xml/users"));
            if (userType == "1") { $j("#sltuser").val(currUserId); $j("#trUser").css("display", "none"); }
            $j("#txtdesc").val("");
            var date = new Date();
            $('txtexpbegintime').value = date.format("yyyy-MM-dd");
            $('txtexpendtime').value = date.addMonth(12).format("yyyy-MM-dd");
            $('txtbegintime').value = "00:00:00";
            $('txtendtime').value = "23:59:59";
            if (!tsrid) { return; }
            var wm = new WebMethodProxy("TimingList.aspx", "GetList");
            wm.AddParam("strparam", "{\"tsrid\":\"" + tsrid + "\"}");
            wm.Call(LoadData);
        }


        var mModuleName = "";
        function LoadData(ret) {
            var mXml = CreateXmlFromString(ret);
            var rootNode = mXml.selectSingleNode("xml");
            mModuleName = GetAttributeValue(rootNode, "value");
            $j("#txtpackname").val(rootNode.selectSingleNode("items/item/PACKNAME").text);
            $j("#sltuser").val(rootNode.selectSingleNode("items/item/USERID").text);
            //$j("#sltequipment").val(rootNode.selectSingleNode("items/item/EIID").text);
            $j("#txtequipments").val(rootNode.selectSingleNode("items/item/EINAME").text);
            $j("#txtequipments").data("eiid", rootNode.selectSingleNode("items/item/EIID").text);
            $j("#txtbegintime").val(rootNode.selectSingleNode("items/item/STARTDATE").text.split(' ')[1]);
            $j("#txtendtime").val(rootNode.selectSingleNode("items/item/ENDDATE").text.split(' ')[1]);
            $j("#txtexpbegintime").val(rootNode.selectSingleNode("items/item/EXPSTARTDATE").text.split(' ')[0]);
            $j("#txtexpendtime").val(rootNode.selectSingleNode("items/item/EXPENDDATE").text.split(' ')[0]);
            $j("#sltrelease").val(rootNode.selectSingleNode("items/item/RELEASE").text);
            $j("#txtdesc").val(rootNode.selectSingleNode("items/item/DESCRIPTION").text);
        }

        function AddorEdit() { 
            if ($j("#txtpackname").val().trim().length == 0) { alert("包名称必须输入。"); return; }
            else if ($j("#txtpackname").val().length2() > 100) { alert("包名称输入内容超出限制。"); return; }
            else if ($j("#txtexpbegintime").val().trim().length == 0) { alert("有效期限开始时间必须输入。"); return; }
            else if ($j("#txtexpendtime").val().trim().length == 0) { alert("有效期限结束时间必须输入。"); return; }
            else if ($j("#txtbegintime").val().trim().length == 0) { alert("运行开始时间必须输入。"); return; }
            else if ($j("#txtendtime").val().trim().length == 0) { alert("运行结束时间必须输入。"); return; }
                //else if (GetInt($j("#txtendtime").val().replaceAll(':', ''), 0) < GetInt($j("#txtbegintime").val().replaceAll(':', ''), 0))
                //{ alert("运行结束时间必须大于开始时间。"); return; }
            else if ($j("#txtdesc").val().length2() > 200) { alert("描述输入内容超出限制。"); return; }
            else { }
            var hashtable = new Hashtable();
            hashtable.add("tsrid", (tsrid) ? tsrid : "-1");
            hashtable.add("userid", ($j("#sltuser").val().trim()));
            hashtable.add("username", ($j("#sltuser").find("option:selected").text().trim().split("【")[0]));
            //hashtable.add("eiid", ($j("#sltequipment").val().trim()));
            //hashtable.add("einame", ($j("#sltequipment").find("option:selected").text().trim()));
            hashtable.add("eiid", ($j("#txtequipments").data("eiid")));
            hashtable.add("einame", ($j("#txtequipments").val().trim()));
            hashtable.add("packname", ($j("#txtpackname").val().trim()));
            hashtable.add("begintime", ($j("#txtbegintime").val().trim()));
            hashtable.add("endtime", ($j("#txtendtime").val().trim()));
            hashtable.add("description", ($j("#txtdesc").val().trim()));
            hashtable.add('expbegintime', $j("#txtexpbegintime").val());
            hashtable.add('expendtime', $j("#txtexpendtime").val());
            hashtable.add('release', $j("#sltrelease").val());

            var wm = new WebMethodProxy("TimingDetail.aspx", (act == "add") ? "UserEquipmentsGrant" : "AddorEdit");
            if (act == "edit" && ($j("#txtequipments").data("eiid")).split(',').length > 1) { alert("修改时授权设备只允许选择一个，请重新选择。"); return; }
            else if (act == "add") {
                if (!$j("#txtequipments").data("eiid") || $j("#txtequipments").data("eiid").length == 0) { alert("至少选择一个授权设备。"); return; }
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

        //选择授权设备
        function SelectExamEquipment() {
            var xmlDoc = CreateXmlFromString(mXml.selectSingleNode("xml/grantequipments").xml.replaceAll("grantequipments", "xml").replaceAll("item", "data").replaceAll("key", "value").replaceAll("name", "data"));
            ShowMultiSelectWindow("请选择已授权设备", xmlDoc, $("txtequipments").tag, function (value, text) {
                $j("#txtequipments").val(text);
                $j("#txtequipments").data("eiid", value);
            }, 500);
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">


    <table class="tb_form" cellspacing="1">
        <tr>
            <td class="tit" style="text-align: center;">包名称
            </td>
            <td colspan=3>
                <input type="text" id="txtpackname" style="width:100%;" maxlength="100" />
            </td>
            <td>
                <label style="color: gray">(1～100，中文占2个)</label>

            </td>
        </tr>
        <tr id="trUser">
            <td class="tit" style="text-align: center">用户
            </td>
            <td colspan=3>
                <select id="sltuser" style="width:100%;">
                    <%--<option value=""></option>--%>
                </select>
            </td>
            <td></td>
        </tr>
        <%--  <tr>
            <td class="tit" style="text-align: center">设备名称
            </td>
            <td>
                <select id="sltequipment" style="width: 200px">
                    <option value=""></option>
                </select>
            </td>
        </tr>--%>
        <tr>
            <td class="tit" style="text-align: center">授权设备</td>
            <td colspan=3>
                <input type="text" id="txtequipments" style="width:100%;" value="" />
            </td>
            <td>
                <input type="button" value="..." onclick="SelectExamEquipment()" />
            </td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">有效期限
            </td>
            <td>
                <input type="text" id="txtexpbegintime" onfocus=" WdatePicker({ dateFmt: 'yyyy-MM-dd', onpicked: function () { $('txtexpendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtexpbegintime\')}' })" class="Wdate" style="width:100%;" />
            </td>
             <td>
                 -
             </td>
            <td>
                <input type="text" id="txtexpendtime" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtexpbegintime\')}' })" class="Wdate" style="width:100%;" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">运行时间
            </td>
            <td>
                <input type="text" id="txtbegintime" onfocus=" WdatePicker({ dateFmt: 'HH:mm:ss', onpicked: function () { $('txtendtime').focus(); }, maxDate: '#F{$dp.$D(\'txtendtime\')}' })" class="Wdate" style="width:100%;" />
            </td>
             <td>
                -
             </td>
            <td>
                    <input type="text" id="txtendtime" onfocus="WdatePicker({ dateFmt: 'HH:mm:ss', minDate: '#F{$dp.$D(\'txtbegintime\')}' })" class="Wdate" style="width:100%;" />
            </td>

            <td></td>
        </tr>
        <tr>

            <td class="tit" style="text-align: center">状态
            </td>
            <td colspan=3>
                <select id="sltrelease" style="width:100%;" >
                    <option value="0">发布</option>
                    <option value="1" selected="selected">未发布</option>
                </select></td>
            <td></td>
        </tr>
        <tr>
            <td class="tit" style="text-align: center">描述
            </td>
            <td colspan=3>
                <textarea rows="5" cols="25" id="txtdesc"  style="width:100%;">

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

