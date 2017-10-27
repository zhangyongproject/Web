self.onError=null;
currentX = currentY = 0;
whichIt = null;
lastScrollX = 0; lastScrollY = 0;
FloatFlag=true;

function MoveMessageBox() {
    if(!FloatFlag) return;
    var theMessageFloater=document.all.floater;
    if(theMessageFloater==null) return;
    if(theMessageFloater.style.visibility=="hidden")return;
    
    var diffY = document.documentElement.scrollTop; 
    var diffX = document.documentElement.scrollLeft; 
    if(diffY != lastScrollY) { 
        percent = .1 * (diffY - lastScrollY);
        if(percent > 0)
            percent = Math.ceil(percent);
        else
            percent = Math.floor(percent);
        theMessageFloater.style.pixelTop += percent;
        lastScrollY = lastScrollY + percent;
    }
    if(diffX != lastScrollX) {
        percent = .1 * (diffX - lastScrollX);
        if(percent > 0)
            percent = Math.ceil(percent);
        else
            percent = Math.floor(percent);
        theMessageFloater.style.pixelLeft += percent;
        lastScrollX = lastScrollX + percent;
    }
}

var mLastFloater=null; //上次抓取的floater
function grabIt(e) {
    var event = e || window.event;
    whichIt = event.srcElement;
	if(whichIt==null) {return true;}
    
    var isFloaterHead=false;
    while (whichIt.id.indexOf("floater") == -1) {
        if(whichIt.id.indexOf("floathead")>-1) isFloaterHead=true;
	    whichIt = whichIt.parentElement;
	    if (whichIt == null) { return true; }
    }    
    
    BlurFloater();
    FocusFloter(whichIt);

    //如果不是点击头部，不作移动处理
    if(!isFloaterHead){
        whichIt=null;
        return true;
    }
    
    whichIt.style.pixelLeft = whichIt.offsetLeft;
    whichIt.style.pixelTop = whichIt.offsetTop;
    currentX = (event.clientX + document.body.scrollLeft);
    currentY = (event.clientY + document.body.scrollTop);
    return true;
}

function BlurFloater()
{
    if(mLastFloater==null)return;
    mLastFloater.style.zIndex=mLastFloater.style.zIndex-1;
    if(mLastFloater.onBlur!=null) //回调函数入口
        mLastFloater.onBlur(mLastFloater);      
    mLastFloater=null;
}

function FocusFloter(mWhichIt)
{
    mLastFloater=mWhichIt;
    if(mWhichIt==null){whichIt = null; return;}
    mLastFloater.style.zIndex=mLastFloater.style.zIndex+1;
    if(mLastFloater.onFocus!=null)//回调函数入口
        mLastFloater.onFocus(mLastFloater);
}

function moveIt(e) {
    var event = e || window.event;
    if (whichIt == null) { return true; }
    //if(event.button!=1) { dropIt();return true;} //此句不兼容IE11
    try{
	    newX = (event.clientX + document.body.scrollLeft);
	    newY = (event.clientY + document.body.scrollTop);
	    distanceX = (newX - currentX);    
	    distanceY = (newY - currentY);
	    currentX = newX;    
	    currentY = newY;
	    whichIt.style.pixelLeft += distanceX;
	    whichIt.style.pixelTop += distanceY;
		
        var theParent=document.documentElement ;
	    if (whichIt.style.pixelTop < theParent.scrollTop) whichIt.style.pixelTop = theParent.scrollTop;
	    if (whichIt.style.pixelTop > theParent.offsetHeight + theParent.scrollTop - whichIt.offsetHeight)
	        whichIt.style.pixelTop = theParent.offsetHeight + theParent.scrollTop - whichIt.offsetHeight;

	    if (whichIt.style.pixelLeft < theParent.scrollLeft) whichIt.style.pixelLeft = theParent.scrollLeft;
	    if (whichIt.style.pixelLeft > theParent.offsetWidth + theParent.scrollLeft - whichIt.offsetWidth)
	        whichIt.style.pixelLeft = theParent.offsetWidth + theParent.scrollLeft - whichIt.offsetWidth;

	    event.returnValue = false;  
		
	    if(whichIt.onMove!=null)//回调函数入口
            whichIt.onMove(whichIt,currentX,currentY);
	    return true;   
    }catch(e){
	    return false;
    }
}

function dropIt(e) {
    var event = e || window.event;
	whichIt = null;
	return true;
}

function AutoMoveMessageBox(){
    window.setInterval("MoveMessageBox()",1);
}

document.onmousedown = grabIt;    
document.onmousemove = moveIt;   
document.onmouseup = dropIt;          
