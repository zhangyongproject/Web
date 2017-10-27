using System;

namespace Pro.Common
{
    public class RadioUtil
    {
        /// <summary>
        /// 将以kHz,MHz,GHz为单位的频率值转换为以Hz为单位的频率值
        /// </summary>
        /// <param name="dValue">以kHz,MHz,GHz为单位的频率值</param>
        /// <param name="strUnit">频率值单位 K,M,G</param>
        /// <returns>以Hz为单位的频率值</returns>
        public static double ToHz(double dValue, string strUnit)
        {
            if (dValue < 0)
                return -1;
            switch (strUnit.ToUpper())
            {
                case "K":
                    dValue = dValue * Math.Pow(10, 3);
                    break;
                case "M":
                    dValue = dValue * Math.Pow(10, 6);
                    break;
                case "G":
                    dValue = dValue * Math.Pow(10, 9);
                    break;
                default:
                    break;
            }
            return dValue;
        }

        /// <summary>
        /// 将以Hz为单位的频率值转换为以kHz,MHz,GHz为单位的频率值
        /// </summary>
        /// <param name="dValue">以Hz为单位的频率值</param>
        /// <param name="strUnit">要转换为的频率值单位 K M G</param>
        /// <returns>以kHz,MHz,GHz为单位的频率值</returns>
        public static double FmtHz(double dValue, string strUnit)
        {
            switch (strUnit.ToUpper())
            {
                case "K":
                    dValue = dValue / Math.Pow(10, 3);
                    break;
                case "M":
                    dValue = dValue / Math.Pow(10, 6);
                    break;
                case "G":
                    dValue = dValue / Math.Pow(10, 9);
                    break;
                default:
                    break;
            }
            return dValue;
        }

        /// <summary>
        /// 经纬度格式转换( 0-> 000°00'00.0E)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatDegreeToString(string value)
        {
            try
            {
                if (value == "0")
                    return value + "°";
                string[] strArray = value.Split(new char[] { '.' });
                string str2 = strArray[0] + "°";
                if (value.IndexOf(".") == -1)
                    return str2;
                double num = double.Parse("0." + strArray[1]) * 60;
                string[] strArray2 = num.ToString().Split(new char[] { '.' });
                string str3 = strArray2[0] + "'";
                string str6 = "";
                if (strArray2.Length != 1)
                {
                    str6 = (double.Parse("0." + strArray2[1]) * 60).ToString();
                    if (str6.Length > 7)
                    {
                        str6 = str6.Substring(0, 7);
                    }
                    double tmp = double.Parse(str6);
                    int temp = (int)tmp;
                    string str4 = temp + "''";
                    return (str2 + str3 + str4);
                }
                return (str2 + str3);
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 经纬度格式转换(000°00'00.0E -> 0.00)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FormatDegree(string value)
        {
            string[] strArray = value.ToString().Split(new char[] { '°' });
            string str2 = strArray[0] + ".0";

            double num1 = double.Parse(str2);

            char c = '′';
            string[] strArray2 = strArray[1].Split(c);

            double num2 = double.Parse(strArray2[0]) / 60;

            string str = strArray2[1].Replace("″", "");
            double num3 = double.Parse(str) / 3600;

            return num1 + num2 + num3;
        }

        public static string GetWeek(int num)
        {
            string str = string.Empty;
            switch (num)
            {
                case 1:
                    str = "星期日";
                    break;
                case 2:
                    str = "星期一";
                    break;
                case 3:
                    str = "星期二";
                    break;
                case 4:
                    str = "星期三";
                    break;
                case 5:
                    str = "星期四";
                    break;
                case 6:
                    str = "星期五";
                    break;
                case 7:
                    str = "星期六";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
}
