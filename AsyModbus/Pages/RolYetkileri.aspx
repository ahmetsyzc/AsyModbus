<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RolYetkileri.aspx.cs" Inherits="AsyModbus.Pages.RolYetkileri" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container py-4 mt-5">

        <div class="card shadow">

            <div class="card-header bg-dark text-white fw-bold text-center">
                Rol Yetkileri Yönetim Paneli
           
            </div>

            <div class="card-body">

                <div class="row mb-4">

                    <label class="col-12 col-md-3 col-form-label">
                        Rol
                   
                    </label>

                    <div class="col-12 col-md-9">

                        <asp:DropDownList
                            ID="ddlRoller"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlRoller_SelectedIndexChanged">
                        </asp:DropDownList>

                    </div>

                </div>

                <div class="table-responsive">

                    <table class="table table-bordered border-dark align-middle text-center">

                        <thead class="table-dark">

                            <tr>
                                <th class="text-start">Sayfa</th>
                                <th>Getirme</th>
                                <th>Ekleme</th>
                                <th>Güncelleme</th>
                                <th>Silme</th>
                            </tr>

                        </thead>

                        <tbody>

                            <asp:Repeater
                                ID="rptYetkiler"
                                runat="server">

                                <ItemTemplate>

                                    <tr>

                                        <td class="text-start">

                                            <asp:HiddenField
                                                ID="hfYetkiId"
                                                runat="server"
                                                Value='<%# Eval("id") %>' />

                                            <asp:HiddenField
                                                ID="hfSayfaAdi"
                                                runat="server"
                                                Value='<%# Eval("sayfa_adi") %>' />

                                            <asp:Label
                                                ID="lblSayfaAdi"
                                                runat="server"
                                                Text='<%# Eval("sayfa_adi") %>'>
                                            </asp:Label>

                                        </td>

                                        <td>

                                            <asp:CheckBox
                                                ID="chkGetirme"
                                                runat="server"
                                                Checked='<%# Convert.ToBoolean(Eval("getirme")) %>' />

                                        </td>

                                        <td>

                                            <asp:CheckBox
                                                ID="chkEkleme"
                                                runat="server"
                                                Checked='<%# Convert.ToBoolean(Eval("ekleme")) %>' />

                                        </td>

                                        <td>

                                            <asp:CheckBox
                                                ID="chkGuncelleme"
                                                runat="server"
                                                Checked='<%# Convert.ToBoolean(Eval("guncelleme")) %>' />

                                        </td>

                                        <td>

                                            <asp:CheckBox
                                                ID="chkSilme"
                                                runat="server"
                                                Checked='<%# Convert.ToBoolean(Eval("silme")) %>' />

                                        </td>

                                    </tr>

                                </ItemTemplate>

                            </asp:Repeater>

                        </tbody>

                    </table>

                </div>

                <div class="text-end mt-3">

                    <asp:Button
                        ID="btnKaydet"
                        runat="server"
                        Text="Kaydet"
                        CssClass="btn btn-success"
                        Visible="false"
                        OnClick="btnKaydet_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:Content>
