<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true" CodeFile="about.aspx.cs" Inherits="T00Frame_about" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        window.onload = function () { SetAutoClose(""); }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table width="100%">
        <tr>
            <td align="left">
                <table align="center">
                    <tr>
                        <td>
                            <img src="../images/frame/logo.png" width="170" /></td>
                    </tr>
                    <tr>
                        <td style="font-size: 10pt;">深圳市XXX技术有限公司</td>
                    </tr>
                    <tr>
                        <td align="center" height="40" valign="bottom" style="font-weight: bold; font-size: 14pt;"><%=GetAppTitle() %></td>
                    </tr>
                    <tr>
                        <td align="center"><%=GetAppVersion()%></td>
                    </tr>
                    <tr>
                        <td>
                            <hr>
                        </td>
                    </tr>
                    <tr>
                        <td>地　址： 深圳市XX区科技中二路深圳软件园</td>
                    </tr>
                    <tr>
                        <td>邮　编： 518057</td>
                    </tr>
                    <tr>
                        <td>电　话： 0755-88000000</td>
                    </tr>
                    <tr>
                        <td>传　真： 0755-88000000</td>
                    </tr>
                    <tr>
                        <td>热　线： 800-400-0000</td>
                    </tr>
                    <tr>
                        <td>邮　箱： <a href="mailto:info@kirisun.net.cn">info@XXX.com</a></td>
                    </tr>
                    <tr>
                        <td>网　址： <a href="http://www.baidu.com" target="_blank">www.baidu.com</a></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:Content>

