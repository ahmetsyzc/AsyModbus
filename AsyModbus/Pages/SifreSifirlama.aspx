<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SifreSifirlama.aspx.cs" Inherits="AsyModbus.Pages.SifreSifirlama" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Şifre Sıfırlama Sayfası</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="ortadiv">
            <table>

                <tr>
                    <td colspan="2">
                        <asp:Image CssClass="image"
                            ImageUrl="~/Files/Images/Icons/logo.png"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label Text="Modbus Şifre Sıfırlama Paneli" CssClass="girispanelyazisi" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtMail" placeholder="Mail Adresinizi Giriniz" TextMode="Email" CssClass="txtbox" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtCepNo" placeholder="Cep Telefonunuzu Giriniz" Style="margin: auto; width: 350px; height: 30px;" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Button ID="btnSifreSifirla" Text="Sıfırla" CssClass="grsButon" runat="server" OnClick="btnSifreSifirla_Click" />
                    </td>
                    <td>
                        <a href="Login.aspx">
                            <asp:Label Text="Geri Dön" CssClass="lblSifreSifirlama" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblUyari" ForeColor="DarkRed" runat="server" />
                    </td>
                </tr>


            </table>



        </div>
    </form>
</body>
</html>
