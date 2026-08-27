<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RolIslemleri.aspx.cs" Inherits="AsyModbus.Pages.RolIslemleri" %>

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
                        Modbus Rol İşlemleri Paneli
                   
                    </div>

                    <div class="card-body">

                        <%-- Rol seçimi --%>
                        <div class="row mb-3">

                            <label
                                for="ddlRoller"
                                class="col-12 col-md-4 col-form-label fw-semibold">
                                Rol Seçiniz
                           
                            </label>

                            <div class="col-12 col-md-8">
                                <asp:DropDownList
                                    ID="ddlRoller"
                                    runat="server"
                                    CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlRoller_SelectedIndexChanged">
                                </asp:DropDownList>

                            </div>

                        </div>

                        <%-- Rol ID --%>
                        <div class="row mb-3">

                            <label
                                for="txtId"
                                class="col-12 col-md-4 col-form-label fw-semibold">
                                Rol ID
                           
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

                        <%-- Rol adı --%>
                        <div class="row mb-4">

                            <label
                                for="txtAd"
                                class="col-12 col-md-4 col-form-label fw-semibold">
                                Rol Adı
                           
                            </label>

                            <div class="col-12 col-md-8">

                                <asp:TextBox
                                    ID="txtAd"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="50">
                                </asp:TextBox>

                            </div>

                        </div>

                        <%-- Butonlar --%>
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
                                Visible="false"
                                OnClick="btnGuncelle_Click" />

                            <asp:Button
                                ID="btnSil"
                                runat="server"
                                Text="Sil"
                                CssClass="btn btn-danger"
                                Visible="false"
                                OnClick="btnSil_Click"
                                OnClientClick="return confirm('Seçili rolü silmek istediğinize emin misiniz?');" />

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
