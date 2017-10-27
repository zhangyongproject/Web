using System;
using System.Collections.Generic;
using System.Text;

using log4net.Layout.Pattern;
using log4net.Layout;
using log4net.Core;

namespace Pro.Base.Logs
{
    /// <summary>
    /// 自定义布局，对自定义日志信息支持
    /// </summary>
    public class ProLayout : PatternLayout
    {
        public ProLayout()
        {
            this.AddConverter("property", typeof(ProMessagePatternConverter));
        }
    }
}
