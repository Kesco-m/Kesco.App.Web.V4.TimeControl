<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmplDaysDetailsForm.aspx.cs" Inherits="Kesco.App.Web.TimeControl.Forms.EmplDaysDetailsForm" %>
<%@ Register TagPrefix="cs" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="cs" Namespace="Kesco.Lib.Web.Controls.V4.PagingBar" Assembly="Controls.V4" %>
<%@ Import Namespace="Kesco.App.Web.TimeControl" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title><%=Resx.GetString("hPageTitle")%></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
</head>
<body>
    <script type="text/javascript">
        isAddFunc = true;
        paramAddFunc = 1;
    </script>
    <table id="mtable" cellspacing="0" style="margin: auto" width="1035px" border="0">
    <tr height="10">
		<td align="left">
				<h2><%=Resx.GetString("hPageTitle")%></h2>
                <h2><%=Resx.GetString("hPageDetailsTitle")%></h2>
		</td>
        <td align="right" valign="top">
			<input runat="server" ID="btnHlp" type="button" style="BACKGROUND: url(/styles/Help.gif) no-repeat center center; height:20px; width:20px;" />
		</td>
	</tr>
	<tr>
	    <td style="WIDTH: 100%" colspan="2">
			<table width="100%" cellpadding="0" cellspacing="0" style="BORDER-COLLAPSE:collapse" border="0">
				<tr>
					<td style="WIDTH: 60%">
						<span id="EmployeeInfo" style="cursor: pointer" class="v4_callerControl" data-id="<%=EmployeeId %>" caller-type="2" onmouseout="mouseOut();"></span>
					</td>
					<td style="WIDTH: 30%"></td>
					<td style="WIDTH: 10%"></td>
					<td rowspan="2">
                        <input type="button" value="&nbsp;&nbsp;<%=Resx.GetString("listEmpl")%>" title="<%=Resx.GetString("listEmpl")%>"
                        style="BACKGROUND: url(/styles/DocPageBack.gif) no-repeat left center;height:25px; width:150px" onclick="ShowWaitLayer('GoBack');" />
                        <input type="button" value="<%=Resx.GetString("lRefresh")%>" title="<%=Resx.GetString("lRefresh")%>"
                        style="BACKGROUND: url(/styles/Search.gif) no-repeat left center;height:25px; width:150px" onclick="ShowWaitLayer('Refresh');" />
                        <input type="button" value="<%=Resx.GetString("lPrint")%>" title="<%=Resx.GetString("lPrint")%>" id="btnPrint"
                        style="BACKGROUND: url(/styles/Print.gif) no-repeat left center;height:25px; width:150px" 
                        onclick="v4_windowOpen('<%=Path%>?idpage=<%=IDPage%>&view=print');" />
					</td>
				</tr>
				<tr>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
								    <cs:PeriodTimePicker id="Period" HtmlID="Period" runat="server" OnChanged="PeriodChanged" IsRequired="True" />
								</td>
							</tr>
						</table>
					</td>   
					<td></td>
					<td></td>
					<td></td>
				</tr>
				<tr>
					<td>
					    <table>
					        <tr>
					            <td style="position: relative; top:-2px">
					                <%=Resx.GetString("lFlagCaption")%>:
					            </td>
                                <td>
                                    <cs:CheckBox id="cbNotDisplayEmpty" HtmlID="cbNotDisplayEmpty" runat="server" TabIndex="6" OnChanged="CbNotDisplayEmptyChanged" />
                                </td>
					        </tr>
					    </table>
					</td>
					<td valign="top" align="right">
					    <%=Resx.GetString("lRules")%>:&nbsp;
					</td>
				    <td colspan="2">
				        <input type="radio" runat="server" id="ep1" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'true');" />&nbsp;<%=Resx.GetString("lPrimaryEmpl")%><br />
					    <input type="radio" runat="server" id="ep2" name="empl_primary" onclick="cmd('cmd', 'SetPrimaryCalc', 'val', 'false');" />&nbsp;<%=Resx.GetString("lPrimaryComp")%>
					</td>
				</tr>
				<tr>
					<td colspan="4">
					    <cs:PagingBar id="pagerBar" runat="server"></cs:PagingBar>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
	    <td colspan="2">
			<div id="tableDiv" style="overflow: auto; height: 300px">
			    <div id="listTable" runat="server">
					<asp:datagrid id="intervalList" runat="server" AutoGenerateColumns="False" CssClass="grid" HorizontalAlign="Center" Width="100%">
						<HeaderStyle Font-Bold="True" Height="25px" CssClass="gridHeader" VerticalAlign="Top"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="LINK_CELL" ReadOnly="True">
								<HeaderStyle Width="15px"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="DAY_VALUE" ReadOnly="True" DataFormatString="{0:dd.MM.yyyy (ddd)}"></asp:BoundColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									<%=GetStartTimeHeaderText()%>
								</HeaderTemplate>
								<ItemTemplate>
									<%# DataBinder.Eval(Container, "DataItem.isEnterAfterExit")%>
									<%# DataBinder.Eval(Container, "DataItem.START_TIME") %>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									<%=GetEndTimeHeaderText()%>
								</HeaderTemplate>
								<ItemTemplate>
									<%# DataBinder.Eval(Container, "DataItem.END_TIME") %>
                                    <%# DataBinder.Eval(Container, "DataItem.isEnterAfterExit2")%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="INTERVAL" ReadOnly="True">
							    <ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="ABSENT_TIME" ReadOnly="True">
							    <ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="INTERNET_ACCESS_COUNT" ReadOnly="True"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="INTERNET_ACCESS_TOTALTIME" ReadOnly="True"></asp:BoundColumn>
						</Columns>
					</asp:datagrid>
                    </div>
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
<span id="wait_screen"></span>
</body>
</html>
