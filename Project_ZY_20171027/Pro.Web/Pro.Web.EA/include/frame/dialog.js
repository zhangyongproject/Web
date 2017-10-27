function SetAutoClose(defaultcontrol){
  var ctrl=$(defaultcontrol);
  if(ctrl!=null) ctrl.focus();

  //某些情况下，按ESC关闭窗口不是一个好现象，手工开启此功能
  document.onkeyup = function() {
    if (event.keyCode == 27)
        if (window.parent.__UnMaskBG) window.parent.__UnMaskBG();
  }
}

document.oncontextmenu = function() {
    return false;
}

function GetInitParam() {
    try{
        obj = window.parent.document.getElementById("floater_alert");
        return obj.initParam;
    }catch (e) {
        return null;
    }
}

function ResizeDlg(mWidth, mHeight) {
    if (!mWidth) mWidth = 400;
    if (!mHeight) mHeight = 300;
    var pDocument=window.parent.document;
    objDiv = pDocument.getElementById("floater_alert");
    if (!objDiv || !objDiv.frame) return;
    objDiv.frame.style.width = mWidth;
    objDiv.frame.style.height = mHeight;

    var mLeft = (pDocument.documentElement.clientWidth - objDiv.offsetWidth) / 2 + pDocument.documentElement.scrollLeft;
    objDiv.style.left = (mLeft > 0 ? mLeft : 0) + "px";
    var mTop = (pDocument.documentElement.clientHeight - objDiv.offsetHeight) / 2 + pDocument.documentElement.scrollTop;
    objDiv.style.top = (mTop > 0 ? mTop : 0) + "px";
}

function callback(retCode, retValue) {
    if (!window.parent.__UnMaskBG) return;
    window.parent.__UnMaskBG();
//    obj = window.parent.document.getElementById("floater_alert");
//    var onPageCallback = obj.pageCallback;
//    if (onPageCallback != null)
//        onPageCallback(retCode, retValue);
    callbackA(retCode, retValue);
}

//不关闭当前窗口的情况下，调用背景页面函数
function callbackA(retCode, retValue) {
    obj = window.parent.document.getElementById("floater_alert");
    var onPageCallback = obj.pageCallback;
    if (onPageCallback != null)
        onPageCallback(retCode, retValue);
}