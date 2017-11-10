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
using System.Threading;


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
            string strjson = client.UpdateIPList(MakeEquipmentIPListParameter("XYKf001", "192.168.1.105,127.0.0.1"));

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
            string strjson = client.GetTimingList(MakeGetTimingListParameter("XYKf001"));

            JObject jo      = (JObject)JsonConvert.DeserializeObject(strjson);
            JArray  ja      = (JArray)jo["data"];
        }
        //VPNHelper vpnhelp = new VPNHelper("192.168.1.107", "vpnwin101", "vpn", "12345678");

        static string vpnName = "Yixiuyun";
        static string vpnIP = "192.168.1.107";
        static string userN = "vpn";
        static string userP = "12345678";

        VPNHelper vpnhelp = new VPNHelper(vpnIP, vpnName, "", "optional", vpnName, "mschapv2", "false");

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //vpnhelp.CreateOrUpdateVPN(vpnName, vpnIP);
            //vpnhelp.TryConnectVPN(vpnName, userN, userP);

            //bool    bConnect    = true;
            //int     count       = 0;
            //while(!vpnhelp.IsCurrentConnectingVPNName())
            //{
            //    Thread.Sleep(100);
            //    count++;
            //    if(count > 30)
            //    {
            //        bConnect = false;
            //        break;
            //    }
            //}

            //Logion();
            //GetEquipmentInfo();
            //ActiveEquipment();
            UpdateEquipmentIPList();
            //GetTimingList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vpnhelp.TryDisConnectVPN(vpnName);

            bool bConnect = false;
            int  count = 0;
            while (vpnhelp.IsCurrentConnectingVPNName())
            {
                Thread.Sleep(100);
                count++;
                if (count > 30)
                {
                    bConnect = true;
                    break;
                }
            }

            vpnhelp.TryDeleteVPN();
        }


    }



    //public partial class Form1 : Form
    //{
    //    public RasPhoneBook book;
    //    public RasEntry entry;
    //    public DotRas.RasDialer dailer = new DotRas.RasDialer();
    //    //稍后在断开连接时候需要用到
    //    private RasHandle Rashandler = null;
    //    public Form1()
    //    {
    //        InitializeComponent();
    //    }

    //    private void Form1_Load(object sender, EventArgs e)
    //    {
    //        book = new DotRas.RasPhoneBook();
    //        book.Open();

    //        dailer.StateChanged += new EventHandler(dailer_StateChanged);
    //        dailer.DialCompleted += new EventHandler(dailer_DialCompleted);
    //        dailer.SynchronizingObject = this;

    //    }

    //    void dailer_DialCompleted(object sender, DialCompletedEventArgs e)
    //    {
    //        if (e.Cancelled)
    //        {
    //            this.txtStatus.AppendText("Cancelled!");
    //        }
    //        else if (e.TimedOut)
    //        {
    //            this.txtStatus.AppendText("Connection attempt timed out!");
    //        }
    //        else if (e.Error != null)
    //        {
    //            this.txtStatus.AppendText(e.Error.ToString());
    //        }
    //        else if (e.Connected)
    //        {
    //            this.txtStatus.AppendText("Connection successful!");
    //        }

    //        if (!e.Connected)
    //        {
    //            this.btnDisconnect.Enabled = false;
    //        }
    //        else
    //        {
    //            this.btnDisconnect.Enabled = true;
    //        }
    //    }

    //    void dailer_StateChanged(object sender, StateChangedEventArgs e)
    //    {
    //        this.txtStatus.AppendText(e.State.ToString() + "\r\n");
    //    }

    //    private void button1_Click(object sender, EventArgs e)
    //    {
    //        //假设8.8.8.8为远程VPN服务器地址，vpn类型为pptp
    //        entry = RasEntry.CreateVpnEntry("VpnCreateByC#", "8.8.8.8", RasVpnStrategy.PptpOnly, RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn));
    //        book.Entries.Add(entry);
    //        txtStatus.Clear();
    //        //示例程序不做vpn连接已存在的exception处理
    //        dailer.EntryName = "VpnCreateByC#";
    //        dailer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
    //        try
    //        {
    //            dailer.Credentials = new System.Net.NetworkCredential("username", "password");
    //            //异步进行拨号，并把RAS的handler传递给定义好的Rashandler变量，以便在断开连接中进行处理
    //            Rashandler = dailer.DialAsync();
    //        }
    //        catch (Exception ex)
    //        { }
    //    }

    //    private void btnDisconnect_Click(object sender, EventArgs e)
    //    {
    //        if (dailer.IsBusy)
    //        { }
    //        else
    //        {
    //            RasConnection connection = RasConnection.GetActiveConnectionByHandle(this.Rashandler);
    //            if (connection != null)
    //            {
    //                connection.HangUp();
    //            }

    //        }
    //    }
    //}
}
