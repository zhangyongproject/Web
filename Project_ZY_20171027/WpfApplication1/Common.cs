using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DotRas;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;

namespace WpfApplication1
{
    public  class VPNHelper
    {
        // 系统路径 C:\windows\system32\
        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\";
        // rasdial.exe
        private static string RasDialFileName = "rasdial.exe";
        // VPN路径 C:\windows\system32\rasdial.exe
        private static string VPNPROCESS = WinDir + RasDialFileName;
        // VPN地址
        public string IPToPing { get; set; }
        // VPN名称
        public string VPNName { get; set; }
        // VPN用户名
        //public string UserName { get; set; }
        // VPN密码
        //public string PassWord { get; set; }
        //加密等级
        public string EncryptionLevel { get; set; }
        //隧道类型
        public string TunnelType { get; set; }
        public RasVpnStrategy VpnStrategy { get; set; }
        public RasEncryptionType EncryptionType { get; set; }
        //L2tpPsk
        public string L2tpPsk { get; set; }
        //认证方式
        public string AuthenticationMethod { get; set; }
       //隧道分离
        public bool SplitTunneling { get; set; }
        public VPNHelper()
        {
            IPToPing                = "";
            VPNName                 = "";
            EncryptionLevel         = "";
            TunnelType              = "";
            L2tpPsk                 = "";
            AuthenticationMethod    = "";
            SplitTunneling          = false;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="_vpnIP"></param>
        /// <param name="_vpnName"></param>
        /// <param name="_userName"></param>
        /// <param name="_passWord"></param>
        public VPNHelper(string _vpnIP, string _vpnName, string _tunnelType, string _encryptionLevel, string _l2tpPsk, string _authenticationMethod, string _splitTunneling)
        {
            this.IPToPing = _vpnIP;
            this.VPNName = _vpnName;
            this.TunnelType = _tunnelType;
            this.EncryptionLevel = _encryptionLevel;
            this.L2tpPsk = _l2tpPsk;
            this.AuthenticationMethod = _authenticationMethod;
            this.SplitTunneling = _splitTunneling.ToLower()=="true";
        }
        /// <summary>
        /// 尝试连接VPN(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN()
        {
            //this.TryConnectVPN(this.VPNName, this.UserName, this.PassWord);
        }
        /// <summary>
        /// 尝试断开连接(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN()
        {
            this.TryDisConnectVPN(this.VPNName);
        }
        /// <summary>
        /// 创建或更新一个默认的VPN连接
        /// </summary>
        public void CreateOrUpdateVPN()
        {
            this.CreateOrUpdateVPN(this.VPNName, this.IPToPing);
        }
        /// <summary>
        /// 尝试删除连接(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDeleteVPN()
        {
            this.TryDeleteVPN(this.VPNName);
        }
        /// <summary>
        /// 尝试连接VPN(指定VPN名称，用户名，密码)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN(string connVpnName, string connUserName, string connPassWord)
        {
            try
            {
                string args = string.Format("{0} {1} {2}", connVpnName, connUserName, connPassWord);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);
            }
            catch (Exception Ex)
            {
                //LogUtil.Write("TryConnectVPN Exception:" + Ex);
            }
        }
        /// <summary>
        /// 尝试断开VPN(指定VPN名称)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN(string disConnVpnName)
        {
            try
            {
                string args = string.Format(@"{0} /d", disConnVpnName);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);

            }
            catch (Exception Ex)
            {
                //LogUtil.Write("TryDisConnectVPN Exception:" + Ex);
            }
        }
        /// <summary>
        /// 创建或更新一个VPN连接(指定VPN名称，及IP)
        /// </summary>
        public void CreateOrUpdateVPN(string updateVPNname, string updateVPNip)
        {
            try
            {
               
                switch (TunnelType.ToLower())
                {
                    case "pptp":
                        VpnStrategy = RasVpnStrategy.PptpOnly;
                        break;
                    case "l2tp":
                        VpnStrategy = RasVpnStrategy.L2tpOnly;
                        break;
                    case "sstp":
                        VpnStrategy = RasVpnStrategy.SstpOnly;
                        break;
                    case "ikev2":
                        VpnStrategy = RasVpnStrategy.IkeV2Only;
                        break;
                    case "automatic":
                        VpnStrategy = RasVpnStrategy.Default;
                        break;
                    default:
                        VpnStrategy = RasVpnStrategy.PptpOnly;
                        break;
                }
                switch (EncryptionLevel.ToLower())
                {
                    case "optional":
                        EncryptionType = RasEncryptionType.Optional;
                        break;
                    case "noencryption":
                        EncryptionType = RasEncryptionType.None;
                        break;
                    case "required":
                        EncryptionType = RasEncryptionType.Require;
                        break;
                    case "maximum":
                        EncryptionType = RasEncryptionType.RequireMax;
                        break;
                    default:
                        EncryptionType = RasEncryptionType.None;
                        break;
                }
                RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
                allUsersPhoneBook.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
                // 如果已经该名称的VPN已经存在，则更新这个VPN服务器地址
                if (allUsersPhoneBook.Entries.Contains(updateVPNname))
                {
                    allUsersPhoneBook.Entries[updateVPNname].PhoneNumber = updateVPNip;
                    allUsersPhoneBook.Entries[updateVPNname].EncryptionType = EncryptionType;
                    allUsersPhoneBook.Entries[updateVPNname].VpnStrategy = VpnStrategy;
                    allUsersPhoneBook.Entries[updateVPNname].Options.RemoteDefaultGateway = SplitTunneling;
                    allUsersPhoneBook.Entries[updateVPNname].Options.IPv6RemoteDefaultGateway = SplitTunneling;
                    switch (AuthenticationMethod.ToLower())
                    {
                        case "pap":
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequirePap = true;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireChap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSChap2 = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEncryptedPassword = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireDataEncryption = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSEncryptedPassword = false;
                            break;
                        case "eap":
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEap = true;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireChap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequirePap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSChap2 = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEncryptedPassword = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireDataEncryption = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSEncryptedPassword = false;
                            break;
                        case "chap":
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireChap = true;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequirePap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSChap2 = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEncryptedPassword = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireDataEncryption = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSEncryptedPassword = false;
                            break;
                        case "mschapv2":
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSChap2 = true;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequirePap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireChap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEncryptedPassword = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireDataEncryption = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSEncryptedPassword = false;
                            break;
                        default:
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequirePap = true;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireChap = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSChap2 = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireEncryptedPassword = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireDataEncryption = false;
                            allUsersPhoneBook.Entries[updateVPNname].Options.RequireMSEncryptedPassword = false;
                            break;

                    }
                }
                // 创建一个新VPN
                else
                {
                    RasEntry entry = RasEntry.CreateVpnEntry(updateVPNname, updateVPNip, VpnStrategy,  RasDevice.GetDevices().First(o=>o.DeviceType==RasDeviceType.Vpn));
                    entry.EncryptionType = EncryptionType;
                    entry.Options.RemoteDefaultGateway = SplitTunneling;
                    entry.Options.IPv6RemoteDefaultGateway = SplitTunneling;
                    switch (AuthenticationMethod.ToLower())
                    {
                        case "pap":
                            entry.Options.RequirePap = true;
                            entry.Options.RequireEap = false;
                            entry.Options.RequireChap = false;
                            entry.Options.RequireMSChap2 = false;
                            entry.Options.RequireEncryptedPassword = false;
                            entry.Options.RequireDataEncryption = false;
                            entry.Options.RequireMSEncryptedPassword = false;
                            break;
                        case "eap":
                            entry.Options.RequireEap = true;
                            entry.Options.RequireChap = false;
                            entry.Options.RequirePap = false;
                            entry.Options.RequireMSChap2 = false;
                            entry.Options.RequireEncryptedPassword = false;
                            entry.Options.RequireDataEncryption = false;
                            entry.Options.RequireMSEncryptedPassword = false;
                            break;
                        case "chap":
                            entry.Options.RequireChap = true;
                            entry.Options.RequirePap = false;
                            entry.Options.RequireEap = false;
                            entry.Options.RequireMSChap2 = false;
                            entry.Options.RequireEncryptedPassword = false;
                            entry.Options.RequireDataEncryption = false;
                            entry.Options.RequireMSEncryptedPassword = false;
                            break;
                        case "mschapv2":
                            entry.Options.RequireMSChap2 = true;
                            entry.Options.RequirePap = false;
                            entry.Options.RequireEap = false;
                            entry.Options.RequireChap = false;
                            entry.Options.RequireEncryptedPassword = false;
                            entry.Options.RequireDataEncryption = false;
                            entry.Options.RequireMSEncryptedPassword = false;
                            break;
                        default:
                            entry.Options.RequirePap = true;
                            entry.Options.RequireEap = false;
                            entry.Options.RequireChap = false;
                            entry.Options.RequireMSChap2 = false;
                            entry.Options.RequireEncryptedPassword = false;
                            entry.Options.RequireDataEncryption = false;
                            entry.Options.RequireMSEncryptedPassword = false;
                            break;

                    }
                    allUsersPhoneBook.Entries.Add(entry);
                }
                if (VpnStrategy == RasVpnStrategy.L2tpOnly)
                {
                    allUsersPhoneBook.Entries[updateVPNname].Options.UsePreSharedKey = true;
                    allUsersPhoneBook.Entries[updateVPNname].UpdateCredentials(RasPreSharedKey.Client, L2tpPsk);
                }
                // 不管当前VPN是否连接，服务器地址的更新总能成功，如果正在连接，则需要VPN重启后才能起作用
                allUsersPhoneBook.Entries[updateVPNname].Update();
                allUsersPhoneBook.Dispose();
            }
            catch (Exception ex)
            {
                //LogUtil.Write("CreateOrUpdateVPN Error:" + ex);
            }
        }
        /// <summary>
        /// 删除指定名称的VPN
        /// 如果VPN正在运行，一样会在电话本里删除，但是不会断开连接，所以，最好是先断开连接，再进行删除操作
        /// </summary>
        /// <param name="delVpnName"></param>
        public void TryDeleteVPN(string delVpnName)
        {
           // RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            allUsersPhoneBook.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
            //allUsersPhoneBook.Open(true);
            if (allUsersPhoneBook.Entries.Contains(delVpnName))
            {
                allUsersPhoneBook.Entries.Remove(delVpnName);
            }
        }
        /// <summary>
        /// 获取当前正在连接中的VPN名称
        /// </summary>
        public List<string> GetCurrentConnectingVPNNames()
        {
            List<string> ConnectingVPNList = new List<string>();
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe ";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;//不显示cmd窗口
            proIP.StartInfo.Verb = "RunAs";//以管理员方式
            proIP.Start();
            proIP.StandardInput.WriteLine(RasDialFileName);
            proIP.StandardInput.WriteLine("exit");
            // 命令行运行结果
            string strResult = proIP.StandardOutput.ReadToEnd();
            proIP.Close();
            // 用正则表达式匹配命令行结果，只限于简体中文系统哦^_^
            Regex regger = new Regex("(?<=已连接\r\n)(.*\n)*(?=命令已完成)", RegexOptions.Multiline);
            // 如果匹配，则说有正在连接的VPN
            if (regger.IsMatch(strResult))
            {
                string[] list = regger.Match(strResult).Value.ToString().Split('\n');
                for (int index = 0; index < list.Length; index++)
                {
                    if (list[index] != string.Empty)
                        ConnectingVPNList.Add(list[index].Replace("\r", ""));
                }
            }
            // 没有正在连接的VPN，则直接返回一个空List<string>
            return ConnectingVPNList;
        }
        /// <summary>
        /// 获取当前正在连接中的VPN名称
        /// </summary>
        public bool IsCurrentConnectingVPNName()
        {
            List<string> ConnectingVPNList = new List<string>();
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe ";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;//不显示cmd窗口
            proIP.StartInfo.Verb = "RunAs";//以管理员方式
            proIP.Start();
            proIP.StandardInput.WriteLine(RasDialFileName);
            proIP.StandardInput.WriteLine("exit");
            // 命令行运行结果
            string strResult = proIP.StandardOutput.ReadToEnd();
            proIP.Close();
            // 用正则表达式匹配命令行结果，只限于简体中文系统哦^_^

            if (strResult.IndexOf(VPNName) > 0)
                return true;
            else
                return false;
        }
    }
}
