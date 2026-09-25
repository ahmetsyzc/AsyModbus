<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ParametreIslemleri.aspx.cs" Inherits="AsyModbus.Pages.ParametreIslemleri" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .arkaplan {
            background-color: #e1e1e1;
        }

        .bolum-ayirici {
            border-top: 1px solid #bdbdbd;
            margin: 1.25rem 0 1.25rem 0;
            padding-top: 1.25rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid py-4 mt-5">

        <div class="row justify-content-center">

            <div class="col-12 col-md-10 col-lg-8">

                <div class="arkaplan card shadow">

                    <div class="card-header bg-dark text-white text-center fw-bold">
                        Parametre İşlemleri Paneli
                    </div>

                    <div class="card-body">

                        <div class="row mb-3">
                            <label for="ddlParametreGruplari" class="col-12 col-md-4 col-form-label fw-semibold">
                                Parametre Grubu
                            </label>
                            <div class="col-12 col-md-8">
                                <asp:DropDownList
                                    ID="ddlParametreGruplari"
                                    runat="server"
                                    CssClass="form-select"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlParametreGruplari_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <asp:Panel ID="pnlParametreBolumu" runat="server" Visible="false">

                            <div class="row mb-3">
                                <label for="ddlParametreler" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Parametre
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:DropDownList
                                        ID="ddlParametreler"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlParametreler_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label for="txtParametreId" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Parametre ID
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtParametreId" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label for="txtParametreKod" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Parametre Kodu
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtParametreKod" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label for="txtParametreAd" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Parametre Adı
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtParametreAd" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-4">
                                <label for="txtParametreAciklama" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Parametre Açıklama
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtParametreAciklama" runat="server" CssClass="form-control" MaxLength="250"></asp:TextBox>
                                </div>
                            </div>

                            <div class="d-flex justify-content-end gap-2">
                                <asp:Button ID="btnParametreEkle" runat="server" Text="Parametre Ekle" CssClass="btn btn-success" OnClick="btnParametreEkle_Click" />
                                <asp:Button ID="btnParametreGuncelle" runat="server" Text="Parametre Güncelle" CssClass="btn btn-warning" OnClick="btnParametreGuncelle_Click" />
                                <asp:Button ID="btnParametreSil" runat="server" Text="Parametre Sil" CssClass="btn btn-danger" OnClick="btnParametreSil_Click"
                                    OnClientClick="return confirm('Seçili parametreyi silmek istediğinize emin misiniz?');" />
                            </div>

                        </asp:Panel>

                        <asp:Panel ID="pnlGrupAlanlari" runat="server">

                            <div class="row mb-3">
                                <label for="txtGrupId" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Grup ID
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtGrupId" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label for="txtGrupKod" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Grup Kodu
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtGrupKod" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label for="txtGrupAd" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Grup Adı
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtGrupAd" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mb-4">
                                <label for="txtGrupAciklama" class="col-12 col-md-4 col-form-label fw-semibold">
                                    Grup Açıklama
                                </label>
                                <div class="col-12 col-md-8">
                                    <asp:TextBox ID="txtGrupAciklama" runat="server" CssClass="form-control" MaxLength="250"></asp:TextBox>
                                </div>
                            </div>

                            <div class="d-flex justify-content-end gap-2">
                                <asp:Button ID="btnGrupEkle" runat="server" Text="Grup Ekle" CssClass="btn btn-success" OnClick="btnGrupEkle_Click" />
                                <asp:Button ID="btnGrupGuncelle" runat="server" Text="Grup Güncelle" CssClass="btn btn-warning" OnClick="btnGrupGuncelle_Click" />
                                <asp:Button ID="btnGrupSil" runat="server" Text="Grup Sil" CssClass="btn btn-danger" OnClick="btnGrupSil_Click"
                                    OnClientClick="return confirm('Seçili grubu silmek istediğinize emin misiniz?');" />
                            </div>

                        </asp:Panel>

                    </div>
                </div>

            </div>
        </div>
    </div>

</asp:Content>