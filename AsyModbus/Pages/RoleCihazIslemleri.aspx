<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RoleCihazIslemleri.aspx.cs" Inherits="AsyModbus.Pages.RoleCihazIslemleri" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .arkaplan {
            background-color: #e1e1e1;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid py-4 mt-5">

        <div class="row justify-content-center">

            <div class="col-12 col-md-8 col-lg-6">

                <div class="arkaplan card shadow">

                    <div class="card-header bg-dark text-white text-center fw-bold">
                        Modbus Role Cihaz İşlemleri Paneli
                    </div>

                    <div class="card-body">

                        <div class="row mb-3">
                            <label for="ddlCihazlar" class="col-12 col-md-4 col-form-label fw-semibold">
                                Cihaz Seçiniz
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:DropDownList
                                    ID="ddlCihazlar"
                                    runat="server"
                                    CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlCihazlar_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtId" class="col-12 col-md-4 col-form-label fw-semibold">
                                Cihaz ID
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:TextBox
                                    ID="txtId"
                                    runat="server"
                                    CssClass="form-control"
                                    Enabled="false">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtAd" class="col-12 col-md-4 col-form-label fw-semibold">
                                Cihaz Adı
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:TextBox
                                    ID="txtAd"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="100">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtIp" class="col-12 col-md-4 col-form-label fw-semibold">
                                IP
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:TextBox
                                    ID="txtIp"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="20">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtPort" class="col-12 col-md-4 col-form-label fw-semibold">
                                Port
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:TextBox
                                    ID="txtPort"
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <label for="txtKanalSayisi" class="col-12 col-md-4 col-form-label fw-semibold">
                                Kanal Sayısı
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:TextBox
                                    ID="txtKanalSayisi"
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="d-flex justify-content-end gap-2">
                            <asp:Button
                                ID="btnEkle"
                                runat="server"
                                Text="Ekle"
                                CssClass="btn btn-success"
                                OnClick="btnEkle_Click" />

                            <asp:Button
                                ID="btnGuncelle"
                                runat="server"
                                Text="Güncelle"
                                CssClass="btn btn-warning"
                                OnClick="btnGuncelle_Click" />

                            <asp:Button
                                ID="btnSil"
                                runat="server"
                                Text="Sil"
                                CssClass="btn btn-danger"
                                OnClick="btnSil_Click"
                                OnClientClick="return confirm('Seçili cihazı silmek istediğinize emin misiniz?');" />
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>