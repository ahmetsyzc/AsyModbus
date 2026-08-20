<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciListele.aspx.cs" Inherits="AsyModbus.Pages.KullaniciListele" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx"
    TagPrefix="uc"
    TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciListele.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="ana-div">

        <div class="panelBaslik">
            <h2>Kullanıcı Listesi</h2>
            <span>MODBUS kullanıcı yönetimi</span>
        </div>

        <div class="tabloScroll">
            <table class="personeltablo">

                <uc:MyGrid
                ID="ucMyGrid1"
                runat="server"
                OnButonaBasildi="ucMyGrid1_ButonaBasildi" />

            </table>
        </div>
    </div>

</asp:Content>
