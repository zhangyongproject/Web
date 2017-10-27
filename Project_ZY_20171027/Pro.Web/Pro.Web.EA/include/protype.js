

// --------------------------------------------------------------------
// script type's prototypies (string)
// --------------------------------------------------------------------


String.prototype.padLeft = function(len, str) {
    var ret = this;
    while (ret.length < len) { ret = str + ret; }
    return ret;
}
String.prototype.padRight = function(len, str) {
    var ret = this;
    while (ret.length < len) { ret = ret + str; }
    return ret;
}
String.prototype.replaceAll = function(oldstr, newstr) {
    raRegExp = new RegExp(oldstr, "g");
    return this.replace(raRegExp, newstr);
}
String.prototype.trim = function() {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
String.prototype.format = function() {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g, function(m, i) { return args[i]; });
}
String.prototype.codeLength = function() {
    var len = 0;
    for (i = 0; i < this.length; i++) {
        if (this.charCodeAt(i) > 0 && this.charCodeAt(i) < 128) len++;
        else len += 2;
    }
    return len;
}
String.prototype.length2 = function() {
    var cArr = this.match(/[^x00-xff]/ig);
    return this.length + (cArr == null ? 0 : cArr.length);
}

String.prototype.htmlEncode = function() {
    return this.replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;").replaceAll("\'", "&#34;").replaceAll("\"", "&#39;");
}
String.prototype.htmlDecode = function() {
    return this.replaceAll("&amp;", "&").replaceAll("&lt;", "<").replaceAll("&gt;", ">").replaceAll("&#34;", "\'").replaceAll("&#39;", "\"");
}
String.prototype.validReg = function(check) {
    if (!check.test(this)) { return false; } return true;
}
String.prototype.toArray = function() {
    var args = arguments; var thisStr = this;
    if (args.length < 1) args = [','];
    else if (IsArr(args[0])) args = args[0]
    var sptr = args[0];
    for (var i = 1; i < args.length; i++)
        thisStr = thisStr.replaceAll(args[i], sptr);
    return thisStr.split(sptr);
}
String.prototype.toDate = function(format, arrSptrs) {
    // "09.01.01 12:01:01".toDate("yyyy.MM.dd hh:mm:ss", ['.',' ', ':'])
    // return null while failure
    format = GetStr(format, 'yyyy-MM-dd hh:mm:ss');
    if (!IsArr(arrSptrs)) arrSptrs = ['-', ' ', ':'];
    var dateArr = this.toArray(arrSptrs);
    var fmtArr = format.toArray(arrSptrs);
    if (dateArr.length < fmtArr.length) return null;
    var y = 1970; var M = 1; var d = 1; var h = 0; var m = 0; var s = 0; var S = 0;
    for (var i = 0; i < fmtArr.length; i++) {
        switch (fmtArr[i].substr(0, 1)) {
            case "y": y = GetInt(dateArr[i], y); break;
            case "M": M = GetInt(dateArr[i], M); break;
            case "d": d = GetInt(dateArr[i], d); break;
            case "h": h = GetInt(dateArr[i], h); break;
            case "m": m = GetInt(dateArr[i], m); break;
            case "s": s = GetInt(dateArr[i], s); break;
            case "S": S = GetInt(dateArr[i], S); break;
        }
    }
    if (y < 100) y += 2000;
    return new Date(y, M - 1, d, h, m, s, S);
}

String.prototype.ToJSON = function() {
    //var str = "{ID:12,Name:'Tom',Age:21}";
    try {
        var json = eval('(' + this + ')');
        return json;
    } catch (e) {
        return { error: e };
    }
}

// --------------------------------------------------------------------
// valid type value
// --------------------------------------------------------------------

String.prototype.validNum = function() {
    if (g_re_float.test(this) || g_re_int.test(this))
        return true;
    return false;
}
String.prototype.validInt = function() {
    if (g_re_int.test(this))
        return true;
    return false;
}
String.prototype.validInt1 = function() {
    if (g_re_upint1.test(this))
        return true;
    return false;
}
String.prototype.validFloat = function() {
    if (g_re_float.test(this))
        return true;
    return false;
}
String.prototype.validAccount = function() {
    if (g_re_account.test(this))
        return true;
    return false;
}
String.prototype.IsLegality = function() {
    return (!g_re_exChar.test(obj));
}


// --------------------------------------------------------------------
// script type's prototypies (date) 
// --------------------------------------------------------------------

Date.prototype.format = function(fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份         
        "d+": this.getDate(), //日         
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
        "H+": this.getHours(), //小时         
        "m+": this.getMinutes(), //分         
        "s+": this.getSeconds(), //秒         
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
        "S": this.getMilliseconds() //毫秒         
    };
    var week = {
        "0": "/u65e5",
        "1": "/u4e00",
        "2": "/u4e8c",
        "3": "/u4e09",
        "4": "/u56db",
        "5": "/u4e94",
        "6": "/u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

// 在日期值上增加年份
Date.prototype.addYear = function(val) {
    if (typeof (val) != "number") return this;
    return new Date(this.setFullYear(this.getFullYear() + val));
}

// 在日期值上增加月份
Date.prototype.addMonth = function(val) {
    if (typeof (val) != "number") return this;
    var months = this.getFullYear() * 12 + this.getMonth() + val;
    var years = Math.floor(months / 12);
    months = months - (years * 12);
    var newLastDate = new Date(years, months, 0).getDate();
    if (this.getDate() > newLastDate)
        return new Date(years, months, 0);
    return new Date(years, months, this.getDate());
}

// 在日期值上增加天数
Date.prototype.addDate = function(val) {
    if (typeof (val) != "number") return this;
    var ms = this - new Date(0);
    var val = val * 24 * 60 * 60 * 1000;
    return new Date(ms + val);
}

// 获取日期月份中的最大日期数
Date.prototype.getLastDate = function() {
    return (new Date(this.getFullYear(), this.getMonth() + 1, 0)).getDate();
}

// --------------------------------------------------------------------
// script type's prototypies (array & object) 
// --------------------------------------------------------------------
//取消，以下函数与openlayers冲突
//Array.prototype.indexOf = function(val) {
//    // alert([1,2,3,"2"].indexOf("2")); //3
//    var nTemp = -1;
//    for (var i = 0; i < this.length; i++) {
//        if (this[i] == val) nTemp = i;
//        if (this[i].toString() == val.toString() && typeof (this[i]) == typeof (val)) return i;
//    }
//    return nTemp;
//}
//Array.prototype.getValue = function() {
//    // alert([1,2,3,["1","2","3"]].getValue(3,2));  // "2"
//    var arr = this;
//    for (var i = 0; i < arguments.length; i++) {
//        if (typeof (arguments[i]) == "number" && arr.length > arguments[i]) { arr = arr[arguments[i]]; } else { return null; }
//    }
//    return arr;
//}
//Array.prototype.remove = function(index) {
//    if (index >= 0 && index < this.length) {
//        this.splice(index, 1);
//    }
//}

function GetObjAttr(obj, attrName) {
    return eval("obj." + attrName);
}


// --------------------------------------------------------------------
// about valid (return true or false)
// --------------------------------------------------------------------

function IsStr(obj) {
    return Object.prototype.toString.call(obj) === '[object String]';
}
function IsNum(obj) {
    return Object.prototype.toString.call(obj) === '[object Number]';
}
function IsDate(obj) {
    return Object.prototype.toString.call(obj) === '[object Date]';
}
function IsArr(obj) {
    return Object.prototype.toString.call(obj) === '[object Array]';
}
function IsObj(obj) {
    return Object.prototype.toString.call(obj) === '[object Object]';
}
function IsReg(obj) {
    return Object.prototype.toString.call(obj) === '[object RegExp]';
}
function IsElem(obj) {
    if (!obj) return false;
    return (typeof obj == 'object') && typeof obj.document != 'undefined';
}
function IsDOM(obj) {
    if (!obj) return false;
    return (typeof obj == 'object') && typeof obj.parseError != 'undefined';
}
function IsHtmlObj(obj) {
    if (!obj) return false;
    return (typeof obj == 'object') && typeof obj.all != 'undefined';
}
function IsDocument(obj) {
    if (!obj) return false;
    return (typeof obj == 'object') && typeof obj.documentElement != 'undefined';
}
function IsFunction(obj) {
    if (!obj) return false;
    return (typeof obj == 'function') && obj.constructor == Function;
}
function IsLegality(obj) {
    return (!g_re_exChar.test(obj));
}

// 除法函数
// 说明：javascript的算法结果会有误差，在两个浮点数相除的时候会比较明显。
// accDiv(4,2) //2
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
    try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""))
        r2 = Number(arg2.toString().replace(".", ""))
        return (r1 / r2) * pow(10, t2 - t1);
    }
}

// 给Number类型增加一个div方法
// (9).div(3); //3
Number.prototype.div = function(arg) {
    return accDiv(this, arg);
}

// 乘法函数
// accMul(2,2)  //4
function accMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try { m += s1.split(".")[1].length } catch (e) { }
    try { m += s2.split(".")[1].length } catch (e) { }
    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m)
}

// 给Number类型增加一个mul方法
// (6).mul(3); //18
Number.prototype.mul = function(arg) {
    return accMul(arg, this);
}

// 加法函数
// accAdd(1,2) //3
function accAdd(arg1, arg2) {
    var r1, r2, m;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2));
    return (arg1 * m + arg2 * m) / m;
}

// 给Number类型增加一个add方法
// (37).add(3); //40
Number.prototype.add = function(arg) {
    return accAdd(arg, this);
}

// 减法函数
// Subtr(5,4) //1
function Subtr(arg1, arg2) {
    var r1, r2, m, n;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2));
    n = (r1 >= r2) ? r1 : r2;
    return ((arg1 * m - arg2 * m) / m).toFixed(n);
}

// 给Number类型增加一个add方法
// (5).sub(37) //42
Number.prototype.sub = function(arg) {
    return Subtr(arg, this);
}

//--------------------------------------------------------------

function Hashtable() {
    this._hash = new Object();
    this.add = function(key, value) {
        if (typeof (key) != "undefined") {
            //            if (!this.contains(key) ) {
            //                this._hash[key] = typeof (value) == "undefined" ? null : value;
            //                return true;
            //            } else {
            //                return false;
            //            }
            this._hash[key] = typeof (value) == "undefined" ? null : value;
        } else {
            return false;
        }
    }
    this.remove = function(key) { delete this._hash[key]; }
    this.count = function() { var i = 0; for (var k in this._hash) { i++; } return i; }
    this.items = function(key) { return this._hash[key]; }
    this.contains = function(key) { return typeof (this._hash[key]) != "undefined"; }
    this.clear = function() { for (var k in this._hash) { delete this._hash[k]; } }

    /** hashTable 2 json */
   
    this.toJson = function() {
        var str = "";
        for (var attr in this._hash) {
            if (typeof (this._hash[attr]) != "number") {
                str += "," + attr + ":\"" + this._hash[attr].replaceAll("\"","\\\"")+ "\"";
            } else {
                str += "," + attr + ":" + this._hash[attr];
            }
            //str += ",\"" + attr + "\":\"" + this._hash[attr].replace("\"", "").replace("'", "").replace(":", "").replace(",", "") + "\"";
        }
        if (str.length > 0) { str = str.substr(1, str.length); }
        return "{" + str + "}";
    };

    this.toJson2 = function () {
        var str = "";
        for (var attr in this._hash) {
            if (typeof (this._hash[attr]) != "number") {
                str += ",\"" + attr + "\":\"" + this._hash[attr].replaceAll("\"", "\\\"") + "\"";
            } else {
                str += ",\"" + attr + "\":" + this._hash[attr];
            }
            //str += ",\"" + attr + "\":\"" + this._hash[attr].replace("\"", "").replace("'", "").replace(":", "").replace(",", "") + "\"";
        }
        if (str.length > 0) { str = str.substr(1, str.length); }
        return "{" + str + "}";
    };

    this.toArrayKey = function() {
        var str = ""; 
        for (var attr in this._hash) {
            str += "," + attr;
        }
        if (str.length > 0) { str = str.substr(1, str.length); }
        return str;
    };
}
