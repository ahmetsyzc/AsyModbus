<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMyGrid.ascx.cs" Inherits="AsyModbus.UserControls.ucMyGrid" %>

<div class="my-grid-container">

    <div class="mb-3">

        <div class="mb-3 ">
            <asp:Label
                ID="lblKayitSayisi"
                runat="server"
                CssClass="d-inline-block w-100 bg-dark text-white fw-bold px-3 py-2 rounded">
        </asp:Label>
        </div>


        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">

            <div class="d-flex align-items-center gap-2">

                <span>Göster:</span>

                <asp:DropDownList ID="ddlKayitSayisi" runat="server" ClientIDMode="Static" CssClass="form-select form-select-sm">
                    <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="50" Value="50"></asp:ListItem>
                    <asp:ListItem Text="100" Value="100"></asp:ListItem>
                    <asp:ListItem Text="Tümü" Value="-1"></asp:ListItem>
                </asp:DropDownList>

                <span>Kayıt</span>
            </div>

            <div class="d-flex align-items-center gap-2">
                <span>Bul:</span>
                <asp:TextBox ID="txtBul" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm"></asp:TextBox>
            </div>

        </div>

    </div>

    <div class="my-grid-scroll overflow-auto">

        <asp:GridView
            ID="grdMyGrid"
            runat="server"
            ClientIDMode="Static"
            AutoGenerateColumns="false"
            CssClass="table table-hover table-bordered border-dark align-middle text-center my-grid"
            Width="100%"
            OnRowCommand="grdMyGrid_RowCommand">
            <HeaderStyle CssClass="table-dark" />
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton
                            ID="btnAc"
                            runat="server"
                            Text="Aç"
                            CssClass="btn btn-dark btn-sm"
                            CommandName="AC"
                            CommandArgument='<%# Eval("id") %>'>
                </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </div>


    <div id="gridSayfalama" class="d-flex justify-content-center gap-1 mt-auto pb-3"></div>

</div>


<script src="../Scripts/ucMyGrid.js"></script>
