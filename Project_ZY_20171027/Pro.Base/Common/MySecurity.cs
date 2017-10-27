using System;
using System.Text;
using System.Security.Cryptography;

namespace Pro.Common
{
    public class MySecurity
    {
        #region 系统变量定义

        private const string ConBaseString = "$yeah!OK"; //一个随机字符

        #endregion

        #region 加密解密（可逆加密）

        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位，不足8位系统自动补齐。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public static string Encrypt(string pToEncrypt, string _Key)
        {
            string sKey = (_Key + ConBaseString).Substring(0, 8);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 进行DES解密。
        /// </summary>
        /// <param name="pToDecrypt">要解密的以Base64</param>
        /// <param name="sKey">密钥，且必须为8位，不足8位系统自动补齐。</param>
        /// <returns>已解密的字符串。</returns>
        public static string Decrypt(string pToDecrypt, string _Key)
        {
            string sKey = (_Key + ConBaseString).Substring(0, 8);
            try
            {
                byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    string str = "";
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);

                            cs.FlushFinalBlock();
                            cs.Close();
                            str = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                    catch
                    {
                        str = pToDecrypt;
                    }
                    ms.Close();
                    return str;
                }
            }
            catch (Exception e)
            {
                MyLog.WriteExceptionLog("MySecurity.Decrypt", e, pToDecrypt);
                return "";
            }
        }

        #endregion

        #region 不可逆加密

        /// <summary>
        /// MD5 离散加密
        /// </summary>
        /// <param name="theStr">加密前字符串</param>
        /// <returns>加密后字符串</returns>
        public static string EnCodeByMD5(string theStr)
        {
            theStr += ConBaseString;
            MD5CryptoServiceProvider theMD5 = new MD5CryptoServiceProvider();
            byte[] theResult = theMD5.ComputeHash(System.Text.Encoding.Default.GetBytes(theStr));
            return BitConverter.ToString(theResult).Replace("-", "");
        }

        public static string EnCodeByMD5WithOutRandomKey(string theStr)
        {
            MD5CryptoServiceProvider theMD5 = new MD5CryptoServiceProvider();
            byte[] theResult = theMD5.ComputeHash(System.Text.Encoding.Default.GetBytes(theStr));
            return BitConverter.ToString(theResult).Replace("-", "");
        }

        /// <summary>
        /// SHA1 离散加密
        /// </summary>
        /// <param name="theStr">加密前字符串</param>
        /// <returns>加密后字符串</returns>
        public static string EnCodeBySHA1(string theStr)
        {
            theStr += ConBaseString;
            SHA1CryptoServiceProvider theSHA1 = new SHA1CryptoServiceProvider();
            byte[] theResult = theSHA1.ComputeHash(System.Text.Encoding.Default.GetBytes(theStr));
            return BitConverter.ToString(theResult).Replace("-", "");
        }

        #endregion
    }
}
