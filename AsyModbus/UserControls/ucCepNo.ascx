<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucCepNo.ascx.cs" Inherits="AsyModbus.UserControls.ucCepNo" %>

<asp:TextBox
    ID="txtCepNo"
    runat="server"
    TextMode="SingleLine"
    MaxLength="14"
    ClientIDMode="Static"
    placeholder="(5__) ___-____" />