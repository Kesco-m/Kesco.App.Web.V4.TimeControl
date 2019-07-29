<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Kesco.App.Web.TimeControl.Forms.Error" %>

<html>
<head>
    <title><%= Resx.GetString("alertError") %></title>
</head>
<body>
<% RenderError(Response.Output); %>
</body>
</html>