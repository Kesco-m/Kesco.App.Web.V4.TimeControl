using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kesco.App.Web.TimeControl.Common;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Settings;
using Kesco.Lib.Web.SignalR;

namespace Kesco.App.Web.TimeControl.Forms
{
    /// <summary>
    ///     Детализация рабочего времени сотрудника
    /// </summary>
    public partial class EmplDaysDetailsForm : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!V4IsPostBack)
            {
                if (Request.QueryString["view"] == "print")
                {
                    Print();
                    return;
                }

                Filter.Clid = string.IsNullOrEmpty(Request.QueryString["clid"]) ? "0" : Request.QueryString["clid"];
                InitBindColumns();
                //SetProperty();
                SetHandlers();
                GetParams();
            }
        }

        private void UpdateTotalCount()
        {
            //JS.Write("window.status = '" + String.Format(Resx.GetString("lTotalFound"), _totalRecord) + "';");
            JS.Write("gi('countDiv').innerHTML = '" + string.Format(Resx.GetString("lTotalFound"), _totalRecord) +
                     "';");
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "Refresh":
                    RefreshList();
                    break;
                case "GoBack":
                    //JS.Write("window.location.assign('http://{4}{5}/Forms/Default.aspx?from={0}&to={1}&period={2}&prim={3}');",
                    //    HttpUtility.UrlEncode(Period.ValueFrom), HttpUtility.UrlEncode(Period.ValueTo), Period.ValuePeriod, PrimaryEmployeeCalc, 
                    //    V4Request.Url.Host, V4Request.ApplicationPath);
                    JS.Write("window.location.assign('http://{0}{1}/Forms/Default.aspx?isback=1');", V4Request.Url.Host,
                        V4Request.ApplicationPath);
                    break;
                case "PageClose":
                    SaveParams(param["paramSave"]);
                    break;
                case "setSortOrder":
                    ClickSortColumn(param["col"]);
                    JS.Write("ShowWaitLayer('Refresh');");
                    break;
                case "SetPrimaryCalc":
                    PrimaryEmployeeCalc = bool.Parse(param["val"]);
                    SetGrayStyle();
                    break;
                case "showLookup":
                    ShowLookup(param["id"]);
                    break;
                case "call":
                    DoMakeCall(param["number"], param["inter"], param["phone"]);
                    break;
                case "openDetails":
                    RedirectToDetails(param["id"], param["from"]);
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        private void RefreshList()
        {
            CompleteDaysList();
            TextWriter stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            if (_tempPeriodsList.Rows.Count > 0 && Period.ValuePeriod !=
                ((int) PeriodsEnum.Undefined).ToString(CultureInfo.InvariantCulture) &&
                Period.ValueDateTo != null && !Period.ValueDateTo.Value.Equals(string.Empty) &&
                (Period.ValueDateFrom != null && !Period.ValueDateFrom.Value.Equals(string.Empty) ||
                 Period.ValuePeriod == ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture)))
            {
                intervalList.RenderControl(writer);
                var tableContent = stringWriter.ToString().Replace("</table>", _totalTag).Replace("\n", "")
                    .Replace("\r", "").Replace("\t", "").Replace("'", "\\'");

                JS.Write("document.getElementById('listTable').innerHTML = '{0}';", tableContent);
                pagerBar.SetDisabled(false);
                pagerBar.RowsPerPage = RowsPerPageSetting;
            }
            else
            {
                JS.Write("document.getElementById('listTable').innerHTML = '<P align=\\'center\\'><BR>{0}</P>';",
                    Resx.GetString("lNoData"));
                pagerBar.SetDisabled(true, false);
            }

            //JS.Write("HideWaitLayer();");
            JS.Write("di('btnPrint');SetTableDetailsSize();");
            if (!JS.ToString().Contains("v4_setToolTip()"))
                JS.Write("v4_setToolTip();");
            //JS.Write("UTC2Local();");
            JS.Write(SourceCash.IsError ? "di('descDiv');" : "hi('descDiv');");
            SetEmployeeInfo();
            UpdateTotalCount();
           // Period.DisableListing(false);
        }

        protected void Print()
        {
            var idpage = Request.QueryString["idpage"];
            if (!string.IsNullOrEmpty(idpage))
            {
                var p = KescoHub.GetPage(idpage);
                if (p != null)
                {
                    var dt = ((EmplDaysDetailsForm) p).SourceCash.GetSourceTable(!((EmplDaysDetailsForm) p)
                        ._notDisplayEmpty);
                    if (dt.Rows.Count > 0)
                    {
                        WrResponse =
                            string.Format("<html><head><title>{0}</title></head><body onload=\"window.print()\">",
                                Resx.GetString("lPrint"));
                        WrResponse += "<b>" + Resx.GetString("hPageDetailsTitle") + "</b> (" +
                                      DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ")";
                        WrResponse += "<div style=\"margin-top:10px;margin-bottom:10px;\"><b>" +
                                      ((EmplDaysDetailsForm) p).SourceCash.GetEmployeeName(IsRusLocal) + "</b></div>";
                        WrResponse += "<table style=\"border-collapse:collapse;\">";
                        WrResponse += string.Format(
                            "<tr style=\"height: 25px; font-weight: bold; text-align: center;\"><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                            Resx.GetString("lblDate"), Resx.GetString("cStartTime"), Resx.GetString("cEndTime"),
                            Resx.GetString("cInterval"), Resx.GetString("сAbsent"));
                        foreach (DataRow row in dt.Rows)
                        {
                            string descr, start = "", end = "", work = "", notwork = "";
                            var isRedDay = ((EmplDaysDetailsForm) p).IsRedDay((DateTime) row["DAY_VALUE"], out descr);
                            var dateValue = string.Format(
                                "{0}(<font color=\"{2}\">{1}</font>)&nbsp<font color=\"{2}\">{3}</font>",
                                ((DateTime) row["DAY_VALUE"]).ToString("dd.MM.yyyy"),
                                ((DateTime) row["DAY_VALUE"]).ToString("ddd",
                                    IsRusLocal ? new CultureInfo("ru-RU") : new CultureInfo("en-US")),
                                isRedDay ? "#FF0000" : "#000000",
                                descr);
                            if (!string.IsNullOrEmpty(row[0].ToString()))
                            {
                                start = row[2].ToString();
                                end = row[3].ToString();
                                work = row[5].ToString() == "00:00:00" ? "" : row[5].ToString();
                                notwork = row[4].ToString() == "00:00:00" ? "" : row[4].ToString();
                            }

                            WrResponse += "<tr>";
                            WrResponse +=
                                "<td style=\"border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                                dateValue + "</td>";
                            WrResponse +=
                                "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                                start + "</td>";
                            WrResponse +=
                                "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                                end + "</td>";
                            WrResponse +=
                                "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                                work + "</td>";
                            WrResponse += "<td style=\"text-align: center;border-bottom: darkgray 1px solid;\">" +
                                          notwork + "</td>";
                            WrResponse += "</tr>";
                        }

                        if (dt.Rows.Count > 1)
                            WrResponse += string.Format(
                                "<tr><td colspan='3' align='right'><b>{0}&nbsp</b></td><td align='center'><b>{1}:{2}:{3}</b></td><td align='center'><b>{4}:{5}:{6}</b></td></tr>",
                                Resx.GetString("lTotal"),
                                ((EmplDaysDetailsForm) p)._summaryWorkTime.Hours +
                                ((EmplDaysDetailsForm) p)._summaryWorkTime.Days * 24,
                                ((EmplDaysDetailsForm) p)._summaryWorkTime.Minutes.ToString("D2"),
                                ((EmplDaysDetailsForm) p)._summaryWorkTime.Seconds.ToString("D2"),
                                ((EmplDaysDetailsForm) p)._summaryAbsentTime.Hours +
                                ((EmplDaysDetailsForm) p)._summaryAbsentTime.Days * 24,
                                ((EmplDaysDetailsForm) p)._summaryAbsentTime.Minutes.ToString("D2"),
                                ((EmplDaysDetailsForm) p)._summaryAbsentTime.Seconds.ToString("D2"));
                        WrResponse += "</table>";
                        if (((EmplDaysDetailsForm) p).SourceCash.IsError) WrResponse += Resx.GetString("lErrorDesc");
                        WrResponse += "</body></html>";
                    }
                }
            }
        }

        private void CompleteDaysList()
        {
            _tempPeriodsList = GetDataTable();

            if (Period.ValuePeriod == ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture))
                SourceCash.CompleteDaysTable(Period.ValueDateFrom, Period.ValueDateFrom, EmployeeId, Tz);
            else
                SourceCash.CompleteDaysTable(Period.ValueDateFrom, Period.ValueDateTo, EmployeeId, Tz);

            var tAllRows = SourceCash.GetSourceTable(!_notDisplayEmpty);

            _summaryWorkTime = SourceCash.GetSummaryWorkTime();
            _summaryAbsentTime = SourceCash.GetSummaryAbsentTime();

            _totalRecord = tAllRows.Rows.Count;

            var sortTagDayValue = string.Empty;
            SortTagStartTime = string.Empty;
            SortTagEndTime = string.Empty;
            var sortTagAbsentTime = string.Empty;
            var sortTagInterval = string.Empty;
            var sortTagInternetAccessCount = string.Empty;
            var sortTagInternetAccessTotal = string.Empty;

            foreach (SortInfoItem item in SortParams)
                if (SortParams.IndexOf(item) == 0)
                {
                    var strUpEnabledGif = string.Format("<IMG SRC=\"{0}ScrollUpEnabled.GIF\" border=0>", Global.Styles);
                    var strDownEnabledGif =
                        string.Format("<IMG SRC=\"{0}ScrollDownEnabled.gif\" border=0>", Global.Styles);

                    switch (item.ColumnName)
                    {
                        case "DAY_VALUE":

                            if (item.ItemSortType == SortType.Asc)
                                sortTagDayValue = sortTagDayValue + strUpEnabledGif;
                            else
                                sortTagDayValue = sortTagDayValue + strDownEnabledGif;
                            break;

                        case "START_TIME":

                            if (item.ItemSortType == SortType.Asc)
                                SortTagStartTime = SortTagStartTime + strUpEnabledGif;
                            else
                                SortTagStartTime = SortTagStartTime + strDownEnabledGif;
                            break;

                        case "END_TIME":

                            if (item.ItemSortType == SortType.Asc)
                                SortTagEndTime = SortTagEndTime + strUpEnabledGif;
                            else
                                SortTagEndTime = SortTagEndTime + strDownEnabledGif;
                            break;

                        case "ABSENT_TIME_SORT":

                            if (item.ItemSortType == SortType.Asc)
                                sortTagAbsentTime = sortTagAbsentTime + strUpEnabledGif;
                            else
                                sortTagAbsentTime = sortTagAbsentTime + strDownEnabledGif;
                            break;

                        case "INTERVAL_SORT":

                            if (item.ItemSortType == SortType.Asc)
                                sortTagInterval = sortTagInterval + strUpEnabledGif;
                            else
                                sortTagInterval = sortTagInterval + strDownEnabledGif;
                            break;
                        case "INTERNET_ACCESS_COUNT":
                            if (item.ItemSortType == SortType.Asc)
                                sortTagInternetAccessCount = sortTagInternetAccessCount + strUpEnabledGif;
                            else
                                sortTagInternetAccessCount = sortTagInternetAccessCount + strDownEnabledGif;
                            break;
                        case "INTERNET_ACCESS_TOTALTIME":
                            if (item.ItemSortType == SortType.Asc)
                                sortTagInternetAccessTotal = sortTagInternetAccessTotal + strUpEnabledGif;
                            else
                                sortTagInternetAccessTotal = sortTagInternetAccessTotal + strDownEnabledGif;
                            break;
                    }
                }

            bcDayValue.HeaderText = string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'DAY_VALUE');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'DAY_VALUE');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("lblDate"), sortTagDayValue);
            bcAbsentTime.HeaderText = string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'ABSENT_TIME_SORT');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'ABSENT_TIME_SORT');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("сAbsent"), sortTagAbsentTime);
            bcInterval.HeaderText = string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERVAL_SORT');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERVAL_SORT');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cInterval"), sortTagInterval);
            bcInternetAccessCount.HeaderText = string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERNET_ACCESS_COUNT');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERNET_ACCESS_COUNT');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cInternetAccessCount"), sortTagInternetAccessCount);
            bcInternetAccessTotalTime.HeaderText = string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERNET_ACCESS_TOTALTIME');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'INTERNET_ACCESS_TOTALTIME');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cInternetAccessTotalTime"), sortTagInternetAccessTotal);

            var pageNo = pagerBar.Disabled ? 1 : pagerBar.CurrentPageNumber - 1;
            var pageRowCount = pagerBar.Disabled ? RowsPerPageSetting : pagerBar.RowsPerPage;
            pageRowCount = pageRowCount == 0 ? 35 : pageRowCount;

            var rows = tAllRows.Select("", SortString);
            var lastPageNo = (rows.Length - 1) / pageRowCount;
            pagerBar.MaxPageNumber = lastPageNo + 1;

            if (rows.Length > 0)
            {
                if (pageNo > lastPageNo) pageNo = lastPageNo;

                int copyLen;
                if (pageNo < lastPageNo)
                    copyLen = pageRowCount;
                else
                    copyLen = rows.Length - pageRowCount * pageNo;

                for (var i = pageNo * pageRowCount; i < pageNo * pageRowCount + copyLen; i++)
                    _tempPeriodsList.Rows.Add(rows[i].ItemArray);
            }

            _tempPeriodsList.DefaultView.Sort = SortString;
            if (tAllRows.Rows.Count > 1)
                _totalTag = string.Format(
                    "<tr><td colspan='4' align='right'><b>{0}&nbsp</b></td><td align='center'><b>{1}:{2}:{3}</b></td><td align='center'><b>{4}:{5}:{6}</b></td></tr></table>",
                    Resx.GetString("lTotal"),
                    _summaryWorkTime.Hours + _summaryWorkTime.Days * 24,
                    _summaryWorkTime.Minutes.ToString("D2"),
                    _summaryWorkTime.Seconds.ToString("D2"),
                    _summaryAbsentTime.Hours + _summaryAbsentTime.Days * 24,
                    _summaryAbsentTime.Minutes.ToString("D2"),
                    _summaryAbsentTime.Seconds.ToString("D2"));
            else
                _totalTag = "</table>";

            intervalList.DataSource = _tempPeriodsList;
            intervalList.DataBind();

            foreach (DataGridItem item in intervalList.Items)
            {
                var index = item.DataSetIndex;
                if (index < _tempPeriodsList.Rows.Count)
                {
                    string descr;
                    var isRedDay = IsRedDay((DateTime) _tempPeriodsList.Rows[index]["DAY_VALUE"], out descr);
                    var dateValue = string.Format(
                        "{0}(<font color=\"{2}\">{1}</font>)&nbsp<font color=\"{2}\">{3}</font>",
                        ((DateTime) _tempPeriodsList.Rows[index]["DAY_VALUE"]).ToString("dd.MM.yyyy"),
                        ((DateTime) _tempPeriodsList.Rows[index]["DAY_VALUE"]).ToString("ddd",
                            IsRusLocal ? new CultureInfo("ru-RU") : new CultureInfo("en-US")),
                        isRedDay ? "#FF0000" : "#000000",
                        descr);
                    item.Cells[1].Text = "<div>" + dateValue + "</div>";
                }
            }
        }

        private void RedirectToDetails(string empl, string startDate)
        {
            JS.Write(
                "v4_windowOpen('TimeDetailsForm.aspx?id={0}&date={1}&prim={2}&personid={3}&tz={4}', '_blank', 'left=340,top=240,width=750,height=700,resizable=yes ,scrollbars=yes');",
                empl, HttpUtility.UrlEncode(startDate), PrimaryEmployeeCalc, PersonId, Tz);
        }

        private bool IsRedDay(DateTime val, out string descr)
        {
            descr = string.Empty;
            var res = val.DayOfWeek == DayOfWeek.Saturday || val.DayOfWeek == DayOfWeek.Sunday;
            var info = SourceCash.GetHolidayInfo(val);

            if (info != null)
            {
                res = info.РабочийВыходной == 0;
                // MTODO: Отсутствует англоязычный вариант названия праздника. Как внедрят необходимо поправить.
                descr = info.Название;
            }

            return res;
        }

        /// <summary>
        ///     Сохранение периода в модели данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PeriodChanged(object sender, ProperyChangedEventArgs e)
        {
            if (e.IsChange && e.OldValue == "cmd" && (e.NewValue == "prev" || e.NewValue == "next"))
            {
                pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
                RefreshList();
                return;
            }

            pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
            SetGrayStyle();
        }

        /// <summary>
        ///     Отображать/не отображать пустые строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbNotDisplayEmptyChanged(object sender, ProperyChangedEventArgs e)
        {
            pagerBar.CurrentPageNumber = 1;
            _notDisplayEmpty = cbNotDisplayEmpty.Value.Equals("1");
            RefreshList();
        }

        protected void SetEmployeeInfo()
        {
            JS.Write("EmployeeInfo.innerHTML = '{0}';",
                string.Format("<font size=4pt color=#000088 onclick=\"ItemClick(\\'{1}\\', \\'{2}\\');\">{0}</font>",
                    SourceCash.GetEmployeeName(IsRusLocal), EmployeeId, Config.user_form));
        }

        private void SetProperty()
        {
            EmployeeId = Request.QueryString["id"];
            Tz = Request.QueryString["tz"];
            PersonId = new ExecQuery().GetPersonIdByEmployeeId(EmployeeId);
            Period.ValuePeriod = Request.QueryString["period"];
            PrimaryEmployeeCalc = Request.QueryString["prim"] == "True";
            Period.ValueFrom = Request.QueryString["from"];
            Period.ValueTo = Request.QueryString["to"];

            if (PrimaryEmployeeCalc)
                ep1.Checked = true;
            else
                ep2.Checked = true;
            HelpUrl = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath +
                      "/Forms/hlp/help.htm?id=1";
        }

        private void GetParams()
        {
            SetProperty();
            Filter = new ExecQuery().GetSettings(Filter);

            cbNotDisplayEmpty.Checked = _notDisplayEmpty = Filter.IsDetailNoEmpty;
            if (Filter.RowsPerPageDetails > 0)
                pagerBar.RowsPerPage = RowsPerPageSetting = Filter.RowsPerPageDetails;
            else
                pagerBar.RowsPerPage = RowsPerPageSetting;
            pagerBar.MaxPageNumber = DefaultMaxPage;
            pagerBar.CurrentPageNumber = CurrentPageSetting;
            pagerBar.MaxPageNumber = 1;

            if (Request.QueryString["prim"] == null)
                PrimaryEmployeeCalc = Filter.IsEmployeePrimary;
            if (PrimaryEmployeeCalc)
                ep1.Checked = true;
            else
                ep2.Checked = true;

            if (Request.QueryString["period"] == null)
            {
                if (Filter.PeriodInfo.DateFrom == null || Filter.PeriodInfo.DateTo == null)
                {
                    Period.ValuePeriod = ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture);
                    Period.ValueDateFrom = DateTime.Now;
                }
                else
                {
                    Period.ValuePeriod = Filter.PeriodInfo.PeriodType ?? "";
                    if (Period.ValuePeriod != ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture))
                    {
                        Period.ValueDateFrom = Filter.PeriodInfo.DateFrom;
                        Period.ValueDateTo = Filter.PeriodInfo.DateTo;
                    }
                    else
                    {
                        Period.ValueDateFrom = DateTime.Now;
                    }
                }
            }

            var param = "";
            if (Filter.PairCallerPosition != null || Filter.PairCallerSize != null)
                param += string.Format("['|Phone',{0},{1},{2},{3}],",
                    Filter.PairCallerPosition == null ? "" : Filter.PairCallerPosition.First,
                    Filter.PairCallerPosition == null ? "" : Filter.PairCallerPosition.Second,
                    Filter.PairCallerSize == null ? "" : Filter.PairCallerSize.First,
                    Filter.PairCallerSize == null ? "" : Filter.PairCallerSize.Second);
            if (Filter.PairAlertPosition != null || Filter.PairAlertSize != null)
                param += string.Format("['|Alert',{0},{1},{2},{3}],",
                    Filter.PairAlertPosition == null ? "" : Filter.PairAlertPosition.First,
                    Filter.PairAlertPosition == null ? "" : Filter.PairAlertPosition.Second,
                    Filter.PairAlertSize == null ? "" : Filter.PairAlertSize.First,
                    Filter.PairAlertSize == null ? "" : Filter.PairAlertSize.Second);
            if (!string.IsNullOrEmpty(param) && param.Length > 1)
            {
                param = param.Remove(param.Length - 1, 1);
                JS.Write("itemsParam = [{0}];", param);
            }

            SetEmployeeInfo();
            RefreshList();
            JS.Write("SetTableDetailsSize();");
        }

        private void SaveParams(string paramSave)
        {
            var arr = paramSave.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
            Filter.PeriodInfo.PeriodType = Period.ValuePeriod;
            Filter.PeriodInfo.DateFrom = Period.ValueDateFrom;
            Filter.PeriodInfo.DateTo = Period.ValueDateTo;

            Filter.RowsPerPageDetails = RowsPerPageSetting;
            Filter.IsDetailNoEmpty = cbNotDisplayEmpty.Checked;
            Filter.IsEmployeePrimary = PrimaryEmployeeCalc;

            foreach (var s in arr)
                if (!string.IsNullOrEmpty(s))
                {
                    var param = s.Split(new[] {","}, StringSplitOptions.None);
                    if (param.Length > 0 && param[0] == "Phone")
                    {
                        if (param.Length > 2)
                            Filter.PairCallerPosition = new Pair(param[1], param[2]);
                        if (param.Length > 4)
                            Filter.PairCallerSize = new Pair(param[3], param[4]);
                    }

                    if (param.Length > 0 && param[0] == "Alert")
                    {
                        if (param.Length > 2)
                            Filter.PairAlertPosition = new Pair(param[1], param[2]);
                        if (param.Length > 4)
                            Filter.PairAlertSize = new Pair(param[3], param[4]);
                    }
                }

            new ExecQuery().SaveSettings(Filter);
        }

        private void SetHandlers()
        {
            pagerBar.CurrentPageChanged += OnCurrentPageChanged;
            pagerBar.RowsPerPageChanged += OnRowsPerPage;
        }

        private void ClickSortColumn(string columnName)
        {
            SortInfoItem resultItem = null;
            foreach (SortInfoItem item in SortParams)
                if (item.ColumnName.Equals(columnName))
                {
                    resultItem = item;
                    break;
                }

            if (resultItem == null)
            {
                SortParams.Insert(0, new SortInfoItem(SortType.Asc, columnName));
            }
            else if (resultItem.ItemSortType == SortType.Asc && SortParams.IndexOf(resultItem) == 0)
            {
                resultItem.ItemSortType = SortType.Desc;
            }
            else
            {
                SortParams.Remove(resultItem);
                SortParams.Insert(0, new SortInfoItem(SortType.Asc, columnName));
            }
        }

        private void SetGrayStyle()
        {
            SourceCash.ClearCash();
            pagerBar.SetDisabled(true, false);
            JS.Write("$('#btnPrint').hide();");
            JS.Write("if (document.getElementById('intervalList')) document.getElementById('intervalList').className = 'Grid8Grayed';");
        }

        private void ShowLookup(string emplId)
        {
            JS.Write("mouseOver('{0}', '{1}');", emplId, Global.UserPhoto);
        }

        private void OnCurrentPageChanged(object sender, EventArgs args)
        {
            CurrentPageSetting = pagerBar.CurrentPageNumber;
            RefreshList();
            //JS.Write("ShowWaitLayer('Refresh');");
        }

        private void OnRowsPerPage(object sender, EventArgs args)
        {
            RowsPerPageSetting = pagerBar.RowsPerPage;
            CurrentPageSetting = pagerBar.CurrentPageNumber = 1;
            RefreshList();
            //JS.Write("ShowWaitLayer('Refresh');");
        }

        protected string GetStartTimeHeaderText()
        {
            return string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'START_TIME');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'START_TIME');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cStartTime"), SortTagStartTime);
        }

        protected string GetEndTimeHeaderText()
        {
            return string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'END_TIME');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'END_TIME');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cEndTime"), SortTagEndTime);
        }

        private void InitBindColumns()
        {
            foreach (DataGridColumn bcol in intervalList.Columns)
                if (bcol is BoundColumn)
                {
                    var col = (BoundColumn) bcol;
                    switch (col.DataField)
                    {
                        case "DAY_VALUE":
                            bcDayValue = col;
                            break;
                        case "START_TIME":
                            bcStartTime = col;
                            break;
                        case "END_TIME":
                            bcEndTime = col;
                            break;
                        case "ABSENT_TIME":
                            bcAbsentTime = col;
                            break;
                        case "INTERVAL":
                            bcInterval = col;
                            break;
                        case "LINK_CELL":
                            bcLinkCell = col;
                            break;
                        case "INTERNET_ACCESS_COUNT":
                            bcInternetAccessCount = col;
                            break;
                        case "INTERNET_ACCESS_TOTALTIME":
                            bcInternetAccessTotalTime = col;
                            break;
                        case "isEnterAfterExit":
                            bcIsEnterAfterExit = col;
                            break;
                        case "isEnterAfterExit2":
                            bcIsEnterAfterExit2 = col;
                            break;
                    }
                }
        }

        private DataTable GetDataTable()
        {
            var tPeriodsList = new DataTable();

            LINK_CELL = new DataColumn("LINK_CELL", typeof(string));
            DAY_VALUE = new DataColumn("DAY_VALUE", typeof(DateTime));
            START_TIME = new DataColumn("START_TIME", typeof(string));
            END_TIME = new DataColumn("END_TIME", typeof(string));
            ABSENT_TIME = new DataColumn("ABSENT_TIME", typeof(string));
            INTERVAL = new DataColumn("INTERVAL", typeof(string));
            ABSENT_TIME_SORT = new DataColumn("ABSENT_TIME_SORT", typeof(TimeSpan));
            INTERVAL_SORT = new DataColumn("INTERVAL_SORT", typeof(TimeSpan));
            ERROR_CSS = new DataColumn("ERROR_CSS", typeof(string));
            INTERNET_ACCESS_COUNT = new DataColumn("INTERNET_ACCESS_COUNT", typeof(string));
            INTERNET_ACCESS_TOTALTIME = new DataColumn("INTERNET_ACCESS_TOTALTIME", typeof(string));
            isEnterAfterExit = new DataColumn("isEnterAfterExit", typeof(string));
            isEnterAfterExit2 = new DataColumn("isEnterAfterExit2", typeof(string));

            tPeriodsList.TableName = "tPeriodsList";
            tPeriodsList.Columns.Add(LINK_CELL);
            tPeriodsList.Columns.Add(DAY_VALUE);
            tPeriodsList.Columns.Add(START_TIME);
            tPeriodsList.Columns.Add(END_TIME);
            tPeriodsList.Columns.Add(ABSENT_TIME);
            tPeriodsList.Columns.Add(INTERVAL);
            tPeriodsList.Columns.Add(ABSENT_TIME_SORT);
            tPeriodsList.Columns.Add(INTERVAL_SORT);
            tPeriodsList.Columns.Add(ERROR_CSS);
            tPeriodsList.Columns.Add(INTERNET_ACCESS_COUNT);
            tPeriodsList.Columns.Add(INTERNET_ACCESS_TOTALTIME);
            tPeriodsList.Columns.Add(isEnterAfterExit);
            tPeriodsList.Columns.Add(isEnterAfterExit2);

            return tPeriodsList;
        }

        #region Private Members

        public string EmployeeId = string.Empty;
        public string PersonId = string.Empty;

        protected string SortTagStartTime = string.Empty;
        protected string SortTagEndTime = string.Empty;

        protected int CurrentPageSetting = 1;
        protected int RowsPerPageSetting = 35;
        protected int DefaultMaxPage = 99999;
        protected DaysTableCash Dtc;
        protected SortInfoSetting Info;

        protected DataColumn LINK_CELL;
        protected DataColumn DAY_VALUE;
        protected DataColumn START_TIME;
        protected DataColumn END_TIME;
        protected DataColumn ABSENT_TIME;
        protected DataColumn INTERVAL;
        protected DataColumn ABSENT_TIME_SORT;
        protected DataColumn INTERVAL_SORT;
        protected DataColumn ERROR_CSS;
        protected DataColumn INTERNET_ACCESS_COUNT;
        protected DataColumn INTERNET_ACCESS_TOTALTIME;
        protected DataColumn isEnterAfterExit;
        protected DataColumn isEnterAfterExit2;
        protected BoundColumn bcDayValue;
        protected BoundColumn bcStartTime;
        protected BoundColumn bcEndTime;
        protected BoundColumn bcAbsentTime;
        protected BoundColumn bcInterval;
        protected BoundColumn bcLinkCell;
        protected BoundColumn bcInternetAccessCount;
        protected BoundColumn bcInternetAccessTotalTime;
        protected BoundColumn bcIsEnterAfterExit;
        protected BoundColumn bcIsEnterAfterExit2;

        private int _totalRecord;
        private DataTable _tempPeriodsList;
        private bool _notDisplayEmpty;
        private string _totalTag = string.Empty;
        private TimeSpan _summaryWorkTime;
        private TimeSpan _summaryAbsentTime;

        private DaysTableCash SourceCash => PrimaryEmployeeCalc ? DaysListCashEmpl : DaysListCashComp;

        private DaysTableCash DaysListCashEmpl
        {
            get
            {
                if (Dtc == null || !Dtc.PrimaryEmployeeCalc) Dtc = new DaysTableCash(GetDataTable(), true);
                return Dtc;
            }
        }

        private DaysTableCash DaysListCashComp
        {
            get
            {
                if (Dtc == null || Dtc.PrimaryEmployeeCalc) Dtc = new DaysTableCash(GetDataTable(), false);
                return Dtc;
            }
        }

        private SortInfoSetting SortParams
        {
            get
            {
                if (Info == null) Info = new SortInfoSetting();
                return Info;
            }
        }

        private string SortString
        {
            get
            {
                var result = string.Empty;

                foreach (SortInfoItem itemp in SortParams)
                {
                    if (!result.Equals(string.Empty)) result = result + ", ";

                    result = result + itemp.ColumnName;
                    if (itemp.ItemSortType == SortType.Desc) result = result + " DESC";
                }

                if (result.Equals(string.Empty)) result = "DAY_VALUE";
                return result;
            }
        }

        #endregion

        /// <summary>
        ///     Подготовка данных для отрисовки заголовка страницы(панели с кнопками)
        /// </summary>
        /// <returns></returns>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    ClearMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException("Не удалось сформировать кнопки формы: " + e.Message, e);
                    Logger.WriteEx(dex);
                    throw dex;
                }

                return w.ToString();
            }
        }
    }
}