<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AsyModbus.Pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Sayfası</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="ortadiv">
            <table>

                <tr>
                    <td colspan="2">
                        <asp:Image  cssclass="image"
                            ImageUrl="~/Files/Images/Icons/logo.png"
                            runat="server" />
                    </td>
                </tr>  
                   <tr>
                     <td></td>
                </tr>  
                <tr>
                    <td colspan="2" > <asp:Label Text="Modbus Kullanıcı Giriş Paneli" CssClass="girispanelyazisi" runat="server" /> </td>
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
                         <asp:TextBox ID="txtSifre" placeholder="Şifrenizi Giriniz" TextMode="Password" CssClass="txtbox" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Button ID="btnGiris" Text="Giriş Yap" CssClass="grsButon" runat="server" OnClick="btnGiris_Click"  />
                    </td>
                     <td>
                         <a href="SifreSifirlama.aspx">
                        <asp:Label ID="lblSifreSifirlama" Text="Şifremi Unuttum" CssClass="lblSifreSifirlama" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label  ID="lblUyari" ForeColor="DarkRed" runat="server"  />
                    </td>
                </tr>


            </table>



        </div>
    </form>
</body>
</html>
