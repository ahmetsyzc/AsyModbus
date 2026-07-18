<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="KullaniciListele.aspx.cs" Inherits="AsyModbus.Pages.KullaniciListele" %>

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
                <tr>
                    <th>Kullanıcı ID</th>
                    <th>Kullanıcı Ad</th>
                    <th>Kullanıcı Soyad</th>
                    <th>Kullanıcı TCKNO</th>
                    <th>Düzenle</th>
                </tr>

                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("kullanici_id") %></td>
                            <td><%# Eval("kullanici_ad") %></td>
                            <td><%# Eval("kullanici_soyad") %></td>
                            <td><%# Eval("kullanici_tckno") %></td>
                            <td>
                                <asp:Button ID="btnGuncelle" Text="Güncelle" runat="server" OnClick="btnGuncelle_Click" CommandArgument='<%# Eval("kullanici_id") %>' />
                            </td>
                        </tr>

                    </ItemTemplate>

                </asp:Repeater>

            </table>

        </div>
    </div>

</asp:Content>
