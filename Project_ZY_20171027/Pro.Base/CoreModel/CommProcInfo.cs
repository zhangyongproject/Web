using System;

using Pro.Common;

namespace Pro.CoreModel
{
    /// <summary>
    /// 存储过程接口相关信息
    /// </summary>
    public class CommProcInfo
    {
        #region 属性


        private int _ActType = -1;
        private string _StartDate = string.Empty;
        private string _EndDate = string.Empty;
        private int _StartNum = -1;
        private int _EndNum = -1;
        private int _StartRec = 0;
        private int _EndRec = 0;
        private string _OrderBy = string.Empty;
        private string _ParamFlag = string.Empty;
        private string _StrFlag = string.Empty;
        private int _NumFlag = -1;
        private int _OutCount = 0;

        /// <summary>
        /// 操作类型,一般用于存储过程中的actType参数
        /// </summary>
        public int ActType
        {
            set { _ActType = value; }
            get { return _ActType; }
        }

        /// <summary>
        /// 起始时间,一般用于查询时的时间区间 格式:yyyy-MM-dd[ hh24:mi:ss]
        /// </summary>
        public string StartDate
        {
            set { _StartDate = value; }
            get { return _StartDate; }
        }

        /// <summary>
        /// 结束时间,一般用于查询时的时间区间 格式:yyyy-MM-dd[ hh24:mi:ss]
        /// </summary>
        public string EndDate
        {
            set { _EndDate = value; }
            get { return _EndDate; }
        }

        /// <summary>
        /// 起始数值,一般用于查询时的数字区间
        /// </summary>
        public int StartNum
        {
            set { if (value >= 0) _StartNum = value; else throw new Exception("数字区间中的起始值不能小于零!"); }
            get { return _StartNum; }
        }

        /// <summary>
        /// 结束数值,一般用于查询时的数字区间
        /// </summary>
        public int EndNum
        {
            set { if (value >= _StartNum) _EndNum = value; else throw new Exception("数字区间中的结束值不能小于起始值!"); }
            get { return _EndNum; }
        }

        /// <summary>
        /// 起始记录,一般用户查询时的记录区间
        /// </summary>
        public int StartRec
        {
            set { if (value >= 0) _StartRec = value; else throw new Exception("数据集记录中的起始值不能小于零!"); }
            get { return _StartRec; }
        }

        /// <summary>
        /// 结束记录,一般用户查询时的记录区间
        /// </summary>
        public int EndRec
        {
            set { if (value >= _StartRec) _EndRec = value; else throw new Exception("数据集记录中的结束值不能小于起始值!"); }
            get { return _EndRec; }
        }

        /// <summary>
        /// OrderBy条件,参数数据库中关于OrderBy的说明
        /// </summary>
        public string OrderBy
        {
            set { _OrderBy = value; }
            get { return _OrderBy; }
        }

        /// <summary>
        /// 参数有效性标识,用于申明参数集中哪些参数是有效的
        /// </summary>
        public string ParamFlag
        {
            set { _ParamFlag = value; }
            get { return _ParamFlag; }
        }

        /// <summary>
        /// 备用字符型标识,用于扩展
        /// </summary>
        public string StrFlag
        {
            set { _StrFlag = value; }
            get { return _StrFlag; }
        }

        /// <summary>
        /// 备用数字型标识,用于扩展
        /// </summary>
        public int NumFlag
        {
            set { _NumFlag = value; }
            get { return _NumFlag; }
        }

        /// <summary>
        /// 输入/输出参数out_count初始值(一般0=统计总数,-1代表不统计总数)
        /// </summary>
        public int OutCount
        {
            set { _OutCount = value; }
            get { return _OutCount; }
        }

        #endregion

        #region 备用属性


        //备用字段
        private int _Num1 = -1;
        private int _Num2 = -1;
        private int _Num3 = -1;
        private int _Num4 = -1;
        private string _Str1 = null;
        private string _Str2 = null;
        private string _Str3 = null;
        private string _Str4 = null;

        public int Num1
        {
            get { return _Num1; }
            set { _Num1 = value; }
        }
        public int Num2
        {
            get { return _Num2; }
            set { _Num2 = value; }
        }
        public int Num3
        {
            get { return _Num3; }
            set { _Num3 = value; }
        }
        public int Num4
        {
            get { return _Num4; }
            set { _Num4 = value; }
        }

        public string Str1
        {
            get { return _Str1; }
            set { _Str1 = value; }
        }
        public string Str2
        {
            get { return _Str2; }
            set { _Str2 = value; }
        }
        public string Str3
        {
            get { return _Str3; }
            set { _Str3 = value; }
        }
        public string Str4
        {
            get { return _Str4; }
            set { _Str4 = value; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 设置分页信息(设置开始与结束记录的索引)
        /// </summary>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前显示第几页</param>
        public void SetPagerInfo(int pageSize, int pageIndex)
        {
            this._StartRec = Tools.GetStartRec(ref pageSize, ref pageIndex);
            this._EndRec = Tools.GetEndRec(pageSize, pageIndex);
        }

        #endregion
    }
}
