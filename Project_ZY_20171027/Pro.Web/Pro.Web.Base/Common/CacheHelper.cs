using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace Pro.Web.Common
{
    /// <summary>
    /// CacheHelper(适用于ASP.NET WEB项目)
    /// </summary>
    public class CacheHelper
    {
        public CacheHelper() { }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        public static void SetCache(string cacheName, object val)
        {
            HttpRuntime.Cache.Insert(cacheName, val);
        }

        /// <summary>
        /// 设置缓存,并设置缓存时长
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="cacheTime">要缓存的时长(分钟)</param>
        public static void SetCache(string cacheName, object val, int cacheTime)
        {
            if (cacheTime == 0) return;
            HttpRuntime.Cache.Insert(cacheName, val, null, DateTime.Now.AddMinutes(cacheTime), TimeSpan.Zero);
        }

        /// <summary>
        /// 设置缓存,并建立依赖关系
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="cacheDepKeys">缓存的依赖关系</param>
        public static void SetCache(string cacheName, object val, string[] cacheDepKeys)
        {
            if (cacheDepKeys.Length == 0) return;
            System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(cacheDepKeys);

            HttpRuntime.Cache.Insert(cacheName, val, dependency);
          //  HttpRuntime.Cache.Insert(cacheName, val);
          //TimeSpan ts = new TimeSpan(1, 0, 0);
            //HttpRuntime.Cache.Insert(cacheName, val, dependency, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
               // , DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            //DateTime absoluteexpiration = DateTime.maxvalue;
            //TimeSpan slidingexpiration = TimeSpan.FromMinutes(10);//.fromminutes(10);
            //HttpRuntime.Cache.Insert("txt", "a", null,
            //absoluteexpiration, slidingexpiration,
            //System.Web.Caching.CacheItemPriority.High, null);

        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        public static void DelCache(string cacheName)
        {
            HttpRuntime.Cache.Remove(cacheName);
        }

        /// <summary>
        /// 取缓存对象
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <returns></returns>
        public static object GetCache(string cacheName)
        {
            if (HttpRuntime.Cache[cacheName] != null)
                return HttpRuntime.Cache[cacheName];
            else
                return null;
        }

        /// <summary>
        /// 检验缓存是否存在
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <returns></returns>
        public static bool CheckCache(string cacheName)
        {
            return (HttpRuntime.Cache[cacheName] != null);
        }
    }
}
