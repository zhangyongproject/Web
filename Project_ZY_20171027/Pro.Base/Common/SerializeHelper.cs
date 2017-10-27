using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Pro.Common
{
    /// <summary>
    ///  ���м������л�������
    /// </summary>
    public class SerializeHelper
    {
        private SerializeHelper() { }
        #region ���м������л�������

        /// <summary>
        ///  ���л�����
        /// </summary>
        /// <param name="data">Ҫ���л��Ķ���</param>
        /// <returns>���л���Ľ��Ϊbyte����</returns>
        public static byte[] Serialize(object data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream();
            formatter.Serialize(rems, data);
            return rems.GetBuffer();
        }

        /// <summary>
        ///  ���л�����
        /// </summary>
        /// <param name="data">Ҫ���л��Ķ���</param>
        /// <returns>���л���Ľ��Ϊbyte����</returns>
        public static byte[] Serialize(object data, bool bFlag)
        {
            byte[] bAry = null;

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream();
            formatter.Serialize(rems, data);

            if (bFlag)
            {
                bAry = System.Text.Encoding.Convert(Encoding.Default, Encoding.ASCII, rems.GetBuffer());
            }
            else
            {
                bAry = rems.GetBuffer();
            }

            return bAry;
        }

        /// <summary>
        ///  �����л�����
        /// </summary>
        /// <param name="data">���л���Ķ���byte����</param>
        /// <returns>����</returns>
        public static object Deserialize(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream(data);
            data = null;
            return formatter.Deserialize(rems);
        }

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

        /// <summary>
        ///  ѹ�����л��Ķ���
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SerializeZip(byte[] bytData)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                Stream s = new GZipStream(ms, CompressionMode.Compress);
                s.Write(bytData, 0, bytData.Length);
                s.Close();
                byte[] compressedData = (byte[])ms.ToArray();
                return compressedData;
            }
            catch
            {
                return bytData;
            }
        }

        /// <summary>
        ///  ��ѹ�����л��Ķ���
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        public static byte[] DeserializeUnZip(byte[] bytData)
        {
            string strResult = "";
            int totalLength = 0;
            byte[] writeData = new byte[100000];
            Stream stm = new GZipStream(new MemoryStream(bytData), CompressionMode.Decompress);
            try
            {
                while (true)
                {
                    int size = stm.Read(writeData, 0, writeData.Length);
                    if (size > 0)
                    {
                        totalLength += size;
                        strResult += System.Text.Encoding.Unicode.GetString(writeData, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
                stm.Close();
                return Encoding.Unicode.GetBytes(strResult);
            }
            catch
            {
                return null;
            }

            //return Decompress(bytData);
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            GZipStream stream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream temp = new MemoryStream();
            byte[] buffer = new byte[1024];
            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    break;
                }
                else
                {
                    temp.Write(buffer, 0, buffer.Length);
                }
            }
            stream.Close();
            return temp.ToArray();
        }


        /// <summary>
        /// ���л�object->byte[]->string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToString(Object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] b;
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            b = new byte[ms.Length];
            ms.Read(b, 0, b.Length);
            ms.Close();
            return Convert.ToBase64String(b);
        }



        /// <summary>
        /// �����л�:��stringתΪbyte[]->object��
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Object Deserialize(string data)
        {
            byte[] BytArray = Convert.FromBase64String(data);

            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(BytArray, 0, BytArray.Length);
            ms.Position = 0;
            Object obj = (Object)formatter.Deserialize(ms);
            return obj;
        }


        #endregion
    }
}
