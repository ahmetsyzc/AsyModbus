<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AsyModbus.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/Default.css" rel="stylesheet" />
    <script src="../Scripts/Default.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid py-4 mt-5">

        <div class="card shadow">

            <div class="card-header bg-dark text-white fw-bold text-center">
                Makine Kontrol Paneli
           
            </div>

            <div class="card-body">

                <asp:Repeater
                    ID="rptBantlar"
                    runat="server"
                    OnItemDataBound="rptBantlar_ItemDataBound">

                    <ItemTemplate>

                        <div class="card border-dark mb-3">

                            <div class="card-header bg-dark text-white fw-bold
            d-flex justify-content-between align-items-center">

                                <span>Bant <%# Eval("band_no") %> </span>

                                <asp:LinkButton
                                    ID="btnBantDuzenle"
                                    runat="server"
                                    CssClass="btn btn-light btn-sm"
                                    CommandArgument='<%# Eval("band_no") %>'
                                    OnClick="btnBantDuzenle_Click">
                                    <i class="fa-solid fa-pen"></i>
                                    Düzenle </asp:LinkButton>

                            </div>

                            <div class="card-body">

                                <div class="row g-3">

                                    <asp:Repeater ID="rptMakineler" runat="server">

                                        <ItemTemplate>

                                            <div class="makine-kolon">

                                                <div class="card border-secondary h-100">

                                                    <div class="card-header text-center">

                                                        <div class="fw-bold mb-1">
                                                            <%# Eval("makine_ad") %>
                                                        </div>
                                                        <div class="text-muted small mb-2">
                                                            Makine <%# Eval("makine_no") %>
                                                        </div>

                                                        <span class='<%# "badge " + DurumCssGetir(Eval("durum")) %>'>
                                                            <%# DurumMetniGetir(Eval("durum")) %>
                                                        </span>

                                                    </div>

                                                    <div class="card-body">

                                                        <p class="mb-2">
                                                            <strong>Model:</strong><br />
                                                            <%# Eval("model_ad") %>
                                                        </p>

                                                        <p class="mb-2">
                                                            <strong>IP:</strong><br />
                                                            <%# Eval("ip") %>
                                                        </p>

                                                        <p class="mb-3">
                                                            <strong>MFG:</strong>
                                                            <%# Eval("mfg") %>
                                                        </p>

                                                        <div class="d-flex gap-2">

                                                            <asp:LinkButton
                                                                ID="btnDetay"
                                                                runat="server"
                                                                CssClass="btn btn-outline-dark btn-sm"
                                                                ToolTip="Makine Detayı"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                OnClick="btnDetay_Click">
                                                                <i class="fa-solid fa-circle-info"></i>
                                                            </asp:LinkButton>

                                                            <asp:Button
                                                                ID="btnDurdur"
                                                                runat="server"
                                                                Text="Durdur"
                                                                CssClass="btn btn-danger btn-sm flex-grow-1"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                Enabled='<%# DurdurButonuAktifMi(Eval("durum")) %>'
                                                                OnClientClick="return confirm('Makineyi durdurmak istediğinize emin misiniz?');"
                                                                OnClick="btnDurdur_Click" />

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>

                                        </ItemTemplate>

                                    </asp:Repeater>

                                </div>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </div>

    </div>

    <div
        class="modal fade"
        id="makineDetayModal"
        tabindex="-1"
        aria-labelledby="makineDetayModalBaslik"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">

            <div class="modal-content">

                <div class="modal-header bg-dark text-white">

                    <h5
                        class="modal-title"
                        id="makineDetayModalBaslik">Makine Detayı
                    </h5>

                    <button
                        type="button"
                        class="btn-close btn-close-white"
                        data-bs-dismiss="modal"
                        aria-label="Kapat">
                    </button>

                </div>

                <div class="modal-body">

                    <div class="table-responsive">

                        <table class="table table-bordered align-middle mb-0">

                            <tbody>

                                <tr>
                                    <th>Makine Numarası</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayMakineNo"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>Güncel Durum</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayGuncelDurum"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>Kaynak</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayKaynak"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>İşlemi Yapan</th>

                                    <td>
                                        <asp:Label
                                            ID="lblDetayIslemiYapan"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>Önceki Durum</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayOncekiDurum"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>Yeni Durum</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayYeniDurum"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>İşlem Sonucu</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayIslemSonucu"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>İşlem Tarihi</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayIslemTarihi"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <th>Detay</th>
                                    <td>
                                        <asp:Label
                                            ID="lblDetayAciklama"
                                            runat="server">
                                        </asp:Label>
                                    </td>
                                </tr>

                            </tbody>

                        </table>

                    </div>

                </div>

                <div class="modal-footer">

                    <button
                        type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Kapat
                    </button>

                </div>

            </div>

        </div>

    </div>


    <%-- Bant içerisindeki makinelerin sıralanacağı modal. --%>
    <div
        class="modal fade"
        id="bantDuzenleModal"
        tabindex="-1"
        aria-labelledby="bantDuzenleModalBaslik"
        aria-hidden="true">

        <%-- Sıralama alanı geniş olacağı için modal-xl kullanıyoruz. --%>
        <div class="modal-dialog modal-dialog-centered modal-siralama">

            <div class="modal-content">

                <%-- Modal başlığı. --%>
                <div class="modal-header bg-dark text-white">

                    <h5
                        class="modal-title"
                        id="bantDuzenleModalBaslik">Bant
                    <asp:Label
                        ID="lblSiralamaBantNo"
                        runat="server">
                    </asp:Label>
                        Makine Sıralaması

                    </h5>

                    <%-- Sağ üst kapatma butonu. --%>
                    <button
                        type="button"
                        class="btn-close btn-close-white"
                        data-bs-dismiss="modal"
                        aria-label="Kapat">
                    </button>

                </div>

                <%-- Modalın içerik bölümü. --%>
                <div class="modal-body">

                    <div class="alert alert-info">
                        <i class="fa-solid fa-circle-info me-2"></i>
                        Makinelerin sırasını değiştirmek için kartları
                    sürükleyip istediğiniz konuma bırakınız.
                    </div>

                    <%-- Seçilen bandın numarasını C# tarafında saklar. --%>
                    <asp:HiddenField
                        ID="hfSiralamaBantNo"
                        runat="server"
                        ClientIDMode="Static" />

                    <%-- JavaScript tarafından oluşturulan yeni makine
                     sırasını saklar. --%>
                    <asp:HiddenField
                        ID="hfMakineSirasi"
                        runat="server" />

                    <%-- Sürüklenebilir makine kartlarının bulunduğu alan. --%>
                    <div
                        id="siralamaAlani"
                        class="row g-3">

                        <asp:Repeater
                            ID="rptSiralamaMakineler"
                            runat="server">

                            <ItemTemplate>

                                <%-- data-makine-id, makinenin veritabanındaki
                                 ID değerini JavaScript'e taşır. --%>
                                <div class="siralama-karti"
                                    data-makine-id='<%# Eval("id") %>'
                                    draggable="true">

                                    <div class="card border-dark h-100">

                                        <div class="card-body text-center p-2">

                                            <!-- Kartın sürüklenebildiğini gösteren ikon -->
                                            <i class="fa-solid fa-grip-vertical mb-2"></i>

                                            <!-- Makinenin adı -->
                                            <div class="fw-bold">
                                                <%# Eval("makine_ad") %>
                                            </div>

                                            <!-- Makine numarası -->
                                            <div class="small text-muted">
                                                Makine <%# Eval("makine_no") %>
                                            </div>

                                            <!-- Makine modeli -->
                                            <div class="small text-muted">
                                                <%# Eval("model_ad") %>
                                            </div>

                                        </div>

                                    </div>

                                </div>

                            </ItemTemplate>

                        </asp:Repeater>

                    </div>

                </div>

                <%-- Modalın alt buton bölümü. --%>
                <div class="modal-footer">

                    <asp:Button
                        ID="btnVarsayilanSirala"
                        runat="server"
                        Text="Varsayılana Dön"
                        CssClass="btn btn-outline-dark"
                        OnClientClick="return confirm('Makineleri makine numarasına göre yeniden sıralamak istediğinize emin misiniz?');"
                        OnClick="btnVarsayilanSirala_Click" />

                    <button
                        type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        İptal

                    </button>

                    <asp:Button
                        ID="btnSiralamaKaydet"
                        runat="server"
                        Text="Sıralamayı Kaydet"
                        CssClass="btn btn-success"
                        OnClientClick="makineSirasiniHazirla();"
                        OnClick="btnSiralamaKaydet_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:Content>
