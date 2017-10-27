using System;
using System.Data;
using System.Data.OracleClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Pro.Base.Logs;

namespace Pro.Common
{
    /// <summary>
    /// Oracle命令执行参数
    /// </summary>
    public class OracleExecEventArgs : EventArgs
    {
        /// <summary>
        /// Oracle命令对象
        /// </summary>
        public OracleCommand Command
        {
            get;
            set;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cmd"></param>
        public OracleExecEventArgs(OracleCommand cmd)
        {
            Command = cmd;
        }
    }

    /// <summary>
    /// Oracle执行方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OracleExecHandler(object sender, EventArgs e);


    /// <summary>
    /// 事务参数
    /// </summary>
    public class OracleTranEventArgs : EventArgs
    {
        /// <summary>
        /// Oracle连接
        /// </summary>
        public OracleConnection Connection
        {
            get;
            set;
        }

        /// <summary>
        /// Oracle事务
        /// </summary>
        public OracleTransaction Transaction
        {
            get;
            set;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="conn">Oracle连接</param>
        /// <param name="tran">Oracle事务</param>
        public OracleTranEventArgs(OracleConnection conn, OracleTransaction tran)
        {
            this.Connection = conn;
            this.Transaction = tran;
        }
    }
    /// <summary>
    /// Oracle事务方法
    /// </summary>
    /// <param name="sender">调用者</param>
    /// <param name="e">参数</param>
    public delegate void OracleTranHandler(object sender, OracleTranEventArgs e);

    /// <summary>
    /// Oracle 命令执行助手类.类型即返回类型,目常只支持int,DataSet,DataTable,OracleDataReader,object.
    /// 要执行带参数的存储过程,使用Execute4SP,事务方式使用Execute4SPByTran
    /// 要执行无参数的存储过程,使用Execute4SP和Execute,事务方式使用Execute4SPByTran或ExecuteByTran.(包括没有输入参数的情况)
    /// 要执行SQL,使用Execute,事务方式使用ExecuteByTran
    /// </summary>
    public class OraHelper
    {
        #region 私有方法和构造器

        // 因为本类为密封类,所以构造函数为私有的

        public OraHelper() {
        }

        /// <summary>
        /// Oracle事务
        /// </summary>
        private OracleTransaction _Tran = null;

        /// <summary>
        /// 是否使用事务,避免第一次设置事务开始,执行事务时,事务为空.
        /// </summary>
        private bool _UsingTran = false;

        /// <summary>
        /// 是否正在使用事务
        /// </summary>
        public bool UsingTran
        {
            get
            {
                return _UsingTran;
            }
        }

        /// <summary>
        /// 当前连接,事务提交或回滚后连接为空；如果传入连接字符串或未打开的连接,执行完毕后,连接为空
        /// </summary>
        public OracleConnection Connection
        {
            get
            {
                return _conn;
            }
            //set
            //{
            //    _conn = value;
            //}
        }
        /// <summary>
        /// 当前数据连接
        /// </summary>
        private OracleConnection _conn = null;

        /// <summary>
        /// 是否要关闭连接
        /// </summary>
        private bool _mustColseConnection = false;

        ///// <summary>
        ///// 是否必须要关闭连接

        ///// </summary>
        //private  bool MustColseConnection
        //{
        //    get
        //    {
        //        return _mustColseConnection;
        //    }
        //    set
        //    {
        //        _mustColseConnection = value;
        //    }
        //}

        /// <summary>
        /// 将参数数组添加至命令对象
        /// 
        /// 如果参数为null,将自动设置为DBNull.Value
        ///
        /// </summary>
        /// <param name="command">要将参数组添加至命令对象</param>
        /// <param name="cmdParams">要添加的参数数组</param>
        protected virtual void AttachParameters(OracleCommand command, OracleParameter[] cmdParams)
        {
            bool flag4DBNULL = false;//什么情况下可以将参数值设置为DBNull.Value

            //参数检验

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (cmdParams == null)
            {
                return;
            }

            //添加参数至命令对象

            foreach (OracleParameter p in cmdParams)
            {
                if (p == null) continue;

                //如果参数为null,将自动设置为DBNull.Value
                flag4DBNULL = p.Value == null;

                if (flag4DBNULL == true)
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// 参数数组赋值:将数据行中的值赋给相近的参数名的参数,参数名需要包含于数据元素对应的列名中
        /// </summary>
        /// <param name="cmdParams">需要赋值的参数数组</param>
        /// <param name="dataRow">赋值来源数据行</param>
        private void AssignParameterValues(OracleParameter[] cmdParams, DataRow dataRow)
        {
            // 数据列集合比较参数名的开始位置,SQL server必须从1开始,因为前面有一个@符号
            int offset = 0;

            //参数索引
            int i = 0;

            //临时参数名

            string tmParam = string.Empty;


            if ((cmdParams == null) || (dataRow == null))
            {
                return;
            }

            // 设置参数值

            foreach (OracleParameter commandParameter in cmdParams)
            {
                // 检查赋值两个对象

                if (string.IsNullOrEmpty(commandParameter.ParameterName) || commandParameter.ParameterName.Length <= offset)
                {
                    throw new Exception(string.Format("请提供一个有效的参数名, 位置:第#{0}个；参数名为: '{1}'.", i, commandParameter.ParameterName));
                }

                //设置临时参数名

                tmParam = commandParameter.ParameterName.Substring(offset);

                if (dataRow.Table.Columns.IndexOf(tmParam) != -1)
                {
                    commandParameter.Value = dataRow[tmParam];
                }
                i++;
            }
        }

        /// <summary>
        /// 参数数组赋值:对象数组赋给参数数组,次序对应
        /// </summary>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="paramValues">对象数组</param>
        private void AssignParameterValues(OracleParameter[] cmdParams, object[] paramValues)
        {
            //参数检验

            if ((cmdParams == null) || (paramValues == null))
            {
                return;
            }

            // 赋值双方的元素量必须一致

            if (cmdParams.Length != paramValues.Length)
            {
                throw new ArgumentException("参数数组与对象数组的大小不匹配.");
            }

            // 对象数组赋给参数数组,次序对应
            for (int i = 0, j = cmdParams.Length; i < j; i++)
            {
                // 如果对象值为空

                if (paramValues[i] == null)
                {
                    cmdParams[i].Value = DBNull.Value;
                    continue;
                }

                // 如果对象值为数据参数
                if (paramValues[i] is IDbDataParameter)
                {
                    // If the current array value derives from IDbDataParameter, then assign its Value property

                    IDbDataParameter paramInstance = (IDbDataParameter)paramValues[i];
                    if (paramInstance.Value == null)
                    {
                        cmdParams[i].Value = DBNull.Value;
                    }
                    else
                    {
                        cmdParams[i].Value = paramInstance.Value;
                    }
                    continue;
                }

                // 对象值为其它类型
                cmdParams[i].Value = paramValues[i];
            }
        }

        /// <summary>
        /// 对命令对象进行设置连接、事务,类型等,连接未打开将会自动打开
        /// </summary>
        /// <param name="command">需要设置的命令对象</param>
        /// <param name="connection">连接对象</param>
        /// <param name="transaction">事务对象,有效则使用事务,无效则不使用事务</param>
        /// <param name="commandType">命令的类型</param>
        /// <param name="commandText">存储过程名或SQL</param>
        /// <param name="cmdParams">命令参数</param>
        /// <param name="mustCloseConnection"><c>true</c> 是否要关闭连接</param>
        private void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] cmdParams, out bool mustCloseConnection)
        {
            // 检验参数

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException("commandText");
            }

            // 连接未打开将会自动打开,mustCloseConnection设置为true,调用者必须要关闭连接
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // 保存当前连接对象
            _conn = connection;

            // 以下为将命令对象与其它对象关联起来

            // 关联连接
            command.Connection = connection;

            // 设置SQL命令或存储名
            command.CommandText = commandText;

            // 设置命令与事务关联

            PreStartTran();

            // 设置命令类型
            command.CommandType = commandType;

            // 设置命令参数
            if (cmdParams != null)
            {
                AttachParameters(command, cmdParams);
            }
            return;
        }

        #endregion 私有方法和构造器

        #region 事件

        /// <summary>
        /// 执行命令前
        /// </summary>
        public event OracleExecHandler Executing;

        /// <summary>
        /// 执行命令后
        /// </summary>
        public event OracleExecHandler Executied;

        /// <summary>
        /// 开始事务前
        /// </summary>
        public event OracleTranHandler BeforeBeginTran;

        /// <summary>
        /// 开始事务后
        /// </summary>
        public event OracleTranHandler AfterBeginTran;

        /// <summary>
        /// 提交事务前
        /// </summary>
        public event OracleTranHandler BeforeCommit;

        /// <summary>
        /// 提交事务后
        /// </summary>
        public event OracleTranHandler AfterCommit;

        /// <summary>
        /// 回滚事务前

        /// </summary>
        public event OracleTranHandler BeforeRollback;

        /// <summary>
        /// 回滚事务后
        /// </summary>
        public event OracleTranHandler AfterRollback;

        #endregion

        #region 事务

        /// <summary>
        /// 开始事务根据连接串,事务完成后将会关闭连接
        /// </summary>
        /// <param name="connstr">连接串</param>
        /// <returns></returns>
        public void BeginTran(string connstr)
        {
            // 检验参数

            if (string.IsNullOrEmpty(connstr) == true)
            {
                throw new Exception("oracle connection is not open");
            }

            // 调用重载函数
            BeginTran(new OracleConnection(connstr));
        }

        /// <summary>
        /// 开始事务根据连接,如果该连接没有被打开,将自动打开 
        /// </summary>
        /// <param name="connection">数据连接</param>        
        public void BeginTran(OracleConnection connection)
        {
            // 调用开始事务后事件
            if (BeforeBeginTran != null)
            {
                BeforeBeginTran(this, new OracleTranEventArgs(_conn, _Tran));
            }

            // 检验参数

            if (connection == null)
            {
                throw new ArgumentException("connection is null");
            }

            // 打开连接
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // 保存连接
            _conn = connection;

            // 设置事务标志
            _UsingTran = true;

            // 调用开始事务后事件
            if (AfterBeginTran != null)
            {
                AfterBeginTran(this, new OracleTranEventArgs(_conn, _Tran));
            }
        }

        /// <summary>
        /// 协调命令与事务关联,因为几乎每一个命令都是自动生成的,必须在准备方法中处理
        /// </summary>
        private void PreStartTran()
        {
            // 未使用BeginTran,忽略使用事务
            if (_UsingTran == false)
            {
                return;
            }

            // 事务进行中,不可以重新开始

            if (_Tran != null)
            {
                return;
            }

            //开启事务

            _Tran = _conn.BeginTransaction();
        }

        /// <summary>
        /// 结束事务
        /// </summary>
        /// <returns></returns>
        public void Commit()
        {
            // 调用提交事务前事件

            if (BeforeCommit != null)
            {
                BeforeCommit(this, new OracleTranEventArgs(_conn, _Tran));
            }

            // 是否存在事务
            if (_UsingTran == false)
            {
                throw new Exception("事务未运行!");
            }

            // 如果事务中没有任何存储过程或SQL执行,直接关闭连接
            if (_Tran == null)
            {
                _conn.Close();
                _UsingTran = false;
                _conn = null;
                return;
            }

            // 提交事务
            _Tran.Commit();

            // 关闭连接
            _conn.Close();

            // 设置事务标志
            _UsingTran = false;

            // 连接和事务置空

            _conn = null;
            _Tran = null;

            // 调用提交事务后事件

            if (AfterCommit != null)
            {
                AfterCommit(this, new OracleTranEventArgs(_conn, _Tran));
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public void Rollback()
        {
            // 调用回滚事务前事件

            if (BeforeRollback != null)
            {
                BeforeRollback(this, new OracleTranEventArgs(_conn, _Tran));
            }

            // 是否存在事务
            if (_UsingTran == false)
            {
                throw new Exception("事务未运行!");
            }

            // 如果事务中没有任何存储过程或SQL执行,直接关闭连接
            if (_Tran == null)
            {
                _conn.Close();
                _UsingTran = false;
                _conn = null;
                return;
            }

            // 提交事务
            _Tran.Rollback();

            // 关闭连接
            _conn.Close();

            // 设置事务标志
            _UsingTran = false;

            // 连接和事务置空

            _conn = null;
            _Tran = null;

            // 调用回滚事务后事件

            if (AfterRollback != null)
            {
                AfterRollback(this, new OracleTranEventArgs(_conn, _Tran));
            }
        }
        #endregion

        #region Execute

        #region My custom

        #region 事务
        /// <summary>
        /// 执行存储过程,事务方式
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <returns></returns>
        public T Execute4SPByTran<T>(string spName, Dictionary<string, object> paramNameValues)
        {
            // 声明一个替代者

            OracleParameterCollection tmpParams;

            // 调用重载函数
            return Execute4SP<T>(_conn, spName, paramNameValues, out tmpParams);
        }

        /// <summary>
        /// 执行存储过程,事务方式
        /// </summary>        
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <param name="returnParameters">返回执行参数集合</param>
        /// <returns></returns>
        public T Execute4SPByTran<T>(string spName, Dictionary<string, object> paramNameValues, out OracleParameterCollection returnParameters)
        {
            // 调用重载函数
            return Execute4SP<T>(_conn, spName, paramNameValues, out returnParameters);
        }
        #endregion

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <returns></returns>
        public T Execute4SP<T>(string connectionString, string spName, Dictionary<string, object> paramNameValues)
        {
            // 调用重载函数
            return Execute4SP<T>(new OracleConnection(connectionString), spName, paramNameValues);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <returns></returns>
        public T Execute4SP<T>(OracleConnection connection, string spName, Dictionary<string, object> paramNameValues)
        {
            // 声明一个替代者

            OracleParameterCollection tmpParams;

            // 调用重载函数
            return Execute4SP<T>(connection, spName, paramNameValues, out tmpParams);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <param name="returnParameters">返回执行参数集合</param>
        /// <returns></returns>
        public T Execute4SP<T>(string connectionString, string spName, Dictionary<string, object> paramNameValues, out OracleParameterCollection returnParameters)
        {
            // 调用重载函数
            return Execute4SP<T>(new OracleConnection(connectionString), spName, paramNameValues, out returnParameters);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <param name="returnParameters">返回执行参数集合</param>
        /// <returns></returns>
        public T Execute4SP<T>(OracleConnection connection, string spName, Dictionary<string, object> paramNameValues, out OracleParameterCollection returnParameters)
        {
            // 0:检查参数

            if (connection == null)
            {
                throw new Exception("Oracle连接为空");
            }
            if (string.IsNullOrEmpty(spName) == true)
            {
                throw new ArgumentException("param spName is null or empty");
            }

            T retVal = default(T);//返回值


            // 得到该存储过程的参数列表,没有对参数传值,也需要查找存储过程参数列表

            // 该集合中的元素即为Command中的参数
            OracleParameter[] tmpParams = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

            // 1:无参数或者采用默认参数的处理
            if (paramNameValues == null || paramNameValues.Count < 1)
            {
                // 调用重载函数                
                retVal = Execute<T>(connection, CommandType.StoredProcedure, spName, null);

                // 输出执行参数集合
                returnParameters = new OracleParameterCollection();
                returnParameters.AddRange(tmpParams);

                // 返回值

                return retVal;
            }

            // 2:有参数值传入的处理

            // 2.1 支持对参数名大小写忽略

            // 增加一个替代者 为参数列表

            Dictionary<string, object> nameValues = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in paramNameValues)
            {
                nameValues.Add(item.Key.Trim().ToUpper(), item.Value);
            }

            // 2.2用替代者给参数集合赋值,根据参数名与键名关联
            foreach (OracleParameter item in tmpParams)
            {
                string paraName = item.ParameterName.Trim().ToUpper();

                if (nameValues.ContainsKey(paraName) == false)
                {
                    continue;// 没有完全匹配,继续
                }

                // 给命令参数赋值,对null和string.Empty转换为DBNull.Value
                object tmpValue = nameValues[paraName];
                item.Value = tmpValue ?? DBNull.Value;

                if (tmpValue is string)
                {
                    if (string.IsNullOrEmpty((string)tmpValue))
                    {
                        item.Value = DBNull.Value;
                    }
                    //各个页面自行过滤 by qiubaosheng 2011-01-24
                    //else
                    //{
                    //    item.Value = ((string)tmpValue).Replace("'",string.Empty).Trim();
                    //}
                }
            }

            // 2.2 善后处理,移除所有替代者的所有元素,避免冲突
            nameValues.Clear();
            nameValues = null;

            // 2.3 调用重载函数
            retVal = Execute<T>(connection, CommandType.StoredProcedure, spName, tmpParams);

            // 2.4 输出命令执行后的参数列表
            returnParameters = new OracleParameterCollection();
            returnParameters.AddRange(tmpParams);

            // 返回值

            return retVal;
        }
        #endregion

        #region Stored Procedure

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <returns></returns>
        internal T Execute<T>(OracleConnection connection, string spName, OracleParameter[] paramNameValues)
        {
            // 调用重载函数
            return Execute<T>(connection, CommandType.StoredProcedure, spName, paramNameValues);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="paramNameValues">传入的参数值对</param>
        /// <returns></returns>
        internal T Execute<T>(string connectionString, string spName, OracleParameter[] paramNameValues)
        {
            // 调用重载函数
            return Execute<T>(new OracleConnection(connectionString), CommandType.StoredProcedure, spName, paramNameValues);
        }
        #endregion Stored Procedure

        #region base
        /// <summary>
        /// 执行命令或存储过程

        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <returns></returns>
        public virtual T Execute<T>(OracleCommand cmd)
        {
            object retval = default(T);// 返回值

            Type type = typeof(T);// 返回值类型

            try
            {
                // 关联事务,若为空,则不使用事务操作
                cmd.Transaction = _Tran;
                // 根据返回值类型来判断调用哪个方法
                if (type == typeof(int))
                {
                    // 只返回影响行数
                    retval = cmd.ExecuteNonQuery();
                    // retval = cmd.ExecuteOracleNonQuery(
                }
                else if (type == typeof(DataSet))
                {
                    // 返回DataSet
                    DataSet ds = new DataSet();

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                    retval = ds;
                }
                else if (type == typeof(DataTable))
                {
                    // 返回DataTable
                    DataTable dt = new DataTable();

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    retval = dt;
                }
                else if (type == typeof(OracleDataReader))
                {
                    // 返回OracleDataReader
                    retval = cmd.ExecuteReader();
                }
                else if (type == typeof(object))
                {
                    // 返回第一个值

                    retval = cmd.ExecuteScalar();
                }
                else
                {
                    LogMessageHelper.LogERROR(string.Format("The type({0}) is not supported", type));
                    throw new Exception(string.Format("The type({0}) is not supported", type));
                }

                // 清除命令
                cmd.Parameters.Clear();
            }
            catch (OracleException ex)
            {
                //记录数据库执行错误日志
                StringBuilder parsdb =  new StringBuilder();
                parsdb.Append("{");
                foreach(OracleParameter par in cmd.Parameters)
                {
                    parsdb.AppendFormat("{0}:{1}",par.ParameterName,par.Value);
                }
                parsdb.Append("}");
                 
                Dictionary<string,string> pars = new Dictionary<string,string>();
                pars.Add("command",cmd.CommandText);
                pars.Add("type",cmd.CommandType.ToString());
                pars.Add("pars",parsdb.ToString());
                LogMessageHelper.LogERROR("数据库执行错误", pars, null, ex);

                //重新抛出异常
                throw ex;
            }
            finally
            {
                // 是否要关闭连接
                if (type != typeof(OracleDataReader))
                {
                    if (_mustColseConnection == true)
                    {
                        _conn.Close();

                        _conn = null;
                    }
                }
            }

            // 返回值

            return (T)retval;
        }

        /// <summary>
        /// 执行命令或存储过程
        /// </summary>
        /// <param name="connection">数据连接</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns> 
        internal T Execute<T>(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] cmdParams)
        {
            // 检查是否存在事务

            if (connection != _conn && _UsingTran == true)
            {
                throw new ArgumentException("事务进行中,不允许更改连接");
            }

            OracleCommand cmd = new OracleCommand();

            // 准备命令对象
            PrepareCommand(cmd, connection, null, commandType, commandText, cmdParams, out _mustColseConnection);

            // 调用重载函数
            return Execute<T>(cmd);
        }

        /// <summary>
        /// 执行命令或存储过程
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">存储过程名或SQL</param>
        /// <param name="cmdParams">参数名值对</param>
        /// <returns></returns> 
        public T Execute<T>(string connectionString, CommandType commandType, string commandText, params OracleParameter[] cmdParams)
        {
            // 调用重载函数
            return Execute<T>(new OracleConnection(connectionString), commandType, commandText, cmdParams);
        }

        /// <summary>
        /// 执行无参数的存储过程或命令
        /// </summary>      
        /// <param name="connectionString">连接串</param>
        /// <param name="commandType">类型</param>
        /// <param name="commandText">存储名或SQL语句</param>
        /// <returns></returns>
        public T Execute<T>(string connectionString, CommandType commandType, string commandText)
        {
            // 调用重载函数
            return Execute<T>(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// 执行命令或存储过程,使用事务.
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">存储过程名或SQL</param>
        /// <returns></returns> 
        public T ExecuteByTran<T>(CommandType commandType, string commandText)
        {
            // 执行
            return Execute<T>(_conn, commandType, commandText, (OracleParameter[])null);
        }
        /// <summary>
        /// 执行命令或存储过程,使用事务.
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">存储过程名或SQL</param>
        /// <param name="cmdParams">参数集合</param>
        /// <returns></returns> 
        public T ExecuteByTran<T>(CommandType commandType, string commandText, OracleParameter[] cmdParams)
        {
            return Execute<T>(_conn, commandType, commandText, cmdParams);
        }

        #endregion base

        #endregion Execute
    }

    /// <summary>
    /// OracleHelperParameterCache 提供存储过程参数缓存功能,以及根据存储过程名得到其参数列表
    /// 
    /// </summary>
    public sealed class OracleHelperParameterCache
    {
        #region 私有方法,变量及构造器

        //静态构造函数

        private OracleHelperParameterCache() { }

        /// <summary>
        /// 缓存,缓存名为 连接串 : 存储过程名或Sql:include ReturnValue Parameter,其中:include ReturnValue Parameter为包含返回参数才有的
        /// </summary>
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        ///// <summary>
        ///// 是否要强制关闭连接
        ///// </summary>
        //private static bool mustCloseConnection = false; 

        /// <summary>
        /// 返回一个存储过程的参数数组
        /// </summary>
        /// <param name="connection">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回找到的参数数组.</returns>
        private static OracleParameter[] DiscoverSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter)
        {
            // 检验参数

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("spName");
            }

            // 定义一个模拟命令

            OracleCommand cmd = new OracleCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // 打开连接
            connection.Open();

            // 查找存储过程的参数

            OracleCommandBuilder.DeriveParameters(cmd);
            //// 是否要关闭连接

            //if (mustCloseConnection == true)
            //{
            //    connection.Close();
            //}

            connection.Close();

            // 如果包含返回参数,则删除第一个参数,该情况存在于SQL SERVER中

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            // 返回找到的参数的副本
            OracleParameter[] discoveredParameters = new OracleParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // 参数一刀两断,没有关系了.
            cmd.Parameters.Clear();

            // 初始化各参数为DBNull
            foreach (OracleParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// 深拷贝参数数组
        /// </summary>
        /// <param name="originalParameters">源参数数组</param>
        /// <returns>返回参数数组的副本</returns>
        private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            OracleParameter[] clonedParameters = new OracleParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion 私有方法,变量及构造器

        #region 缓存

        /// <summary>
        /// 将参数保存至缓存,手动缓存,用处不大
        /// </summary>
        /// <param name="connectionString">有效的数据连接</param>
        /// <param name="commandText">存储过程或SQL</param>
        /// <param name="cmdParams">An array of SqlParamters to be cached</param>
        internal static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] cmdParams)
        {
            // 检验参数

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException("commandText");
            }

            //保存至缓存

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = cmdParams;
        }

        /// <summary>
        /// 从缓存中得到参数数组的克隆,手动缓存,用处不大
        /// </summary>
        /// <param name="connectionString">一个有效的参数数组</param>
        /// <param name="commandText">存储过程名或SQL</param>
        /// <returns>返回参数数组</returns>
        internal static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            // 检验参数

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException("commandText");
            }

            // 如果缓存中存在,则深拷贝一个参数数组返回

            string hashKey = connectionString + ":" + commandText;

            OracleParameter[] cachedParameters = paramCache[hashKey] as OracleParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void ClearCache()
        {
            paramCache.Clear();
        }

        /// <summary>
        /// 清除指定的缓存,如果commandText为空串,则清除所有 该连接串的所有缓存

        /// </summary>
        /// <param name="commandText">命令或存储过程</param>
        /// <param name="connectionString">连接串</param>
        public static void ClearCache(string connectionString, string commandText)
        {
            // 检验参数

            if (string.IsNullOrEmpty(connectionString) == false)
            {
                throw new ArgumentException(string.Format(" connectionString param ('{0}') is null or empty", connectionString));
            }
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }


            // 缓存键

            string hashKey = connectionString + ":" + commandText;
            // 需要清除的缓存键

            List<string> tmpKeys = new List<string>();

            // 记录需要消除的缓存
            foreach (string key in paramCache.Keys)
            {
                if (key.Contains(hashKey) == false)
                {
                    continue;
                }

                tmpKeys.Add(key);
            }

            // 清除所有记录的缓存
            foreach (string key in tmpKeys)
            {
                paramCache.Remove(key);
            }
        }


        #endregion 缓存

        #region 查找存储过程参数

        /// <summary>
        /// 得到存储过程对应的参数数组,自动缓存
        /// </summary>
        /// <remarks>
        /// 查找存储过程的参数数组,并且将该参数数组保存至缓存中
        /// </remarks>
        /// <param name="connectionString">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>参数数组</returns>
        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            // 是否包含返回值

            // 在SQL SERVER中查找存储过程的参数数组,第一个参数值为返回值,无用,所以置为false,将其丢弃
            // 在ORACLE中查找存储过程的参数数组时,第一个参数值是有效的,所以置为true
            bool includeReturnValueParameter = true;

            return GetSpParameterSet(connectionString, spName, includeReturnValueParameter);
        }

        /// <summary>
        /// 得到存储过程对应的参数数组,自动缓存
        /// </summary>
        /// <remarks>
        /// 查找存储过程的参数数组,并且将该参数数组保存至缓存中
        /// </remarks>
        /// <param name="connectionString">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>参数数组</returns>
        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            // 检验参数

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }

            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("spName");
            }

            //调用内部方法,返回参数数组
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// 得到存储过程对应的参数数组,自动缓存
        /// </summary>
        /// <remarks>
        /// 查找存储过程的参数数组,并且将该参数数组保存至缓存中
        /// </remarks>
        /// <param name="connection">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>参数数组</returns>
        internal static OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, true);
        }

        /// <summary>
        /// 得到存储过程对应的参数数组,自动缓存
        /// </summary>
        /// <remarks>
        /// 查找存储过程的参数数组,并且将该参数数组保存至缓存中
        /// </remarks>
        /// <param name="connection">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>参数数组</returns>
        internal static OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter)
        {
            // 检验参数

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            //
            using (OracleConnection clonedConnection = (OracleConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// 得到存储过程对应的参数数组,自动缓存
        /// </summary>
        /// <param name="connection">有效的数据连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>参数数组</returns>
        private static OracleParameter[] GetSpParameterSetInternal(OracleConnection connection, string spName, bool includeReturnValueParameter)
        {
            // 检验参数

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("spName");
            }

            ////执行完毕后要关闭连接
            //mustCloseConnection = ( connection.State != ConnectionState.Open );


            // 缓存规则
            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            // 从缓存中取参数数组根据缓存规则

            // 如果找不到,则从数据库中查找,并将找到的参数数组保存至缓存            
            OracleParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as OracleParameter[];
            if (cachedParameters == null)
            {
                OracleParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            // 深拷贝一个参数数组返回,不管是缓存中,还是从数据库返回的

            return CloneParameters(cachedParameters);
        }

        #endregion 查找存储过程参数
    }
}
