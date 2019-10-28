<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeDetailsForm.aspx.cs" Inherits="Kesco.App.Web.TimeControl.Forms.TimeDetailsForm" %>
<%@ Import Namespace="System.Globalization" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title><%= Resx.GetString("hPageTitle") %></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
</head>
<body>
<%= RenderDocumentHeader() %>
<span id="wait_screen"></span>
<table width="100%" border="0">
    <tr height="10">
        <td align="left">
            <h2><%= Resx.GetString("hPageTitle") %></h2>
            <h2><%= Resx.GetString("hPageSubTitleDetails2") %></h2>
        </td>
        <td align="right" valign="top">
        </td>
    </tr>
    <tr>
        <td style="WIDTH: 100%" colspan="2">
            <table width="100%" border="0">
                <tr>
                    <td>
                        <span id="EmployeeInfo" style="cursor: pointer" class="v4_callerControl" data-id="<%= EmployeeId %>" caller-type="2" onmouseout="mouseOut();"></span>
                        <font size="4pt" color="#000088">&nbsp;<%= Resx.GetString("lFor") %>&nbsp;<%= StartPeriod.Day %>.<%= StartPeriod.Month %>.<%= StartPeriod.Year %>&nbsp;(<%= StartPeriod.ToString("dddd", CultureInfo.CreateSpecificCulture(CurrentUser.Language)) %>)</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%= LNoEntrance %>
                        <%= LNoExit %>
                    </td>
                </tr>
            </table>
            <span id="calc_type">
                <table width="100%" border="0">
                    <tr>
                        <td valign="top" align="right">
                            <%= Resx.GetString("lRules") %>:&nbsp;
                        </td>
                        <td width="170" rowspan="2">
                            <input type="radio" runat="server" id="ep1" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'true');"/>&nbsp;<%= Resx.GetString("lPrimaryEmpl") %><br/>
                            <input type="radio" runat="server" id="ep2" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'false');"/>&nbsp;<%= Resx.GetString("lPrimaryComp") %>
                        </td>
                    </tr>
                </table>
            </span>
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <b><%= Resx.GetString("lWorkCaption") %></b>
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <div id="listTableWork" runat="server">
                <asp:DataGrid id="intervalList" runat="server" Width="100%" HorizontalAlign="Center" CssClass="grid" AutoGenerateColumns="False">
                    <HeaderStyle Font-Bold="True" Height="25px" CssClass="gridHeader"></HeaderStyle>
                    <Columns>
                        <asp:BoundColumn DataField="PLACE" ReadOnly="True"></asp:BoundColumn>
                        <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center">
                            </ItemStyle>
                            <HeaderTemplate>
                                <%= Resx.GetString("cStartTime") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div id="<%= GetTimeDivID() %>"><%# DataBinder.Eval(Container, "DataItem.START_TIME") %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center">
                            </ItemStyle>
                            <HeaderTemplate>
                                <%= Resx.GetString("cEndTime") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div id="<%= GetTimeDivID2() %>"><%# DataBinder.Eval(Container, "DataItem.END_TIME") %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="INTERVAL" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <b><%= Resx.GetString("lAbsCaption") %></b>
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <div id="listTableAbs" runat="server">
                <asp:DataGrid id="intervalListAbs" runat="server" Width="100%" HorizontalAlign="Center" CssClass="grid" AutoGenerateColumns="False">
                    <HeaderStyle Font-Bold="True" Height="25px" CssClass="gridHeader"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center">
                            </ItemStyle>
                            <HeaderTemplate>
                                <%= Resx.GetString("cStartTimeAbs") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div id="<%= GetTimeDivID() %>"><%# DataBinder.Eval(Container, "DataItem.START_TIME") %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center">
                            </ItemStyle>
                            <HeaderTemplate>
                                <%= Resx.GetString("cEndTimeAbs") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div id="<%= GetTimeDivID() %>"><%# DataBinder.Eval(Container, "DataItem.END_TIME") %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="INTERVAL" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
            <span id="lookup"></span>
        </td>
    </tr>
</table>

</body>
</html>