<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="MakineListele.aspx.cs" Inherits="AsyModbus.Pages.MakineListele" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx"
    TagPrefix="uc"
    TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">

        <div class="card">

            <div class="arkaplan card-header" style="background-color: #e1e1e1">
                <h4 class="mb-0">Makine Listesi</h4>
                <span class="text-muted">MODBUS Makine Yönetimi</span>
            </div>
            <div class="card-body" style="background-color: #e1e1e1">
                <uc:MyGrid
                    ID="ucMyGrid1"
                    runat="server" />
            </div>

        </div>
    </div>


</asp:Content>
