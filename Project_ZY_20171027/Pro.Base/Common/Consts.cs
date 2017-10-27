

namespace Pro.Common
{
    public class Consts
    {
        
        #region 错误与异常
        /// <summary>
        /// 错误信息_执行存储过程时出现未知错误
        /// </summary>
        public const string ERR_DBUnKnow = "执行存储过程时出现未知错误。";

        /// <summary>
        /// 错误信息_返回对象无效
        /// </summary>
        public const string ERR_InvalidObj = "无效的对象。";

        /// <summary>
        /// 错误信息_参数错误
        /// </summary>
        public const string ERR_P = "参数错误。";

        /// <summary>
        /// 统一提示信息:出现异常，请与系统管理员联系。
        /// </summary>
        public const string EXP_Info = "出现异常，请与系统管理员联系。";
        #endregion

        #region 提示信息

        /// <summary>
        /// 频道为空:请输入监测频道(0～300 GHz).
        /// </summary>
        public const string Freq_Empty = "请输入监测频道(0～300 GHz)。";

        /// <summary>
        /// 频道错误:监测频道范围输入有误(0～300 GHz).
        /// </summary>
        public const string Freq_Right = "监测频道范围输入有误(0～300 GHz)。";

        /// <summary>
        /// 频道格式错误:监测频道输入格式有误(数字类型,0～300 GHz).
        /// </summary>
        public const string Freq_Format = "监测频道输入格式有误(数字类型,0～300 GHz)。";

        /// <summary>
        /// 频段未选:请选择监测频段.
        /// </summary>
        public const string Fs_Choose = "请选择监测频段。";

        /// <summary>
        /// 频段错误:监测频段中的结束频率必须大于开始频率.
        /// </summary>
        public const string Fs_Right = "监测频段中的结束频率必须大于开始频率。";

        /// <summary>
        /// 频段_开始频率为空:请输入监测频段中的开始频率(0～300 GHz).
        /// </summary>
        public const string Fs_SFreq_Empty = "请输入监测频段中的开始频率(0～300 GHz)。";

        /// <summary>
        ///  频段_结束频率为空:请输入监测频段中的开始频率(0～300 GHz).
        /// </summary>
        public const string Fs_EFreq_Empty = "请输入监测频段中的结束频率(0～300 GHz)。";

        /// <summary>
        /// 频段_开始频率格式错误:监测频段中的开始频率格式有误(数字类型,0～300 GHz).
        /// </summary>
        public const string Fs_SFreq_Format = "监测频段中的开始频率格式有误(数字类型，0～300 GHz)。";

        /// <summary>
        ///  频段_结束频率格式错误:监测频段中的结束频率格式有误(数字类型,0～300 GHz).
        /// </summary>
        public const string Fs_EFreq_Format = "监测频段中的结束频率格式有误(数字类型，0～300 GHz)。";

        /// <summary>
        ///  频段_开始频率错误:监测频段中的开始频率范围输入有误(0～300 GHz).
        /// </summary>
        public const string Fs_SFreq_Right = "监测频段中的开始频率范围输入有误(0～300 GHz)。";

        /// <summary>
        ///  频段_结束频率错误:监测频段中的结束频率范围输入有误(0～300 GHz).
        /// </summary>
        public const string Fs_EFreq_Right = "监测频段中的结束频率范围输入有误(0～300 GHz)。";

        /// <summary>
        /// 监测站点未选:请选择监测站点.
        /// </summary>
        public const string Site_Choose = "请选择监测站点。";

        /// <summary>
        /// 监测时间为空:请输入监测时间.
        /// </summary>
        public const string Time_Empty = "请输入监测时间。";

        /// <summary>
        /// 统计/测量/时间区间为空:请输入完整的时间区间.
        /// </summary>
        public const string TimeZone_Empty = "请输入完整的统计/测量/时间区间。";

        /// <summary>
        /// 开始时间为空:请输入时间区间中的开始时间.
        /// </summary>
        public const string TimeZone_STime_Empty = "请输入时间区间中的开始时间。";

        /// <summary>
        /// 结束时间为空:请输入时间区间中的结束时间.
        /// </summary>
        public const string TimeZone_ETime_Empty = "请输入时间区间中的结束时间。";

        /// <summary>
        /// 统计/测量/时间区间错误:时间区间中的开始时间不能大于结束时间.
        /// </summary>
        public const string TimeZone_Right = "统计/测量/汇总时间区间中的开始时间不能大于结束时间。";

        /// <summary>
        /// 电平门限为空:请输入电平门限(-50～150 dBμV).
        /// </summary>
        public const string Limit_Empty = "请输入电平门限(-50～150 dBμV)。";

        /// <summary>
        /// 电平门限输入格式错误:电平门限输入的格式有误(整数类型,-50～150 dBμV).
        /// </summary>
        public const string Limit_Format = "电平门限输入的格式有误(整数类型，-50～150 dBμV)。";

        /// <summary>
        /// 电平门限输入范围错误:电平门限输入范围有误(-50～150 dBμV).
        /// </summary>
        public const string Limit_Right = "电平门限输入范围有误(-50～150 dBμV)。";

        /// <summary>
        /// 统计精度未选:请选择统计精度.
        /// </summary>
        public const string Precision_Choose = "请选择统计精度。";

        /// <summary>
        /// 统计门限为空:请输入统计门限(0～100%).
        /// </summary>
        public const string StatLimit_Empty = "请输入频道占用度门限(0～100%)。";

        /// <summary>
        /// 频道占用度门限格式错误:频道占用度门限输入格式有误(数字类型,0～100%).
        /// </summary>
        public const string StatLimit_Format = "频道占用度门限输入格式有误(数字类型，0～100%)。";

        /// <summary>
        /// 频道占用度门限输入范围有误:频道占用度门限输入范围有误(0～100%).
        /// </summary>
        public const string StatLimit_Right = "频道占用度门限输入范围有误(0～100%)。";

        /// <summary>
        /// 非法字符
        /// </summary>
        public const string Unlawful_Format = "存在非法字符(',\\,%,*,^,/,#,!,~)。";

        #endregion

        #region 通用定义
        /// <summary>
        /// 框架密钥串，用于Cookie与License验证
        /// </summary>
        public const string KEY_SECRETSTR = "ShuiP~JieL!5D@MiM#59$-->Ma";
        #endregion

        #region 返回code定义
        /// <summary>
        /// 正确返回值
        /// </summary>
        public const string ResultCode_Succeed = "000";

        /// <summary>
        /// 错误的返回值(参数错误)
        /// </summary>
        public const string ResultCode_ParameterError = "000";
        /// <summary>
        /// 错误的返回值(一般程序发生Exception)
        /// </summary>
        public const string ResultCode_Error = "999";
        #endregion


        #region 权限标记符定义
        /// <summary>
        /// 一般普通权限标识符
        /// </summary>
        public const string RightForGeneral = "right";
        #endregion

        #region 系统配置表Key
        /// <summary>
        /// 用户管理模块ID在系统配置表中的关键字
        /// </summary>
        public const string ConfigKey_UserManagerModuleID = "usermngId";

        /// <summary>
        /// 组管理模块ID在系统配置表中的关键字
        /// </summary>
        public const string ConfigKey_GroupManagerModuleID = "groupmngId";
        #endregion

        #region Cache缓存key
        /// <summary>
        /// 当前用户权限Key 
        /// </summary>
        public const string CacheKey_UserRight = "CurUserRight_{0}";

        /// <summary>
        /// 当前系统的MNGCode缓存对象Key
        /// </summary>
        public const string CacheKey_CurMngCode = "CurMngCodeKey";
        ///// <summary>
        ///// 当前用户用户管理等级cache缓存key
        ///// </summary>
        //public const string CacheKey_UserGrade = "CurUserManagerGrade_{0}";
        ///// <summary>
        ///// 当前用户组管理等级cache缓存key
        ///// </summary>
        //public const string CacheKey_GroupCrade = "CurGroupManagerGrade_{0}";
        #endregion
    }
}
