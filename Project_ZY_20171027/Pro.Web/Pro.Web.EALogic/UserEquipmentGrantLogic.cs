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
    public class UserEquipmentGrantLogic : BaseLogic
    {
        private UserEquipmentGrantDAL uegDAL = new UserEquipmentGrantDAL();

        /// <summary>
        /// 获取用户设备授权记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue GetUserEquGrant(UserEquipmentGrantInfo info)
        {
            return uegDAL.GetUserEquGrant(info);
        }

        /// <summary>
        /// 添加用户设备授权记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Insert(UserEquipmentGrantInfo info)
        {
            return uegDAL.Insert(info);
        }

        /// <summary>
        /// 修改用户设备授权记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReturnValue Update(UserEquipmentGrantInfo info)
        {
            return uegDAL.Update(info);
        }


        /// <summary>
        /// 删除用户设备授权记录
        /// </summary>
        /// <param name="info">UserId/UEGID/EIID</param>
        /// <returns></returns>
        public ReturnValue Delete(UserEquipmentGrantInfo info)
        {
            return uegDAL.Delete(info);
        }

        /// <summary>
        /// 删除用户授权记录
        /// </summary>
        /// <param name="info">用户ID支持逗号分隔多个</param>
        /// <returns></returns>
        public ReturnValue Delete4Ids(string ids)
        {
            return uegDAL.Delete4Ids(ids);
        }

        /// <summary>
        /// 批量修改时间
        /// </summary>
        /// <param name="ids">用户设备授权ID列表（逗号分割）</param>
        /// <param name="info">对象【StartDate，EndDate】</param>
        /// <returns></returns>
        public ReturnValue BatchEditDate(string ids, UserEquipmentGrantInfo info)
        {
            if (string.IsNullOrEmpty(ids)) { return new ReturnValue(false, -2, "修改对象ID列表为空。"); }
            return uegDAL.BatchEditDate(ids, info);
        }
    }
}
