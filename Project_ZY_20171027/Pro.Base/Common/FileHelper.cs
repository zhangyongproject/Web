using System.Data;
using System.IO;
using System.Text;

namespace Pro.Common
{
    public class FileHelper
    {
        /// <summary>
        /// 检查是否存在文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool IsExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 将指定字符写入一个文本文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="strTxt">要写入文件的字符.为null或空字符串时,创建一个空文件</param>
        public static void WriteTxtFile(string filePath, string strTxt)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            if (strTxt != null && strTxt.Length > 0)
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                sw.Write(strTxt);
                sw.Flush();
                sw.Close();
            }
            fs.Close();
        }

        /// <summary>
        /// 将DataTable数据转换为Xml字符后写入一个文本文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="dt">DataTable数据</param>
        public static void WriteXmlFile(string filePath, DataTable dt)
        {
            DataSet ds = Tools.GetTableDs(dt, true);
            ds.DataSetName = "items";
            ds.Tables[0].TableName = "item";
            string text = ds.GetXml();

            WriteTxtFile(filePath, text);
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void DelFile(string filePath)
        {
            File.Delete(filePath);
        }

        /// <summary>
        /// 获取(文本格式的)文件的内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileText(string filePath)
        {
            string text = "";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB2312"));
            text = sr.ReadToEnd();
            sr.Close();
            fs.Dispose();
            return text;
        }

        /// <summary>
        /// 获取文件所有的内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static string GetAllValue(string filePath)
        {
            string value = "";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB2312"));
            value = sr.ReadToEnd();
            sr.Close();
            fs.Dispose();
            return value;
        }


        #region 文件操作

        /// <summary>
        /// 删除目录和文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void DeleteDir(string Path)
        {
            DirectoryInfo root = new DirectoryInfo(Path);

            FileInfo[] files = root.GetFiles();
            DirectoryInfo[] dirs = root.GetDirectories();

            foreach (FileInfo file in files)
            {
                file.IsReadOnly = false;
                File.Delete(file.FullName);
            }

            foreach (DirectoryInfo dir in dirs)
            {
                DeleteSubDir(dir);
            }
        }

        /// <summary>
        /// 删除目录和文件
        /// </summary>
        /// <param name="parent"></param>
        public static void DeleteSubDir(DirectoryInfo parent)
        {
            FileInfo[] files = parent.GetFiles();
            DirectoryInfo[] dirs = parent.GetDirectories();

            foreach (FileInfo file in files)
            {
                //删除本目录下的所有文件
                File.Delete(file.FullName);
            }
            if (dirs == null || dirs.Length == 0)
            {
                //删除本目录
                Directory.Delete(parent.FullName);
                return;
            }
            foreach (DirectoryInfo dir in dirs)
            {
                //迭代删除子目录
                DeleteSubDir(dir);
            }
        }

        /// <summary>
        /// 删除目录和文件
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="containName">包含的测量名</param>
        public static void DeleteSubDir(DirectoryInfo parent, string containName)
        {
            foreach (DirectoryInfo dir in parent.GetDirectories())
            {
                if (dir.FullName.Contains(containName))
                {
                    Directory.Delete(dir.FullName, true);
                }
            }
        }
        #endregion
    }
}
