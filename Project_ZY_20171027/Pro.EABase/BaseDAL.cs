using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Pro.Common;
using System.Data.SQLite;
using Pro.Common;


namespace Pro.EABase
{
    /// <summary>
    /// 基础数据访问类
    /// </summary>
    public class BaseDAL
    {
        public string DBConnectionString = MyConfig.GetWebConfig("SQLiteConn");

        public BaseDAL()
        {
            if (string.IsNullOrEmpty(DBConnectionString) == false && string.IsNullOrEmpty(SQLiteHelper.ConnectionString))
            {
                SQLiteHelper.ConnectionString = DBConnectionString;
            }
        }

        public BaseDAL(string dbConnectionString)
            : this()
        {
            DBConnectionString = dbConnectionString;
        }
    }

    /// <summary>
    /// SQLite助手类
    /// </summary>
    public class SQLiteHelper
    {
        /// <summary>
        /// ConnectionString样例：Data Source=Test.db3;Pooling=true;FailIfMissing=false
        /// </summary>
        public static string ConnectionString { get; set; }
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, params object[] p)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (p != null)
            {
                foreach (object parm in p)
                    cmd.Parameters.AddWithValue(string.Empty, parm);
            }
        }

        /// <summary>
        /// 执行SQL语句返回DataSet对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    DataSet ds = new DataSet();
                    PrepareCommand(command, conn, cmdText, p);
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);

                    da.Fill(ds);
                    return ds;
                }
            }
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, cmdText, p);
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行SQL语句返回Reader对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, cmdText, p);
                    return command.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
        }

        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, cmdText, p);
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 执行SQL语句返回DataTable对象
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static DataTable Query(string sqlStr)
        {
            return ExecuteQuery(sqlStr, null).Tables[0];
        }
    }
}
