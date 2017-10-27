using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;


namespace Pro.Common
{

    /// <summary>
    /// 用OleDB访问Access数据库的基类
    /// </summary>
    public class AccessDb
    {
        private string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}";
        private string ConnectionString2 = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};;Persist Security Info=False;Jet OLEDB:Database Password={1}";


        public AccessDb(string mdbPath)
        {
            ConnectionString = string.Format(ConnectionString, mdbPath);
        }

        public AccessDb(string mdbPath, string pwd)
        {
            ConnectionString = string.Format(ConnectionString2, mdbPath, pwd);
        }

        public DataSet GetDataSet(string sql)
        {
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbCommand command = new OleDbCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            connection.Close();
            connection.Dispose();
            return ds;

        }

        public OleDbDataReader GetReader(string sql)
        {
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbCommand command = new OleDbCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public int ExecuteSql(string sql)
        {
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbCommand command = new OleDbCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            connection.Open();
            int ret = command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
            return ret;
        }

    }
}
