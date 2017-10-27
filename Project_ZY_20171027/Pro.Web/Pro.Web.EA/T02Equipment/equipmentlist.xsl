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
        <xsl:attribute name="id">table_equipmentlist</xsl:attribute>
        <tr class="gd_hdtr" >
          <!--<th style="width:50px">
            <input id="chk_all" type="checkbox" name="chk_all" onclick="SelectChecks('chk_item',this.checked);"></input>
          </th>-->
          <th style="">
            设备名称
          </th>
          <th style="">
            IP地址列表
          </th>
          <th style="">
            激活码
          </th>
          <th style="width:150px;">
            创建时间
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
                  <input type="checkbox" name="chk_item"><xsl:attribute name="value"><xsl:value-of select="EIID"/></xsl:attribute>
                  </input>
                </td>-->
                <td align="center">
                  <xsl:value-of select="EINAME"/>                 
                </td>
                <td align="center">
                  <xsl:value-of select="IPLIST"/>
                </td>
                <td align="center">
                  <xsl:value-of select="ACCODE"/>
                </td>
                <td align="center">
                  <xsl:value-of select="CREATETIME"/>
                </td>
                <td align="center">
                  <xsl:value-of select="DESCRIPTION"/>
                </td>
                <td align="center">
                  <a><xsl:attribute name="onclick">EditEquipment('<xsl:value-of select="EIID"/>','<xsl:value-of select="EINAME"/>')</xsl:attribute>修改</a>
                  <a style="margin-left:5px"><xsl:attribute name="onclick">DelEquipment('<xsl:value-of select="EIID"/>')</xsl:attribute>删除</a>
                </td>
              </tr>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <tr>
              <td>
                <!--表格的列数量 需根据显示的列数量设置-->
                <xsl:attribute name="colSpan">5</xsl:attribute>
                <div style="height:200px;line-height:200px; text-align:center;">无数据记录</div>
              </td>
            </tr>
          </xsl:otherwise>
        </xsl:choose>
      </table>
    </div>
  </xsl:template>
</xsl:stylesheet>

