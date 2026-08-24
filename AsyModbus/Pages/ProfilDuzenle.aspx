<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProfilDuzenle.aspx.cs" Inherits="AsyModbus.Pages.ProfilDuzenle" %>

<%@ Register Src="~/UserControls/ucCepNo.ascx"
    TagPrefix="uc"
    TagName="CepNo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciForm.css" rel="stylesheet" />
    <script src="../Scripts/KullaniciForm.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-12 col-md-10 col-lg-7 col-xl-6">
                <div class="card">

                    <div class="card-header text-center fw-bold bg-dark text-white">
                        Modbus Profil Düzenleme Paneli
                    </div>

                    <div class="arkaplan card-body">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Ad :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtAd"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Soyad :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtSoyad"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Mail :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtMail"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Email" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Şifre :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtSifre"
                                    runat="server"
                                    MaxLength="14"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Cep No :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <uc:CepNo ID="ucCepNo" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 text-center">

                                <asp:Image
                                    ID="imgProfil"
                                    runat="server"
                                    CssClass="profilResim" />

                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Profil Resmi :" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:FileUpload
                                    ID="FileUpload1"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                       <div class="row mb-3">
                            <div class="col-12 text-center">
                                <asp:Button
                                    ID="btnKaydet"
                                    runat="server"
                                    Text="Kaydet"
                                    CssClass="btn btn-success mt-2 px-5"
                                    OnClientClick="return KullaniciDogrula();"
                                    OnClick="btnKaydet_Click" />
                            </div>
                        </div>

                    </div>


                </div>
            </div>
        </div>
    </div>

</asp:Content>
