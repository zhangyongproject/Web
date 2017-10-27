
////////////////////////////////////////////////////////////////////////////////////////// Combo - ComboBox
// 说明:    应用ComboBox控件,首页应该在页面中申明一个具有id标识的Select,
//          且ComboApplied不等于"true". 在window.onload时,调用方法:SetCombo()
//          Combo的生成将以异步方式实现. 如果无需异步方式,可调用方法:SetComboEx()
function SetCombo() {
    window.setTimeout("SetComboEx();", 0);
}
function SetComboEx() {
    var combos = GetElemsByClassName("combo", document.documentElement);
    var nLen = combos.length;
    for (var i = 0; i < nLen; i++) {
        var e = combos[i];
        if (!e.id) e.id = "SetCombo_" + GetRd();
        if (e.getAttribute("ComboApplied") == "true") continue;
        e.setAttribute("ComboApplied", "true");
        // Objects
        var box = document.createElement("span");
        var ipt = document.createElement("input");
        var sbox = document.createElement("span");
        ipt.id = "combo_ipt__" + e.id;
        box.className = "combo_box";
        ipt.className = "combo_ipt";
        sbox.className = "combo_sbox";
        // Style
        var styleInfo = GetStyleInfo(e);
        ipt.style.width = styleInfo.width - styleInfo.height - 3 + "px";
        sbox.style.marginLeft = "-" + (styleInfo.width - styleInfo.height + 3) + "px";
        sbox.style.clip = "rect(" + 0 + "px " + styleInfo.width + "px " + (styleInfo.height + 1) + "px " + (styleInfo.width - styleInfo.height) + "px)";
        // Layout
        e.insertAdjacentElement("beforeBegin", box);
        box.insertAdjacentElement("afterBegin", sbox);
        sbox.insertAdjacentElement("afterBegin", e);
        box.insertAdjacentElement("afterBegin", ipt);
        // Events & Value
        var eEvent = e.onpropertychange; e.onpropertychange = null;
        if (e.options.length > 0)
            ipt.value = e.options[e.selectedIndex].text;
        //e.attachEvent("onpropertychange", eEvent);
        e.attachEvent("onpropertychange", PropChangeOnCombo);
        ipt.attachEvent("onchange", PropChangeOnIpt);
    }
}
function PropChangeOnCombo(o) {
    if (o.propertyName != "value") return;
    if (o.srcElement.selectedIndex > -1)
        SetElemValue("combo_ipt__" + o.srcElement.id, o.srcElement.options[o.srcElement.selectedIndex].text);
}
function PropChangeOnIpt(o) {
    var val = o.srcElement.value;
    var e = o.srcElement.id.toArray("__")[1];
    SetSelectItem(e, val, val, -1, true);
    GetElem(e).fireEvent("onchange");
}
function SetComboValue(elemId, val) {
    SetSelectItem(elemId, val, val, -1, true);
}
function GetComboValue(elemId) {
    return GetElemValue(elemId);
}
////////////////////////////////////////////////////////////////////////////////////////// 布局美化 Edge3 & Sub3

// Edge边角HTML化{0}=ID,{1}=样式表,{2}=Style串,{3}=onmouseover事件,{4}=onmouseout事件,{5}=onclick事件,{6}=内部文字或HTML
var m_e03_html = '<table cellspacing="0" id="{0}" class="{1}" style="{2}" onmouseover="{3}" onmouseout="{4}" onclick="{5}">' +
    '<tr><td class="e03l"><div class="none" /></td><td class="e03m"><div id="{0}__cm">{6}</div></td>' +
    '<td class="e03r"><div class="none" /></td></tr></table>';

// 3列条HTML{1}=ID,{0}=样式表,{2}=主操作区,{3}=扩展区,{4}=副操作区)
var m_s03_html = '<table cellspacing="0" id="{0}" class="{1}"><tr><td class="s03_c1">{2}</td>' +
    '<td class="s03_c2">{3}</td><td class="s03_c3">{4}</td></tr></table>';

function GetE03(id, classname, style, onmouseover, onmouseout, onclick, innerhtml) {
    id = (!IsStr(id)) ? GetRd() : id;
    classname = (!IsStr(classname)) ? "e03" : classname;
    style = (!style || typeof (style) != "string") ? "" : style;
    onmouseover = (!IsStr(onmouseover)) ? "javascript:void(0);" : onmouseover;
    onmouseout = (!IsStr(onmouseout)) ? "javascript:void(0);" : onmouseout;
    onclick = (!IsStr(onclick)) ? "javascript:void(0);" : onclick;
    innerhtml = (!IsStr(innerhtml)) ? "" : innerhtml;
    return m_e03_html.format(id, classname, style, onmouseover, onmouseout, onclick, innerhtml);
}
function GetS03(id, classname, innerhtml, html2, html3) {
    id = (!IsStr(id)) ? GetRd() : id;
    classname = (!IsStr(classname)) ? "s03" : classname;
    innerhtml = (!IsStr(innerhtml)) ? "" : innerhtml;
    html2 = (!IsStr(html2)) ? "" : html2;
    html3 = (!IsStr(html3)) ? "" : html3;
    return m_s03_html.format(id, classname, innerhtml, html2, html3);
}

function Wrt(html) {
    document.write(html);
}

////////////////////////////////////////////////////////////////////////////////////////// Labbar

// Labbar定义
var labbar_classFlag = "lbr";

function LabbarObj(LabbarObjName, contrElemId) {
    // Labbar(标签栏)对象
    if (!LabbarObjName) alert("程序错误: 请传入你为该Labbar对象定义的名称.");
    this.id = LabbarObjName;
    this.Labs = [];
    this.CurrentLabId = null;
    this.ContrElemId = contrElemId;
    this.IsLoaded = false;
    this.ShowDispBtn = true;

    this.DivClickEvent = null;

    this.AddLab = function (labId, blockId, labText, isHidden, strBeforeClickEvent, strAfterClickEvent) {
        if (this.IsLoaded) { alert("程序错误: 己装载的Labbar不能再增加Lab."); return false; }
        if (!labId) { alert("程序错误: 请指定Lab的标识."); return false; }
        if (!strBeforeClickEvent) strBeforeClickEvent = "";
        if (!strAfterClickEvent) strAfterClickEvent = "";
        this.Labs.push({ labId: labId, blockId: blockId, labText: labText, isHidden: isHidden, strBeforeClickEvent: strBeforeClickEvent, strAfterClickEvent: strAfterClickEvent });
        return true;
    };

    this.GetLab = function (labId) {
        if (!labId) labId = this.CurrentLabId;
        var fstEnLab = null;
        for (var i = 0; i < this.Labs.length; i++) {
            if (this.Labs[i].labId == labId) { return this.Labs[i]; }
            else if (!this.Labs[i].isHidden && !fstEnLab) { fstEnLab = this.Labs[i]; }
        }
        if (this.Labs.length > 0) return fstEnLab;
        return null;
    };
    this.SetLab = function (labId, blockId, labText, isHidden) {
        var labObj = this.GetLab(labId);
        if (!labObj) return false;
        if (blockId) labObj.blockId = blockId;
        if (labText) labObj.labText = labText;
        if (isHidden == true || isHidden == false) labObj.isHidden = isHidden;
        GetElem(blockId).style.display = isHidden == false ? "block" : "none";
        if (this.IsLoaded) {
            var labElem = GetElem(labObj.labId);
            var labElemMain = GetElem(labObj.labId + "__cm")
            if (!labElem || !labElemMain) return false;
            if (isHidden == true) labElem.style.display = "none"; else if (isHidden == false) labElem.style.display = "block";
            if (labText) labElemMain.innerHTML = labText;
        }
    };
    this.Load = function (isReturnHtml) {
        var labObj = this.GetLab(); if (!labObj) return;
        this.CurrentLabId = labObj.labId;
        var strOnClick = "javascript:{0}.Open('{1}');";
        var strOnClickTemp = "";
        var strOnMOver = "javascript:void(0);";
        var strOnMOut = "javascript:void(0);";
        var classFlagTemp = "";
        var styleCode = "display:{0};";
        var styleCodeTemp = "";
        var labHtml = "";
        var dispHtml = (this.ShowDispBtn) ? '<div id="' + this.id + '__disp" class="' + labbar_classFlag + '_none" title="隐藏区块内容" onclick="javascript:' + this.id + '.Disp();"></div>' : "";
        var html = "";
        for (var i = 0; i < this.Labs.length; i++) {
            strOnClickTemp = strOnClick.format(this.id, this.Labs[i].labId);
            classFlag = labbar_classFlag + ((this.Labs[i].labId == this.CurrentLabId) ? "_on" : "_no");
            styleCodeTemp = (this.Labs[i].isHidden) ? styleCode.format("none") : styleCode.format("block");
            html += GetE03(this.Labs[i].labId, classFlag, styleCodeTemp, strOnMOver, strOnMOut, strOnClickTemp, this.Labs[i].labText);
        }
        html = GetS03('', 'lbr_s', html, '', dispHtml);
        //lab增加id by qiubaosheng
        html = GetE03(LabbarObjName + "_" + this.CurrentLabId, labbar_classFlag, "", "javascript:void(0);", "javascript:void(0);", "javascript:void(0);", html);

        if (this.ContrElemId && !isReturnHtml) { var e = GetElem(this.ContrElemId); if (e) e.innerHTML = html; this.IsLoaded = true; }
        else if (!isReturnHtml) { Wrt(html); this.IsLoaded = true; }
        else { return html; }
    };
    this.Open = function (labId) {
        var labObj = this.GetLab(labId);
        if (!labObj) return false;
        if (labObj.strBeforeClickEvent) try { eval(labObj.strBeforeClickEvent); } catch (e) { }
        if (labObj.blockId) {
            var oldLabObj = this.GetLab(this.CurrentLabId);
            if (this.CurrentLabId == labObj.labId) {//edit by qbs 20121203 区域块为当前块时不需要隐藏再显示
                SetDisplay(labObj.blockId, "block"); SetClass(labObj.labId, labbar_classFlag + "_on");
                return true;
            }
            if (oldLabObj) { if (oldLabObj.blockId) SetDisplay(oldLabObj.blockId, "none"); SetClass(oldLabObj.labId, labbar_classFlag + "_no"); }
            SetDisplay(labObj.blockId, "block"); SetClass(labObj.labId, labbar_classFlag + "_on");
            this.CurrentLabId = labObj.labId;
        }
        if (labObj.strAfterClickEvent) try { eval(labObj.strAfterClickEvent); } catch (e) { }
        if (this.DivClickEvent != null) { this.DivClickEvent(labId); }
        return true;
    };
    this.Close = function (labId) {
        var labObj = this.GetLab(labId);
        if (!labObj) return false;
        if (labObj.blockId) {
            SetDisplay(labObj.blockId, "none");
        }
        return true;
    };
    this.Disp = function () {
        var labObj = this.GetLab(this.CurrentLabId);
        if (!labObj) return false;
        if (labObj.blockId) SetDisplay(labObj.blockId);
        if (GetElem(labObj.blockId).style.display == "block") {
            SetClass(this.id + "__disp", labbar_classFlag + "_none");
        }
        else {
            SetClass(this.id + "__disp", labbar_classFlag + "_block");
        }
    };
}

// --------------------------------------------------------------------
// about elem's style (include valid)
// --------------------------------------------------------------------

function GetStyleInfo(elem) {
    var info = { screenWidth: 0, screenHeight: 0, scrollWidth: 0, scrollHeight: 0, scrollLeft: 0, scrollTop: 0, width: 0, height: 0, left: 0, top: 0, right: 0, bottom: 0 };
    elem = GetElem(elem);
    if (!IsElem(elem)) return info;
    var rect = elem.getBoundingClientRect();
    var de = elem.document.body;
    var de1 = elem.document.documentElement;

    info.screenWidth = (de1.clientWidth > 0) ? de1.clientWidth : de.clientWidth;
    info.screenHeight = (de1.clientHeight > 0) ? de1.clientHeight : de.clientHeight;
    info.scrollWidth = de1.scrollWidth;
    info.scrollHeight = de1.scrollHeight;
    info.scrollLeft = de1.scrollLeft;
    info.scrollTop = de1.scrollTop;

    if (rect.left > 0) info.left = rect.left;
    if (rect.right > 0) info.right = rect.right;
    if (rect.top > 0) info.top = rect.top;
    if (rect.bottom > 0) info.bottom = rect.bottom;
    if (rect.right > rect.left && rect.left >= 0) info.width = rect.right - rect.left;
    else if (GetInt(elem.clientWidth, 0) > 0) info.width = GetInt(elem.clientWidth, 0);
    else if (GetInt(elem.style.width, 0) > 0) info.width = GetInt(elem.style.width, 0);
    if (rect.bottom >= rect.top && rect.top >= 0) info.height = rect.bottom - rect.top;
    else if (GetInt(elem.clientHeight, 0) > 0) info.width = GetInt(elem.clientHeight, 0);
    else if (GetInt(elem.style.height, 0) > 0) info.height = GetInt(elem.style.height, 0);
    return info;
}
function SetDisplay(elem, value) {
    elem = GetElem(elem); if (!IsElem(elem)) return;
    value = GetStr(value, null);
    if (value == null) {
        elem.style.display = ((elem.style.display == 'none') ? 'block' : 'none'); return;
    }
    if (value == 'block' || value == 'true' || value == '1') value = 'block'; else value = 'none';
    elem.style.display = value;
    return;
}
function SetClass(elem, className) {
    elem = GetElem(elem); if (!IsElem(elem)) return;
    className = GetStr(className, '');
    elem.className = className;
    return;
}
function AppClass(elem, className) {
    elem = GetElem(elem); if (!IsElem(elem)) return;
    className = GetStr(className, '');
    elem.className = elem.className + " " + className;
}
function RemoveClass(elem, className) {
    elem = GetElem(elem); if (!IsElem(elem)) return;
    className = GetStr(className, '');
    elem.className = elem.className.replaceAll(className, "");
}


/////////////////////////////////////////////////////////常用的HTML对象方法
// --------------------------------------------------------------------
// get type value (include valid, return null or type value)
// --------------------------------------------------------------------

function GetStr(src, defVal) {
    // 当src为null/undefined时返回defVal
    if (IsStr(src)) return src;
    if (typeof (src) == "undefined" || src == null) return defVal;
    return src.toString();
}
function GetInt(src, defVal) {
    // 当src为null/undefined或parseInt后为NaN时,返回defVal
    // 可选参数,传入radix时,可指定parseInt时的进制数,默认为10
    src = parseInt(src, 10);
    if (isNaN(src)) return defVal;
    return src;
}
function GetNum(src, defVal) {
    // 与getInt类似,但支持Float类型
    src = parseFloat(src);
    if (isNaN(src)) return defVal;
    return src;
}
function GetElem(elemId) {
    // 当参数elemId实际是一个elem时直接返回,当无法获取elem时返回null
    if (IsElem(elemId)) return elemId;
    return document.getElementById(GetStr(elemId, null));
}
function GetElemUP(byelem, attrName, lookVal) {
    // 依指定属性名,获取父元素
    byelem = GetElem(byelem); attrName = GetStr(attrName, null);
    if (!byelem || !attrName) return null;
    while (byelem.getAttribute(attrName) != lookVal) {
        byelem = byelem.parentElement;
        if (byelem.tagName == "BODY") break;
    }
    return byelem;
}
function GetElems(elemName) {
    // 等同于document.getElementsByName,如果elemName本身己是elems集合,则直接返回
    if (elemName.length) if (elemName[0]) if (IsElem(elemName[0])) return elemName;
    return document.getElementsByName(GetStr(elemName, ''));
}
function GetElemsByClassName(className, rootNode) {
    // 依据className与父级elem获取对象集合,rootNode无效时默认为document
    // 当className无效时返回null,否则返回0长度集合或对象集合
    var ret = []; if (!IsStr(className)) return ret; var elems = null;
    if (!rootNode) { elems = document.all; } else { elems = GetElem(rootNode); if (elems) elems = elems.all; }
    var nLen = elems.length; var pattern = new RegExp('(?:^|\\s+)' + className + '(?:\\s+|$)');
    for (var i = 0; i < nLen; i++) { if (pattern.test(elems[i].className)) { ret.push(elems[i]); } }
    return ret;
}

function CheckDecimalLen(src, len) {
    var re = new RegExp("^([1-9]\\d*|0)$|^([1-9]\\d*\\.\\d{1," + len + "})$");
    return re.test(src);
}
// --------------------------------------------------------------------
// other
// --------------------------------------------------------------------
//function GetTimeZone(startDatetime, endDatetime) {
//    startDatetime = (IsDate(startDatetime)) ? startDatetime : startDatetime.toDate();
//    endDatetime = (IsDate(endDatetime)) ? endDatetime : endDatetime.toDate();
//    var timeZone = endDatetime - startDatetime;
//    var h = GetInt(timeZone / 1000 / 60 / 60);
//    var m = GetInt((timeZone - (h * 1000 * 60 * 60)) / 1000 / 60);
//    return (h + "") + "时" + (m + "").padLeft(2, "0") + "分";
//}
function FormatNum(num, len) {
    num = (num.indexOf(".") >= 0) ? num + "" : num + ".";
    num1 = num.substr(num.indexOf(".") + 1);
    num1 = num1.padRight(len, "0").substr(0, len);
    return num.split(".")[0] + "." + num1;
}

function GetRd(n) {
    var ret = "";
    n = GetInt(n, 1); n = (n < 1) ? 1 : n;
    for (var i = 0; i < n; i++) ret += Math.random().toString().split(".")[1];
    return ret;
}

// --------------------------------------------------------------------
// about elem's value (include valid, return null or type value)
// --------------------------------------------------------------------

function GetElemValue(elem) {
    // 取input's value,select[single mode]'s selectedValue,其它元素的innerHTML,未获取对象则返回null
    elem = GetElem(elem); var retVal = null; if (!elem) return retVal;
    if (elem.tagName == 'INPUT') {
        if (elem.type == 'radio' || elem.type == 'checkbox') {
            if (elem.checked == true) retVal = elem.value;
        }
        else { retVal = elem.value; }
    }
    else if (elem.tagName == 'SELECT') retVal = elem.value;
    else retVal = elem.innerHTML;
    return retVal;
}
function SetElemValue(elem, value) {
    // 取input's value,select's selectedValue,其它元素的innerHTML
    // input[checkbox/radio]的值与参数value匹配时,使其checked
    elem = GetElem(elem); if (!elem) return;
    if (elem.tagName == 'INPUT') {
        if (elem.type == 'radio' || elem.type == 'checkbox') {
            if (elem.value == value) elem.checked = true;
        }
        else { elem.value = value; }
    }
    else if (elem.tagName == 'SELECT') {
        try {
            elem.value = value;
            if (elem.selectedIndex == -1 && elem.options.length > 0) elem.selectedIndex = 0;
        } catch (e) { }
    }
    else { elem.innerHTML = value; }
    return;
}
function GetChecksValue(elems) {
    // 传入checkbox's name或checkbox集合,获取选定的值
    // 返回值以数组形式存储
    elems = GetElems(elems); var retVal = [];
    for (var i = 0; i < elems.length; i++)
        if (elems[i].checked == true)
            retVal.push(elems[i].value);
    return retVal;
}
function SetChecksValue(elems, arrValues, bAddOnly) {
    // 设置checkboxs's的选定状态
    // bAddOnly[default:false]表示仅增加选定,不操作己选定项
    if (!IsArr(arrValues)) return; elems = GetElems(elems); if (!elems) return;
    for (var i = 0; i < elems.length; i++) {
        if (arrValues.indexOf(elems[i].value) >= 0) elems[i].checked = true;
        else if (bAddOnly != true) elems[i].checked = false;
    }
    return;
}
function GetSelectValues(elem) {
    // 获取selected option的己选定值, 适合于多选select
    var retVal = [];
    elem = GetElem(elem);
    if (elem.tagName != 'SELECT') return;
    for (var i = 0; i < elem.options.length; i++)
        if (elem.options[i].selected == true)
            retVal.push(elem.options[i].value);
    return retVal;
}
function SetSelectValues(elem, arrValues, bAddOnly) {
    // 设置select的值,更适合于多选select
    if (!IsArr(arrValues)) return;
    elem = GetElem(elem);
    for (var i = 0; i < elem.options.length; i++) {
        if (arrValues.indexOf(elem.options[i].value) >= 0) elem.options[i].selected = true;
        else if (bAddOnly != true) elem.options[i].selected = false;
    }
    return;
}
function GetRadiosValue(elems) {
    // 传入radio's name或radio集合,获取选定的值
    // 返回值为字符串,当无选定值时返回null
    elems = GetElems(elems);
    for (var i = 0; i < elems.length; i++)
        if (elems[i].checked == true)
            return elems[i].value;
    return null;
}
function SetRadiosValue(elems, value) {
    // 设置radios's的选定状态
    elems = GetElems(elems);
    for (var i = 0; i < elems.length; i++) {
        if (value == elems[i].value) { elems[i].checked = true; return; }
    }
    return;
}
function SetSelectItem(elem, value, text, iPos, isSelected) {
    // 选择(或增加并选择)一个Select's Item, iPos为无效值时表示追加
    elem = GetElem(elem); if (!elem) return; if (elem.tagName != 'SELECT') return;
    value = GetStr(value, ''); text = GetStr(text, '');
    iPos = GetInt(iPos, elem.options.length);
    var hasThisOpt = false;
    for (var i = elem.options.length - 1; i > -1; i--) {
        if (elem.options[i].value == value || elem.options[i].text == text) {
            hasThisOpt = true; if (isSelected) elem.options[i].selected = true; break;
        }
    }
    if (!hasThisOpt) {
        var opt = document.createElement('option');
        opt.value = value; opt.text = text;
        elem.options.add(opt, iPos); if (isSelected) SetSelectValues(elem, [value], true);
    }
    return;
}
function AddSelectItem(elem, value, text, iPos, isSelected) {
    // 选择(或增加并选择)一个Select's Item, iPos为无效值时表示追加
    elem = GetElem(elem); if (!elem) return; if (elem.tagName != 'SELECT') return;
    value = GetStr(value, ''); text = GetStr(text, '');

    var txtHtml = ">" + text + "<";
    if (elem.outerHTML.indexOf(txtHtml) != -1)
        return;
    iPos = GetInt(iPos, elem.options.length);
    var opt = document.createElement('option');
    opt.value = value; opt.text = text;
    elem.options.add(opt, iPos); if (isSelected == true) SetSelectValues(elem, [value], true);
    return;
}
// 判断select选项中是否存在Value="paraValue"的Item
function SelectIsExitItem(objSelect, objItemValue) {
    var isExit = false;
    objSelect = GetElem(objSelect); if (!objSelect) return isExit;
    for (var i = 0; i < objSelect.options.length; i++) {
        if (objSelect.options[i].value == objItemValue) {
            isExit = true;
            break;
        }
    }
    return isExit;
}
function SelectChecks(elems, type) {
    // 设置elems(Checkboxs)的值为checked=真(1)/假(0)/反选(2)/依第一项反选(3)
    elems = GetElems(elems); if (!elems) return;
    var checkType;
    if (type == '1') checkType = true;
    else if (type == '0') checkType = false;
    else if (type == '2') checkType = 2;
    else checkType = (elems[0].checked) ? false : true;

    if (type == 2) {
        for (var i = 0; i < elems.length; i++)
            elems[i].checked = !elems[i].checked;
    }
    else {
        for (var i = 0; i < elems.length; i++)
            elems[i].checked = checkType;
    }
    return;
}

function SelectChecks2(elems, type) {
    // 设置elems(Checkboxs)的值为checked=真(1)/假(0)/反选(2)/依第一项反选(3)
    elems = GetElems(elems); if (!elems) return;
    var checkType;
    if (type == '1') checkType = true;
    else if (type == '0') checkType = false;
    else if (type == '2') checkType = 2;
    else checkType = (elems[0].checked) ? false : true;

    if (type == 2) {
        for (var i = 0; i < elems.length; i++) {
            if (elems[i].disabled == true) { continue; }
            elems[i].checked = !elems[i].checked;
        }
    }
    else {
        for (var i = 0; i < elems.length; i++) {
            if (elems[i].disabled == true) { continue; }
            elems[i].checked = checkType;
        }
    }
    return;
}

function SelectChecksByCkb(elems, elem) {
    // 依据一个checkbox值设置一组checkbox值
    elems = GetElems(elems);
    elem = GetElem(elem);
    if (!elems || !elem) return;
    if (elem.tagName != 'INPUT' || elem.type != 'checkbox') return;
    if (elem.checked) SelectChecks(elems, 1);
    else SelectChecks(elems, 0);
    return;
}
function SetInput(elem, inputWidth, maxLength) {
    elem = GetElem(elem); if (!elem) return; if (elem.getAttribute("SetInput") == "true") return;
    var strMaxLength = "";
    if (arguments.length == 3) { strMaxLength = " maxLength=" + maxLength; }
    var val = GetInnerText(elem); elem.setAttribute("SetInput", "true"); elem.innerHTML = "";
    inputWidth = (IsStr(inputWidth) && inputWidth.length > 0) ? inputWidth : elem.clientWidth - 10 + "px";
    elem.innerHTML = '<input type="text" ' + strMaxLength + ' style="width:' + inputWidth + ';" name="SetInput_name" id="SetInput__' + elem.id + '" value=""/>' +
        '<input type="hidden" name="SetInputHid_name" id="SetInputHid__' + elem.id + '"/>';
    SetElemValue("SetInput__" + elem.id, val);
    SetElemValue("SetInputHid__" + elem.id, val);
}
function GetElemValue_SetInput(elem) {
    elem = GetElem(elem); if (!elem) return;
    return GetElemValue("SetInput__" + elem.id)
}
function SetInput_Reset(isResetToOld) {
    var elems = (isResetToOld) ? GetElems("SetInput_name") : GetElems("SetInputHid_name"); var len = elems.length;
    for (var i = len - 1; i > -1; i--) {
        elems[i].parentElement.removeAttribute("SetInput");
        elems[i].parentElement.innerText = GetElemValue(elems[i]);
    }
}
function SetSelect(elem, options, width) {
    elem = GetElem(elem); if (!elem) return; if (!options) return;
    if (elem.getAttribute("SetSelect") == "true") return;
    var val = GetElem(elem).innerText; elem.setAttribute("SetSelect", "true"); elem.innerHTML = "";
    if (!IsArr(options) && !IsArr(options[0])) return; var html = "";
    for (i = 0; i < options.length; i++) {
        strs = options[i]; html = html + "<option value='" + strs[0] + "'>" + strs[1] + "</option>";
    }
    elem.innerHTML = "<select style='width:" + width + ";' name='SetSelect_name' id='SetSelect__" + elem.id + "' value=''>" + html + "</select>" +
    "<input type='hidden' name='SetSelectHid_name' id='SetSelectHid__" + elem.id + "'/>";
    for (i = 0; i < GetElem("SetSelect__" + elem.id).length; i++) {
        if (GetElem("SetSelect__" + elem.id)[i].text == val) {
            GetElem("SetSelect__" + elem.id)[i].selected = true;
            break;
        }
    }
    SetElemValue("SetSelectHid__" + elem.id, val);
}
function GetElemValue_SetSelect(elem) {
    elem = GetElem(elem); if (!elem) return;
    return GetElemValue("SetSelect__" + elem.id)
}
function SetSelect_Reset(isResetToOld) {
    var elems = (isResetToOld) ? GetElems("SetSelect_name") : GetElems("SetSelectHid_name"); var len = elems.length;
    for (var i = len - 1; i > -1; i--) {
        elems[i].parentElement.removeAttribute("SetSelect");
        if (isResetToOld) {
            for (j = 0; j < elems[i].length; j++) {
                if (elems[i][j].selected) {
                    elems[i].parentElement.innerHTML = elems[i][j].text; break;
                }
            }
        }
        else
            elems[i].parentElement.innerHTML = GetElemValue(elems[i]);
    }
}
function GetInnerText(elem) {
    // 获取元素的innerText
    elem = GetElem(elem); if (!IsElem(elem)) return '';
    if (elem.text) return elem.text;
    else if (elem.innerText) return elem.innerText;
    else if (elem.textContent) return elem.textContent;
    else return '';
}

function GetValueByText(elementName, text) {
    if (text == "") return "";
    var elements = GetElem(elementName);
    for (var i = 0; i < elements.length; i++) {
        if (elements.options[i].text == text)
            return elements[i].value;
    }
    return "";
}



//--------------------------------------------------------------------------------------------------------------------------
//设置查询区域按钮按钮
function SetExtendIcon(imgId, divId) {
    var mImg = $(imgId);
    mImg.divId = divId;
    mImg.style.visibility = "visible";
    mImg.style.cursor = "hand";
    mImg.cookName = getUrlPage() + "_ex_" + imgId;
    $(mImg.divId).className = "div_querycondition";
    var bExtend = (GetCookie(mImg.cookName) != "0");
    mImg.bExtend = !bExtend;
    mImg.onclick = function () { OnExtendIconClick(this); }
    OnExtendIconClick(mImg);
}

function OnExtendIconClick(sender) {
    var mImg = sender;  //||event.srcElement
    mImg.bExtend = !mImg.bExtend;
    SetCookie(mImg.cookName, mImg.bExtend ? "1" : "0");

    mImg.title = mImg.bExtend ? "隐藏查询条件" : "显示查询条件";
    mImg.style.cursor = "pointer";
    mImg.src = mImg.bExtend ? "../images/controls/buttonbarquery2.png" : "../images/controls/buttonbarquery1.png";
    $(mImg.divId).style.display = mImg.bExtend ? 'block' : 'none';
}
//--------------------------------------------------------------------------------------------------------------------------

function SetElemEnable(elem, enable) {
    elem = GetElem(elem); if (!elem) { return; }
    if (elem.tagName == 'INPUT') {
        if (elem.type == 'text') {
            elem.readOnly = !enable;
            elem.className = enable ? "input_enable" : "input_disable";
        } else {
            elem.disabled = !enable;
        }
    } else {
        elem.disabled = !enable;
    }
}

function SetElemsEnable(elems, enable) {
    elems = GetElems(elems); if (!elems) return;
    for (var i = 0; i < elems.length; i++)
        elems[i].disabled = !enable;
}

function CheckNull(controlId, msg) {
    var value = $(controlId).value;
    value = value.replaceAll("[^A-Za-z0-9\u4e00-\u9fa5]", "");
    if (value == "") {
        ShowAlert(msg + "不能为空。");
        $(controlId).focus();
        return true;
    } else {
        $(controlId).value = value;
        return false;
    }
}

var regCode = /^[A-Za-z0-9]{3,10}$/;
var strCodeInfo = "\r\n\r\n代码必须由3-10数字或字母组成";
function CheckCode(strCode) {
    return regCode.test(strCode);
}