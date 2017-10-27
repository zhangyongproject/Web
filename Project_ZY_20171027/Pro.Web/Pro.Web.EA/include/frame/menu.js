function InitMenu() {
    var wm = new WebMethodProxy(document.location, "GetMenuInfo");
    wm.Call(OnInitMenu); 
}

var CanExchange = false;
var MenuVisible = true;
function ExchangeMenuArea() {
    if (!CanExchange) return;
    var slideSpeed = 100;
    if (MenuVisible) {
        $j("#divMainMenu").slideUp(slideSpeed);
        $j("#divMoreInfo").attr("display", "block").slideDown(slideSpeed);
        $j("#divMainHead").attr("class", "MenuHead1");
    } else {
        $j("#divMoreInfo").slideUp(slideSpeed).attr("display", "none");
        $j("#divMainMenu").slideDown(slideSpeed);
        $j("#divMainHead").attr("class", "MenuHead");
    }
    MenuVisible = !MenuVisible;
}

function ShowExtArea() {
    CanExchange = true;
    MenuVisible = true;
    ExchangeMenuArea();
    $j("#divMainHead").css("cursor", "hand");
}

function ClearExtArea() {
    if (!CanExchange) return;
    if (!MenuVisible)
        ExchangeMenuArea();
    CanExchange = false;
    $j("#divMainHead").css("cursor", "default");
}

function SetPageUnitName(name) {
    if ($("div_PageUnitName") != null)
        $("div_PageUnitName").innerHTML = name;
}

var xmlMenu = null;
function OnInitMenu(xmlstr) {
    //alert(xmlstr);
    //return;

    xmlMenu = CreateXmlFromString(xmlstr);
    var rootNode = xmlMenu.selectSingleNode("xml");
    var idxMenu1 = GetAttributeValue(rootNode, "idxMenu1"); mLastMainMenuIdx = idxMenu1;
    var idxMenu2 = GetAttributeValue(rootNode, "idxMenu2");
    var idxMenu3 = GetAttributeValue(rootNode, "idxMenu3");
    var mName = GetAttributeValue(rootNode, "name");
    var mName1 = "";
    var mName2 = "";
    var userName = GetAttributeValue(rootNode, "username");
    var userCall = GetAttributeValue(rootNode, "usercall");
    $("span_username").innerHTML = userCall;    //userName;

    var strHtml = "";
    for (var i = 0; i < rootNode.childNodes.length; i++) { 
        var itemNode = rootNode.childNodes[i];
        var title = GetAttributeValue(itemNode, "title");
        var icon = GetAttributeValue(itemNode, "icon"); if (icon == "") icon = "icon0.png";

        var IsCurMainItem = (i == idxMenu1);
        if (IsCurMainItem) mName1 = title;
        strHtml += "<div class=MainMenuItem onclick=MainMenuClick(" + i + ")>";
        strHtml += "<table cellpadding=0 cellpadding=0 border=0 width=100% height=100%><tr>";
        //strHtml += "<td style='width:50px;text-align:right;'><img src='../images/frame/" + icon + "' /></td>";
        //strHtml += "<td style='font-weight:" + (IsCurMainItem ? "bold" : "normal") + "'>" + title + "</td>";
        strHtml += "<td style='padding-left:25px;'>" + title + "</td>";
        strHtml += "</tr></table></div>";

        strHtml += "<div class=MainMenuArea id=MainMenuArea" + i + " style='display:" + (IsCurMainItem ? "block" : "none") + "'>";
        for (var j = 0; j < itemNode.childNodes.length; j++) {
            var subItemNode = itemNode.childNodes[j];
            title = GetAttributeValue(subItemNode, "title");
            var IsCurNode = IsCurMainItem && (j == idxMenu2); //当前菜单项

            var divIdx = "div_" + i + "_" + j;
            if (!IsCurNode)
                strHtml += "<div class=SubMenuItem onclick=SubMenuClick(" + i + "," + j + ") idx='" + divIdx + "'>";
            else {
                strHtml += "<div class=SubMenuItemSelected idx='" + divIdx + "'>";
                mName2 = title;
            }

            icon = GetAttributeValue(subItemNode, "icon");
            iconidx = ((1000 + parseInt(icon) * 10) + "").substr(1); // 3->030

            var IconPath = "../images/frame/menuicon/";
            icon0 = IconPath + "icon" + iconidx + "1.png";
            icon1 = IconPath + "icon" + iconidx + "2.png";
            icon2 = IconPath + "icon" + iconidx + "3.png";

            strHtml += "<table cellpadding=0 cellpadding=0 border=0 width=100% height=100%><tr>";
            strHtml += "<td style='width:80px;text-align:right;vertical-align: middle'>";
            strHtml += "<img src=" + (IsCurNode ? icon2 : icon0) + " idx='" + divIdx + "' icon0='" + icon0 + "' icon1='" + icon1 + "'/>";
            strHtml += "</td>";
            strHtml += "<td style='padding-left :6px;'>" + title + "</td>";
            strHtml += "</tr></table></div>";
        }
        strHtml += "</div>";
    }

    $("divMainMenu").innerHTML = strHtml;

    strHtml = "";  //"<img src='../images/frame/nav_icon.jpg' />";
    strHtml += mName1 + " &gt; ";
    strHtml += "<a onclick=SubMenuClick(" + idxMenu1 + "," + idxMenu2 + ")>" + mName2 + "</a>";
    if (idxMenu3 > -1)
        strHtml += " &gt; <span id=div_PageUnitName>" + mName + "</span>";
    //strHtml += "<hr />";
    $("divNavigator").innerHTML = strHtml;

    //    $j(".MainMenuItem").mouseover(function() {
    //        $j(this).removeClass("MainMenuItem").addClass("MainMenuItemhover");
    //    }).mouseout(function() {
    //        $j(this).removeClass("MainMenuItemhover").addClass("MainMenuItem");
    //    });

    $j(".SubMenuItem").mouseover(function() {
        //$j(this).removeClass("SubMenuItem").addClass("SubMenuItemhover");
        var divIdx = ($j(this).attr("idx"));
        var img = $j("img[idx=" + divIdx + "]"); //.attr("src"));
        img.attr("src", img.attr("icon1"));
    }).mouseout(function() {
        //$j(this).removeClass("SubMenuItemhover").addClass("SubMenuItem");
        var divIdx = ($j(this).attr("idx"));
        var img = $j("img[idx=" + divIdx + "]"); //.attr("src"));
        img.attr("src", img.attr("icon0"));
    });
}

var mLastMainMenuIdx = -1;
function MainMenuClick(idx) {
    if (idx == mLastMainMenuIdx) return;
    $j("#MainMenuArea" + mLastMainMenuIdx).slideUp(200);
    mLastMainMenuIdx = idx;
    $j("#MainMenuArea" + mLastMainMenuIdx).slideDown(200);
}

function SubMenuClick(idx1, idx2) {
    if (xmlMenu == null) return;
    var rootNode = xmlMenu.selectSingleNode("xml");
    var itemNode = rootNode.childNodes[idx1].childNodes[idx2];
    var url = GetAttributeValue(itemNode, "url");
    if (url != "")
        window.open(url, "_self");
    else {
        var title = GetAttributeValue(itemNode, "title");
        var description = GetAttributeValue(itemNode, "description");
        ShowAlert("功能保留:" + title + "<hr />" + description);
    }
}
