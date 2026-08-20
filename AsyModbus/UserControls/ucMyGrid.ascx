<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMyGrid.ascx.cs" Inherits="AsyModbus.UserControls.ucMyGrid" %>

<div class="my-grid-header">

    <asp:Label ID="lblKayitSayisi" runat="server"></asp:Label>

    <div class="my-grid-search">
        <span>Bul:</span>
        <asp:TextBox ID="txtBul" runat="server" ClientIDMode="Static"></asp:TextBox>
    </div>

</div>

<asp:GridView
    ID="grdMyGrid"
    runat="server"
    ClientIDMode="Static"
    AutoGenerateColumns="false"
    CssClass="my-grid"
    Width="100%"
    OnRowCommand="grdMyGrid_RowCommand">

    <Columns>
        <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:LinkButton
                    ID="btnAc"
                    runat="server"
                    Text="Aç"
                    CommandName="AC"
                    CommandArgument='<%# Eval("id") %>'>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<script src="../Scripts/ucMyGrid.js"></script>