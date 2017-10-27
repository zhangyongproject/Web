//------------------------------------------------------------------------------------------------------------------------------------
//
//  Message box
//
//------------------------------------------------------------------------------------------------------------------------------------

//在屏幕右下角显示信息窗口（每次只能显示一个窗口）
function ShowMessageBox(strMessage, strTitle, strTime) {
    if (!strTitle || strTitle == "") strTitle = "信息提示";
    if (!strTime || strTime == "") strTime = (new Date()).format("yyyy-MM-dd HH:mm");

    var objMessageHead = $("floathead");
    var objMessageContent = $("MessageContent");

    var msg = strMessage;//.Trim();
    msg = "<font color=gray>[" + strTime + "]</font><br><br>" + msg;

    var objFloater = $("floater");
    objMessageContent.style.overflow = "";
    objFloater.style.visibility = "visible";

    objFloater.style.pixelTop = document.documentElement.clientHeight + document.documentElement.scrollTop;
    objFloater.style.pixelLeft = document.documentElement.clientWidth + document.documentElement.scrollLeft - objFloater.clientWidth;
    objMessageHead.innerText = strTitle;
    objMessageContent.innerHTML = msg;

    try {
        window.focus();
    } catch (e) { }

    //声音提示  
    var msgoundid = "msgsound";
    var msgoundx = $(msgoundid);
    msgoundx.src = mImgMsgFilePath + "/Msg.wav";
    //msgoundx.loop=0;   
    //setTimeout(__TurnOffSound,4000);

    FloatFlag = false;
    setTimeout(__MoveMessageBox, 10);
}

function __TurnOffSound() {
    var msgoundid = "msgsound";
    var msgoundx = $(msgoundid);
    msgoundx.src = "";
}

function __MoveMessageBox() {
    var objFloater = $("floater");

    //    objFloater.style.pixelTop = document.documentElement.clientHeight + document.documentElement.scrollTop - objFloater.clientHeight;
    //    FloatFlag = true;
    //    return;

    if (parseInt(objFloater.style.pixelTop) > document.documentElement.clientHeight + document.documentElement.scrollTop - objFloater.clientHeight - 5) {
        objFloater.style.pixelTop = parseInt(objFloater.style.pixelTop) - 5;
        setTimeout(__MoveMessageBox, 12);
    }
    else {
        objFloater.style.pixelTop = document.documentElement.clientHeight + document.documentElement.scrollTop - objFloater.clientHeight;
        $("MessageContent").style.overflow = "auto";

        var theParent = document.documentElement;
        if (objFloater.style.pixelTop < theParent.scrollTop) objFloater.style.pixelTop = theParent.scrollTop;
        if (objFloater.style.pixelTop > theParent.clientHeight + theParent.scrollTop - objFloater.clientHeight)
            objFloater.style.pixelTop = theParent.clientHeight + theParent.scrollTop - objFloater.clientHeight;

        if (objFloater.style.pixelLeft < theParent.scrollLeft) objFloater.style.pixelLeft = theParent.scrollLeft;
        if (objFloater.style.pixelLeft > theParent.clientWidth + theParent.scrollLeft - objFloater.clientWidth)
            objFloater.style.pixelLeft = theParent.clientWidth + theParent.scrollLeft - objFloater.clientWidth;


        lastScrollY = document.documentElement.scrollTop;  //document.body.scrollTop;
        lastScrollX = document.documentElement.scrollLeft;  //document.body.scrollLeft; 
        FloatFlag = true;
    }
}

function __CloseMessageBox() {
    $("floater").style.visibility = "hidden";
    __TurnOffSound();
}

//------------------------------------------------------------------------------------------------------------------------------------
//
//  diolog window
//
//------------------------------------------------------------------------------------------------------------------------------------

function ShowAlert(strMessage, strTitle, mIconIndex) {
    ShowConfirm(strMessage, "确定", null, strTitle, mIconIndex);
}

function ShowConfirm(strMessage, btnInfo, onBtnClickCallback, strTitle, mIconIndex) {
    if (!btnInfo || btnInfo == "") btnInfo = onBtnClickCallback ? "确定;取消" : "确定";

    // 1/2/3/4 info/warning/error/query
    if (mIconIndex == null) mIconIndex = onBtnClickCallback ? 4 : 1;
    if ((mIconIndex < 1) || (mIconIndex > 4)) mIconIndex = 0; //不绘制图标

    var strHtml = '<table border="0" width="100%" cellpadding="0" cellspacing="10">';

    strHtml += '<tr style ="vertical-align :middle">';
    if (mIconIndex != 0)
        strHtml += '<td width="48" style="text-align:center"><img src ="' + mImgMsgFilePath + '/icon' + mIconIndex + '.gif" /></td>';
    strHtml += '<td width="200" id="td_alertmsg" style="text-align:left">' + strMessage + '</td>';
    strHtml += '</tr>';

    strHtml += '<tr style ="vertical-align :middle;height:40"><td ' + ((mIconIndex != 0) ? 'colspan=2 ' : '') + 'style="text-align:center">';
    var btnList = btnInfo.split(";");
    for (i = 0; i < btnList.length; i++) {
        strHtml += '<input type=button id="btnComfirm' + i + '" class=input_btn value="' + btnList[i] + '" onclick=__OnMessageButtonClick(' + i + ',"' + btnList[i] + '")> ';
    }
    strHtml += '</td></tr>';
    strHtml += '</table>';

    __ShowWindow(strHtml, strTitle, onBtnClickCallback);
    $("btnComfirm0").focus();
}

function ShowInput(strInfo, defaultvalue, onInputCallback, defaultwidth) {
    return ShowInputA(strInfo, false, defaultvalue, onInputCallback, defaultwidth);
}

function ShowInputA(strInfo, IsPassword, defaultvalue, onInputCallback, defaultwidth) {
    var strMessage = "<table><tr><td>" + strInfo + "</td></tr>";
    strMessage += "<tr><td>" + "<input type=" + (IsPassword ? "password maxlength=10" : "text") + " id=txt_commoninput onFocus=\"this.select();\" onkeyup=\" if(event.keyCode==13){__OnMessageButtonClick(0,this.value);} \" >" + "</td></tr></table>";
    ShowConfirm(strMessage, "", function (retCode, retValue) { if ((retCode == 0) && (onInputCallback != null)) onInputCallback($("txt_commoninput").value); }, "输入框", 1);
    defaultwidth = (defaultwidth != null) ? parseInt(defaultwidth) : 0;
    if (defaultwidth < 200) defaultwidth = 200;
    if (defaultwidth > 400) defaultwidth = 400;
    $("txt_commoninput").style.width = defaultwidth;
    $("txt_commoninput").value = defaultvalue;
    $("txt_commoninput").focus();
}

function ShowSelectWindow(strInfo, xmlDoc, onSelectCallback, defaultwidth) {
    var rootNode = xmlDoc.selectSingleNode("xml"); if (rootNode == null) { alert("传入的xmlDoc参数异常，ShowSelectWindow函数调用出错"); return; }
    var strMessage = "<table><tr><td>" + strInfo + "</td></tr>";
    strMessage += "<tr><td>" + '<select id="commonselect" size="10" id="tableSelect"">';
    for (var i = 0; i < rootNode.childNodes.length; i++) {
        var itemNode = rootNode.childNodes[i];
        strMessage += '<option value="' + GetAttributeValue(itemNode, "data") + '" ' + (i == 0 ? "selected>" : '>') + GetAttributeValue(itemNode, "value") + '</option>';
    }
    strMessage += "</select>" + "</td></tr></table>";
    ShowConfirm(strMessage, "", function (retCode, retValue) {
        if ((retCode == 0) && (onSelectCallback != null) && ($("commonselect").value != null) && ($("commonselect").value != "")) onSelectCallback($("commonselect").value);
    }, "选择框", 1);
    defaultwidth = (defaultwidth != null) ? parseInt(defaultwidth) : 0;
    if (defaultwidth < 200) defaultwidth = 200;
    if (defaultwidth > 350) defaultwidth = 350;
    $("commonselect").style.width = defaultwidth;
    $("commonselect").focus();
}

function ShowMultiSelectWindow(strInfo, xmlDoc, defaultval, onSelectCallback, defaultwidth) {

    defaultwidth = (defaultwidth) ? parseInt(defaultwidth) : 200;
    var rootNode = xmlDoc.selectSingleNode("xml"); if (rootNode == null) { alert("传入的xmlDoc参数异常，ShowSelectWindow函数调用出错"); return; }
    var strMessage = "<table style='width:" + defaultwidth + "px;'><tr><td>"
        + "<input type=checkbox onclick=CheckAllClick(this," + rootNode.childNodes.length + ") id=c'hkitem_multi_all' style='vertical-align: middle;' />"
        + " <label for=chkitem_multi_all title='全选/全清'>" + strInfo + "</label>"
        + "</td></tr>";
    strMessage += "<tr><td><div style='width:100%;height:150px;OVERFLOW-Y: auto; OVERFLOW-X:hidden;border-style: inset; border-width: 2px;'><table width=100%>";
    for (var i = 0; i < rootNode.childNodes.length; i++) {
        var itemNode = rootNode.childNodes[i];
        var checked = ("," + defaultval + ",").indexOf("," + GetAttributeValue(itemNode, "data") + ",") > -1;
        strMessage += "<tr class=gd_item>";
        strMessage += "<td width=20><input id=chkitem_multi_" + i + " type=checkbox " + (checked ? "checked=true" : "") + " onclick=CheckRowClick(" + i + ")" + "></td>";
        strMessage += "<td onclick=CheckRowClick(" + i + ") title='" + GetAttributeValue(itemNode, "value") + "'>" + GetAttributeValue(itemNode, "data") + "</td></tr>";
    }
    strMessage += "</table></div>" + "</td></tr></table>";

    ShowConfirm(strMessage, "", function (retCode, retValue) {
        if ((retCode == 0) && (onSelectCallback != null)) {
            var txt = "";
            var val = "";
            for (var i = 0; i < rootNode.childNodes.length; i++) {
                var itemNode = rootNode.childNodes[i];
                var chkbox = $("chkitem_multi_" + i);
                if (chkbox.checked) {
                    if (txt != "") { txt += ","; val += ","; }
                    txt += GetAttributeValue(itemNode, "value");
                    val += GetAttributeValue(itemNode, "data");
                }
            }
            onSelectCallback(txt, val);
        }
    }, "选择框", 0);
    defaultwidth = (defaultwidth != null) ? parseInt(defaultwidth) : 0;
    if (defaultwidth < 200) defaultwidth = 200;
    if (defaultwidth > 400) defaultwidth = 400;
}


function ShowSingleSelectWindow(strInfo, xmlDoc, defaultval, onSelectCallback, defaultwidth) {
    var rootNode = xmlDoc.selectSingleNode("xml"); if (rootNode == null) { alert("传入的xmlDoc参数异常，ShowSelectWindow函数调用出错"); return; }
    var strMessage = "<table class =gd style='width:100%;'>";
    strMessage += "<tr><td><div style='width:100%;height:150px;OVERFLOW-Y: auto; OVERFLOW-X:hidden;border-style: inset; border-width: 1px;'><table width=100%>";
    for (var i = 0; i < rootNode.childNodes.length; i++) {
        var itemNode = rootNode.childNodes[i];
        var checked = ("," + defaultval + ",").indexOf("," + GetAttributeValue(itemNode, "data") + ",") > -1;
        strMessage += "<tr class=gd_item>";
        strMessage += "<td width=20><input name='rdo_item' id=radio_" + i + " type=radio " + (checked ? "checked=true" : "") + " onclick=CheckRowClick(" + i + ")" + "></td>";
        strMessage += "<td onclick=CheckRowClick(" + i + ") title='" + GetAttributeValue(itemNode, "value") + "'>" + GetAttributeValue(itemNode, "data") + "</td></tr>";
    }
    strMessage += "</table></div>" + "</td></tr></table>";

    ShowConfirm(strMessage, "", function (retCode, retValue) {
        if ((retCode == 0) && (onSelectCallback != null)) {
            var txt = "";
            var val = "";
            for (var i = 0; i < rootNode.childNodes.length; i++) {
                var itemNode = rootNode.childNodes[i];
                var chkbox = $("radio_" + i);
                if (chkbox.checked) {
                    if (txt != "") { txt += ","; val += ","; }
                    txt += GetAttributeValue(itemNode, "value");
                    val += GetAttributeValue(itemNode, "data");
                }
            }
            onSelectCallback(txt, val);
        }
    }, strInfo, 1);
    defaultwidth = (defaultwidth != null) ? parseInt(defaultwidth) : 0;
    if (defaultwidth < 200) defaultwidth = 200;
    if (defaultwidth > 400) defaultwidth = 400;
}

function CheckRowClick(idx) {
    var chkbox = $("chkitem_multi_" + idx);
    //chkbox.checked = !chkbox.checked;
    $j("input[type=button][value='确定']").attr("disabled", $j("input[type=radio]:checked ,input[type=checkbox]:checked").size() == 0);
}
function CheckAllClick(sender, cnt) {
    for (var i = 0; i < cnt; i++) {
        var chkbox = $("chkitem_multi_" + i);
        chkbox.checked = sender.checked;
    }
    $j("input[type=button][value='确定']").attr("disabled", $j("input[type=radio]:checked ,input[type=checkbox]:checked").size() == 0);
}

function __OnMessageButtonClick(retCode, retValue) {
    __UnMaskBG();
    var onPageCallback = $("floater_alert").pageCallback;
    if (onPageCallback != null)
        onPageCallback(retCode, retValue);
}

var __IsPopupPage = false;
var __Extend = null;
function ShowPage(url, strTitle, onPageCallback, mWidth, mHeight, initParam) {
    var str = "<img src='" + mImgMsgFilePath + "/loading.gif' id='img_frame_loading' style='position: absolute;cursor:wait;display:' />";
    str += "<iframe id='iframe_" + (Math.random() * 10000000) + "' src=" + url + (((url.indexOf('?') > -1) ? "&" : "?") + "rdn=" + (Math.random() * 100000)) + " onload=__ResetFrameSize(this," + mWidth + "," + mHeight + ") frameborder=\"0\"></iframe>";
    __IsPopupPage = true;
    __ShowWindow(str, strTitle, onPageCallback, initParam);
}

function __ResetFrameSize(mFrame, mWidth, mHeight) {
    if (!mWidth) mWidth = 700;
    if (!mHeight) mHeight = 400;
    mFrame.style.width = mWidth;
    mFrame.style.height = mHeight;
    mFrame.width = mWidth;
    mFrame.height = mHeight;

    img_loading = $("img_frame_loading");
    if (img_loading != null)
        img_loading.style.display = "none";

    div_alertX = $("floater_alert");
    __MoveCenter(div_alertX);

    div_alertX = $("floater_alert");
    div_alertX.frame = mFrame;
}

var __mWindowShowed = false; //是否已显示弹出窗口
document.onkeyup = function () {
    if (__mWindowShowed && (!__IsPopupPage) && event.keyCode == 27)
        //__UnMaskBG();
        __OnMessageButtonClick(-1, __Extend);
}

//显示弹出窗口（同时只能弹出一个窗口）
function __ShowWindow(strHtml, strTitle, onPageCallback, initParam) {
    __MaskBG();
    __mWindowShowed = true;
    div_alertX = $("floater_alert");
    div_alertX.initParam = initParam;
    div_alertX.pageCallback = onPageCallback;

    $("td_alerttitle").innerText = (strTitle != null) ? strTitle : ((onPageCallback != null) ? "确认框" : "提示框");
    $("td_alertcontent").innerHTML = strHtml;

    //    div_alertX.style.visibility = "visible";
    div_alertX.style.display = "";
    __MoveCenter(div_alertX);
}

function __MoveCenter(objDiv) {
    var mLeft = (document.documentElement.clientWidth - objDiv.offsetWidth) / 2 + document.documentElement.scrollLeft;
    objDiv.style.left = (mLeft > 0 ? mLeft : 0) + "px";
    var mTop = (document.documentElement.clientHeight - objDiv.offsetHeight) / 2 + document.documentElement.scrollTop;
    objDiv.style.top = (mTop > 0 ? mTop : 0) + "px";
}

//显示蒙板
function __MaskBG() {
    var div_Mask = $("BackGroundMask");
    if (!div_Mask) { return; }
    //    div_Mask.style.top = document.body.scrollTop;
    div_Mask.style.backgroundColor = ie11 ? "rgba(127, 127, 127,.50)" : "#808080";

    div_Mask.style.width = document.body.scrollWidth + "px";
    div_Mask.style.height = document.body.scrollHeight + "px";
    div_Mask.style.display = "";
}

//关闭蒙板（以及对话框）
function __UnMaskBG() {
    var div_Mask = $("BackGroundMask");
    if (div_Mask) {
        div_Mask.style.display = "none";
        div_Mask.visibility = "hidden";
    }

    //    $("td_alertcontent").innerHTML = ""; //这句话清除了网页元素，可能会引起后续调用出错
    //    $("floater_alert").style.visibility = "hidden";
    $("floater_alert").style.display = "none";
    __mWindowShowed = false;
    __IsPopupPage = false;
    window.focus();
}
