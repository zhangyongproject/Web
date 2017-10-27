using System;
using System.IO;

namespace Pro.Common
{
    public class MyLog
    {
        /// <summary>
        /// ����ϵͳ������־��ֱ�ӱ��浽�ļ���
        /// </summary>
        /// <param name="ErrorMsg"></param>
        public static void WriteLogInfo(string LogMsg)
        {
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "log";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
            string FileName = string.Format("{0}\\{1}.log", FilePath, DateTime.Now.ToString("yyyyMMdd"));
            writeToFile(FileName, LogMsg);  
        }

        public static void WriteExceptionLog(string funcname, Exception e, string strSource)
        {
            MyLog.WriteLogInfo(string.Format("{0}ִ�г���{1}\r\n�����Ϣ��{2}", funcname, e.Message, strSource));
        }

        #region �ļ�������˽�к���

        private static bool createFile(string fileName)
        {
            try
            {
                StreamWriter sr = File.CreateText(fileName);
                sr.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool writeToFile(string fileName, string sMsg)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    createFile(fileName);
                }

                using (StreamWriter sw = File.AppendText(fileName))
                {
                    // �ı��ļ���ʽ
                    sw.Write("ʱ�䣺");
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("��ϸ��Ϣ��");
                    sw.WriteLine(sMsg);
                    sw.WriteLine("-----------------------------");

                    /*.
                    //Html�ļ�
                    string textColorPre ="<font color=black>";
                    string fontEnd ="</font>";
                    string msgColorPre ="<font color=blue>";
                    string pEnd ="<br>";
                    sw.Write(textColorPre +"<b>ʱ�䣺</b>");
                    sw.WriteLine(msgColorPre +DateTime.Now +fontEnd +pEnd);
                    sw.WriteLine(textColorPre +"<b>��ϸ��Ϣ��</b>" +fontEnd +pEnd);
                    sw.WriteLine(msgColorPre +sMsg +fontEnd +pEnd);
                    sw.WriteLine(textColorPre +"-------------------" +fontEnd +pEnd);
                    */
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
