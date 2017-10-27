using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using System.Data;
using System.Collections;
using log4net.Core;
using System.IO;
using System.Globalization;
using log4net.Util;

namespace Pro.Base.Logs
{
    public class DataBaseAppender : AdoNetAppender
    {
        // Fields 
        private string m_commandText;
        private CommandType m_commandType = CommandType.Text;
        private string m_connectionString;
        private string m_connectionType = "System.Data.OleDb.OleDbConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private IDbCommand m_dbCommand;
        private IDbConnection m_dbConnection;
        //protected ArrayList m_parameters = new ArrayList();
        private bool m_reconnectOnError = false;
        private SecurityContext m_securityContext;
        //protected bool m_usePreparedCommand;
        private bool m_useTransactions = true;

        // Methods 
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            this.m_usePreparedCommand = (this.m_commandText != null) && (this.m_commandText.Length > 0);
            if (this.m_securityContext == null)
            {
                this.m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }
            this.InitializeDatabaseConnection();
            this.InitializeDatabaseCommand();
        }

        public void AddParameter(AdoNetAppenderParameter parameter)
        {
            this.m_parameters.Add(parameter);
        }

        protected virtual string GetLogStatement(LoggingEvent logEvent)
        {
            if (this.Layout == null)
            {
                this.ErrorHandler.Error("ADOAppender: No Layout specified.");
                return "";
            }
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            this.Layout.Format(writer, logEvent);
            return writer.ToString();
        }

        private void InitializeDatabaseCommand()
        {
            if ((this.m_dbConnection != null) && this.m_usePreparedCommand)
            {
                Exception exception2;
                try
                {
                    if (this.m_dbCommand != null)
                    {
                        try
                        {
                            this.m_dbCommand.Dispose();
                        }
                        catch (Exception exception)
                        {
                            LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
                        }
                        this.m_dbCommand = null;
                    }
                    this.m_dbCommand = this.m_dbConnection.CreateCommand();
                    this.m_dbCommand.CommandText = this.m_commandText;
                    this.m_dbCommand.CommandType = this.m_commandType;
                }
                catch (Exception exception3)
                {
                    exception2 = exception3;
                    this.ErrorHandler.Error("Could not create database command [" + this.m_commandText + "]", exception2);
                    if (this.m_dbCommand != null)
                    {
                        try
                        {
                            this.m_dbCommand.Dispose();
                        }
                        catch
                        {
                        }
                        this.m_dbCommand = null;
                    }
                }
                if (this.m_dbCommand != null)
                {
                    try
                    {
                        foreach (AdoNetAppenderParameter parameter in this.m_parameters)
                        {
                            try
                            {
                                parameter.Prepare(this.m_dbCommand);
                            }
                            catch (Exception exception4)
                            {
                                exception2 = exception4;
                                this.ErrorHandler.Error("Could not add database command parameter [" + parameter.ParameterName + "]", exception2);
                                throw;
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            this.m_dbCommand.Dispose();
                        }
                        catch
                        {
                        }
                        this.m_dbCommand = null;
                    }
                }
                if (this.m_dbCommand != null)
                {
                    try
                    {
                        this.m_dbCommand.Prepare();
                    }
                    catch (Exception exception5)
                    {
                        exception2 = exception5;
                        this.ErrorHandler.Error("Could not prepare database command [" + this.m_commandText + "]", exception2);
                        try
                        {
                            this.m_dbCommand.Dispose();
                        }
                        catch
                        {
                        }
                        this.m_dbCommand = null;
                    }
                }
            }
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                Exception exception;
                if (this.m_dbCommand != null)
                {
                    try
                    {
                        this.m_dbCommand.Dispose();
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
                    }
                    this.m_dbCommand = null;
                }
                if (this.m_dbConnection != null)
                {
                    try
                    {
                        this.m_dbConnection.Close();
                    }
                    catch (Exception exception3)
                    {
                        exception = exception3;
                        LogLog.Warn("AdoNetAppender: Exception while disposing cached connection object", exception);
                    }
                    this.m_dbConnection = null;
                }
                this.m_dbConnection = (IDbConnection)Activator.CreateInstance(this.ResolveConnectionType());
                this.m_dbConnection.ConnectionString = this.m_connectionString;
                using (this.SecurityContext.Impersonate(this))
                {
                    this.m_dbConnection.Open();
                }
            }
            catch (Exception exception2)
            {
                this.ErrorHandler.Error("Could not open database connection [" + this.m_connectionString + "]", exception2);
                this.m_dbConnection = null;
            }
        }

        protected override void OnClose()
        {
            Exception exception;
            base.OnClose();
            if (this.m_dbCommand != null)
            {
                try
                {
                    this.m_dbCommand.Dispose();
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
                }
                this.m_dbCommand = null;
            }
            if (this.m_dbConnection != null)
            {
                try
                {
                    this.m_dbConnection.Close();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    LogLog.Warn("AdoNetAppender: Exception while disposing cached connection object", exception);
                }
                this.m_dbConnection = null;
            }
        }

        protected virtual Type ResolveConnectionType()
        {
            Type type;
            try
            {
                type = SystemInfo.GetTypeFromString(this.m_connectionType, true, false);
            }
            catch (Exception exception)
            {
                this.ErrorHandler.Error("Failed to load connection type [" + this.m_connectionType + "]", exception);
                throw;
            }
            return type;
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            if ((this.m_dbConnection != null) && (this.m_dbConnection.State != ConnectionState.Open))
            {
                m_reconnectOnError = true;
            }
            if (this.m_reconnectOnError && ((this.m_dbConnection == null) || (this.m_dbConnection.State != ConnectionState.Open)))
            {
                LogLog.Debug("AdoNetAppender: Attempting to reconnect to database. Current Connection State: " + ((this.m_dbConnection == null) ? "<null>" : this.m_dbConnection.State.ToString()));
                this.InitializeDatabaseConnection();
                this.InitializeDatabaseCommand();
            }
            if ((this.m_dbConnection != null) && (this.m_dbConnection.State == ConnectionState.Open))
            {
                if (this.m_useTransactions)
                {
                    IDbTransaction dbTran = null;
                    try
                    {
                        dbTran = this.m_dbConnection.BeginTransaction();
                        this.SendBuffer(dbTran, events);
                        dbTran.Commit();
                    }
                    catch (Exception exception)
                    {
                        if (dbTran != null)
                        {
                            try
                            {
                                dbTran.Rollback();
                            }
                            catch (Exception)
                            {
                            }
                        }
                        this.ErrorHandler.Error("Exception while writing to database", exception);
                    }
                }
                else
                {
                    this.SendBuffer(null, events);
                }
            }
        }

        protected virtual void SendBuffer(IDbTransaction dbTran, LoggingEvent[] events)
        {
            if (this.m_usePreparedCommand)
            {
                if (this.m_dbCommand != null)
                {
                    if (dbTran != null)
                    {
                        this.m_dbCommand.Transaction = dbTran;
                    }
                    foreach (LoggingEvent event2 in events)
                    {
                        foreach (AdoNetAppenderParameter parameter in this.m_parameters)
                        {
                            parameter.FormatValue(this.m_dbCommand, event2);
                        }
                        this.m_dbCommand.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (IDbCommand command = this.m_dbConnection.CreateCommand())
                {
                    if (dbTran != null)
                    {
                        command.Transaction = dbTran;
                    }
                    foreach (LoggingEvent event2 in events)
                    {
                        string logStatement = this.GetLogStatement(event2);
                        LogLog.Debug("AdoNetAppender: LogStatement [" + logStatement + "]");
                        command.CommandText = logStatement;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Properties 
        public string CommandText
        {
            get
            {
                return this.m_commandText;
            }
            set
            {
                this.m_commandText = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return this.m_commandType;
            }
            set
            {
                this.m_commandType = value;
            }
        }

        protected IDbConnection Connection
        {
            get
            {
                return this.m_dbConnection;
            }
            set
            {
                this.m_dbConnection = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.m_connectionString;
            }
            set
            {
                this.m_connectionString = value;
            }
        }

        public string ConnectionType
        {
            get
            {
                return this.m_connectionType;
            }
            set
            {
                this.m_connectionType = value;
            }
        }

        public bool ReconnectOnError
        {
            get
            {
                return this.m_reconnectOnError;
            }
            set
            {
                this.m_reconnectOnError = value;
            }
        }

        public SecurityContext SecurityContext
        {
            get
            {
                return this.m_securityContext;
            }
            set
            {
                this.m_securityContext = value;
            }
        }

        public bool UseTransactions
        {
            get
            {
                return this.m_useTransactions;
            }
            set
            {
                this.m_useTransactions = value;
            }
        }

    }
}
