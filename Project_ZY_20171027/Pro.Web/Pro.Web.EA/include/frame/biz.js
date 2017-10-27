var mFramePath = "../T00Frame";                 //框架文件路径
var mImgFilePath = "../images";                 //图片文件路径
var mImgMsgFilePath = mImgFilePath + "/message";  //信息图片文件路径

//与系统有关的共用函数

function DownLoadFile(blobID, verifycode) {
    var url = mFramePath+ "/getfile.ashx?blobid=" + blobID + "&verify=" + verifycode;
    window.open(url, "_blank");
}

//blobID：原大字段记录ID，若上传新记录，则设置为“NewFile”
//verifycode： blobID存在时，此值为验证码（防止非法上传覆盖原记录）
//filter：文件过滤条件，如　zip、　jpg|jpeg|png 等
//onPageCallback：回调函数，当文件上传成功时，返回数据库大字段记录BlotID(retvalue)
function UploadFile(blobID, verifycode, filter, onPageCallback) {
    var url = mFramePath+ "/upload.aspx?blobid=" + blobID + "&verify=" + verifycode;
    ShowPage(url, "文件上传", function(retcode, retvalue) { if (onPageCallback != null) onPageCallback(retcode, retvalue); }, 350, 160, filter);
}

function SetSelOption(select, node) {
    if (node == null) return;
    for (var i = 0; i < node.childNodes.length; i++) {
        var mName = GetAttributeValue(node.childNodes[i], "name");
        var mID = GetAttributeValue(node.childNodes[i], "key")
        select.add(new Option(mName, mID));
    }
    if (node.childNodes.length == 1) {
        select.options.remove(0);
        SetElemEnable(select, false);
    }
}

function SetSelOption2(select, node) {
    if (node == null) return;
    for (var i = 0; i < node.childNodes.length; i++) {
        var mName = GetAttributeValue(node.childNodes[i], "name");
        var mID = GetAttributeValue(node.childNodes[i], "key")
        select.add(new Option(mName, mID));
    } 
}

function SelectExamType() {
    var wm = new WebMethodProxy("../T01Site/SiteManager.aspx", "GetListIDAndName");
    wm.AddParam("strparam", "{\"status\":\"0\"}");
    wm.AddParam("pageIndex", "1");
    wm.AddParam("pageSize", "10000");
    wm.AddParam("sort", "5");
    wm.Call(function (xml) {
        var xmlDoc = CreateXmlFromString(xml);
        ShowMultiSelectWindow("请选择采集点", xmlDoc, $("txtsiteids").tag, function (text, value) {
            $j("#txtsiteids").val(text);
        }, 240);
    });
}