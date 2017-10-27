using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Pro.Common
{
    public class MyType
    {
        #region Integer

        public static bool IsInt(string str)
        {
            int value = 0;
            return int.TryParse(str, out value);
        }

        public static int ToInt(string str)
        {
            return ToInt(str, 0);
        }

        public static int ToInt(string str, int defaultValue)
        {
            int value = defaultValue;
            bool parseOk = int.TryParse(str, out value);
            return parseOk ? value : defaultValue;
        }

        #endregion

        #region Double

        public static bool IsDouble(string str)
        {
            Double value = 0;
            return Double.TryParse(str, out value);
        }

        public static Double ToDouble(string str)
        {
            return ToDouble(str, 0);
        }

        public static Double ToDouble(string str, Double defaultValue)
        {
            Double value = defaultValue;
            bool parseOk = Double.TryParse(str, out value);
            return parseOk ? value : defaultValue;
        }

        #endregion

        #region DateTime

        public static bool IsDateTime(string str)
        {
            DateTime value = DateTime.Now;
            return DateTime.TryParse(str, out value);
        }

        public static DateTime ToDateTime(string str)
        {
            return ToDateTime(str, DateTime.Now);
        }

        public static DateTime ToDateTime(string str, DateTime defaultValue)
        {
            DateTime value = defaultValue;
            bool parseOk = DateTime.TryParse(str, out value);
            return parseOk ? value : defaultValue;
        }

        #endregion

        #region byte[]/string

        /// <summary>
        /// ���ַ���ת���ɶ�����
        /// </summary>
        /// <param name="str">��ת�����ַ���</param>
        /// <returns></returns>
        public static byte[] ToBytes(string str)
        {
            //return Encoding.GetEncoding("gb2312").GetBytes(str);
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// ���ֽ���ת���ɶ�����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(System.IO.Stream theStream)
        {
            byte[] tmpBuffer = new byte[theStream.Length];
            theStream.Read(tmpBuffer, 0, (int)theStream.Length);
            theStream.Close();
            return tmpBuffer;
        }

        /// <summary>
        /// ��������ת�����ַ���
        /// </summary>
        /// <param name="bytes">��ת���Ķ�����</param>
        /// <returns></returns>
        public static string ToString(byte[] bytes)
        {
            //return Encoding.GetEncoding("gb2312").GetString(bytes);
            return Encoding.UTF8.GetString(bytes);
        }
        /// <summary>
        /// ��������ת�����ַ���
        /// </summary>
        /// <param name="bytes">��ת���Ķ�����</param>
        /// <returns></returns>
        public static string ToString(byte[] bytes, int index, int count)
        {
            //return Encoding.GetEncoding("gb2312").GetString(bytes, index, count);
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        /// <summary>
        /// ������������ת����16�����ַ���������ʾ
        /// </summary>
        /// <param name="bytes">����������</param>
        /// <param name="bytecountPerRow">ÿ����ʾ���ֽ���</param>
        /// <param name="showChar">�Ƿ���ÿһ�к�����ʾ�������ֽڶ�Ӧ�Ŀɼ��ַ�</param>
        /// <returns>�ַ�����������У�</returns>
        public static string ToHexString(byte[] bytes, int bytecountPerRow, bool showChar)
        {
            StringBuilder sb = new StringBuilder();
            string str1 = "";
            string str2 = "";
            for (int i = 0; i <= bytes.Length; i++)
            {
                if ((i % bytecountPerRow == 0) || (i == bytes.Length))
                {
                    if (str1 != "")
                    {
                        if (showChar)
                        {
                            str1 = str1.PadRight(3 * bytecountPerRow);
                            str1 += "\t" + str2;
                        }
                        sb.AppendLine(str1);
                        str1 = "";
                        str2 = "";
                    }
                    if (i == bytes.Length)
                        break;
                }

                str1 += string.Format(" {0:X2}", bytes[i]);
                if (('a' <= bytes[i] && bytes[i] <= 'z') || ('A' <= bytes[i] && bytes[i] <= 'Z') || ("~!@#$%^&*()_+<>?,./:\"\\\'`1234567890".IndexOf((char)bytes[i]) > -1))
                    str2 += (char)bytes[i];
                else
                    str2 += ".";
            }
            return sb.ToString();
        }

        #endregion

        #region ͼƬ/byte[]

        /// <summary>
        ///  ImageToByte(Image img) ��ͼƬת���ɶ����ƴ��룬Ȼ��洢���ݿ���
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image img)
        {
            byte[] byt = null;
            ImageConverter imgCvt = new ImageConverter();
            object obj = imgCvt.ConvertTo(img, typeof(byte[]));
            byt = (byte[])obj;
            return byt;
        }

        /// <summary>
        ///  ByteToImage(byte[] byt) ��ȡ���ݿ��еĶ������ļ�����ת��ΪImage  
        /// </summary>
        /// <param name="bytImage"></param>
        /// <returns></returns>
        public static Image ByteToImage(byte[] bytImage)
        {
            Image img = null;
            ImageConverter imgCvt = new ImageConverter();

            object obj = imgCvt.ConvertFrom(bytImage);
            img = (Image)obj;
            return img;
        }

        #endregion

        #region �ַ������⴦��

        #region ���ַ���ת���� Html ����ģʽ

        /// <summary>
        /// ���ַ���ת���� Html ����ģʽ
        /// </summary>
        /// <param name="ResStr">ԭ�ַ���</param>
        /// <returns>ת������ַ���</returns>
        public static string StrToHtml(string ResStr)
        {
            if (ResStr == null) 
                return "";
            if (ResStr == "")
                return "";

            string tmpstr = ResStr;

            tmpstr = tmpstr.Replace("<", "&lt;");
            tmpstr = tmpstr.Replace(">", "&gt;");
            tmpstr = tmpstr.Replace("\n", "<br>");
            tmpstr = tmpstr.Replace("  ", "&nbsp; ");

            return tmpstr;
        }

        #endregion

        #region TrimChineseStr(DB���ã�

        /// <summary>
        /// ���ո����ĳ��ȣ��س��ַ���(���ֵ�����������)
        /// </summary>
        /// <param name="theValue">�������ַ���</param>
        /// <param name="Length">�����ĳ�������</param>
        /// <returns>������</returns>
        public static string TrimChineseStr(string theValue, int Length)
        {
            string str = theValue;
            if (theValue != null && theValue != "")
            {
                string tmpStr = theValue;
                int mi = 0;
                int ml = 0;
                foreach (char mc in tmpStr.ToCharArray())
                {
                    //if ((long)mc > 383) //�ԡ����ж�ʧ��2015/3/10
                    if ((long)mc > 127) 
                        ml += 2;
                    else
                        ml += 1;

                    if (ml > Length)
                        break;
                    mi += 1;
                }
                if (ml > Length)
                    str = tmpStr.Substring(0, mi);
            }
            return str;
        }

        #endregion

        #region value2string(DB���ã�

        public static string value2string(object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                return obj.ToString();
            }
            else
                return "'" + obj.ToString() + "'";
        }

        #endregion

        #endregion
    }
}
