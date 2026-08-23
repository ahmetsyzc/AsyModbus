<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SifreSifirlama.aspx.cs" Inherits="AsyModbus.Pages.SifreSifirlama" %>

<%@ Register Src="~/UserControls/ucCepNo.ascx"
    TagPrefix="uc"
    TagName="CepNo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Şifre Sıfırlama Sayfası</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="../Styles/ucCepNo.css" rel="stylesheet" />
    <script src="../Scripts/KullaniciForm.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div style="display: none;">

            <asp:Label ID="lbl_success" runat="server" ClientIDMode="Static" />
            <asp:Label ID="lbl_warning" runat="server" ClientIDMode="Static" />
            <asp:Label ID="lbl_info" runat="server" ClientIDMode="Static" />
            <asp:Label ID="lbl_error" runat="server" ClientIDMode="Static" />

        </div>

        <div class="container min-vh-100 d-flex align-items-center justify-content-center">

            <div class="row w-100 justify-content-center">

                <div class="col-12 col-md-7 col-lg-5 col-xl-4">

                    <div class="card shadow-sm">

                        <div class="card-body p-4">


                            <div class="text-center mb-4">
                                <asp:Image
                                    ImageUrl="~/Files/Images/Icons/logo.png"
                                    CssClass="img-fluid mb-3"
                                    runat="server" />

                                <div class="fw-bold fs-4">
                                    Modbus Şifre Sıfırlama Paneli
                                </div>
                            </div>

                            <div class="mb-3">
                                <asp:TextBox
                                    ID="txtMail"
                                    runat="server"
                                    TextMode="Email"
                                    CssClass="form-control"
                                    placeholder="Mail Adresinizi Giriniz" />
                            </div>

                            <div class="mb-3">
                                <uc:CepNo
                                    ID="ucCepNo"
                                    CssClass="form-control"
                                    runat="server" />
                            </div>

                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <asp:Button
                                    ID="btnSifreSifirla"
                                    runat="server"
                                    Text="Sıfırla"
                                    CssClass="btn btn-dark px-4"
                                    OnClientClick="return SifreSifirlamaDogrulama();"
                                    OnClick="btnSifreSifirla_Click" />

                                <a href="Login.aspx"
                                    class="text-dark text-decoration-none small">Geri Dön
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script>
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": true,
                "progressBar": true,
                "positionClass": "toast-top-full-width",
                "preventDuplicates": false,
                "onclick": null,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            };

            var successMesaji = document.getElementById("lbl_success").innerText;
            var warningMesaji = document.getElementById("lbl_warning").innerText;
            var infoMesaji = document.getElementById("lbl_info").innerText;
            var errorMesaji = document.getElementById("lbl_error").innerText;
            if (successMesaji != "") {
                toastr.success(successMesaji);
            }
            if (warningMesaji != "") {
                toastr.warning(warningMesaji);
            }
            if (infoMesaji != "") {
                toastr.info(infoMesaji);
            }
            if (errorMesaji != "") {
                toastr.error(errorMesaji);
            }
        </script>
    </form>

</body>

</html>
