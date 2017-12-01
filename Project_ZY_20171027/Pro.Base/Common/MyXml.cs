using System.Xml;
using System.Data;
using System;
using System.Collections.Generic;

namespace Pro.Common
{
    public class MyXml
    {

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
            XmlNode xmlRoot = xmlDoc.CreateElement(RootNodeText);
            xmlDoc.AppendChild(xmlRoot);

            if (!AttribName.Equals(""))
            {
                AddAttribute(xmlRoot, AttribName, AttribValue.ToString());
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
            return GetAttributeValue(pNode, AttribName, "");
        }

        public static string GetAttributeValue(XmlNode pNode, string AttribName, string defaultvalue)
        {
            if (pNode.Attributes[AttribName] != null)
                return pNode.Attributes[AttribName].Value;
            else
                return defaultvalue;
        }

        #endregion


        #region ����WebService���صı�׼xml

        public static XmlDocument CreateResultXml(int mCode, string strMessage, object mValue)
        {
            //<xml code = "0" msg = "�ɹ�" value="123" />
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlRoot = xmlDoc.CreateElement("xml");
            xmlDoc.AppendChild(xmlRoot);

            AddAttribute(xmlRoot, "code", mCode.ToString());
            AddAttribute(xmlRoot, "msg", strMessage);
            AddAttribute(xmlRoot, "value", mValue.ToString());

            return xmlDoc;
        }

        #region �������

        public static bool ParseResultOK(string strResultXml, ref int mCode, ref string strMessage, ref string strResult)
        {
            XmlDocument xmlDoc = LoadXml(strResultXml);
            if (xmlDoc == null) return false;

            XmlNode xmlRoot = xmlDoc.SelectSingleNode("xml");
            if (xmlRoot == null) return false;

            mCode = Tools.GetInt32(GetAttributeValue(xmlRoot, "code"), -1);
            strMessage = GetAttributeValue(xmlRoot, "msg");
            strResult = GetAttributeValue(xmlRoot, "value");
            return mCode == 0;
        }

        public static bool ParseResultOK(string strResultXml)
        {
            int mCode = 0;
            string strMessage = "";
            string strResult = "";
            return ParseResultOK(strResultXml, ref  mCode, ref  strMessage, ref  strResult);
        }

        public static string ParseResultValue(string strResultXml)
        {
            return ParseResultValue(strResultXml, string.Empty);
        }

        public static string ParseResultValue(string strResultXml, string strDefault)
        {
            int mCode = 0;
            string strMessage = "";
            string strResult = "";
            bool mResult = ParseResultOK(strResultXml, ref  mCode, ref  strMessage, ref  strResult);

            return mResult ? strResult : strDefault;
        }

        #endregion

        /// <summary>
        /// �����¼���Էֶ���ҳ��0����¼��1ҳ����
        /// </summary>
        /// <param name="pageSize">ÿҳ��¼��</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <returns>��ҳ��</returns>
        public static int CalcMaxPage(int pageSize, int recordCount)
        {
            if (pageSize <= 0)
                return 0;

            if (recordCount <= 0)
                return 1;

            return (recordCount - 1) / pageSize + 1;
        }

        public static XmlDocument CreateTabledResultXml(DataTable dt)
        {
            return CreateTabledResultXml(dt, 0, 0, 0);
        }

        public static XmlDocument CreateTabledResultXml(DataTable dt, int pageIndex, int pageSize, int recordCount)
        {
            XmlDocument xmlDoc = CreateResultXml(0, "", "");
            return CreateTabledResultXml(xmlDoc, dt, pageIndex, pageSize, recordCount);

        }

        public static XmlDocument CreateTimingTabledResultXml(DataTable dt, int pageIndex, int pageSize, int recordCount)
        {
            XmlDocument xmlDoc = CreateResultXml(0, "", "");
            return CreateTimingTabledResultXml(xmlDoc, dt, pageIndex, pageSize, recordCount);

        }

        public static XmlDocument CreateTimingTabledResultXml(XmlDocument xmlDoc, DataTable dt, int pageIndex, int pageSize, int recordCount)
        {

            // <xml code = "0" msg = "�ɹ�" value="123">
            //      <items pageindex="��ǰҳ" pagesize="ÿҳ��ʾ����" pagecount="��ҳ��" recordcount="�ܼ�¼��">         
            //            <item><id>1</id><name>����</name><age>18</age></item>
            //            <item><id>2</id><name>����</name><age>22</age></item>
            //            ...
            //      </items>
            //</xml>
            if (dt != null) //&& dt.Rows.Count > 0
            {
                XmlNode xmlRoot = xmlDoc.SelectSingleNode("xml");

                int pageCount = CalcMaxPage(pageSize, recordCount);

                XmlNode xmlItems = AddXmlNode(xmlRoot, "items");
                AddAttribute(xmlItems, "pageindex", (pageIndex >= 0) ? pageIndex : 0);
                AddAttribute(xmlItems, "pagesize", (pageSize > 0) ? pageSize : recordCount);
                AddAttribute(xmlItems, "pagecount", (pageCount > 0) ? pageCount : 1);
                AddAttribute(xmlItems, "recordcount", (recordCount > 0) ? recordCount : dt.Rows.Count);
                AddAttribute(xmlItems, "curdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    if (pageIndex != 0 && pageSize != 0)
                    {
                        if (i < startIndex)
                            continue;
                        if (i >= endIndex)
                            break;
                    }

                    DataRow dr = dt.Rows[i];
                    XmlNode xmlItem = AddXmlNode(xmlItems, "item");
                    //foreach (DataColumn dc in dt.Columns)
                    //{
                    //    if (dr[dc] is DateTime)
                    //    {
                    //        AddXmlNode(xmlItem, dc.ColumnName, ((DateTime)dr[dc]).ToString("yyyy-MM-dd HH:mm:ss").Replace("<", "&lt;").Replace(">", "&gt;"));
                    //        if (dc.ColumnName == "ENDDATE")
                    //            AddAttribute(xmlItem, "IsExpired", ((DateTime)dr[dc]) < DateTime.Now ? 1 : 0);
                    //    }
                    //    else
                    //        AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("<", "&lt;").Replace(">", "&gt;"));
                    //}

                    foreach (DataColumn dc in dt.Columns)
                        AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
                }
                return xmlDoc;
            }
            else
            {
                return CreateResultXml(-1000, "the datatable is null", "");
            }
        }

        /// <summary>
        /// DataTable ת���� XML
        /// </summary>
        /// <param name="xmlDoc">XmlDoc������</param>
        /// <param name="dt">���ݼ�</param>
        /// <param name="pageIndex">ҳ����</param>
        /// <param name="pageSize">ҳ��ʾ��¼��</param>
        /// <param name="recordCount">��¼����</param>
        /// <returns></returns>
        public static XmlDocument CreateTabledResultXml(XmlDocument xmlDoc, DataTable dt, int pageIndex, int pageSize, int recordCount)
        {

            // <xml code = "0" msg = "�ɹ�" value="123">
            //      <items pageindex="��ǰҳ" pagesize="ÿҳ��ʾ����" pagecount="��ҳ��" recordcount="�ܼ�¼��">         
            //            <item><id>1</id><name>����</name><age>18</age></item>
            //            <item><id>2</id><name>����</name><age>22</age></item>
            //            ...
            //      </items>
            //</xml>
            if (dt != null) //&& dt.Rows.Count > 0
            {
                XmlNode xmlRoot = xmlDoc.SelectSingleNode("xml");

                int pageCount = CalcMaxPage(pageSize, recordCount);

                XmlNode xmlItems = AddXmlNode(xmlRoot, "items");
                AddAttribute(xmlItems, "pageindex", (pageIndex >= 0) ? pageIndex : 0);
                AddAttribute(xmlItems, "pagesize", (pageSize > 0) ? pageSize : recordCount);
                AddAttribute(xmlItems, "pagecount", (pageCount > 0) ? pageCount : 1);
                AddAttribute(xmlItems, "recordcount", (recordCount > 0) ? recordCount : dt.Rows.Count);
                AddAttribute(xmlItems, "curdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                int startIndex = (pageIndex-1) * pageSize;
                int endIndex = pageIndex * pageSize;

                for (int i = 0; i < dt.Rows.Count; ++i )
                {
                    if (pageIndex != 0 && pageSize != 0)
                    {
                        if (i < startIndex)
                            continue;
                        if (i >= endIndex)
                            break;
                    }

                    DataRow dr = dt.Rows[i];
                    XmlNode xmlItem = AddXmlNode(xmlItems, "item");
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dr[dc] is DateTime)
                        {
                            AddXmlNode(xmlItem, dc.ColumnName, ((DateTime)dr[dc]).ToString("yyyy-MM-dd HH:mm:ss").Replace("<", "&lt;").Replace(">", "&gt;"));
                            if(dc.ColumnName == "ENDDATE")
                                AddAttribute(xmlItem, "IsExpired", ((DateTime)dr[dc]) < DateTime.Now?1:0);
                        }
                        else
                            AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                        
                    //AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
                }
                return xmlDoc;
            }
            else
            {
                return CreateResultXml(-1000, "the datatable is null", "");
            }
        }

        /// <summary>
        /// DataTable ת���� XML
        /// </summary>
        /// <param name="xmlDoc">XmlDoc������</param>
        /// <param name="dt">���ݼ�</param>
        /// <param name="pageIndex">ҳ����</param>
        /// <param name="pageSize">ҳ��ʾ��¼��</param>
        /// <param name="recordCount">��¼����</param>
        /// <returns></returns>
        public static XmlNode CreateTabledResultXml(XmlNode xmlNode, DataTable dt, int pageIndex, int pageSize, int recordCount)
        {

            return CreateTabledResultXml(xmlNode, "items", dt, pageIndex, pageSize, recordCount);
        }


        public static XmlNode CreateTabledResultXml(XmlNode xmlNode, string nodeName, DataTable dt, int pageIndex, int pageSize, int recordCount)
        {

            // <xml code = "0" msg = "�ɹ�" value="123">
            //      <items pageindex="��ǰҳ" pagesize="ÿҳ��ʾ����" pagecount="��ҳ��" recordcount="�ܼ�¼��">         
            //            <item><id>1</id><name>����</name><age>18</age></item>
            //            <item><id>2</id><name>����</name><age>22</age></item>
            //            ...
            //      </items>
            //</xml>
            if (dt != null) //&& dt.Rows.Count > 0
            {
                //XmlNode xmlRoot = xmlDoc.SelectSingleNode("xml");

                int pageCount = CalcMaxPage(pageSize, recordCount);

                XmlNode xmlItems = AddXmlNode(xmlNode, nodeName);
                AddAttribute(xmlItems, "pageindex", (pageIndex >= 0) ? pageIndex : 0);
                AddAttribute(xmlItems, "pagesize", (pageSize > 0) ? pageSize : recordCount);
                AddAttribute(xmlItems, "pagecount", (pageCount > 0) ? pageCount : 1);
                AddAttribute(xmlItems, "recordcount", (recordCount > 0) ? recordCount : dt.Rows.Count);

                foreach (DataRow dr in dt.Rows)
                {
                    XmlNode xmlItem = AddXmlNode(xmlItems, "item");
                    foreach (DataColumn dc in dt.Columns)
                        AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("<", "&lt;").Replace(">", "&gt;"));
                    //AddXmlNode(xmlItem, dc.ColumnName, dr[dc].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
                }
                return xmlNode;
            }
            else
            {
                return CreateResultXml(-1000, "the datatable is null", "");
            }
        }

        #endregion


    }
}
