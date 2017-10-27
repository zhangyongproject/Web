using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Pro.Base.Common
{
    public class ExcelHelper
    {
        /// <summary>
        /// 将Excel文件导入到DataTable
        /// </summary>
        /// <param name="xlsFileName">源Excel文件名</param>
        /// <param name="xlsSheetName">Excel工作表名</param>
        /// <param name="headers">Excel中的列名</param>
        /// <param name="dtSchema">目标DataTable的结构</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportExcel(string xlsFileName, string xlsSheetName, string[] headers, DataTable dtSchema)
        {
            if (headers.Length != 0 && headers.Length != dtSchema.Columns.Count)
            {
                throw new ApplicationException("Excel中的列数与DataTable中的列数不匹配");
            }

            string connString = "Provider = Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + xlsFileName;

            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            if (headers.Length == 0) // 取全部列
            {
                sb.Append("*,");
            }
            else // 取指定的列
            {
                for (int i = 0; i < dtSchema.Columns.Count; i++)
                {
                    sb.AppendFormat("[{0}] as [{1}], ", headers[i], dtSchema.Columns[i].ColumnName);
                }
            }
            sb.Remove(sb.Length - 2, 2);
            if (xlsSheetName == "") // 没有表名,默认取第一个Sheet
            {
                xlsSheetName = "[Sheet1$]";
            }
            sb.AppendFormat(" from [{0}]", xlsSheetName);


            OleDbConnection conn = new OleDbConnection(connString);

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandText = sb.ToString();
            cmd.Connection = conn;

            OleDbDataAdapter da = new OleDbDataAdapter();
            da.SelectCommand = cmd;

            conn.Open();

            DataSet ds = new DataSet();
            DataTable destTable;

            try
            {
                da.Fill(ds);

                if (dtSchema != null) // 有格式化的Table,则向Table复制数据返回
                {
                    destTable = dtSchema.Clone();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow srcDr = ds.Tables[0].Rows[i];
                        DataRow destDr = destTable.NewRow();
                        if (srcDr[0].ToString().Length > 0 && srcDr[1].ToString().Length > 0)
                        {
                            for (int j = 0; j < destTable.Columns.Count; j++)
                            {
                                destDr[j] = srcDr[j];
                            }
                            destTable.Rows.Add(destDr);
                        }
                    }
                }
                else // 如果没有格式化的Table,则直接返回
                {
                    destTable = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return destTable;
        }

    }
}
