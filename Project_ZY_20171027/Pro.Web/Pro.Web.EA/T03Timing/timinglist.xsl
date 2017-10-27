<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="no"/>
  <xsl:template match="/xml">
    <xsl:variable name="DivObjName" select="@DivObjName"/>
    <div>
      <xsl:attribute name="id">
        <xsl:value-of select="$DivObjName"/>
      </xsl:attribute>
      <table class="gd">
        <xsl:attribute name="id">table_timinglist</xsl:attribute>
        <tr class="gd_hdtr" >
          <!--<th style="width:50px">
            <input id="chk_all" type="checkbox" name="chk_all" onclick="SelectChecks('chk_item',this.checked);"></input>
          </th>-->
          <th style="">
            用户名
          </th>
          <th style="">
            设备名称
          </th>
          <th style="">
            包名称
          </th>
          <th style="width:60px">
            状态
          </th>
          <th style="width:250px">
            有效期限
          </th>          
          <th style="width:250px">
            运行时间
          </th>            
          <th style="">
            备注
          </th>
          <th style="">
            操作
          </th>
        </tr>
        <!--<xsl:apply-templates/>-->
        <xsl:choose>
          <xsl:when test="items/item">
            <xsl:for-each select="items/item">
              <tr>
                <xsl:choose>
                  <xsl:when test="position() mod 2=1">
                    <xsl:attribute name="class">gd_item</xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="class">gd_alt</xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>
                <!--<td align="center">
                  <input type="checkbox" name="chk_item"><xsl:attribute name="value"><xsl:value-of select="TSRID"/></xsl:attribute>
                  </input>
                </td>-->
                <td align="center">
                  <xsl:value-of select="USERNAME"/>                 
                </td>
                <td align="center">
                  <xsl:value-of select="EINAME"/>
                </td>
                <td align="center">
                  <xsl:value-of select="PACKNAME"/>
                </td>
                <td align="center" id="tdRelease">
                  <xsl:choose>
                    <xsl:when test="RELEASE =0">
                      <xsl:attribute name="style">color:lime</xsl:attribute>发布</xsl:when>
                    <xsl:when test="RELEASE =1">
                      <xsl:attribute name="style">color:red</xsl:attribute>未发布</xsl:when>
                    <xsl:otherwise></xsl:otherwise>
                </xsl:choose>
                </td>
                <td align="center">
                 <xsl:value-of select="substring-before(concat(EXPSTARTDATE/.,' '),' ')"/>～<xsl:value-of select="substring-before(concat(EXPENDDATE/.,' '),' ')"/>
                </td> 
                <td align="center">
                 <xsl:value-of select="substring-after(concat(STARTDATE/.,' '),' ')"/>～<xsl:value-of select="substring-after(concat(ENDDATE/.,' '),' ')"/>
                </td> 
                <td align="center">
                  <xsl:value-of select="DESCRIPTION"/>
                </td>
                <td align="center">                  
                  <a id="aRelease"><xsl:attribute name="onclick">Release('<xsl:value-of select="TSRID"/>','<xsl:value-of select="RELEASE"/>')</xsl:attribute>
                    <xsl:choose>
                      <xsl:when test="RELEASE =0">取消发布</xsl:when>
                      <xsl:when test="RELEASE =1">发布</xsl:when>
                      <xsl:otherwise></xsl:otherwise>
                    </xsl:choose>
                  </a>
                  <a style="margin-left:5px"><xsl:attribute name="onclick">Edit('<xsl:value-of select="TSRID"/>')</xsl:attribute>修改</a>
                  <a style="margin-left:5px"><xsl:attribute name="onclick">Del('<xsl:value-of select="TSRID"/>')</xsl:attribute>删除</a>                  
                </td>
              </tr>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <tr>
              <td>
                <!--表格的列数量 需根据显示的列数量设置-->
                <xsl:attribute name="colSpan">8</xsl:attribute>
                <div style="height:200px;line-height:200px; text-align:center;">无数据记录</div>
              </td>
            </tr>
          </xsl:otherwise>
        </xsl:choose>
      </table>
    </div>
  </xsl:template>
</xsl:stylesheet>

