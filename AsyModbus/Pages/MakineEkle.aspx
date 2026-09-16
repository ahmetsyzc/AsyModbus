<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="MakineEkle.aspx.cs" Inherits="AsyModbus.Pages.MakineEkle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .arkaplan {
            background-color: #e1e1e1;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">

        <div class="row justify-content-center">

            <div class="col-12 col-md-10 col-lg-7 col-xl-6">

                <div class="card">

                    <div class="card-header text-center fw-bold bg-dark text-white">
                        Modbus Makine Ekleme Paneli
                    </div>

                    <div class="arkaplan card-body">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="ID :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtID"
                                    runat="server"
                                    CssClass="form-control"
                                    placeholder="ID Otomatik Belirlenir"
                                    Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Makine Adı :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtMakineAd"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="20" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Model Adı :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtModelAd"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Entegrasyon Kodu :" runat="server" CssClass="text-nowrap" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtEntegrasyonKod"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="GG Numarası :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtGgNo"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Makine Numarası :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtMakineNo"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Band Numarası :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtBandNo"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="IP :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtIp"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="MFG :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtMfg"
                                    runat="server"
                                    CssClass="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Röle Cihazı :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:DropDownList
                                    ID="ddlRoleCihaz"
                                    runat="server"
                                    CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlRoleCihaz_SelectedIndexChanged" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Röle Kanalı :" runat="server" />
                            </div>
                            <div class="col-12 col-md-9">
                                <asp:DropDownList
                                    ID="ddlRoleKanal"
                                    runat="server"
                                    CssClass="form-select" />
                            </div>
                        </div>

                        <div class="text-center">
                            <asp:Button
                                ID="btnKaydet"
                                runat="server"
                                Text="Kaydet"
                                CssClass="btn btn-success mt-2 px-5"
                                OnClick="btnKaydet_Click" />
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
