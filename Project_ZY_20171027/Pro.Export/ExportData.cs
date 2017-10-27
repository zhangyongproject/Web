using System;
using System.Collections.Generic;
using System.Text;

using Pro.Export.Module;

namespace Pro.Export
{
    public class ExportData
    {
        public static RetValue Export<T>(ExporyType et, T info)
        {
            RetValue retValue = new RetValue();
            switch (et)
            {
              
                default:
                    retValue.Msg = "没有找到匹配的导出类型。";
                    break;
            }
            return retValue;
        }
    }
    public enum ExporyType
    {
    }

    public struct Point
    {
        public int X;
        public int Y;
        /// <summary>
        /// 是否需格式化
        /// </summary>
        public bool bFmt;
    }
}
