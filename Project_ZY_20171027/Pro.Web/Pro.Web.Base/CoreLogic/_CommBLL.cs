using System;
using System.IO;
using System.Web;

using Pro.Common;

namespace Pro.Web.CoreLogic
{
    public abstract class CommBLL
    {
        public string[] GetCacheDeps(params string[] depsCacheName)
        {
            string cacheDepDir = HttpContext.Current.Server.MapPath("/prowebequactive/cacheDeps");
            String[] retVals = new String[depsCacheName.Length];
            string retVal = "";
            string path = "";
            for (int i = 0; i < depsCacheName.Length; i++)
            {
                path = cacheDepDir + "\\" + depsCacheName[i];

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                retVal = path +"\\"+ depsCacheName[i] + ".txt";  //cacheDepDir + "\\" + depsCacheName[i] + "\\" + depsCacheName[i] + ".txt";
                if (!FileHelper.IsExistsFile(cacheDepDir))
                     FileHelper.WriteTxtFile(retVal, null);
                retVals[i] = retVal;
            }
            return retVals;
        }
    }
}
