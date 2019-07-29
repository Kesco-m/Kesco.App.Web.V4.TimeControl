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
using Kesco.Lib.BaseExtention.Enums.Corporate;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Settings;
using Item = Kesco.Lib.Entities.Item;
using Page = Kesco.Lib.Web.Controls.V4.Common.Page;

namespace Kesco.App.Web.TimeControl.Forms
{
    /// <summary>
    ///     Начальная страница учета рабочего времени
    /// </summary>
    public partial class Default : BasePage
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
                SetProperty();
                SetHandlers();
                InitBindColumns();
                GetParams();
                //ShowMessage("rtty");
            }
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
                case "ShowPersonById":
                    var url = Config.user_form + "?id=" + param["id"];
                    JS.Write("v3s_openForm('{0}','Поиск лиц');", url);
                    break;
                case "ClearCashRefresh":
                    ClearCashRefresh();
                    break;
                case "Refresh":
                    RefreshListInternal();
                    break;
                case "PageClose":
                    SaveParams(param["paramSave"]);
                    break;
                case "setSortOrder":
                    ClickSortColumn(param["col"]);
                    JS.Write("ShowWaitLayer('Refresh');");
                    break;
                case "SetPrimaryCalc":
                    Filter.IsEmployeePrimary = PrimaryEmployeeCalc = bool.Parse(param["val"]);
                    SetGrayStyle();
                    break;
                case "showLookup":
                    ShowLookup(param["id"]);
                    break;
                case "call":
                    DoMakeCall(param["number"], param["inter"], param["phone"]);
                    break;
                case "ClearFilter":
                    ClearFilter();
                    break;
                case "openDetails":
                    RedirectToDetails(param["id"], param["from"]);
                    break;
                case "openDaysDetails":
                    RedirectToDaysDetails(param["id"], param["from"], param["to"], param["period"]);
                    break;
            }

            base.ProcessCommand(cmd, param);
        }

        private void RedirectToDetails(string empl, string startDate)
        {
            PersonId = new ExecQuery().GetPersonIdByEmployeeId(empl);
            JS.Write(
                "v4_windowOpen('TimeDetailsForm.aspx?id={0}&date={1}&prim={2}&personid={3}&tz={4}', '_blank', 'left=340,top=240,width=750,height=700,resizable=yes,scrollbars=yes');",
                empl, HttpUtility.UrlEncode(startDate), PrimaryEmployeeCalc, PersonId, Tz);
        }

        private void RedirectToDaysDetails(string empl, string startDate, string endDate, string periodType)
        {
            JS.Write(
                "window.location.assign('EmplDaysDetailsForm.aspx?id={0}&from={1}&to={2}&period={3}&prim={4}&tz={5}');",
                empl, HttpUtility.UrlEncode(startDate), HttpUtility.UrlEncode(endDate), periodType, PrimaryEmployeeCalc,
                Tz);
        }

        private void ClearFilter()
        {
            Company.ValueSelectEnum = Subdivision.ValueSelectEnum = Position.ValueSelectEnum =
                EmplName.ValueSelectEnum =
                    Subdivision.Filter.PcId.CompanyHowSearch = EmplName.Filter.IdsCompany.CompanyHowSearch =
                        EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch =
                            Position.Filter.PcId.CompanyHowSearch = EmplName.Filter.Ids.EmployeeHowSearch = "0";
            Company.ClearSelectedItems();
            Subdivision.ClearSelectedItems();
            Position.ClearSelectedItems();
            EmplName.ClearSelectedItems();
            Company.SetPropertyChanged("ListChanged");
            Subdivision.SetPropertyChanged("ListChanged");
            Position.SetPropertyChanged("ListChanged");
            EmplName.SetPropertyChanged("ListChanged");
            SetGrayStyle();
            JS.Write("hi('tableDiv');hi('descDiv');hi('btnPrint');");
        }

        /// <summary>
        ///     Меняем пользовательский интерфейс на неактуальный данным фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EmplNameChanged(object sender, ProperyChangedEventArgs e)
        {
            EmplName.Filter.Ids.EmployeeHowSearch = ((Select) sender).ValueSelectEnum;
            SetGrayStyle();
            if (JS.ToString().Contains("SetTableDivSize();"))
                return;
            JS.Write("SetTableDivSize();");
        }

        /// <summary>
        ///     Сохраняем компанию в модели данных
        ///     Меняем пользовательский интерфейс на неактуальный данным фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CompanyChanged(object sender, ProperyChangedEventArgs e)
        {
            Subdivision.Filter.PcId.Value = ((Select) sender).SelectedItemsString;
            Subdivision.Filter.PcId.CompanyHowSearch = ((Select) sender).ValueSelectEnum;
            Position.Filter.PcId.Value = ((Select) sender).SelectedItemsString;
            Position.Filter.PcId.CompanyHowSearch = ((Select) sender).ValueSelectEnum;
            EmplName.Filter.IdsCompany.Value = ((Select) sender).SelectedItemsString;
            EmplName.Filter.IdsCompany.CompanyHowSearch = ((Select) sender).ValueSelectEnum;
            CheckCompany();
            if (JS.ToString().Contains("SetTableDivSize();"))
                return;
            JS.Write("SetTableDivSize();");
        }

        private void CheckCompany()
        {
            CheckSubdivision();
            CheckPosition();
            CheckPerson();
            SetGrayStyle();
        }

        /// <summary>
        ///     Сохраняем должность в модели данных
        ///     Меняем пользовательский интерфейс на неактуальный данным фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PositionChanged(object sender, ProperyChangedEventArgs e)
        {
            EmplName.Filter.PositionIDs.Value = ((Select) sender).SelectedItemsString;
            EmplName.Filter.PositionIDs.PositionHowSearch = ((Select) sender).ValueSelectEnum;
            CheckPositionByRevers();
            if (JS.ToString().Contains("SetTableDivSize();"))
                return;
            JS.Write("SetTableDivSize();");
        }

        private void CheckPositionByRevers()
        {
            CheckPersonByPosition();
            SetGrayStyle();
        }

        /// <summary>
        ///     Сохраняем подразделение в модели данных
        ///     Меняем пользовательский интерфейс на неактуальный данным фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubdivisionChanged(object sender, ProperyChangedEventArgs e)
        {
            Position.Filter.SubdivisionIDs.Value = ((Select) sender).SelectedItemsString;
            Position.Filter.SubdivisionIDs.SubdivisionHowSearch = ((Select) sender).ValueSelectEnum;
            EmplName.Filter.SubdivisionIDs.Value = ((Select) sender).SelectedItemsString;
            EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch = ((Select) sender).ValueSelectEnum;
            CheckSubdivisionByRevers();
            if (JS.ToString().Contains("SetTableDivSize();"))
                return;
            JS.Write("SetTableDivSize();");
        }

        private void CheckSubdivisionByRevers()
        {
            CheckPositionBySubdivision();
            CheckPersonBySubdivision();
            SetGrayStyle();
        }

        /// <summary>
        ///     Сохранение периода в модели данных
        ///     Меняем пользовательский интерфейс на неактуальный данным фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PeriodChanged(object sender, ProperyChangedEventArgs e)
        {
            if (e.IsChange && e.OldValue == "cmd" && (e.NewValue == "prev" || e.NewValue == "next"))
            {
                pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
                Period.DisableListing(true);
                JS.Write("ShowWaitLayer('Refresh');");
                return;
            }

            SetTimeControl(FilterTimeExit.Value);
            pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
            SetGrayStyle();
        }

        private void OnCurrentPageChanged(object sender, EventArgs args)
        {
            CurrentPageSetting = pagerBar.CurrentPageNumber;
            RefreshListInternal();
        }

        private void OnRowsPerPage(object sender, EventArgs args)
        {
            Filter.RowsPerPage = RowsPerPageSetting = pagerBar.RowsPerPage;
            CurrentPageSetting = pagerBar.CurrentPageNumber = 1;
            RefreshListInternal();
        }

        protected void FilterTimeExitChanged(object sender, ProperyChangedEventArgs e)
        {
            SetTimeControl(FilterTimeExit.Value);
            Filter.TimeExitAvaible = e.NewValue == "1";
        }

        protected void FilterEmplAvaibleChanged(object sender, ProperyChangedEventArgs e)
        {
            SetGrayStyle();
            Filter.EmplAvaible = e.NewValue == "1";
            EmplName.Filter.EmployeeAvaible.ValueEmployeeAvaible = e.NewValue != "1";
        }

        protected void FilterSubEmplChanged(object sender, ProperyChangedEventArgs e)
        {
            SetGrayStyle();
            Filter.IsSubEmployee = e.NewValue == "1";
            //EmplName.Filter.EmployeeAvaible.ValueEmployeeAvaible = e.NewValue != "1";
        }

        protected string GetStartTimeHeaderText()
        {
            return string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'StartTime');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'StartTime');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cStartTime"), SortTagStartTime);
        }

        protected string GetEndTimeHeaderText()
        {
            return string.Format(
                "<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'EndTime');\">{0}</A>&nbsp&nbsp&nbsp<A href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'EndTime');\"><FONT style='font-size:7pt; color:#C00000;'>{1}</FONT></A>",
                Resx.GetString("cEndTime"), SortTagEndTime);
        }

        private void ShowLookup(string emplId)
        {
            JS.Write("mouseOver('{0}', '{1}');", emplId, Global.UserPhoto);
        }

        private void CompleteAdditionalInfo(DataRow row)
        {
            var emplId = (int) row["EmplID"];
            var personId = (int) row["PersonId"];
            object linkCell;
            var from = Period.ValueDateFrom.HasValue ? Period.ValueDateFrom.Value.ToString("dd.MM.yyyy") : "";
            var from2 = Period.ValueDateFrom.HasValue
                ? Period.ValueDateFrom.Value.AddDays(1).ToString("dd.MM.yyyy")
                : "";
            var to = Period.ValueDateTo.HasValue ? Period.ValueDateTo.Value.ToString("dd.MM.yyyy") : "";
            if (DetailsByDay)
                linkCell = string.Format(
                    "<A href=\"#\" onclick=\"cmd('cmd', 'openDetails', 'id', '{0}', 'from', '{1}', 'to', '{2}');return false;\"><IMG src=\"{3}detail.GIF\" border=\"0\" title=\"{4}\"></A>",
                    emplId, from, from2, Global.Styles, Resx.GetString("hPageSubTitleDetails2"));
            else
                linkCell = string.Format(
                    "<A href=\"#\" onclick=\"cmd('cmd', 'openDaysDetails', 'id', '{0}', 'from', '{1}', 'to', '{2}', 'period', '{3}');return false;\"><IMG src=\"{4}detail.GIF\" border=\"0\" title=\"{5}\"></A>",
                    emplId, from, to, Period.ValuePeriod, Global.Styles, Resx.GetString("hPageDetailsTitle"));
            object employeeCell =
                string.Format(
                    "<A href=\"#\" onclick=\"ItemClick('{0}', '{3}');\" class=\"v4_callerControl\" caller-type=\"2\" data-id=\"{0}\"  onmouseout=\"mouseOut();\">{1}</A>",
                    emplId, row["EmplNameSort"], personId, Config.user_form);

            var isShowLink = !((int) ((TimeSpan) row[8]).TotalSeconds == 0 &&
                               (int) ((TimeSpan) row[9]).TotalSeconds == 0);

            row["LinkCell"] = isShowLink ? linkCell : null;
            row["EmplNameCol"] = employeeCell;
        }

        private void CompleteEmployeersList()
        {
            var stm = TimeExit.Value;
            if (stm.Length == 0) stm = "00:00:00";
            stm = stm.Replace(":", "");
            var temp = Period.ValueDateFrom ?? new DateTime(1753, 1, 2);
            var tm = Str2DateTime(temp.ToString("yyyyMMdd") + stm);

            if (DetailsByDay)
                SourceCash.CompleteEmplTable(Period.ValueDateFrom, Period.ValueDateFrom, EmplName.SelectedItemsString,
                    Company.SelectedItemsString, Position.SelectedItemsString, Subdivision.SelectedItemsString,
                    Filter.TimeExitAvaible, tm, Filter.EmplAvaible, Tz, Company.ValueSelectEnum,
                    Subdivision.ValueSelectEnum,
                    Position.ValueSelectEnum, EmplName.ValueSelectEnum, CurrentPageSetting, RowsPerPageSetting,
                    FilterSubEmpl.Checked);
            else
                SourceCash.CompleteEmplTable(Period.ValueDateFrom, Period.ValueDateTo, EmplName.SelectedItemsString,
                    Company.SelectedItemsString, Position.SelectedItemsString, Subdivision.SelectedItemsString,
                    Filter.TimeExitAvaible, tm, Filter.EmplAvaible, Tz, Company.ValueSelectEnum,
                    Subdivision.ValueSelectEnum,
                    Position.ValueSelectEnum, EmplName.ValueSelectEnum, CurrentPageSetting, RowsPerPageSetting,
                    FilterSubEmpl.Checked);

            var tAllRows = SourceCash.GetSourceTable();
            var tPeriodsList = GetDataTable();

            //_totalRecord = tAllRows.Rows.Count;
            _totalRecord = SourceCash.CountEmployee;

            var sortTagEmplName = string.Empty;
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
                        case "EmplNameSort":

                            if (item.ItemSortType == SortType.Asc)
                                sortTagEmplName = sortTagEmplName + strUpEnabledGif;
                            else
                                sortTagEmplName = sortTagEmplName + strDownEnabledGif;
                            break;

                        case "StartTime":

                            if (item.ItemSortType == SortType.Asc)
                                SortTagStartTime = SortTagStartTime + strUpEnabledGif;
                            else
                                SortTagStartTime = SortTagStartTime + strDownEnabledGif;
                            break;

                        case "EndTime":

                            if (item.ItemSortType == SortType.Asc)
                                SortTagEndTime = SortTagEndTime + strUpEnabledGif;
                            else
                                SortTagEndTime = SortTagEndTime + strDownEnabledGif;
                            break;

                        case "AbsentTimeSort":

                            if (item.ItemSortType == SortType.Asc)
                                sortTagAbsentTime = sortTagAbsentTime + strUpEnabledGif;
                            else
                                sortTagAbsentTime = sortTagAbsentTime + strDownEnabledGif;
                            break;

                        case "IntervalSort":
                            if (item.ItemSortType == SortType.Asc)
                                sortTagInterval = sortTagInterval + strUpEnabledGif;
                            else
                                sortTagInterval = sortTagInterval + strDownEnabledGif;
                            break;

                        case "InternetAccessCount":
                            if (item.ItemSortType == SortType.Asc)
                                sortTagInternetAccessCount = sortTagInternetAccessCount + strUpEnabledGif;
                            else
                                sortTagInternetAccessCount = sortTagInternetAccessCount + strDownEnabledGif;
                            break;

                        case "InternetAccessTotaltime":
                            if (item.ItemSortType == SortType.Asc)
                                sortTagInternetAccessTotal = sortTagInternetAccessTotal + strUpEnabledGif;
                            else
                                sortTagInternetAccessTotal = sortTagInternetAccessTotal + strDownEnabledGif;
                            break;
                    }
                }

            BcEmplName.HeaderText = string.Format(
                "<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'EmplNameSort');\">{0}</a>&nbsp&nbsp&nbsp<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'EmplNameSort');\"><font style='font-size:7pt; color:#C00000;'>{1}</font></a>",
                Resx.GetString("cEmplName"), sortTagEmplName);
            BcAbsentTime.HeaderText = string.Format(
                "<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'AbsentTimeSort');\">{0}</a>&nbsp&nbsp&nbsp<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'AbsentTimeSort');\"><font style='font-size:7pt; color:#C00000;'>{1}</font></a>",
                Resx.GetString("сAbsent"), sortTagAbsentTime);
            BcInterval.HeaderText = string.Format(
                "<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'IntervalSort');\">{0}</a>&nbsp&nbsp&nbsp<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'IntervalSort');\"><font style='font-size:7pt; color:#C00000;'>{1}</font></a>",
                Resx.GetString("cInterval"), sortTagInterval);
            BcInternetAccessCount.HeaderText = string.Format(
                "<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'InternetAccessCount');\">{0}</a>&nbsp&nbsp&nbsp<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'InternetAccessCount');\"><font style='font-size:7pt; color:#C00000;'>{1}</font></a>",
                Resx.GetString("cInternetAccessCount"), sortTagInternetAccessCount);
            BcInternetAccessTotalTime.HeaderText = string.Format(
                "<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'InternetAccessTotaltime');\">{0}</a>&nbsp&nbsp&nbsp<a href=\"#\" onclick=\"cmd('cmd', 'setSortOrder', 'col', 'InternetAccessTotaltime');\"><font style='font-size:7pt; color:#C00000;'>{1}</font></a>",
                Resx.GetString("cInternetAccessTotalTime"), sortTagInternetAccessTotal);


            var pageNo = pagerBar.CurrentPageNumber - 1;
            var pageRowCount = pagerBar.RowsPerPage;

            var rows = tAllRows.Select("", SortString);
            //int lastPageNo = (rows.Length - 1) / pageRowCount;
            var lastPageNo = (_totalRecord - 1) / pageRowCount;
            pagerBar.MaxPageNumber = lastPageNo + 1;

            if (rows.Length > 0)
            {
                //if (pageNo > lastPageNo)
                //{
                //    pageNo = lastPageNo;
                //}

                //int copyLen;
                //if (pageNo < lastPageNo)
                //{
                //    copyLen = pageRowCount;
                //}
                //else
                //{
                //    //copyLen = rows.Length - pageRowCount * pageNo;
                //    copyLen = _totalRecord - pageRowCount * pageNo;
                //}

                //for (int i = pageNo * pageRowCount; i < pageNo * pageRowCount + copyLen; i++)
                var curCount = pageRowCount;
                if (curCount > rows.Length)
                    curCount = rows.Length;
                for (var i = 0; i < curCount; i++)
                {
                    CompleteAdditionalInfo(rows[i]);
                    tPeriodsList.Rows.Add(rows[i].ItemArray);
                }
            }

            tPeriodsList.DefaultView.Sort = SortString;

            intervalList.DataSource = tPeriodsList;
            intervalList.DataBind();
        }

        private void InitBindColumns()
        {
            foreach (DataGridColumn bcol in intervalList.Columns)
                if (bcol is BoundColumn)
                {
                    var col = (BoundColumn) bcol;
                    switch (col.DataField)
                    {
                        case "EmplNameCol":
                            BcEmplName = col;
                            break;
                        case "StartTime":
                            BcStartTime = col;
                            break;
                        case "EndTime":
                            BcEndTime = col;
                            break;
                        case "AbsentTime":
                            BcAbsentTime = col;
                            break;
                        case "Interval":
                            BcInterval = col;
                            break;
                        case "LinkCell":
                            //col.HeaderImageUrl = Global.Styles + "Print.gif";
                            BcLinkCell = col;
                            break;
                        case "InternetAccessCount":
                            BcInternetAccessCount = col;
                            break;
                        case "InternetAccessTotaltime":
                            BcInternetAccessTotalTime = col;
                            break;
                        case "isEnterAfterExit":
                            BcIsEnterAfterExit = col;
                            break;
                        case "isEnterAfterExit2":
                            BcIsEnterAfterExit2 = col;
                            break;
                    }
                }
        }

        private void SetProperty()
        {
            var fio = "Employee";
            var nameCompany = "Name";
            if (CurrentUser.Language.ToLower() == "ru") fio = "FullName";

            EmplName.ValueField = fio;
            Company.ValueField = nameCompany;
            Company.Filter.PersonType = 1;

            EmplName.Filter.Status.ValueStatus = СотоянияСотрудника.Все;
            EmplName.Filter.HasVirtual.ValueHasVirtual = ВиртуальныйСотрудник.ИсключитьВиртуальныхСотрудников;

            pagerBar.RowsPerPage = RowsPerPageSetting;
            pagerBar.MaxPageNumber = DefaultMaxPage;
            pagerBar.CurrentPageNumber = CurrentPageSetting;
            //pagerBar.MaxPageNumber = 1;
            HelpUrl = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath +
                      "/Forms/hlp/help.htm?id=0";
            btnHlp.Attributes.Add("onclick", string.Format("v4_windowOpen('{0}');", HelpUrl));
            btnHlp.Attributes.Add("title", Resx.GetString("cmdHelp"));
        }

        private void SetHandlers()
        {
            pagerBar.CurrentPageChanged += OnCurrentPageChanged;
            pagerBar.RowsPerPageChanged += OnRowsPerPage;
        }

        private void SetTimeControl(string inx)
        {
            SetGrayStyle();
            JS.Write("displayTimeExit({0}, {1});",
                Period.ValuePeriod == ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture) ||
                Period.ValueFrom.Length > 0 && Period.ValueDateFrom == Period.ValueDateTo
                    ? 1
                    : 0, inx.Length == 0 ? "0" : inx);
        }

        private void ClearCashRefresh()
        {
            SourceCash.ClearCash();
            RefreshListInternal();
            pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
        }

        private void RefreshListInternal()
        {
            SaveParams("");


            if (string.IsNullOrEmpty(Tz))
            {
                JS.Write("ShowWaitLayer('Refresh');");
            }
            else
            {
                CompleteEmployeersList();
                TextWriter stringWriter = new StringWriter();
                var writer = new HtmlTextWriter(stringWriter);
                SetAdditionalColumnsVisiblity();
                if (SourceCash.GetSourceTable().Rows.Count > 0 &&
                    Period.ValuePeriod != ((int) PeriodsEnum.Undefined).ToString(CultureInfo.InvariantCulture) &&
                    Period.ValueDateTo != null && !Period.ValueDateTo.Value.Equals(string.Empty) &&
                    (Period.ValueDateFrom != null && !Period.ValueDateFrom.Value.Equals(string.Empty) ||
                     Period.ValuePeriod == ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture)))
                {
                    intervalList.RenderControl(writer);
                    var tableContent = stringWriter.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "")
                        .Replace("'", "\\'");
                    JS.Write("document.all('listTable').innerHTML = '{0}';", tableContent);
                    pagerBar.SetDisabled(false);
                }
                else
                {
                    JS.Write("document.all('listTable').innerHTML = '<P align=\\'center\\'><BR>{0}</P>';",
                        Resx.GetString("lNoData"));
                    pagerBar.SetDisabled(true, false);
                }

                UpdateTotalCount();
                JS.Write("di('btnPrint');SetTableDivSize();");
                JS.Write("UTC2Local();");
                if (!JS.ToString().Contains("v4_setToolTip()"))
                    JS.Write("v4_setToolTip();");
                JS.Write(SourceCash.IsError ? "di('descDiv');" : "hi('descDiv');");
                Period.DisableListing(false);
            }
        }

        protected void Print()
        {
            var idpage = Request.QueryString["idpage"];
            if (!string.IsNullOrEmpty(idpage))
            {
                var p = Application[idpage] as Page;
                if (p != null)
                {
                    var ds = ((Default) p).SourceCash.GetDS();
                    if (ds.Сотрудники.Rows.Count > ((Default) p).RowsPerPageSetting)
                    {
                        var startPeriod = ((Default) p).Period.ValueDateFrom ?? new DateTime(1753, 1, 2);
                        var endPeriod = ((Default) p).Period.ValueDateTo ?? new DateTime(1753, 1, 2);
                        ((Default) p).SourceCash.FillPage(ds, startPeriod, endPeriod, 1, ds.Сотрудники.Rows.Count);
                    }

                    var dt = ((Default) p).SourceCash.GetSourceTable();
                    if (dt.Rows.Count > 0)
                    {
                        if (((Default) p).Period.ValuePeriod ==
                            ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture) ||
                            ((Default) p).Period.ValuePeriod ==
                            ((int) PeriodsEnum.Custom).ToString(CultureInfo.InvariantCulture) &&
                            ((Default) p).Period.ValueDateFrom == ((Default) p).Period.ValueDateTo)
                            FillPrintDataByDay(dt, p);
                        else
                            FillPrintData(dt, p);
                    }
                }
            }
        }

        private void FillPrintDataByDay(DataTable dt, Page p)
        {
            WrResponse = string.Format("<html><head><title>{0}</title></head><body onload=\"window.print()\">",
                Resx.GetString("lPrint"));
            WrResponse += "<b>" + Resx.GetString("hPageTitle") + "</b> (" +
                          DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ")";
            WrResponse += "<table style=\"border-collapse:collapse;\">";
            WrResponse += string.Format(
                "<tr style=\"height: 25px; font-weight: bold; text-align: center;\"><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                Resx.GetString("cEmplName"), Resx.GetString("cStartTime"), Resx.GetString("cEndTime"),
                Resx.GetString("cInterval"), Resx.GetString("сAbsent"));
            foreach (DataRow row in dt.Rows)
            {
                string start = "", end = "", work = "", notwork = "";
                if (!string.IsNullOrEmpty(row[0].ToString()))
                {
                    start = row[4].ToString();
                    end = row[5].ToString();
                    work = row[7].ToString() == "00:00:00" ? "" : row[7].ToString();
                    notwork = row[6].ToString() == "00:00:00" ? "" : row[6].ToString();
                }

                WrResponse += "<tr>";
                WrResponse += "<td style=\"border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                              row[10] + "</td>";
                WrResponse +=
                    "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                    start + "</td>";
                WrResponse +=
                    "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                    end + "</td>";
                WrResponse +=
                    "<td style=\"text-align: center;border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                    work + "</td>";
                WrResponse += "<td style=\"text-align: center;border-bottom: darkgray 1px solid;\">" + notwork +
                              "</td>";
                WrResponse += "</tr>";
            }

            WrResponse += "</table>";
            if (((Default) p).SourceCash.IsError) WrResponse += Resx.GetString("lErrorDesc");
            WrResponse += "</body></html>";
        }

        private void FillPrintData(DataTable dt, Page p)
        {
            WrResponse = string.Format("<html><head><title>{0}</title></head><body onload=\"window.print()\">",
                Resx.GetString("lPrint"));
            WrResponse += "<b>" + Resx.GetString("hPageTitle") + "</b> (" +
                          DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ")";
            WrResponse += "<br />За период: " + (((Default) p).Period.ValueDateFrom.HasValue
                              ? ((Default) p).Period.ValueDateFrom.Value.ToString("yyyy.MM.dd")
                              : "") + " - " +
                          (((Default) p).Period.ValueDateTo.HasValue
                              ? ((Default) p).Period.ValueDateTo.Value.ToString("yyyy.MM.dd")
                              : "");
            WrResponse += "<table style=\"border-collapse:collapse;\">";
            WrResponse += string.Format(
                "<tr style=\"height: 25px; font-weight: bold; text-align: center;\"><td>{0}</td><td>{1}</td></tr>",
                Resx.GetString("cEmplName"), Resx.GetString("cInterval"));
            foreach (DataRow row in dt.Rows)
            {
                WrResponse += "<tr>";
                WrResponse += "<td style=\"border-bottom: darkgray 1px solid;border-right: darkgray 1px solid;\">" +
                              row[10] + "</td>";
                WrResponse += "<td style=\"text-align: center;border-bottom: darkgray 1px solid;\">" + row[7] + "</td>";
                WrResponse += "</tr>";
            }

            WrResponse += "</table>";
            if (((Default) p).SourceCash.IsError) WrResponse += Resx.GetString("lErrorDesc");
            WrResponse += "</body></html>";
        }

        public void UpdateTotalCount()
        {
            //JS.Write("window.status = '" + String.Format(Resx.GetString("lTotalFound"), _totalRecord) + "';");
            JS.Write("gi('countDiv').innerHTML = '" + string.Format(Resx.GetString("lTotalFound"), _totalRecord) +
                     "';");
        }

        private void SetAdditionalColumnsVisiblity()
        {
            BcAbsentTime.Visible = NeedToDisplayAdditionCols;

            foreach (DataGridColumn bcol in intervalList.Columns)
                if (bcol is TemplateColumn)
                    bcol.Visible = NeedToDisplayAdditionCols;
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

        private DataTable GetDataTable()
        {
            var tPeriodsList = new DataTable();

            EmplID = new DataColumn("EmplID", typeof(int));
            PersonID = new DataColumn("PersonID", typeof(int));
            LinkCell = new DataColumn("LinkCell", typeof(string));
            EmplNameCol = new DataColumn("EmplNameCol", typeof(string));
            StartTime = new DataColumn("StartTime", typeof(string));
            EndTime = new DataColumn("EndTime", typeof(string));
            AbsentTime = new DataColumn("AbsentTime", typeof(string));
            Interval = new DataColumn("Interval", typeof(string));
            AbsentTimeSort = new DataColumn("AbsentTimeSort", typeof(TimeSpan));
            IntervalSort = new DataColumn("IntervalSort", typeof(TimeSpan));
            EmplNameSort = new DataColumn("EmplNameSort", typeof(string));
            ErrorCss = new DataColumn("ErrorCss", typeof(string));
            InternetAccessCount = new DataColumn("InternetAccessCount", typeof(string));
            InternetAccessTotaltime = new DataColumn("InternetAccessTotaltime", typeof(string));
            IsEnterAfterExit = new DataColumn("isEnterAfterExit", typeof(string));
            IsEnterAfterExit2 = new DataColumn("isEnterAfterExit2", typeof(string));

            tPeriodsList.TableName = "tPeriodsList";
            tPeriodsList.Columns.Add(EmplID);
            tPeriodsList.Columns.Add(PersonID);
            tPeriodsList.Columns.Add(LinkCell);
            tPeriodsList.Columns.Add(EmplNameCol);
            tPeriodsList.Columns.Add(StartTime);
            tPeriodsList.Columns.Add(EndTime);
            tPeriodsList.Columns.Add(AbsentTime);
            tPeriodsList.Columns.Add(Interval);
            tPeriodsList.Columns.Add(AbsentTimeSort);
            tPeriodsList.Columns.Add(IntervalSort);
            tPeriodsList.Columns.Add(EmplNameSort);
            tPeriodsList.Columns.Add(ErrorCss);
            tPeriodsList.Columns.Add(InternetAccessCount);
            tPeriodsList.Columns.Add(InternetAccessTotaltime);
            tPeriodsList.Columns.Add(IsEnterAfterExit);
            tPeriodsList.Columns.Add(IsEnterAfterExit2);

            return tPeriodsList;
        }

        private void SaveParams(string paramSave)
        {
            var arr = paramSave.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
            Filter.PeriodInfo.PeriodType = Period.ValuePeriod;
            Filter.PeriodInfo.DateFrom = Period.ValueDateFrom;
            Filter.PeriodInfo.DateTo = Period.ValueDateTo;
            Filter.TimeExitAvaible = FilterTimeExit.Checked;
            Filter.TimeExit = TimeExit.ValueTime == null ? "" : TimeExit.ValueTime.Value.ToString("HH:mm");
            Filter.EmplAvaible = FilterEmplAvaible.Checked;
            Filter.IsSubEmployee = FilterSubEmpl.Checked;
            Filter.RowsPerPage = pagerBar.RowsPerPage;

            Filter.CompanyItems = Company.SelectedItemsString;
            Filter.SubdivisionItems = Subdivision.SelectedItemsString;
            Filter.PositionItems = Position.SelectedItemsString;
            Filter.PersonItems = EmplName.SelectedItemsString;

            Filter.PeriodInfo.CompanyHowSearch = Company.ValueSelectEnum;
            Filter.PeriodInfo.SubdivisionHowSearch = Subdivision.ValueSelectEnum;
            Filter.PeriodInfo.PositionHowSearch = Position.ValueSelectEnum;
            Filter.PeriodInfo.PersonHowSearch = EmplName.ValueSelectEnum;

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

        private void GetParams()
        {
            Filter = new ExecQuery().GetSettings(Filter);
            var arr = Filter.CompanyItems.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in arr) Company.SelectedItems.Add(new Item {Id = id, Value = new Person(id)});
            arr = Filter.SubdivisionItems.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in arr)
                Subdivision.SelectedItems.Add(new Item {Id = id, Value = new Subdivision {Id = id, Name = id}});
            arr = Filter.PositionItems.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in arr)
                Position.SelectedItems.Add(new Item {Id = id, Value = new Position {Id = id, Name = id}});
            arr = Filter.PersonItems.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in arr) EmplName.SelectedItems.Add(new Item {Id = id, Value = new Employee(id)});

            Company.ValueSelectEnum = Subdivision.Filter.PcId.CompanyHowSearch = Position.Filter.PcId.CompanyHowSearch =
                EmplName.Filter.IdsCompany.CompanyHowSearch = Filter.PeriodInfo.CompanyHowSearch;
            Subdivision.ValueSelectEnum = Position.Filter.SubdivisionIDs.SubdivisionHowSearch =
                EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch = Filter.PeriodInfo.SubdivisionHowSearch;
            Position.ValueSelectEnum =
                EmplName.Filter.PositionIDs.PositionHowSearch = Filter.PeriodInfo.PositionHowSearch;
            EmplName.ValueSelectEnum = Filter.PeriodInfo.PersonHowSearch;

            Subdivision.Filter.PcId.Value = Position.Filter.PcId.Value =
                EmplName.Filter.IdsCompany.Value = Company.SelectedItemsString;
            Position.Filter.SubdivisionIDs.Value =
                EmplName.Filter.SubdivisionIDs.Value = Subdivision.SelectedItemsString;
            EmplName.Filter.PositionIDs.Value = Position.SelectedItemsString;

            if (Filter.RowsPerPage > 0)
                pagerBar.RowsPerPage = RowsPerPageSetting = Filter.RowsPerPage;
            else
                pagerBar.RowsPerPage = RowsPerPageSetting;
            //pagerBar.CurrentPageNumber = CurrentPageSetting = 1;
            PrimaryEmployeeCalc = Filter.IsEmployeePrimary;
            if (PrimaryEmployeeCalc)
                ep1.Checked = true;
            else
                ep2.Checked = true;

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

            FilterTimeExit.Checked = Filter.TimeExitAvaible;
            if (string.IsNullOrEmpty(Filter.TimeExit))
                TimeExit.ValueTime = DateTime.Now;
            else
                TimeExit.ValueTime = Convert.ToDateTime(Filter.TimeExit);
            FilterEmplAvaible.Checked = Filter.EmplAvaible;
            FilterSubEmpl.Checked = Filter.IsSubEmployee;
            EmplName.Filter.EmployeeAvaible.ValueEmployeeAvaible = !Filter.EmplAvaible;
            SetTimeControl(Filter.TimeExitAvaible ? "1" : "");
            if (Request.QueryString["isback"] == "1") RefreshListInternal();
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
        }

        private void SetGrayStyle()
        {
            SourceCash.ClearCash();
            pagerBar.SetDisabled(true, false);
            //JS.Write("if (document.all('intervalList')) document.all('intervalList').rows(0).className = 'GridHeaderGrayed';");
            if (JS.ToString()
                .Contains("if (document.all('intervalList')) document.all('intervalList').className = 'Grid8Grayed';"))
                return;
            JS.Write("if (document.all('intervalList')) document.all('intervalList').className = 'Grid8Grayed';");
        }

        private void CheckSubdivision()
        {
            if (Subdivision.SelectedItems.Count == 0 || Subdivision.Filter.PcId.CompanyHowSearch == "2") return;
            if (string.IsNullOrEmpty(Subdivision.Filter.PcId.Value) &&
                Subdivision.Filter.PcId.CompanyHowSearch != "3") return;
            for (var i = 0; i < Subdivision.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckSubdivisionByCompanyId(Subdivision.Filter.PcId.Value,
                    Subdivision.SelectedItems[i].Id, Subdivision.Filter.PcId.CompanyHowSearch == "1",
                    Subdivision.Filter.PcId.CompanyHowSearch == "3"))
                {
                    Subdivision.SelectedItems.Remove(
                        Subdivision.SelectedItems.Find(x => x.Id == Subdivision.SelectedItems[i].Id));
                    i--;
                }

            EmplName.Filter.SubdivisionIDs.Value =
                Position.Filter.SubdivisionIDs.Value = Subdivision.SelectedItemsString;
            Subdivision.RefreshDataBlock();
        }

        private void CheckPosition()
        {
            if (Position.SelectedItems.Count == 0 || Position.Filter.PcId.CompanyHowSearch == "2") return;
            if (string.IsNullOrEmpty(Position.Filter.PcId.Value) &&
                Position.Filter.PcId.CompanyHowSearch != "3") return;
            for (var i = 0; i < Position.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckPositionByCompanyId(Position.Filter.PcId.Value, Position.SelectedItems[i].Id,
                    Position.Filter.PcId.CompanyHowSearch == "1",
                    Position.Filter.PcId.CompanyHowSearch == "3"))
                {
                    Position.SelectedItems.Remove(
                        Position.SelectedItems.Find(x => x.Id == Position.SelectedItems[i].Id));
                    i--;
                }

            EmplName.Filter.PositionIDs.Value = Position.SelectedItemsString;
            Position.RefreshDataBlock();
        }

        private void CheckPerson()
        {
            if (EmplName.SelectedItems.Count == 0 || EmplName.Filter.IdsCompany.CompanyHowSearch == "2") return;
            if (string.IsNullOrEmpty(EmplName.Filter.IdsCompany.Value) &&
                EmplName.Filter.IdsCompany.CompanyHowSearch != "3") return;
            for (var i = 0; i < EmplName.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckPersonByCompanyId(EmplName.Filter.IdsCompany.Value,
                    EmplName.SelectedItems[i].Id, EmplName.Filter.IdsCompany.CompanyHowSearch == "1",
                    EmplName.Filter.IdsCompany.CompanyHowSearch == "3"))
                {
                    EmplName.SelectedItems.Remove(
                        EmplName.SelectedItems.Find(x => x.Id == EmplName.SelectedItems[i].Id));
                    i--;
                }

            EmplName.RefreshDataBlock();
        }

        private void CheckPositionBySubdivision()
        {
            if (Position.SelectedItems.Count == 0 || Position.Filter.SubdivisionIDs.SubdivisionHowSearch == "2") return;
            if (string.IsNullOrEmpty(Position.Filter.SubdivisionIDs.Value) &&
                Position.Filter.SubdivisionIDs.SubdivisionHowSearch != "3") return;
            for (var i = 0; i < Position.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckPositionBySubdivisionId(Position.Filter.SubdivisionIDs.Value,
                    Position.SelectedItems[i].Id, Position.Filter.SubdivisionIDs.SubdivisionHowSearch == "1",
                    Position.Filter.SubdivisionIDs.SubdivisionHowSearch == "3"))
                {
                    Position.SelectedItems.Remove(
                        Position.SelectedItems.Find(x => x.Id == Position.SelectedItems[i].Id));
                    i--;
                }

            EmplName.Filter.PositionIDs.Value = Position.SelectedItemsString;
            Position.RefreshDataBlock();
        }

        private void CheckPersonBySubdivision()
        {
            if (EmplName.SelectedItems.Count == 0 || EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch == "2") return;
            if (string.IsNullOrEmpty(EmplName.Filter.SubdivisionIDs.Value) &&
                EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch != "3") return;
            for (var i = 0; i < EmplName.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckPersonBySubdivisionId(EmplName.Filter.SubdivisionIDs.Value,
                    EmplName.SelectedItems[i].Id, EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch == "1",
                    EmplName.Filter.SubdivisionIDs.SubdivisionHowSearch == "3"))
                {
                    EmplName.SelectedItems.Remove(
                        EmplName.SelectedItems.Find(x => x.Id == EmplName.SelectedItems[i].Id));
                    i--;
                }

            EmplName.RefreshDataBlock();
        }

        private void CheckPersonByPosition()
        {
            if (EmplName.SelectedItems.Count == 0 || EmplName.Filter.PositionIDs.PositionHowSearch == "2") return;
            if (string.IsNullOrEmpty(EmplName.Filter.PositionIDs.Value) &&
                EmplName.Filter.PositionIDs.PositionHowSearch != "3") return;
            for (var i = 0; i < EmplName.SelectedItems.Count; i++)
                if (!new ExecQuery().CheckPersonByPositionId(EmplName.Filter.PositionIDs.Value,
                    EmplName.SelectedItems[i].Id, EmplName.Filter.PositionIDs.PositionHowSearch == "1",
                    EmplName.Filter.PositionIDs.PositionHowSearch == "3"))
                {
                    EmplName.SelectedItems.Remove(
                        EmplName.SelectedItems.Find(x => x.Id == EmplName.SelectedItems[i].Id));
                    i--;
                }

            EmplName.RefreshDataBlock();
        }

        #region Members

        /// <summary>
        ///     ID сотрудника
        /// </summary>
        public string PersonId = string.Empty;

        protected string SortTagStartTime = string.Empty;
        protected string SortTagEndTime = string.Empty;
        protected EmplTableCash Etc;
        protected SortInfoSetting Info;
        protected int CurrentPageSetting = 1;
        protected int RowsPerPageSetting = 35;
        protected int DefaultMaxPage = 99999;
        protected DataColumn EmplID;
        protected DataColumn PersonID;
        protected DataColumn LinkCell;
        protected DataColumn EmplNameCol;
        protected DataColumn StartTime;
        protected DataColumn EndTime;
        protected DataColumn AbsentTime;
        protected DataColumn Interval;
        protected DataColumn AbsentTimeSort;
        protected DataColumn IntervalSort;
        protected DataColumn EmplNameSort;
        protected DataColumn ErrorCss;
        protected DataColumn InternetAccessCount;
        protected DataColumn InternetAccessTotaltime;
        protected DataColumn IsEnterAfterExit;
        protected DataColumn IsEnterAfterExit2;
        protected BoundColumn BcEmplName;
        protected BoundColumn BcStartTime;
        protected BoundColumn BcEndTime;
        protected BoundColumn BcAbsentTime;
        protected BoundColumn BcInterval;
        protected BoundColumn BcLinkCell;
        protected BoundColumn BcInternetAccessCount;
        protected BoundColumn BcInternetAccessTotalTime;
        protected BoundColumn BcIsEnterAfterExit;
        protected BoundColumn BcIsEnterAfterExit2;

        private int _totalRecord;

        private bool DetailsByDay =>
            Period.ValuePeriod == ((int) PeriodsEnum.Day).ToString(CultureInfo.InvariantCulture) ||
            Period.ValuePeriod == ((int) PeriodsEnum.Custom).ToString(CultureInfo.InvariantCulture) &&
            Period.ValueDateFrom == Period.ValueDateTo;

        private EmplTableCash SourceCash => PrimaryEmployeeCalc ? EmplListCashEmpl : EmplListCashComp;

        private EmplTableCash EmplListCashEmpl
        {
            get
            {
                if (Etc == null || !Etc.PrimaryEmployeeCalc) Etc = new EmplTableCash(GetDataTable(), true, this);
                return Etc;
            }
        }

        private EmplTableCash EmplListCashComp
        {
            get
            {
                if (Etc == null || Etc.PrimaryEmployeeCalc) Etc = new EmplTableCash(GetDataTable(), false, this);
                return Etc;
            }
        }

        private SortInfoSetting SortParams
        {
            get
            {
                if (Info == null) Info = new SortInfoSetting();

                if (Info.Count == 0) Info.Insert(0, new SortInfoItem(SortType.Asc, "EmplNameSort"));
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

                return result;
            }
        }

        private bool NeedToDisplayAdditionCols => DetailsByDay;

        #endregion
    }
}