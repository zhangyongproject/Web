using System;
using System.Collections.Generic;
using System.Text;

namespace Pro.Base.CoreModel
{
    /// <summary>
    /// 权限待级描述实体
    /// </summary>
    [Serializable]
    public class RightLevelDepict
    {
        private string _flag = string.Empty;

        public string Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        private List<RightLevelValue> _list = new List<RightLevelValue>();
        public List<RightLevelValue> ValueList
        {
            get { return _list; }
        }


        //gradeflag, gradevalue, gradename, gradedesc
    }



    /// <summary>
    /// 权限等级值描述
    /// </summary>
    public class RightLevelValue
    {
        public RightLevelValue()
        { }

        public RightLevelValue(int _value, string name, string desc)
        {
            _levelName = name;
            _levelValue = _value;
            _levelDesc = desc;
        }
        private int _levelValue = 0;
        /// <summary>
        /// 等级值
        /// </summary>
        public int LevelValue
        {
            get { return _levelValue; }
            set { _levelValue = value; }
        }

        private string _levelName = string.Empty;
        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName
        {
            get { return _levelName; }
            set { _levelName = value; }
        }

        private string _levelDesc = string.Empty;
        /// <summary>
        /// 等级描述
        /// </summary>
        public string LevelDesc
        {
            get { return _levelDesc; }
            set { _levelDesc = value; }
        }


 
    }
    /// <summary>
    /// RightLevelValue类排序定义
    /// </summary>
    public class RightLevelValueComparer : IComparer<RightLevelValue>
    {
        #region IComparer<RightLevelValue> 成员

        public int Compare(RightLevelValue x, RightLevelValue y)
        {
            return  x.LevelValue.CompareTo(y.LevelValue);
        }

        #endregion
    }
}
