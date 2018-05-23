<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="Kesco.App.Web.TimeControl.Forms.test" %>
<%@ Register TagPrefix="cs" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Kesco.Lib.Web.Controls.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script type="text/javascript">
        if (window.innerWidth == undefined) {
            document.write('<link href="/Styles/Kesco.V4/CSS/Kesco.ConfirmIE8.css" rel="stylesheet" type="text/css"/>');
            document.write('<scr' + 'ipt src="/Styles/Kesco.V4/JS/Kesco.ConfirmIE8.js" type="text/javascript"></scr' + 'ipt>');
        } else {
            document.write('<link href="/Styles/Kesco.V4/CSS/Kesco.Confirm.css" rel="stylesheet" type="text/css"/>');
            document.write('<scr' + 'ipt src="/Styles/Kesco.V4/JS/Kesco.Confirm.js" type="text/javascript"></scr' + 'ipt>');
        }
    </script>
     <cs:PeriodTimePicker id="Period" HtmlID="Period" runat="server" IsRequired="True" />
    <% RenderAlertConfirm(Response.Output); %>
</body>
</html>
