using System;
using System.Collections.Generic;
using System.Text;
using Pro.CoreModel;
using Pro.Web.Common;
using Pro.EABase;
using System.Data;
using Pro.Common;

namespace Pro.Web.EALogic
{
    public class TimingStartRecordLogic : BaseLogic
    {
        private TimingStartRecordDAL tsrDAL = new TimingStartRecordDAL();

        /// <summary>
        /// /// 获取定时启动记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue GetTimingStartRecord(TiminGstartRecordInfo info)
        {
            return tsrDAL.GetTimingStartRecord(info);
        }

        /// <summary>
        /// 添加定时启动记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(TiminGstartRecordInfo info)
        {
            return tsrDAL.Insert(info);
        }

        /// <summary>
        /// 修改定时启动记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(TiminGstartRecordInfo info)
        {
            //ReturnValue retVal = GetTimingStartRecord(new TiminGstartRecordInfo() { UserID = info.UserID });
            //if (!retVal.IsSuccess) { return new ReturnValue(false, -9, Consts.EXP_Info); }   //执行失败
            //DataTable dt = retVal.RetDt;
            //DataRow[] drs = dt.Select(string.Format(" tsrid={1}", info.UserName, info.TSRID), "tsrid asc");
            //if (drs.Length == 0) { return new ReturnValue(false, -2); } //不存在该记录
            return tsrDAL.Update(info);
        }


        /// <summary>
        /// 删除定时启动记录
        /// </summary>
        /// <param name="info">UserId/UserName</param>
        /// <returns></returns>
        public ReturnValue Delete(TiminGstartRecordInfo info)
        {
            return tsrDAL.Delete(info);
        }
    }
}
