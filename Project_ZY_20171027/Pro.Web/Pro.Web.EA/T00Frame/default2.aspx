<%@ Page Title="" Language="C#" MasterPageFile="~/T00Frame/__MP_Dialog.master" AutoEventWireup="true"
    CodeFile="default2.aspx.cs" Inherits="T00Frame_default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script>

        var $ = function(strID) { return document.getElementById(strID); };
        function myfunc() {

            $("inputtxt").value = "aa " + Math.random();

        }



        function func1() {
            callback(32, $("inputtxt").value);
        }

        window.onload = function() { $("inputtxt").value = GetInitParam(); };
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <input type="text" id="inputtxt" />
    <hr />
    <input type="button" onclick="myfunc()" value="button" />
    <input type="button" onclick="func1()" value="button" />
</asp:Content>
