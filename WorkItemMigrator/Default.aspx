<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WorkItemMigrator.DefaultView" %>

<%@ Register Src="~/Views/SearchView.ascx" TagPrefix="uc" TagName="Search" %>
<%@ Register Src="~/Views/ResultView.ascx" TagPrefix="uc" TagName="Result" %>

<!DOCTYPE html>
<html lang="en">
<head id="PageHeader" runat="server">
    <meta charset="utf-8" />
    <title>Work Item Migrator</title>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <link href="~/Content/Site.css" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width" />
</head>
<body>
    <form id="MigrationForm" runat="server">
    <div id="body">
        <section class="featured">
            <div class="content-wrapper">
                <hgroup class="title">
                    <h1>Work Item Migrator</h1>
                </hgroup>
            </div>
        </section>
        <div class="main-content clear-fix content-wrapper">
            <section id="messaging">
                <p id="message"><%= Model.Message ?? string.Empty %></p>
            </section>
            <uc:Search runat="server" />
            <uc:Result runat="server" />
        </div>
    </div>
    <footer>
        <div class="content-wrapper">
            <div class="float-left">
                <p>&copy; <%: DateTime.Now.Year %></p>
            </div>
        </div>
    </footer>
    <script>
        $(determineView());
        $(displayMessage());
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="Scripts/jquery-1.7.1.js" />
            <asp:ScriptReference Path="Scripts/jquery-ui-1.8.20.js" />
            <asp:ScriptReference Path="Scripts/site.js"/>
        </Scripts>
    </asp:ScriptManager>
    </form>
</body>
</html>
