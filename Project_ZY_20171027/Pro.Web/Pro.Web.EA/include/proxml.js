//--------------------------------------------------------------------------------------
//
//(1)XML基本操作
//   
//--------------------------------------------------------------------------------------

//从字符串创建一个XML
function CreateXmlFromString(theStr) {
    var xmlDoc = new ActiveXObject("MSXML2.DOMDocument.3.0");
    xmlDoc.loadXML(theStr);
    return xmlDoc;
}

//创建一个XML
//参数：结点名（如果为空，则默认为XML）
function CreateXml(RootNodeText) {
    var xmlDoc = new ActiveXObject("MSXML2.DOMDocument.3.0");
    var XmlRoot = xmlDoc.createElement((RootNodeText != null) ? RootNodeText : "xml");
    xmlDoc.appendChild(XmlRoot);
    return xmlDoc;
}

//添加一个子结点
//参数：父结点、结点名、结点值
function AddXmlNode(pNode, NodeName, NodeValue) {
    if (pNode == null) return false;
    var xmlNode = pNode.ownerDocument.createElement(NodeName);
    if (NodeValue != null) xmlNode.text = NodeValue;
    pNode.appendChild(xmlNode);
    return xmlNode;
}

//添加一个属性
//参数：结点、属性名、属性值
function AddAttribute(pNode, AttribName, AttribValue) {
    if (pNode == null) return;
    pNode.setAttribute(AttribName, AttribValue);
}

//获取属性值
//参数：结点、属性名
function GetAttributeValue(pNode, AttribName) {
    if (pNode == null) return "";
    var result = pNode.getAttribute(AttribName);
    return result != null ? result : "";
}

//返回XML文件里某个结点的值。
//参数：1、XML文档；2、结点路径。
function GetNodeValue(xmlDoc, thePath) {
    var xmlNode = xmlDoc.selectSingleNode(thePath);
    return (xmlNode != null) ? xmlNode.text : "";
}

//设置XML文件里某个结点的值。
//参数：1、XML文档；2、结点路径；3、要设置结点的值。
function SetNodeValue(xmlDoc, thePath, theValue) {
    var xmlNode = xmlDoc.selectSingleNode(thePath);
    if (xmlNode != null) xmlNode.text = theValue;
}

//复制xml结点，包括属性以及子结点
//参数：源结点,目标结点
//说明：源结点和目标结点均不能为空
function CopyXmlNode(NodeSrc, NodeTgt) {
    //清除目标结点的所有属性
    while (NodeTgt.attributes.length > 0)
        NodeTgt.removeAttributeNode(NodeTgt.attributes[0]);

    //清除目标结点的所有子结点
    while (NodeTgt.childNodes.length > 0)
        NodeTgt.removeChild(NodeTgt.childNodes[0]);

    //递归复制结点属性及子结点
    CopyXmlNodeA(NodeSrc, NodeTgt);
}

function CopyXmlNodeA(NodeSrc, NodeTgt) {
    for (var i = 0; i < NodeSrc.attributes.length; i++) {
        var src = NodeSrc.attributes[i];
        AddAttribute(NodeTgt, src.nodeName, src.nodeValue);
    }
    for (var i = 0; i < NodeSrc.childNodes.length; i++) {
        var src = NodeSrc.childNodes[i];
        var tgt = AddXmlNode(NodeTgt, src.nodeName, src.nodeValue);
        CopyXmlNodeA(src, tgt); //递归
    }
}

//--------------------------------------------------------------------------------------
//
//(2)Javascript异步调用webservice //xiang 20121129
//   
//--------------------------------------------------------------------------------------

//防止参数中的特殊字符( & < > )，将该符号换成其他字符
function EncodeParamStr(value) {
    return value.toString().replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
function DecodeParamStr(value) {
    return value.toString().replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&amp;/g, "&");
}

function ParseResultXml(strxml) {
    var xmlDoc = CreateXmlFromString(strxml);
    var rootNode = xmlDoc.selectSingleNode("xml");
    var value = GetAttributeValue(rootNode, "value");
    return value;
}

var WebMethodProxy = function (WebMethodUrl, WebMethodName) {

    //函数名
    this._WebMethodName = WebMethodName;

    //URL
    if (WebMethodUrl == null || WebMethodUrl == "")
        WebMethodUrl = document.URL;
    this._GetWebMethodUrl = function (ajaxtype) {
        var _WebMethodUrl = WebMethodUrl + ((WebMethodUrl.toString().indexOf("?") > -1) ? "&" : "?") + "ajaxflag=" + ajaxtype;  //0/1 json/soap
        _WebMethodUrl += "&webmethodname=" + this._WebMethodName;
        return _WebMethodUrl;
    }

    //参数
    this._Param = new Object();
    this.AddParam = function (key, value) {
        if (typeof (key) != "undefined") this._Param[key] = typeof (value) == "undefined" ? null : EncodeParamStr(value);
    }

    //0：Json 方式调用
    this.Call = function (CallBackProc, CallErrorProc, Tag) {
        $j.post(this._GetWebMethodUrl(0), this._Param, function (retXml) {
            //alert(WebMethodName + "\r\n\r\n" + retXml);
            if(CallBackProc != null)
                CallBackProc(retXml.xml, Tag);
        });
    }

    /*
    //1：Soap方式调用
    this.HttpObj = function () {
        var http_request = null;
        if (window.XMLHttpRequest) {
            //说明：本系统ajax对象只能使用浏览器内置的XMLHttpRequest对象，而不能使用ActiveX创建的外置对象，因此只支持IE7及以上版本
            http_request = new XMLHttpRequest();
            if (http_request.overrideMimeType) { http_request.overrideMimeType('text/xml'); }
        }
        http_request._IsRunning = false;
        return http_request;
    }();

    this._BuildWebServiceCmd = function () {
        var strRequest = '<?xml version="1.0" encoding="utf-8"?>';
        strRequest += '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">';
        strRequest += '<soap:Body>';
        strRequest += '<' + this._WebMethodName + ' xmlns="http://tempuri.org/">';
        for (var k in this._Param) strRequest += '<' + EncodeParamStr(k) + '>' + EncodeParamStr(this._Param[k]) + '</' + EncodeParamStr(k) + '>';
        strRequest += '</' + this._WebMethodName + '>';
        strRequest += '</soap:Body>';
        strRequest += '</soap:Envelope>';
        return CreateXmlFromString(strRequest);
    }

    //调用函数，调用成功回调函数/调用失败回调函数/附加信息（异步回调时返回）
    this.Call = function(CallBackProc, CallErrorProc, Tag) {
        if (this.HttpObj == null) {
            if (CallErrorProc != null)
                CallErrorProc("Can not call webmethod.", Tag);
            return null;
        }
        if (this.HttpObj._IsRunning) {
            if (CallErrorProc != null)
                CallErrorProc("webmethod has called and is waiting for response.", Tag);
            //  alert(1);
            return null;
        }
        this.HttpObj._IsRunning = true;
        try {
            this.HttpObj._CallBackProc = CallBackProc;
            this.HttpObj._CallErrorProc = CallErrorProc;
            this.HttpObj._Tag = Tag;
            this.HttpObj._WebMethodName = this._WebMethodName;
            var xmlDoc = this._BuildWebServiceCmd();
            this.HttpObj.onreadystatechange = this._DoCallback;
            this.HttpObj.open('POST', this._GetWebMethodUrl(1), true);
            this.HttpObj.send(xmlDoc);
            return this.HttpObj;
        }
        catch (e) {
            if (this._CallErrorProc != null) {
                var strMsg = "call webmethod " + this._WebMethodName + " failure.\n\n" + e.description;
                this._CallErrorProc(strMsg, Tag);
            }
            return null;
        }
    }

    this._DoCallback = function() {
        if (this.readyState == 4) {
            this._IsRunning = false;
            if (this.status == 200) {
                var mResponseText = "";
                try {
                    mResponseText = this.responseText;
                    var xmlDoc = CreateXmlFromString(mResponseText); //IE11，this.responseXML类型发生了变化
                    var node = xmlDoc.selectSingleNode("soap:Envelope/soap:Body");
                    node = node.childNodes(0).childNodes(0);
                    if (this._CallBackProc != null) this._CallBackProc(node.text, this._Tag);
                    //(DecodeParamStr(node.text)); node.text不是node.innerxml，本身已对转义字符进行处理
                } catch (e) {
                    var strMsg1 = "webmethod (" + this._WebMethodName + ") response invalidate.\n\nresponseText=" + mResponseText;
                    if (this._CallErrorProc != null) this._CallErrorProc(strMsg1, this._Tag);
                }
            }
            else {
                var strMsg2 = "webmethod (" + this._WebMethodName + ") response error.\n\nstatus=" + this.status;
                if (this._CallErrorProc != null) this._CallErrorProc(strMsg2, this._Tag);
            }
        }
    }
    */
}

//--------------------------------------------------------------------------------------
//
//(3)table分页处理
//
//--------------------------------------------------------------------------------------

var DataClient = function (url, methodname, divname, xslname, navStyle) {
    this._DivName = divname;    //html容器ID
    this._XslName = xslname;    //xsl样式文件路径
    this._XmlDoc = "";          //xml内容
    this._navStyle = ((navStyle != null) ? navStyle : "") + "111111111";
    this._Webmethod = new WebMethodProxy(url, methodname);

    this._PageIndex = 1;    //当前页(客户端传入，服务端重新计算，以服务端返回为准）
    this._Param = "";       //查询参数JSON字符串
    this._RecordCount = 0;  //总记录数（服务端返回)
    this._PageCount = 0;    //总页数（服务端返回)
    this._DataSort = -1;//"";    //排序字段
    this._PageSize = 10;    //每页记录数
    this._ShowNav = true;   //是否显示分布导航栏    
    this._PageType = 1; //1 标准模式 2 精简模式
    //this._SortClickElememt = null;  //排序触发元素

    this._pageSizeArr = [10, 20, 30, 50, 100];  //分页下拉框选项

    //----公开事件接口------------------------------------------------------------
    this.OnQuery = null;            //开始查询 function()
    this.OnData = null;             //正常返回数据 function(xmlDoc)
    this.OnError = null;            //返回出错信息 function(errMessage)
    this.OnQueryCompleted = null;   //查询结束 function(queryOK)
    //----------------------------------------------------------------------------
    this.PageQueryFlag = false;
    //----公开方法接口------------------------------------------------------------
    this.Goto = function (PageIndex) {
        if (isNaN(PageIndex)) PageIndex = 1;
        if (PageIndex <= 1) PageIndex = 1;
        if (PageIndex >= this._PageCount && this._PageCount > 0)
            PageIndex = this._PageCount;
        if (this._PageIndex != PageIndex) {
            this._PageIndex = PageIndex;
            if (this._queryed) this._Query();
        }
    }

    this.Jump = function (index) {
        this.Goto(this._PageIndex + index);
    }

    this.Sort = function (sort) {
        if (sort == null || sort == "") return;
        if (this._DataSort.indexOf(sort) > -1) { this._DataSort = this._DataSort.indexOf("ASC") > 0 ? sort + " DESC" : sort + " ASC"; }
        else { this._DataSort = sort + " ASC"; }
        if (this._queryed) this._Query();
    }

    this.SortByFlag = function (typeflag) {
        if (typeflag == null || typeflag == "") return;
        if (this._DataSort.toString().lastIndexOf(typeflag) > 0) { this._DataSort = this._DataSort > 100 ? this._DataSort : typeflag; }
        else { this._DataSort = typeflag; }

        if (this._DataSort < 100) { this._DataSort = typeflag + 100; }
        else { this._DataSort = this._DataSort - 100; }

        if (this._queryed) this._Query();
        //this.PageQueryFlag = false;
    }

    this.SetSortFlag = function () {
        if (this.PageQueryFlag) { return; }
        else { }
        if (this._DataSort < 100) { this._DataSort += 100; }
        else { this._DataSort -= 100; }
        this.PageQueryFlag = true;
    }


    this.Query = function (param) { 
        this.PageQueryFlag = true;
        this._PageIndex = 1;
        this._Param = param;
        this._Query();

    }

    this.GetXmlDoc = function () {
        return this._XmlDoc;
    }

    this.SetPageSize = function (pageSize) {
        var strCookieName = this._XslName + "_pagesize";
        if (pageSize == null) pageSize = GetCookie(strCookieName);
        this._PageSize = parseInt(pageSize);
        if (isNaN(this._PageSize)) this._PageSize = 10;
        if (this._PageSize <= 0) this._PageSize = 10;
        if (this._PageSize >= 100) this._PageSize = 100;
        SetCookie(strCookieName, this._PageSize);
        if (this._queryed) {
            this._PageIndex = 1; this._Query();
        }
    }
    //----------------------------------------------------------------------------

    //查询调用
    this._queryed = false;
    this._Query = function () {
         
        this._RecordCount = 0;
        this._Webmethod.AddParam("strparam", this._Param);
        this._Webmethod.AddParam("pageindex", this._PageIndex);
        this._Webmethod.AddParam("pagesize", this._PageSize);
        this._Webmethod.AddParam("sort", this._DataSort);
        this._queryed = true;

        try {
            if (this.OnQuery != null) this.OnQuery();
            this._Webmethod.Call(this._CallBack, this._CallError, this);
        } catch (e) {
            this._CallError("加载数据时发生异常，异常信息：" + e.description, this);
        }
    }

    this._CallBack = function (retMessage, ObjThis) {
        var xmlDoc = CreateXmlFromString(retMessage);
        if (ObjThis != null) {
            if (ObjThis._BindData(xmlDoc)) {  //绑定数据
                if (ObjThis.OnData != null)             //将返回的数据传向外部调用者（如果需要的话）
                    ObjThis.OnData(xmlDoc);
                if (ObjThis.OnQueryCompleted != null)   //查询成功
                    ObjThis.OnQueryCompleted(true);
            }
        }
    }

    this._CallError = function (retMessage, ObjThis) {
        this._queryed = false;
        if (ObjThis != null) {
            if (ObjThis.OnError != null)            //将出错信息传向外部调用者（如果需要的话）
                ObjThis.OnError(retMessage);
            if (ObjThis.OnQueryCompleted != null)   //查询失败
                ObjThis.OnQueryCompleted(false);
        }
    }

    this._BindData = function (xmlDoc) {
        if (this._ShowNav) {
            this.Init();
            if (!this._inited) { this._CallError("页面对象" + this._DivName + "不存在，数据显示区域初始化失败", this); return false; }
        }
        else { $(this._DivName).ObjThis = this; }

        var rootNode = xmlDoc.selectSingleNode("xml");
        if (rootNode == null) { this._CallError("返回数据格式有误（不包含根结点xml）", this); return false; }

        var mCode = GetAttributeValue(rootNode, "code");
        var mMsg = GetAttributeValue(rootNode, "msg");
        var mValue = GetAttributeValue(rootNode, "value");
        if (mCode != "0") { this._CallError("webmethod return error(code=" + mCode + ") error msg: " + mMsg, this); return false; }

        rootNode.setAttribute("DivObjName", this._DivName); //增加一个本地变量，在进行格式绑定时传入html代码中
        rootNode = xmlDoc.selectSingleNode("xml/items");    //这里绑定了xml的格式
        if (rootNode == null) { this._CallError("返回数据格式有误（不包含结点xml/items）", this); return false; }
        this._PageIndex = parseInt(GetAttributeValue(rootNode, "pageindex"));     //当前页码（客户端设置，服务端返回)
        this._RecordCount = parseInt(GetAttributeValue(rootNode, "recordcount")); //总记录数（服务端返回）
        this._PageCount = parseInt(GetAttributeValue(rootNode, "pagecount"));     //总页数（服务端返回）

        //生成Table
        try {
            var mObjXsl = new ActiveXObject("Microsoft.XMLDOM");
            mObjXsl.async = false;
            mObjXsl.load(this._XslName);
            if (this._ShowNav) {
                $("TDGrid_" + this._DivName).innerHTML = xmlDoc.transformNode(mObjXsl);
                //重置导航栏
                this._ResetNavigatorBar(false);
            } else {
                this._inited = false;
                $(this._DivName).innerHTML = xmlDoc.transformNode(mObjXsl);
            }
            this._XmlDoc = xmlDoc;
            var linkid = this._DataSort;
            if (this._DataSort > 100) { linkid = this._DataSort - 100; }
            if ($("tablelink_" + linkid)) { $("tablelink_" + linkid).innerText += (this._DataSort > 100) ? "▼" : "▲"; }
        } catch (e) {
            this._CallError("绑定数据样式" + this._XslName + "出错，数据显示失败", this); return false;
        }

        return true;
    };

    this._inited = false;
    this.Init = function () {
        if ($(divname) == null) return;
        if (this._inited) return;

        var mDivName = this._DivName;
        this.SetPageSize();

        //画主框架
        var str = '<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">';
        str += '	<TR>';
        str += '		<TD class="QueryContent" id="TDGrid_' + mDivName + '" style="vertical-align:top;overflow-x:none;"><IMG src="' + mImgMsgFilePath + '/loading.gif"></TD>';
        str += '	</TR>';
        str += '	<TR>';
        str += '		<TD id="pagebar" class="QueryPage" vAlign="top" align="center">';
        //str += '			<TABLE cellSpacing="0" cellPadding="0" width="100%" border="1">';
        //str += '				<TR>';
        str += '					<span style="width:150px; display:' + this._setNavigatorBarDisplay(0) + '" id="TD_Item' + mDivName + '">第 0 / 0 条</span>';
        str += '					<span align="center" style="display:' + this._setNavigatorBarDisplay(1) + '">';
        str += '<table border=0 cellspacing=0 cellpadding=0 ><tr>';
        str += '						<td width=48 id="BtnF_' + mDivName + '" onclick=CommonGoto("' + mDivName + '",0) ><IMG src="' + mImgFilePath + '/button/first.gif" align=absmiddle><a>首页</a></td>';
        str += '						<td width=48 id="BtnP_' + mDivName + '" onclick=CommonJump("' + mDivName + '",-1) ><IMG src="' + mImgFilePath + '/button/prior.gif" align=absMiddle><a>上页</a></td>';
        str += '						<td width=48 id="BtnN_' + mDivName + '" onclick=CommonJump("' + mDivName + '",1) ><IMG src="' + mImgFilePath + '/button/next.gif" align=absMiddle><a>下页</a></td>';
        str += '						<td width=48 id="BtnL_' + mDivName + '" onclick=CommonGoto("' + mDivName + '",999999) ><IMG src="' + mImgFilePath + '/button/last.gif" align=absMiddle><a>尾页</a></td>';
        str += '</tr></table>';
        //        str += '						<span id="BtnF_' + mDivName + '" onclick=CommonGoto("' + mDivName + '",0) ><IMG src="' + mImgFilePath + '/button/first.gif" align=absmiddle><a>首页</a></span>';
        //        str += '						<span id="BtnP_' + mDivName + '" onclick=CommonJump("' + mDivName + '",-1) ><IMG src="' + mImgFilePath + '/button/prior.gif" align=absMiddle><a>上页</a></span>';
        //        str += '						<span id="BtnN_' + mDivName + '" onclick=CommonJump("' + mDivName + '",1) ><IMG src="' + mImgFilePath + '/button/next.gif" align=absMiddle><a>下页</a></span>';
        //        str += '						<span id="BtnL_' + mDivName + '" onclick=CommonGoto("' + mDivName + '",999999) ><IMG src="' + mImgFilePath + '/button/last.gif" align=absMiddle><a>尾页</a></span>';
        str += '					</span>';
        str += '					<span style="width:150px;display:' + this._setNavigatorBarDisplay(2) + '">';
        str += '每页显示<select id="CbxPageSize_' + mDivName + '" onchange=CommonSetPageSize("' + mDivName + '",this)>';
        for (var i = 0; i < this._pageSizeArr.length; i++)
            str += '<option value=' + this._pageSizeArr[i] + ((parseInt(this._pageSizeArr[i]) == this._PageSize) ? ' selected' : '') + '>' + this._pageSizeArr[i] + '</option>';
        str += '</select>条记录</span>';
        if (this._PageType == 1) {
            str += '					<span align="right" style="width:200px;display:' + this._setNavigatorBarDisplay(3) + '">';
            str += '						第<INPUT id="EditPage_' + mDivName + '" maxlength="6" value="0" style="width:48px" onfocus="this.select()" onchange="_ResetPageindex(this)">页/共<span id="SPPage_' + mDivName + '"> </span> 页 ';
            str += '						<a id="BtnG_' + mDivName + '" onclick=CommonGotoA("' + mDivName + '") ><IMG src="' + mImgFilePath + '/button/goto.gif" align=absMiddle>跳转</a>';
            str += '					</span>';
        }
        //        str += '				</TR>';
        //        str += '			</TABLE>';
        str += '		</TD>';
        str += '	</TR>';
        str += '</TABLE>';

        $(mDivName).innerHTML = str;
        $(mDivName).ObjThis = this;
        this._PageSize = parseInt($("CbxPageSize_" + mDivName).value);

        this._ResetNavigatorBar(true);
        this._inited = true;
    }

    this._setNavigatorBarDisplay = function (idx) {
        var visible = (this._navStyle.substr(idx, 1) == "1");
        return visible ? "inline-block" : "none";
    }

    this._ResetNavigatorBar = function (mIsLoading) {
        var mDivName = this._DivName;

        var Sno = Math.min(this._PageSize * (this._PageIndex - 1) + 1, this._RecordCount);
        var Eno = Math.min(Sno + this._PageSize - 1, this._RecordCount);
        $("TD_Item" + mDivName).innerHTML = Sno + " - " + Eno + " / " + this._RecordCount + " 条"
        $("CbxPageSize_" + mDivName).value = this._PageSize;
        var editPage_Num = $("EditPage_" + mDivName);
        if (editPage_Num) {
            editPage_Num.value = this._PageIndex;
            editPage_Num.defaultvalue = this._PageIndex;
            editPage_Num.minvalue = 1;
            editPage_Num.maxvalue = this._PageCount;
        }
        if ($("SPPage_" + mDivName)) $("SPPage_" + mDivName).innerHTML = this._PageCount;

        var CanFirst = (this._PageIndex > 1) && (!mIsLoading);
        var CanLast = (this._PageIndex < this._PageCount) && (!mIsLoading);
        var CanQuery = (!mIsLoading);

        $("BtnF_" + mDivName).disabled = !CanFirst;
        $("BtnP_" + mDivName).disabled = !CanFirst;
        $("BtnN_" + mDivName).disabled = !CanLast;
        $("BtnL_" + mDivName).disabled = !CanLast;
        $("CbxPageSize_" + mDivName).disabled = !CanQuery;
        if (editPage_Num) {
            editPage_Num.disabled = !CanQuery;
        }
        if ($("BtnG_" + mDivName)) $("BtnG_" + mDivName).disabled = !CanQuery;
    }

    //界面初始化
    this.Init();
}

//----------------------------------------

function _ResetPageindex(sender) {
    var mvalue = parseInt(sender.value);
    mvalue = isNaN(mvalue) ? sender.defaultvalue : mvalue;
    if (mvalue < sender.minvalue) mvalue = sender.minvalue;
    if (mvalue > sender.maxvalue) mvalue = sender.maxvalue;
    sender.value = mvalue;
}

function _GetDataClientObj(divname) {
    if ($(divname) != null)
        return $(divname).ObjThis;
    else
        return null;
}

function CommonGoto(divname, PageIndex) {
    var obj = _GetDataClientObj(divname);
    if (obj != null) {
        obj.SetSortFlag(); //add by qiubaosheng 保持排序不变
        obj.Goto(PageIndex);
    }
}

function CommonGotoA(divname) {
    var obj = _GetDataClientObj(divname);
    PageIndex = parseInt($("EditPage_" + divname).value);
    if (obj != null) {
        obj.SetSortFlag(); //add by qiubaosheng 保持排序不变
        obj.Goto(PageIndex);
    }
}

function CommonJump(divname, index) {
    var obj = _GetDataClientObj(divname);
    if (obj != null) {
        obj.SetSortFlag(); //add by qiubaosheng 保持排序不变 
        obj.Jump(index);
    }
}

function CommonSort(divname, strSort) {
    var obj = _GetDataClientObj(divname);
    if (obj != null) {
        obj.SetSortFlag(); //add by qiubaosheng 保持排序不变
        obj.Sort(strSort);
    }
}


function CommonSortByFlag(divname, strSort) {
    var obj = _GetDataClientObj(divname);
    if (obj != null) obj.SortByFlag(strSort);
    //obj._SortClickElememt = window.event.srcElement;
}


function CommonSetPageSize(divname, sender) {
    var obj = _GetDataClientObj(divname);
    mPageSize = sender.value;
    if (obj != null) {
        obj.SetSortFlag(); //add by qiubaosheng 保持排序不变
        obj.SetPageSize(mPageSize);
    }
}

//--------------------------------------------------------------------------------------
//
//(4)打印调用
//
//--------------------------------------------------------------------------------------

//显示报表（reportview)
var CommonReporter = function (url, methodname) {
    //todo:
    //客户端传入参数 param
    //服务端事件method：生成一个ds，并指定对应的rdsl路径，保存入session
    //返回sessionID

    //客户端:弹出显示打印预览窗口(这个窗口应该是固定页面),传SessionID
    //打印页窗口，根据传的sessionID得到ds与rdsl，生成显示页面

    this._Webmethod = new WebMethodProxy(url, methodname);

    //----公开事件接口------------------------------------------------------------
    this.OnShow = null;            //开始查询 function()
    this.OnData = null;            //正常返回数据 function(xmlDoc)
    this.OnError = null;           //返回出错信息 function(errMessage)
    this.OnShowCompleted = null;   //显示结束 function(queryOK)
    //----------------------------------------------------------------------------

    //----公开事件接口------------------------------------------------------------
    this.ShowOrDownLoad = true;   //直观显示或者下载文件
    //----------------------------------------------------------------------------

    this.Show = function (param) {
        try {
            if (this.OnShow != null) this.OnShow();
            this._Webmethod.AddParam("strparam", param);
            this._Webmethod.Call(this._CallBack, this._CallError, this);
        } catch (e) {
            this._CallError("加载数据时发生异常，异常信息：" + e.description, this);
        }
    }

    this._CallBack = function (retMessage, ObjThis) {
        var xmlDoc = CreateXmlFromString(retMessage);
        if (ObjThis != null) {
            ObjThis._ShowReport(xmlDoc);  //绑定数据
            if (ObjThis.OnData != null)             //将返回的数据传向外部调用者（如果需要的话）
                ObjThis.OnData(xmlDoc);
            if (ObjThis.OnShowCompleted != null)   //查询成功
                ObjThis.OnShowCompleted(true);
        }
    }

    this._CallError = function (retMessage, ObjThis) {
        if (ObjThis != null) {
            if (ObjThis.OnError != null)            //将出错信息传向外部调用者（如果需要的话）
                ObjThis.OnError(retMessage);
            if (ObjThis.OnShowCompleted != null)   //查询失败
                ObjThis.OnShowCompleted(false);
        }
    }

    this._ShowReport = function (xmlDoc) {
        var rootNode = xmlDoc.selectSingleNode("xml");
        if (rootNode == null) { this._CallError("返回数据格式有误（不包含根结点xml）", this); return; }

        var mCode = GetAttributeValue(rootNode, "code");
        var mMsg = GetAttributeValue(rootNode, "msg");
        var mValue = GetAttributeValue(rootNode, "value");
        if (mCode != "0") { this._CallError("webmethod return error(code=" + mCode + ") error msg: " + mMsg, this); return; }
        // alert(xmlDoc.xml);
        // ShowDebugInfo(xmlDoc.xml);
        window.open(mFramePath + "/report.ashx?id=" + mValue + "&download=" + (this.ShowOrDownLoad ? "0" : "1"), "_report");
    }
}

//--------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------
//
//(4)显示自带格式的xml(xml文件中包含一个xsl节点)
//
//--------------------------------------------------------------------------------------

function ShowFormatXml(strxml, divid) {
    try {
        var xmlDoc = CreateXmlFromString(strxml);
        var rootNode = xmlDoc.selectSingleNode("xml");
        if (rootNode == null) return;
        var xslNode = rootNode.selectSingleNode("xsl");
        if (xslNode == null) return;

        var mObjXsl = new ActiveXObject("Microsoft.XMLDOM");
        mObjXsl.async = false;
        mObjXsl.loadXML(xslNode.text);
        $(divid).innerHTML = xmlDoc.transformNode(mObjXsl);
    } catch (e) {
    }
}

function SetSelOption2(select, node, key, val) {
    if (node == null) return;
    for (var i = 0; i < node.childNodes.length; i++) {
        var mName = GetNodeValue(node.childNodes[i], key);
        var mID = GetNodeValue(node.childNodes[i], val)
        select.add(new Option(mName, mID));
    }
    if (node.childNodes.length == 1) {
        //select.options.remove(0);
        //SetElemEnable(select, false);
    }
}