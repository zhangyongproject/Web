//----------------------------------------------------------------------------------------------------

var $ = function (strID) { return document.getElementById(strID); };

//判断浏览器版本高于IE7
//var ie7X = !!window.ActiveXObject && !!window.XMLHttpRequest;
//var ie7X = ("ActiveXObject" in window) && !!window.XMLHttpRequest;
//var ie11 = ie7X && !window.ActiveXObject; //是否IE11

var Browser = {
    ie: /msie/.test(window.navigator.userAgent.toLowerCase()),
    edge: /edge/.test(window.navigator.userAgent.toLowerCase()),
    moz: /gecko/.test(window.navigator.userAgent.toLowerCase()),
    opera: /opera/.test(window.navigator.userAgent.toLowerCase()),
    safari: /safari/.test(window.navigator.userAgent.toLowerCase())
};
var ie7X = Browser.ie && !!window.XMLHttpRequest;
var ie11 = ie7X && !window.ActiveXObject; //是否IE11

// 正则表达式集合：
//\n\s*\r                             //匹配空白行
//<(\S*?)[^>]*>.*?</\1>|<.*? />       //匹配HTML标记
//[a-zA-z]+://[^\s]*                  //匹配网址URL
//[1-9][0-9]{4,}                      //匹配腾讯QQ号
//\d{15}|\d{18}                       //匹配身份证
//\d+\.\d+\.\d+\.\d+                  //匹配ip地址
//^[A-Za-z0-9]+$　　                  //匹配由数字和26个英文字母组成的字符串
//^\w+$　　                           //匹配由数字、26个英文字母或者下划线组成的字符串

var g_re_cnstr = /[\u4e00-\u9fa5]/;                     //匹配中文字符
var g_re_dbbyte = /[^\x00-\xff]/;                        //匹配双字节字符(包括汉字在内)
var g_re_sidesblk = /^\s*|\s*$/;                           //匹配首尾空白字符
var g_re_email = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;     //匹配Email地址
var g_re_phoneno = /^(\d{3,4}-)?\d{7,8}$/;                  //匹配国内电话号码
var g_re_mobile = /^(13[0-9]|15[0-9]|18[0-9])\d{8}$/;  //匹配手机
var g_re_postcode = /[1-9]\d{5}(?!\d)/;                        //匹配中国邮政编码

var g_re_pwd = /^[A-Za-z0-9]+$/;    //匹配由数字和26个英文字母组成的字符串
var g_re_exChar = /.*\'.*|.*%.*/;
var g_re_empty = /\n\s*\r/; //空白行

var g_re_upInt = /^[1-9]\d*$/;                     //匹配正整数
var g_re_upint1 = /^[0-9]\d*$/;                  //匹配正整数 前面可以有多个0
var g_re_dnInt = /^-[1-9]\d*$/;                      //匹配负整数
var g_re_int = /^(-?[1-9]\d*|0)$/;                    //匹配整数+
var g_re_upFloat = /^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$/;  //匹配正浮点数
var g_re_dnFloat = /^-([1-9]\d*\.\d*|0\.\d*[1-9]\d*)$/; //匹配负浮点数
var g_re_float = /^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$/;   //匹配浮点数+

var g_re_enstr = /^[A-Za-z]+$/;                     //匹配由26个英文字母组成的字符串
var g_re_upEnstr = /^[A-Z]+$/;                        //匹配由26个英文字母的大写组成的字符串
var g_re_dnEnstr = /^[a-z]+$/;                        //匹配由26个英文字母的小写组成的字符串

var g_re_account = /^[a-zA-Z][a-zA-Z0-9_]{1,15}$/;        //匹配帐号是否合法(字母开头，允许2-16字节，允许字母数字下划线)+

var g_re_date = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/; //匹配日期类型是否为YYYY-MM-DD hh:mm:ss格式的类型 
var g_re_date1 = /^(?:(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\/|-|\.)(?:0?2\1(?:29))$)|(?:(?:1[6-9]|[2-9]\d)?\d{2})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8]))$/;
var g_re_time = /^((20|21|22|23|[0-1]\d)\:[0-5][0-9])(\:[0-5][0-9])?$/;   //匹配时间类型为hh:mm:ss格式的类型   
var g_re_ip = /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/;       //匹配ip地址


//----------------------------------------------------------------------------------------------------
//Cookie处理函数
//----------------------------------------------------------------------------------------------------

function __getCookieVal(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1) endstr = document.cookie.length;
    return unescape(document.cookie.substring(offset, endstr));
}

//读Cookie
function GetCookie(name) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg)
            return __getCookieVal(j);
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
    }
    return "";
}

//写Cookie
function SetCookie(name, value) {
    //    var argv = SetCookie.arguments;
    //    var argc = SetCookie.arguments.length;
    var expires = new Date(); //保存COOKIES时间为1年
    expires.setTime(expires.getTime() + (24 * 60 * 60 * 1000 * 365));
    document.cookie = name + "=" + escape(value) + ("; expires=" + expires.toGMTString()) + "; path=/";
}
//----------------------------------------------------------------------------------------------------

//获取URL参数的值
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return "";
}
//-------------------------------------------------------------------------

//获取URL参数的值
function getUrlParam2(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.href.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return "";
}

//解析URL中网页文件名（可能存在返回空值的情况）
function getUrlPage() {
    var str = "/" + window.location.pathname;
    str = str.substr(str.lastIndexOf("/") + 1);
    if (str.indexOf(".") > -1)
        str = str.substr(0, str.lastIndexOf("."));
    return str;
}

//----------------------------------------------------------------------------------------------------

//显示调试信息（显示html代码）
function ShowDebugInfo(strInfo) {
    var newWindow = window.open();
    newWindow.opener = null;
    newWindow.document.write(EncodeParamStr(strInfo));
};

//显示调试信息（显示html内容）
function ShowDebugHtml(strHtml) {
    var newWindow = window.open();
    newWindow.opener = null;
    newWindow.document.write(strHtml);
};

//----------------------------------------------------------------------------------------------------

function SetClip(text) {
    window.clipboardData.setData('text', text);
}

//检查表达式是否合法
function CheckExpress(express) {
    str = "var tempv= (" + express.replace(/ /g, "") + ")&& true||false;";
    try { eval(str); return true; } catch (e) { return false; }
}