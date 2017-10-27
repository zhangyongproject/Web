using System;
using System.Collections.Generic;
using System.Text;

namespace Pro.Export.Module
{
    public struct RetValue
    {
        public int Code;
        public string Msg;

        public RetValue(int Code, string Msg)
        {
            this.Code = Code;
            this.Msg = Msg;
        }
    }
}
