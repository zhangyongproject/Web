using System;
using System.Collections;
using System.Data;

namespace Pro.Common
{
    public interface ITable
    {
        string GetTableName();
        string GetFieldList();
        string GetPrimayField(ref Type type);
        Hashtable GetFields();
    }

    public interface IStoredProc
    {
        string GetProcName();
        string GetCursorFieldList();
        Hashtable GetFields();
    }

    public delegate void OnMessageEvent(string strMsg);

    public interface IDBHelper
    {
        T Insert<T>(ITable objTable, bool CreateNewId); 
        T Update<T>(ITable objTable);                   
        T Delete<T>(ITable objTable, T id);             
        bool ExecSQL(string sqlstr);                    
        bool CallProc(IStoredProc objProc);

        OnMessageEvent OnMessage { get; set; }
        void WriteLogInfo(string strMsg);
        //string TrimChineseStr(string theValue, int Length);
        //string value2string(object obj);

        DataTable GetDtBySql(string sqlstr);
        DataTable GetDtBySql(string sqlstr, int StartRecord, int MaxRecord);
        string GetFieldValue(string sqlstr);
    }

}
