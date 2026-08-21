<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMyGrid.ascx.cs" Inherits="AsyModbus.UserControls.ucMyGrid" %>

<div class="my-grid-header">


    
<div class="my-grid-count">
    <asp:Label ID="lblKayitSayisi" runat="server"></asp:Label>
</div>


    <div class="my-grid-tools">

        <div class="my-grid-page-size">
            <span>Göster:</span>

            <asp:DropDownList ID="ddlKayitSayisi" runat="server" ClientIDMode="Static">
                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                <asp:ListItem Text="100" Value="100"></asp:ListItem>
                <asp:ListItem Text="Tümü" Value="-1"></asp:ListItem>
            </asp:DropDownList>

            <span>Kayıt</span>
        </div>

        <div class="my-grid-search">
            <span>Bul:</span>
            <asp:TextBox ID="txtBul" runat="server" ClientIDMode="Static"></asp:TextBox>
        </div>

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


<div id="gridSayfalama" class="my-grid-pagination"></div>




<script src="../Scripts/ucMyGrid.js"></script>
