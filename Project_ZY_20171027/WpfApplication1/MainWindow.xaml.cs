using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using Pro.Web.EquActive.WebService;
using Newtonsoft.Json.Linq;


namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string MakeLogionParameter(string username, string password)
        {//{"username":"admin","password":"admin"}
            StringBuilder sb    = new StringBuilder();
            StringWriter sw     = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("username");
                jtw.WriteValue(username);
                jtw.WritePropertyName("password");
                jtw.WriteValue(password);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        //网址:http://www.yixiubox.com:5000/Server/AccessService.asmx
        private void Logion()
        {
            var     client  = new AccessServ.AccessServiceSoapClient();

            string  strjson = client.Login(MakeLogionParameter("admin", "admin"));
            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }

        private string MakeGetDeviceInfoParameter(string Equipmentname)
        {//{"equipmentname":"设备名称3"}
            StringBuilder sb    = new StringBuilder();
            StringWriter sw     = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("equipmentname");
                jtw.WriteValue(Equipmentname);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        //网址:http://www.yixiubox.com:5000/Server/EquipmentService.asmx
        private void GetEquipmentInfo()
        {   
            var     client  = new EquipmentServ.EquipmentServiceSoapClient();
            string  strjson = client.GetEquipmentInfo(MakeGetDeviceInfoParameter("device0"));

            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }

        private string MakeActiveEquipmentParameter(string equipmentname, string activecode)
        {//{"code":"设备名称3","equipmentname":"12306"}
            StringBuilder sb    = new StringBuilder();
            StringWriter sw     = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("code");
                jtw.WriteValue(activecode);
                jtw.WritePropertyName("equipmentname");
                jtw.WriteValue(equipmentname);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        //网址:http://www.yixiubox.com:5000/Server/EquipmentService.asmx
        private void ActiveEquipment()
        {
            var     client  = new EquipmentServ.EquipmentServiceSoapClient();
            string  strjson = client.Active(MakeActiveEquipmentParameter("aaaaaa", "000000"));

            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }


        private string MakeEquipmentIPListParameter(string equipmentname, string ipList)
        {//{"code":"设备名称3","equipmentname":"12306"}
            StringBuilder sb    = new StringBuilder();
            StringWriter sw     = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("equipmentname");
                jtw.WriteValue(equipmentname);
                jtw.WritePropertyName("iplist");
                jtw.WriteValue(ipList);
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        //网址:http://www.yixiubox.com:5000/Server/EquipmentService.asmx
        private void UpdateEquipmentIPList()
        {
            var     client  = new EquipmentServ.EquipmentServiceSoapClient();
            string  strjson = client.UpdateIPList(MakeEquipmentIPListParameter("aaaaaa", "1111"));

            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }



        private string MakeGetTimingListParameter(string equipmentname)
        {//{"code":"设备名称3","equipmentname":"12306"}
            StringBuilder sb    = new StringBuilder();
            StringWriter sw     = new StringWriter(sb);
            using (JsonTextWriter jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("equipmentname");
                jtw.WriteValue(equipmentname);
                jtw.WritePropertyName("expstartdate");
                jtw.WriteValue("2017-01-01 00:00:00");
                jtw.WritePropertyName("expenddate");
                jtw.WriteValue("2019-01-01 00:00:00");
                jtw.WriteEndObject();

                return sb.ToString();
            }
        }
        //网址:http://www.yixiubox.com:5000/Server/TimingService.asmx
        private void GetTimingList()
        {
            var     client  = new TimingServ.TimingServiceSoapClient();
            string  strjson = client.GetTimingList(MakeGetTimingListParameter("bbbbbb"));

            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Logion();
            //GetEquipmentInfo();
            //ActiveEquipment();
            UpdateEquipmentIPList();
            //GetTimingList();
        }


    }
}
