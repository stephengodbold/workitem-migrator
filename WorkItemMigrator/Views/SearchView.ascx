<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs" Inherits="WorkItemMigrator.Views.SearchView" %>
<section id="search" class="content-wrapper clear-fix">
    <section>
        <h2>What are you looking for?</h2>
        <asp:TextBox runat="server" ID="Criteria" ClientIDMode="Static" ValidationGroup="Confirmation" CausesValidation="true" />
        <br />
        <asp:RequiredFieldValidator runat="server" CssClass="field-validation-error" ID="WorkItemIdRequired" ControlToValidate="Criteria"
            ErrorMessage="Please enter something for us to find!" />
    </section>
    <asp:Button runat="server" ID="go" ClientIDMode="Static" Text="Search" OnClick="SearchHandler" />
</section>
