<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemView.ascx.cs" Inherits="WorkItemMigrator.Views.ItemView" %>
<section id="detail" class="content-wrapper clear-fix" data-workitem-id="<%= Model.Item.Id %>">
    <h2>Item Migration</h2>    
    <div id="sectionHeading" class="workitem-heading"><%: Model.Item.Id %>: <%: Model.Item.Title %></div>
    <asp:HiddenField runat="server" ID="WorkItemId" Value="<%# Model.Item.Id %>" />
    <asp:HiddenField runat="server" ID="SearchProvider" Value="<%# Model.SearchProvider%>"/>
    <ul class="field-list">
        <li id="description">
            <%= Model.Item.Description %>
        </li>
        <li class="further-information">To verify any other details please see the work item <a href="<%= Model.Item.Url %>">here</a>
        </li>
    </ul>
    <ul class="field-list">
        <li>Migrate to <asp:DropDownList runat="server" ID="Provider" DataSource="<%#Model.Repositories%>" DataValueField="Key" DataTextField="Value" /></li>
        <li>Close Item? <asp:CheckBox runat="server" ID="CloseItem" Checked="False"/></li>
        <li>Comment <br/> <asp:TextBox runat="server" ID="Message" Height="100px" TextMode="MultiLine" /></li>
    </ul>
    <asp:Button runat="server" ID="confirm" Text="Confirm" OnClick="MigrateHandler" />
    <asp:Button runat="server" ID="cancelMigration" Text="Cancel" OnClick="CancelHandler" />
</section>
