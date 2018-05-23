<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Kesco.App.Web.TimeControl.Forms.Default" %>
<%@ Import Namespace="Kesco.App.Web.TimeControl" %>
<%@ Register TagPrefix="cc" Namespace="Kesco.Lib.Web.DBSelect.V4" Assembly="DBSelect.V4" %>
<%@ Register TagPrefix="cs" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="cs" Namespace="Kesco.Lib.Web.Controls.V4.PagingBar" Assembly="Controls.V4" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title><%=Resx.GetString("hPageTitle")%></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
</head>
<body>
    <script type="text/javascript">
        isAddFunc = true;
        paramAddFunc = 0;
        cmdasync('cmd', 'SetTimeZoneOffSet', 'tz', new Date().getTimezoneOffset());
    </script>
    <table id="mtable" cellspacing="0" style="margin: auto" width="1035px" border="0">
		<tr id="mrow_1" height="10">
			<td align="left">
					<h2><%=Resx.GetString("hPageTitle")%></h2>
			</td>
            <td align="right" valign="top" style="width: 20px">
				<input runat="server" ID="btnHlp" type="button" tabindex="11" style="BACKGROUND: url(/styles/Help.gif) no-repeat center center; height:20px; width:20px;" />
			</td>
		</tr>
        <tr id="mrow_2">
            <td style="WIDTH: 100%" colspan="2">
				<table cellspacing="0" style="BORDER-COLLAPSE:collapse" width="100%">
					<tr>
						<td width="100%" colspan="3">
							<table width="100%" cellspacing="0" style="BORDER-COLLAPSE:collapse" border="0">
								<tr>
									<td nowrap valign="top">
                                        <%=Resx.GetString("lFilterCompany")%>:&nbsp;&nbsp;
                                    </td>
									<td width="40%" valign="top">
										<cc:DBSPerson id="Company"  Width="250" runat="server" HtmlID="Company" TabIndex="1" OnChanged="CompanyChanged" IsMultiSelect="True" 
                                        IsUseCondition="True" NextControl="Company" IsRemove="True" IsCaller="True" CallerType="Person" IsNotUseEmpty="True" AutoSetSingleValue="True"/>
									</td>
                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                    <td nowrap valign="top">
                                        <%=Resx.GetString("lFilterSubdivision")%>:&nbsp;&nbsp;
                                    </td>
									<td width="40%" valign="top">
										<cc:DBSSubdivision id="Subdivision" Width="250" runat="server" HtmlID="Subdivision" TabIndex="2" OnChanged="SubdivisionChanged" IsMultiSelect="True" 
                                        IsUseCondition="True" NextControl="Subdivision" IsRemove="True" IsItemCompany="True" AutoSetSingleValue="True"/>
									</td>
                                    <td>&nbsp;&nbsp;&nbsp;</td>
									<td valign="top" rowspan="2">
                                        <input type="button" value="<%=Resx.GetString("lRefresh")%>" title="<%=Resx.GetString("lRefreshAdd")%>"
                                        style="BACKGROUND: url(/styles/Search.gif) no-repeat left center;height:25px; width:150px" onclick="ShowWaitLayer('ClearCashRefresh');" />
                                        <input type="button" value="<%=Resx.GetString("lClear")%>" title="<%=Resx.GetString("lClearAdd")%>"
                                        style="BACKGROUND: url(/styles/Delete.gif) no-repeat left center;height:25px; width:150px" onclick="ShowWaitLayer('ClearFilter');" />
                                        <input type="button" value="<%=Resx.GetString("lPrint")%>" title="<%=Resx.GetString("lPrintAdd")%>" id="btnPrint"
                                        style="BACKGROUND: url(/styles/Print.gif) no-repeat left center;height:25px; width:150px; display: none;" 
                                        onclick="v4_windowOpen('<%=Path%>?idpage=<%=IDPage%>&view=print');" />
									</td>
								</tr>
								<tr>
									<td nowrap valign="top">
                                        <%=Resx.GetString("lFilterPost")%>:&nbsp;&nbsp;
                                    </td>
									<td width="40%" valign="top">
										<cc:DBSPosition id="Position" Width="250" runat="server" HtmlID="Position" TabIndex="3" OnChanged="PositionChanged" IsMultiSelect="True" 
                                        IsUseCondition="True" NextControl="Position" IsRemove="True" AutoSetSingleValue="True"/>
									</td>
                                    <td></td>
                                    <td nowrap valign="top">
                                        <%=Resx.GetString("lFilterEmplName")%>:&nbsp;&nbsp;
                                    </td>
									<td width="40%" valign="top">
                                        <cc:DBSEmployee id="EmplName" Width="250" runat="server" HtmlID="EmplName" TabIndex="4" OnChanged="EmplNameChanged" IsMultiSelect="True" 
                                        IsUseCondition="True" NextControl="EmplName" IsRemove="True" IsCaller="true" CallerType="Employee" IsMultiReturn="True" AdvSearchWindowWidth="1400" 
                                        IsNotUseEmpty="True" AutoSetSingleValue="True"/>
									</td>
                                    <td></td>
									<td></td>
								</tr>
							</table>
						</td>
					</tr>
                    <tr>
						<td width="60%">
							<table cellspacing="0">
								<tr>
								    <td>
								        <cs:PeriodTimePicker id="Period" HtmlID="Period" runat="server" OnChanged="PeriodChanged" TabIndex="5" IsRequired="True" />
								    </td>
                                    <td id="timeExit_TD1" style="DISPLAY:none;">
                                        <%=Resx.GetString("lTimeExit")%>:
                                    </td>
                                    <td id="timeExit_TD2" style="DISPLAY:none;">
                                        <cs:CheckBox id="FilterTimeExit" HtmlID="FilterTimeExit" runat="server" TabIndex="6" OnChanged="FilterTimeExitChanged" />
                                    </td>
                                    <td id="timeExit_TD3" style="DISPLAY:none;">
                                        <cs:Time id="TimeExit" HtmlID="TimeExit" runat="server" TabIndex="7" Width="40px" ValueTime="11:00" TimeFormat="HH:mm" NextControl="ep1" />
                                    </td>
								</tr>
							</table>
						</td>
						<td width="22%" align="right" valign="top">
							<%=Resx.GetString("lRules")%>:&nbsp;
						</td>
						<td width="18%">
							<input type="radio" runat="server" tabindex="8" id="ep1" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'true');" />&nbsp;<%=Resx.GetString("lPrimaryEmpl")%><br />
							<input type="radio" runat="server" tabindex="8" id="ep2" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'false');" />&nbsp;<%=Resx.GetString("lPrimaryComp")%>
						</td>
					</tr>
                    <tr>
						<td>
							<cs:PagingBar id="pagerBar" runat="server" TabIndex="9"></cs:PagingBar>
						</td>
                        <td colspan="2" align="right">
							<table cellspacing="0" style="BORDER-COLLAPSE:collapse">
							    <tr>
								    <td style="position: relative; top:-2px">
								        <%=Resx.GetString("lFilterEmplAvaible")%>:
								    </td>
                                    <td>
                                        <cs:CheckBox id="FilterEmplAvaible" HtmlID="FilterEmplAvaible" runat="server" TabIndex="10" OnChanged="FilterEmplAvaibleChanged" />
                                    </td>
								</tr>
								<tr>
								    <td style="position: relative; top:-2px">
								        <%=Resx.GetString("lFilterSubEmpl")%>:
								    </td>
                                    <td>
                                        <cs:CheckBox id="FilterSubEmpl" HtmlID="FilterSubEmpl" runat="server" TabIndex="10" OnChanged="FilterSubEmplChanged" />
                                    </td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
        </tr>
        <tr>
            <td colspan="2">
				<div id="tableDiv" style="overflow: auto; height: 300px">
					<span id="listTable" runat="server">
						<asp:DataGrid id="intervalList" runat="server" Width="100%" HorizontalAlign="Center" CssClass="grid" AutoGenerateColumns="False">
							<HeaderStyle Font-Bold="True" CssClass="gridHeader" Height="25px" VerticalAlign="Top"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="LinkCell" ReadOnly="True">
									<HeaderStyle Width="15px"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="EmplNameCol" ReadOnly="True"></asp:BoundColumn>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<HeaderTemplate>
										<%=GetStartTimeHeaderText()%>
									</HeaderTemplate>
									<ItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.isEnterAfterExit")%>
										<%# DataBinder.Eval(Container, "DataItem.StartTime")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<HeaderTemplate>
										<%=GetEndTimeHeaderText()%>
									</HeaderTemplate>
									<ItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.EndTime")%>
                                        <%# DataBinder.Eval(Container, "DataItem.isEnterAfterExit2")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Interval" ReadOnly="True">
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="AbsentTime" ReadOnly="True">
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="InternetAccessCount" ReadOnly="True"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="InternetAccessTotaltime" ReadOnly="True"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</span>
				</div>
                <table width="100%">
                    <tr>
                        <td style="width: 100%">
                            <div id="countDiv"></div>
                        </td>
                        <td nowrap="nowrap">
                            <div id="descDiv" align="right">
					            <%=Resx.GetString("lErrorDesc")%>
				            </div>
                        </td>
                    </tr>
                </table>
			</td>
        </tr>
    </table>
    <span id="lookup"></span>
    <div id="wait_screen" style="position: fixed"></div>
</body>
</html>
