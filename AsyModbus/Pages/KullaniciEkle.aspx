<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciEkle.aspx.cs" Inherits="AsyModbus.Pages.KullaniciEkle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciForm.css" rel="stylesheet" />
    <script src="../Scripts/KullaniciForm.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div class="kd-div">
        <table class="kd-table">
            <tr>
                <td colspan="2" class="baslik">
                    <span>Modbus Kullanıcı Ekleme Paneli </span>
                </td>
            </tr>
            <tr style="height: 10px;">
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="ID :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtID" runat="server" placeholder="ID Otomatik Belirlenir" Enabled="false" />
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label Text="Kullanıcı Kodu :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtKullaniciKod" runat="server" placeholder="Kullanıcı Kodu Otomatik Belirlenir" Enabled="false" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Ad :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtAd" runat="server" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Soyad :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtSoyad" runat="server" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="TCKNO :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtTckno" runat="server" TextMode="SingleLine" MaxLength="11" ClientIDMode="Static" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Mail :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtMail" runat="server" TextMode="Email" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Şifre :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtSifre" runat="server" placeholder="Şifre Otomatik Belirlenir" Enabled="false" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Cep No :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtCepNo" runat="server" TextMode="SingleLine" MaxLength="14" ClientIDMode="Static" placeholder="(5__) ___-____"/>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Doğum Tarihi :" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtDogumTarihi" runat="server" TextMode="Date" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Rol Seçimi :" runat="server" />
                </td>
                <td>
                    <asp:DropDownList ID="DropDownList1" runat="server"></asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label Text="Profil Resmi :" runat="server" />
                </td>
                <td>
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                </td>
            </tr>



            <tr>
                <td colspan="2" style="text-align: center;">
                    <asp:Button ID="btnKaydet" CssClass="btnkaydet" Text="Kaydet" runat="server"  OnClientClick="return KullaniciDogrula();" OnClick="btnKaydet_Click" />
                </td>
            </tr>

            <tr>
                <td colspan="2" class="lbluyari" >
                    <asp:Label ID="lblUyari" runat="server" />
                </td>
            </tr>

        </table>
    </div>

</asp:Content>
