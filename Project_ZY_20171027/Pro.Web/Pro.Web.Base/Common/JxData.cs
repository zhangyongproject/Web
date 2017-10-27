using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Pro.Common;

namespace Pro.Web.Common
{
    /// <summary>
    /// XmlData信息类
    /// </summary>
    [Serializable]
    public class JxData
    {
        #region 私有尾性
        string root = "<root>{0}</root>";
        string pager = "<dataInfo><pageIndex>{0}</pageIndex><pageSize>{1}</pageSize><recordCount>{2}</recordCount><orderStr>{3}</orderStr></dataInfo>";
        StringBuilder sb = new StringBuilder();
        #endregion

        #region 属性
        /// <summary>
        /// AjaxDataXml
        /// </summary>
        public string DataXml
        {
            get { return String.Format(root, sb.ToString()); }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加数据(DataTable)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        public void AddData(DataTable dt, string dataName)
        {
            AddData(dt, dataName, -1, -1, -1);
        }
        
        /// <summary>
        /// 添加数据(DataTable),分页信息用于XmlGrid
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        public void AddData(DataTable dt, string dataName, int pageIndex, int pageSize, int recordCount)
        {
            AddData(dt, dataName, pageIndex, pageSize, recordCount, "");
        }

        /// <summary>
        /// 添加数据(DataTable),分页信息用于XmlGrid
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="orderStr"></param>
        public void AddData(DataTable dt, string dataName, int pageIndex, int pageSize, int recordCount, string orderStr)
        {
            DataSet ds = Tools.GetTableDs(dt, true);
            AddData(ds, dataName, pageIndex, pageSize, recordCount, orderStr);
        }

        /// <summary>
        /// 添加数据(DataSet)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        public void AddData(DataSet ds, string dataName)
        {
            AddData(ds, dataName, -1, -1, -1);
        }
        
        /// <summary>
        /// 添加数据(DataSet),分页信息用于XmlGrid
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        public void AddData(DataSet ds, string dataName, int pageIndex, int pageSize, int recordCount)
        {
            AddData(ds, dataName, pageIndex, pageSize, recordCount, "");
        }

        /// <summary>
        /// 添加数据(DataSet),分页信息用于XmlGrid
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="dataName">指定数据标识,默认为unknow</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="orderStr"></param>
        public void AddData(DataSet ds, string dataName, int pageIndex, int pageSize, int recordCount, string orderStr)
        {
            if (dataName == null || dataName.Length < 0)
                dataName = "unknow";
            sb.Append("<" + dataName + ">");
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Columns.Count < 1)
            {
                sb.Append("<items/>");
            }
            else
            {
                ds.DataSetName = "items";
                ds.Tables[0].TableName = "item";

                sb.Append(String.Format(pager, pageIndex, pageSize, recordCount, orderStr));
                sb.Append(ds.GetXml());
            }
            sb.Append("</" + dataName + ">");
        }

        /// <summary>
        /// 添加数据(DataTable),不提供分页
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataName"></param>
        public void AddDataNoPage(DataTable dt, string dataName)
        {
            DataSet ds = Tools.GetTableDs(dt, true);
            AddDataNoPage(ds, dataName);
        }

        /// <summary>
        /// 添加数据(DataSet),不提供分页
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="dataName"></param>
        public void AddDataNoPage(DataSet ds, string dataName)
        {
            if (dataName == null || dataName.Length < 0)
                dataName = "unknow";
            sb.Append("<" + dataName + ">");
            if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Columns.Count < 1)
            {
                sb.Append("<items/>");
            }
            else
            {
                ds.DataSetName = "items";
                ds.Tables[0].TableName = "item";
                sb.Append(ds.GetXml());
            }
            sb.Append("</" + dataName + ">");
        }

        /// <summary>
        /// 添加XMLStr给JXData,格式自己控制
        /// </summary>
        /// <param name="xmlStr"></param>
        public void AddXmlStr(string xmlStr)
        {
            if(xmlStr != null)
                sb.Append(xmlStr);
        }
        #endregion
    
    }
}
