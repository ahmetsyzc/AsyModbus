<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciListele.aspx.cs" Inherits="AsyModbus.Pages.KullaniciListele" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx"
    TagPrefix="uc"
    TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciListele.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">

        <div class="card">

            <div class="arkaplan card-header">
                <h4 class="mb-0">Kullanıcı Listesi</h4>
                <span class="text-muted">MODBUS kullanıcı yönetimi</span>
            </div>

            <div class="arkaplan card-body">
                    <uc:MyGrid
                        ID="ucMyGrid1"
                        runat="server"
                        OnButonaBasildi="ucMyGrid1_ButonaBasildi" />
            </div>

        </div>
    </div>

</asp:Content>
