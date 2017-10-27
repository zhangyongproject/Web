using System;
using System.Collections.Generic;
using System.Xml;
using System.Data;

namespace Pro.Common
{
    public class XmlHelper
    {
        public XmlHelper()
        {
        }

        #region ����XML�ĵ�

        /// <summary>
        /// ����XML�ĵ�
        /// </summary>
        /// <returns></returns>
        public static XmlDocument CreateXml()
        {
            return CreateXml("xml");
        }

        /// <summary>
        /// ����XML�ĵ�
        /// </summary>
        /// <param name="RootNodeText">���ڵ���</param>
        /// <returns></returns>
        public static XmlDocument CreateXml(string RootNodeText)
        {
            return CreateXml(RootNodeText, "", "");
        }

        /// <summary>
        /// ����XML�ĵ�
        /// </summary>
        /// <param name="RootNodeText">���ڵ���</param>
        /// <param name="AttribName">���ڵ�������</param>
        /// <param name="AttribValue">���ڵ�����ֵ</param>
        /// <returns></returns>
        public static XmlDocument CreateXml(string RootNodeText, string AttribName, object AttribValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode XmlRoot = xmlDoc.CreateElement(RootNodeText);
            xmlDoc.AppendChild(XmlRoot);

            if (!AttribName.Equals(""))
            {
                AddAttribute(XmlRoot, AttribName, AttribValue.ToString());
            }

            return xmlDoc;
        }

        /// <summary>
        /// װ��XML�ļ�
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static XmlDocument LoadXml(string xmlStr)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlStr);
                return xmlDoc;
            }
            catch
            {
                //MyLog.WriteLogInfo("LoadXml����" + xmlStr);
                return null;
            }
        }

        #endregion

        #region ����Xml��㡢����

        /// <summary>
        /// ��ӽڵ�
        /// </summary>
        /// <param name="pNode"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public static XmlNode AddXmlNode(XmlNode pNode, string NodeName)
        {
            return AddXmlNode(pNode, NodeName, null);
        }

        public static XmlNode AddXmlNode(XmlNode pNode, string NodeName, object NodeValue)
        {
            return AddXmlNode(pNode, NodeName, NodeValue, false);
        }

        public static XmlNode AddXmlNode(XmlNode pNode, string NodeName, object NodeValue, bool ReplaceIfExist)
        {
            XmlNode xmlNode = pNode.SelectSingleNode(NodeName);
            bool mNeedCreateNewNode = ((!ReplaceIfExist) || (xmlNode == null));
            if (mNeedCreateNewNode)
                xmlNode = pNode.OwnerDocument.CreateElement(NodeName);
            if (NodeValue != null)
                xmlNode.InnerText = NodeValue.ToString();
            if (mNeedCreateNewNode)
                pNode.AppendChild(xmlNode);
            return xmlNode;
        }

        public static XmlAttribute AddAttribute(XmlNode pNode, string AttribName, object AttribValue)
        {
            if (pNode.Attributes[AttribName] == null)
            {
                XmlAttribute attrNode = pNode.OwnerDocument.CreateAttribute(AttribName);
                attrNode.Value = AttribValue.ToString();
                pNode.Attributes.Append(attrNode);
                return attrNode;
            }
            else
            {
                pNode.Attributes[AttribName].Value = AttribValue.ToString();
                return pNode.Attributes[AttribName];
            }
        }

        public static bool RemoveAttribute(XmlNode pNode, string AttribName)
        {
            if (pNode.Attributes[AttribName] != null)
            {
                pNode.Attributes.Remove(pNode.Attributes[AttribName]);
                return true;
            }
            else
                return false;
        }

        public static string GetAttributeValue(XmlNode pNode, string AttribName)
        {
            if (pNode.Attributes[AttribName] != null)
                return pNode.Attributes[AttribName].Value;
            else
                return "";
        }

        #endregion

        #region ͨ��Table�Զ�����xml

        public static string GetXmlFromTable(DataTable theTable, string theNameSpace)
        {
            return GetXmlFromTable(theTable, theNameSpace, false);
        }

        public static string GetXmlFromTable(DataTable theTable, string theNameSpace, bool CanBeNull)
        {
            string aNameSpace = theNameSpace.Trim();
            if (aNameSpace.Equals("")) aNameSpace = "DataSet";

            bool needtrans = false;
            foreach (DataColumn mc in theTable.Columns)
            {
                if (mc.Namespace != "")
                {
                    needtrans = true;
                    break;
                }
            }
            if (!needtrans && CanBeNull)
            {
                DataSet tmpSet = new DataSet(aNameSpace);
                tmpSet.Merge(theTable);
                return tmpSet.GetXml();
            }
            else
            {
                string tmpString = "<" + aNameSpace + ">";
                tmpString += GetXmlFromTable(theTable);
                tmpString += "</" + aNameSpace + ">";
                return tmpString;
            }
        }

        public static string GetXmlFromTable(DataTable theTable)
        {
            string tmpString = "";
            string TName = theTable.TableName;
            if (TName.Equals("")) TName = "RecorderInfo";

            foreach (DataRow mr in theTable.Rows)
            {
                tmpString += "<" + TName + ">";
                foreach (DataColumn mc in theTable.Columns)
                {
                    tmpString += "<" + mc.ColumnName + ">";
                    if (mc.Namespace != "NOHTML")
                    {
                        tmpString += "<![CDATA[";
                    }
                    switch (mc.DataType.Name.ToLower())
                    {
                        case "datetime":
                            if (mc.Namespace == "DAY")
                            {
                                tmpString += Tools.GetDateTime(mr[mc.ColumnName].ToString(), DateTime.Now).ToString("yyyy-MM-dd");
                            }
                            else if (mc.Namespace == "TIME")
                            {
                                tmpString += Tools.GetDateTime(mr[mc.ColumnName].ToString(), DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                tmpString += mr[mc.ColumnName].ToString();
                            }
                            break;
                        case "string":
                            if (mc.Namespace == "STRTOHTML")
                            {
                                //tmpString += MyType.StrToHtml(mr[mc.ColumnName].ToString());
                            }
                            else
                            {
                                tmpString += mr[mc.ColumnName].ToString();
                            }
                            break;
                        case "1":
                            break;
                        default:
                            tmpString += mr[mc.ColumnName].ToString();
                            break;
                    }
                    if (mc.Namespace != "NOHTML")
                    {
                        tmpString += "]]>";
                    }
                    tmpString += "</" + mc.ColumnName + ">";
                }
                tmpString += "</" + TName + ">";
            }
            return tmpString;
        }

        #endregion
    }
}
