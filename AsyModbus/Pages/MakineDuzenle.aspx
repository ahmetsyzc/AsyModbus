<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master"
    AutoEventWireup="true"
    CodeBehind="MakineDuzenle.aspx.cs"
    Inherits="AsyModbus.Pages.MakineDuzenle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciForm.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-12 col-md-10 col-lg-7 col-xl-6">

                <div class="card">

                    <div class="card-header text-center fw-bold bg-dark text-white">
                        Modbus Makine D&#252;zenleme Paneli
                    </div>

                    <div class="arkaplan card-body">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="ID :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtID" runat="server"
                                    CssClass="form-control"
                                    Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Makine Ad&#305; :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtMakineAd" runat="server"
                                    CssClass="form-control"
                                    MaxLength="20" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Model Ad&#305; :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtModelAd" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Entegrasyon Kodu :" runat="server"
                                    CssClass="text-nowrap" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtEntegrasyonKod" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="GG Numaras&#305; :" runat="server"
                                    CssClass="text-nowrap" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtGgNo" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Makine Numaras&#305; :" runat="server"
                                    CssClass="text-nowrap" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtMakineNo" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Band Numaras&#305; :" runat="server"
                                    CssClass="text-nowrap" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtBandNo" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="IP :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIp" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="MFG :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtMfg" runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="text-center">
                            <asp:Button ID="btnKaydet" runat="server"
                                Text="Kaydet"
                                CssClass="btn btn-success mt-2 px-5 me-2"
                                OnClick="btnKaydet_Click" />

                            <asp:Button ID="btnSil" runat="server"
                                Text="Sil"
                                CssClass="btn btn-danger mt-2 px-5"
                                OnClick="btnSil_Click"
                                OnClientClick="return confirm('Bu makineyi silmek istedi&#287;inize emin misiniz?')" />
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </div>

</asp:Content>