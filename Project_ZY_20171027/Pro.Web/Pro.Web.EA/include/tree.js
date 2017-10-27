//----------------------------------------------------------------------------------------------------
//在指定容器内创建一棵树
//参数：容器，生成的树对象ID
//----------------------------------------------------------------------------------------------------
function CreateTree(DivID, defaultTVId) {
    var TreeId;
    if (defaultTVId == null) {
        //自动生成ID
        var index = 0;
        TreeId = "_TreeView_" + index;
        while (document.getElementById(TreeId) != null) {
            index++;
            TreeId = "_Tree_" + index;
        }
    } else {
        TreeId = defaultTVId;
    }

    var constTreePath = "../include/";
    var TempString = "<?XML:NAMESPACE PREFIX=TVNS />";
    TempString += "<?IMPORT NAMESPACE=TVNS IMPLEMENTATION=\"" + constTreePath + "treeview.htc\" />";
    TempString += "<tvns:treeview id=\"" + TreeId + "\""
    TempString += " systemImagesPath=\"" + constTreePath + "treeimages/\""
    TempString += " onexpand=\"javascript: if (this.clickedNodeIndex != null) this.queueEvent('onexpand', this.clickedNodeIndex)\""
    TempString += " oncollapse=\"javascript: if (this.clickedNodeIndex != null) this.queueEvent('oncollapse', this.clickedNodeIndex)\""
    TempString += " oncheck=\"javascript: if (this.clickedNodeIndex != null) this.queueEvent('oncheck', this.clickedNodeIndex)\""
    TempString += " onselectedindexchange=\"javascript: if (event.oldTreeNodeIndex != event.newTreeNodeIndex) this.queueEvent('onselectedindexchange', event.oldTreeNodeIndex + ',' + event.newTreeNodeIndex)\""
    TempString += " style=\"width:100%;height:99%;overflow:auto;padding-top: 5px;padding-left: 5px;padding-right: 5px;margin-bottom: 5px;\">"
    TempString += "</tvns:treeview>";
    document.getElementById(DivID).innerHTML = TempString;
    var ObjTree = document.getElementById(TreeId);
    return ObjTree;
}

//----------------------------------------------------------------------------------------------------
//将Xml绑定到树，并将XML结点属性完全附加到树点上
//参数：XML文件、容器
//----------------------------------------------------------------------------------------------------
function BindTree(xmlDoc,TheDIV)
{
   //创建导航树
   var xmlRoot=xmlDoc.selectSingleNode("root");     //这一句可根据需要调整
   var objTree=CreateTree(TheDIV,"test");           //创建树
   var tvRoot=BindTreeNode(objTree,objTree,xmlRoot);//绑定，得到树的根结点
}

function BindTreeNode(mTree,mTreeNode,XmlNode)
{
    if(XmlNode.attributes==null)return;
    
	var newNode=mTree.createTreeNode();

	//结点名称，对于tree对象，Text、ImageUrl、Expanded、checkBox、checked（true/false）属性对显示内容有影响
	newNode.setAttribute("Text",XmlNode.getAttribute("Value"));
	newNode.setAttribute("ImageUrl","images/status/"+XmlNode.nodeName+".gif");//图标
    //newNode.setAttribute("checkBox","1");
    //newNode.setAttribute("checked",true);
 
    //获取属性
    for(var i=0;i<XmlNode.attributes.length;i++)
    {
       newNode.setAttribute(XmlNode.attributes[i].name,XmlNode.attributes[i].value);
    }         
	newNode.setAttribute("CurNodeType",XmlNode.nodeName.toLowerCase());
    
	//生成下级结点
	for(var i=0;i<XmlNode.childNodes.length;i++)
	{
	    BindTreeNode(mTree,newNode,XmlNode.childNodes[i]);
	}

    mTreeNode.add(newNode);
    if((XmlNode.nodeName.toLowerCase()=="server")||(XmlNode.nodeName.toLowerCase()=="site"))
	    newNode.setAttribute("Expanded",true);
	return newNode;
}

function GetNodeOffset(mTree,mTreeNode,offset){
    var mLevelList=mTreeNode.getNodeIndex().split("."); //.length;
    var mLastIndex=parseInt(mLevelList[mLevelList.length-1]);
    if(mLastIndex+offset <0) return null;
    mIndex="";
    for(var i=0;i<mLevelList.length-1;i++)
      mIndex+=mLevelList[i]+".";
    mIndex+= (mLastIndex+offset);
    return mTree.getTreeNode(mIndex);
}

function MoveNodeOffset(mTree,mTreeNode,offset){
    var pNode=mTreeNode.getParent();
    if(pNode==null) return;
    var mLevelList=mTreeNode.getNodeIndex().split("."); //.length;
    var mLastIndex=parseInt(mLevelList[mLevelList.length-1]);
    mTreeNode.remove();
    pNode.addAt(mLastIndex+offset ,mTreeNode );
    mTree.selectedNodeIndex=mTreeNode.getNodeIndex();
}