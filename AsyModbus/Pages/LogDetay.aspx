<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="LogDetay.aspx.cs" Inherits="AsyModbus.Pages.LogDetay" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">

        <div class="row justify-content-center">

            <div class="col-12 col-md-10 col-lg-8 col-xl-7">

                <div class="card">

                    <div class="card-header text-center fw-bold bg-dark text-white">
                        Modbus Log Detay Paneli
                    </div>

                    <div class="arkaplan card-body">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Log ID:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtId" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Kullanıcı ID:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtEkleyenId" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Kullanıcı IP:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtEkleyenIp" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Eklenme Tarihi:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtEklenmeTarihi" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="İşlem Alanı:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtTabloAd" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="İşlem Adı:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIslemAd" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="İşlem Tipi:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIslemTip" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="URL:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtUrl" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" Enabled="false" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <asp:Label Text="Detay:" runat="server" />
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtDetay" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="13" Enabled="false" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-12 text-center">
                                <a href="LogListele.aspx" class="btn btn-dark px-5">Geri Dön</a>
                            </div>
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>