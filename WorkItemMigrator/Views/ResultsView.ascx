<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResultsView.ascx.cs" Inherits="WorkItemMigrator.Views.ResultsView" %>
<section id="results" data-workitem-id="0">
    <asp:HiddenField runat="server" ID="Query" />
    <asp:HiddenField runat="server" ID="SearchProvider" />
    <asp:DropDownList runat="server" ID="Repository" />

    <asp:CheckBox runat="server" ID="CloseItems" />
    <asp:TextBox runat="server" ID="Message"></asp:TextBox>
</section>
