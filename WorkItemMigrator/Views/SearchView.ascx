<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs" Inherits="WorkItemMigrator.Views.SearchView" %>
<section id="search" class="content-wrapper clear-fix">
    <h2>Single Work Item Migration</h2>
    <p>Please enter a work item to migrate: </p>
    <asp:TextBox runat="server" ID="Id" ClientIDMode="Static" ValidationGroup="Confirmation" CausesValidation="true" />
    <asp:DropDownList runat="server" ID="Provider" DataSource="<%#Model.Providers%>" DataValueField="Key" DataTextField="Value" />
    <asp:Button runat="server" ID="submit" Text="Search" OnClick="SearchHandler" />
    <asp:RequiredFieldValidator runat="server" CssClass="field-validation-error" ID="WorkItemIdRequired" ControlToValidate="id" ErrorMessage="Please enter a work item id" />
</section>
