using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kesco.App.Web.TimeControl.Common;
using Kesco.App.Web.TimeControl.Entities;

namespace Kesco.App.Web.TimeControl.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TimeDetailsForm : BasePage
    {
        #region Private Members

        protected string EmployeeId = String.Empty;
        protected string PersonId = String.Empty;
        protected DateTime StartPeriod;
        protected string LNoEntrance = String.Empty;
        protected string LNoExit = String.Empty;
        protected string SortTagStartTime = String.Empty;
        protected string SortTagEndTime = String.Empty;
        protected EmployeeTimePeriodsInfo periodsInfo;
        protected DataTable tPeriodsList;
        protected DataTable tPeriodsListAbs;
        protected DataColumn START_TIME;
        protected DataColumn END_TIME;
        protected DataColumn INTERVAL;
        protected DataColumn PLACE;
        protected DataColumn ERROR_CSS;
        protected DataColumn START_TIMEAbs;
        protected DataColumn END_TIMEAbs;
        protected DataColumn INTERVALAbs;
        protected DataColumn ERROR_CSSAbs;
        protected BoundColumn bcStartTime;
        protected BoundColumn bcEndTime;
        protected BoundColumn bcInterval;
        protected BoundColumn bcPlace;
        protected BoundColumn bcStartTimeAbs;
        protected BoundColumn bcEndTimeAbs;
        protected BoundColumn bcIntervalAbs;
        private CardPerson _cp;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!V4IsPostBack)
            {
                Filter.Clid = String.IsNullOrEmpty(Request.QueryString["clid"]) ? "0" : Request.QueryString["clid"];
                InitDataTable();
                SetProperty();
                GetParams();
            }
        }

        /// <summary>
        /// Обработка клиентских команд
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "showLookup":
                    ShowLookup(param["id"]);
                    break;
                case "call":
                    DoMakeCall(param["number"], param["inter"], param["phone"]);
                    break;
                case "SetPrimaryCalc":
                    PrimaryEmployeeCalc = bool.Parse(param["val"]);
                    Refresh();
                    break;
                case "PageClose":
                    SaveParams(param["paramSave"]);
                    break;
            }
            base.ProcessCommand(cmd, param);
        }

        private void SaveParams(string paramSave)
        {
            string[] arr = paramSave.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in arr)
            {
                if (!String.IsNullOrEmpty(s))
                {
                    string[] param = s.Split(new[] { "," }, StringSplitOptions.None);
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
            }

            new ExecQuery().SaveSettings(Filter);
        }

        private void GetParams()
        {
            Filter = new ExecQuery().GetSettings(Filter);
            string param = "";
            if (Filter.PairCallerPosition != null || Filter.PairCallerSize != null)
            {
                param += String.Format("['|Phone',{0},{1},{2},{3}],",
                    Filter.PairCallerPosition == null ? "" : Filter.PairCallerPosition.First,
                    Filter.PairCallerPosition == null ? "" : Filter.PairCallerPosition.Second,
                    Filter.PairCallerSize == null ? "" : Filter.PairCallerSize.First,
                    Filter.PairCallerSize == null ? "" : Filter.PairCallerSize.Second);
            }
            if (Filter.PairAlertPosition != null || Filter.PairAlertSize != null)
            {
                param += String.Format("['|Alert',{0},{1},{2},{3}],",
                    Filter.PairAlertPosition == null ? "" : Filter.PairAlertPosition.First,
                    Filter.PairAlertPosition == null ? "" : Filter.PairAlertPosition.Second,
                    Filter.PairAlertSize == null ? "" : Filter.PairAlertSize.First,
                    Filter.PairAlertSize == null ? "" : Filter.PairAlertSize.Second);
            }
            if (!String.IsNullOrEmpty(param) && param.Length > 1)
            {
                param = param.Remove(param.Length - 1, 1);
                JS.Write("itemsParam = [{0}];", param);
            }
        }

        private void SetProperty()
        {
            EmployeeId = Request.QueryString["id"];
            PersonId = Request.QueryString["personid"];
            PrimaryEmployeeCalc = Request.QueryString["prim"] == "True";
            StartPeriod = Convert.ToDateTime(Request.QueryString["date"]);
            Tz = Request.QueryString["tz"];
            _cp = new ExecQuery().GetCardPersonById(EmployeeId);
            if (PrimaryEmployeeCalc)
                ep1.Checked = true;
            else
                ep2.Checked = true;
            SetEmployeeInfo();
            Refresh();
            HelpUrl = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath + "/Forms/hlp/help.htm?id=2";
            btnHlp.Attributes.Add("onclick", String.Format("v4_windowOpen('{0}');", HelpUrl));
            btnHlp.Attributes.Add("title", Resx.GetString("cmdHelp"));
        }

        private void Refresh()
        {
            int tz; try { tz = Convert.ToInt32(Tz); }
            catch { tz = 0; }
            int id; try { id = Convert.ToInt32(EmployeeId); }
            catch { id = 0; }
            periodsInfo = new EmployeeTimePeriodsInfo(StartPeriod, id, PrimaryEmployeeCalc, -tz);
            if (periodsInfo != null && periodsInfo.EntranceTime == null && periodsInfo.HasErrors)
            {
                LNoEntrance = String.Format("<p style=\"color=red\">{0}</p>", Resx.GetString("lNoEntrance"));
            }

            if (periodsInfo != null && periodsInfo.ExitTime == null && periodsInfo.HasErrors)
            {
                if (periodsInfo.EntranceTime != null)
                {
                    if (((DateTime)periodsInfo.EntranceTime).Date != DateTime.Today)
                    {
                        LNoExit = String.Format("<p style=\"color=red\">{0}</p>", Resx.GetString("lNoExit"));
                    }
                }
            }
            CompleteWorkTimePeriodsList();
            CompleteAbsentTimePeriodsList();
            if (periodsInfo != null && !periodsInfo.HasErrors) JS.Write("HideCalcType();");
            JS.Write("UTC2LocalTimeDetails();");
            //S.Write("$(\"*\").css(\"cursor\", \"auto\");");
        }

        private void CompleteWorkTimePeriodsList()
        {
            tPeriodsList.Rows.Clear();
            int count = 1;
            foreach (TimeInterval interval in periodsInfo.EmployeeWorkTimeInervals)
            {
                bool isErrNoExit = false;
                string strStartTime = interval.StartUndefined ? "<b>" + Resx.GetString("lNoFixedEnter") + "</b>" : String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                    (interval.StartTime).Year.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Month.ToString(CultureInfo.InvariantCulture), 
                    (interval.StartTime).Day.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Hour.ToString(CultureInfo.InvariantCulture), 
                    (interval.StartTime).Minute.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Second.ToString(CultureInfo.InvariantCulture));
                string strEndTime = interval.EndUndefined ? "<b>" + Resx.GetString("lNoFixedExit") + "</b>" : String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                    (interval.EndTime).Year.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Month.ToString(CultureInfo.InvariantCulture), 
                    (interval.EndTime).Day.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Hour.ToString(CultureInfo.InvariantCulture), 
                    (interval.EndTime).Minute.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Second.ToString(CultureInfo.InvariantCulture));

                //object period = String.Format("<P align=\"center\">{0}:{1}:{2}</P>", interval.PeriodDuration.Days * 24 + interval.PeriodDuration.Hours, 
                //    interval.PeriodDuration.Minutes.ToString("D2"), interval.PeriodDuration.Seconds.ToString("D2"));
                object period = String.Format("{0}:{1}:{2}", interval.PeriodDuration.Days * 24 + interval.PeriodDuration.Hours,
                    interval.PeriodDuration.Minutes.ToString("D2"), interval.PeriodDuration.Seconds.ToString("D2"));

                String locationName = interval.PlaceName.Equals(String.Empty) ? "<b>" + Resx.GetString("lNoFixed") + "</b>" : interval.PlaceName;
                
                // MTODO: Отсутствует англоязычный вариант названия расположения. Как внедрят необходимо поправить.
                if (periodsInfo.EmployeeWorkTimeInervals.Count == count && !String.IsNullOrEmpty(LNoExit))
                    isErrNoExit = true;

                tPeriodsList.Rows.Add(new object[] { strStartTime, strEndTime, period, locationName, (interval.StartUndefined || interval.EndUndefined || isErrNoExit) ? "errorRow" : "singleRow" });
                count++;
            }

            if (tPeriodsList.Rows.Count == 0)
            {
                JS.Write(String.Format("document.all('listTableWork').innerHTML = '<P align=center>{0}</P>';", Resx.GetString("lNoData")));
            }
            else
            {
                intervalList.DataBind();

                foreach (DataGridItem item in intervalList.Items)
                {
                    int index = item.DataSetIndex;
                    if (index < tPeriodsList.Rows.Count)
                    {
                        item.CssClass = tPeriodsList.Rows[index]["ERROR_CSS"].ToString();
                    }
                }

                TextWriter stringWriter = new StringWriter();
                var writer = new HtmlTextWriter(stringWriter);

                intervalList.RenderControl(writer);
                String tableContent = stringWriter.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "");
                if (tPeriodsList.Rows.Count > 1)
                {
                    string totalTime = String.Format("{0}:{1}:{2}", periodsInfo.TotalWorkTime.Days * 24 + periodsInfo.TotalWorkTime.Hours, 
                        periodsInfo.TotalWorkTime.Minutes.ToString("D2"), periodsInfo.TotalWorkTime.Seconds.ToString("D2"));
                    string newTableEnd = String.Format("<tr><td colspan=3 align=right><b>{0}&nbsp;</b></td><td align=center><b>{1}</b></td></tr></table>", 
                        Resx.GetString("lTotal"), totalTime);
                    tableContent = tableContent.Replace("</table>", newTableEnd);
                }
                JS.Write(String.Format("document.all('listTableWork').innerHTML = '{0}';", tableContent));
            }
        }

        private void CompleteAbsentTimePeriodsList()
        {
            tPeriodsListAbs.Rows.Clear();

            foreach (TimeInterval interval in periodsInfo.EmployeeAbsentTimeInervals)
            {
                if (interval.PeriodDuration.Hours > 7)
                    continue;
                string strStartTime = interval.StartUndefined ? "<b>" + Resx.GetString("lNoFixedEnter") + "</b>" : String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                    (interval.StartTime).Year.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Month.ToString(CultureInfo.InvariantCulture), 
                    (interval.StartTime).Day.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Hour.ToString(CultureInfo.InvariantCulture), 
                    (interval.StartTime).Minute.ToString(CultureInfo.InvariantCulture), (interval.StartTime).Second.ToString(CultureInfo.InvariantCulture));
                string strEndTime = interval.EndUndefined ? "<b>" + Resx.GetString("lNoFixedExit") + "</b>" : String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                    (interval.EndTime).Year.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Month.ToString(CultureInfo.InvariantCulture), 
                    (interval.EndTime).Day.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Hour.ToString(CultureInfo.InvariantCulture), 
                    (interval.EndTime).Minute.ToString(CultureInfo.InvariantCulture), (interval.EndTime).Second.ToString(CultureInfo.InvariantCulture));

                //object period = String.Format("<P align=\"center\">{0}:{1}:{2}</P>", interval.PeriodDuration.Days * 24 + interval.PeriodDuration.Hours, 
                //    interval.PeriodDuration.Minutes.ToString("D2"), interval.PeriodDuration.Seconds.ToString("D2"));
                object period = String.Format("{0}:{1}:{2}", interval.PeriodDuration.Days * 24 + interval.PeriodDuration.Hours,
                    interval.PeriodDuration.Minutes.ToString("D2"), interval.PeriodDuration.Seconds.ToString("D2"));

                tPeriodsListAbs.Rows.Add(new object[] { strStartTime, strEndTime, period, (interval.StartUndefined || interval.EndUndefined) ? "errorRow" : "singleRow" });
            }

            if (tPeriodsListAbs.Rows.Count == 0)
            {
                JS.Write(String.Format("document.all('listTableAbs').innerHTML = '<P align=center>{0}</P>';", Resx.GetString("lNoData")));
            }
            else
            {
                intervalListAbs.DataBind();

                foreach (DataGridItem item in intervalListAbs.Items)
                {
                    int index = item.DataSetIndex;
                    if (index < tPeriodsListAbs.Rows.Count)
                    {
                        item.CssClass = tPeriodsListAbs.Rows[index]["ERROR_CSS"].ToString();
                    }
                }

                TextWriter stringWriter = new StringWriter();
                var writer = new HtmlTextWriter(stringWriter);

                intervalListAbs.RenderControl(writer);
                String tableContent = stringWriter.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "");
                if (tPeriodsListAbs.Rows.Count > 1)
                {
                    string totalTime = String.Format("{0}:{1}:{2}", periodsInfo.TotalAbsentTime.Days * 24 + periodsInfo.TotalAbsentTime.Hours, 
                        periodsInfo.TotalAbsentTime.Minutes.ToString("D2"), periodsInfo.TotalAbsentTime.Seconds.ToString("D2"));
                    string newTableEnd = String.Format("<tr><td colspan=2 align=right><b>{0}&nbsp;</b></td><td align=center><b>{1}</b></td></tr></table>", 
                        Resx.GetString("lTotal"), totalTime);
                    tableContent = tableContent.Replace("</table>", newTableEnd);
                }
                JS.Write(String.Format("document.all('listTableAbs').innerHTML = '{0}';", tableContent));
            }
        }

        protected void SetEmployeeInfo()
        {
            JS.Write("EmployeeInfo.innerHTML = '{0}';", String.Format("<font size=4pt color=#000088 onclick=\"ItemClick(\\'{1}\\', \\'{2}\\');\">{0}</font>", IsRusLocal ? _cp.FIO : _cp.Employee, EmployeeId, Lib.Web.Settings.Config.user_form));
        }

        private void InitDataTable()
        {
            tPeriodsList = new DataTable();
            START_TIME = new DataColumn("START_TIME", typeof(String));
            END_TIME = new DataColumn("END_TIME", typeof(String));
            INTERVAL = new DataColumn("INTERVAL", typeof(String));
            PLACE = new DataColumn("PLACE", typeof(String));
            ERROR_CSS = new DataColumn("ERROR_CSS", typeof(String));

            tPeriodsList.TableName = "tPeriodsList";
            tPeriodsList.Columns.Add(START_TIME);
            tPeriodsList.Columns.Add(END_TIME);
            tPeriodsList.Columns.Add(INTERVAL);
            tPeriodsList.Columns.Add(PLACE);
            tPeriodsList.Columns.Add(ERROR_CSS);

            foreach (DataGridColumn bcol in intervalList.Columns)
            {
                if (bcol is BoundColumn)
                {
                    var col = (BoundColumn)bcol;
                    switch (col.DataField)
                    {
                        case "START_TIME":
                            bcStartTime = col;
                            break;
                        case "END_TIME":
                            bcEndTime = col;
                            break;
                        case "INTERVAL":
                            bcInterval = col;
                            break;
                        case "PLACE":
                            bcPlace = col;
                            break;
                    }
                }
            }

            bcInterval.HeaderText = Resx.GetString("cInterval");
            bcPlace.HeaderText = Resx.GetString("cPlace");

            intervalList.DataSource = tPeriodsList;

            tPeriodsListAbs = new DataTable();

            START_TIMEAbs = new DataColumn("START_TIME", typeof(String));
            END_TIMEAbs = new DataColumn("END_TIME", typeof(String));
            INTERVALAbs = new DataColumn("INTERVAL", typeof(String));
            ERROR_CSSAbs = new DataColumn("ERROR_CSS", typeof(String));

            tPeriodsListAbs.TableName = "tPeriodsListAbs";
            tPeriodsListAbs.Columns.Add(START_TIMEAbs);
            tPeriodsListAbs.Columns.Add(END_TIMEAbs);
            tPeriodsListAbs.Columns.Add(INTERVALAbs);
            tPeriodsListAbs.Columns.Add(ERROR_CSSAbs);

            foreach (DataGridColumn bcol in intervalListAbs.Columns)
            {
                if (bcol is BoundColumn)
                {
                    var col = (BoundColumn)bcol;
                    switch (col.DataField)
                    {
                        case "START_TIME":
                            bcStartTimeAbs = col;
                            break;
                        case "END_TIME":
                            bcEndTimeAbs = col;
                            break;
                        case "INTERVAL":
                            bcIntervalAbs = col;
                            break;
                    }
                }
            }
            bcIntervalAbs.HeaderText = Resx.GetString("сAbsent");
            intervalListAbs.DataSource = tPeriodsListAbs;
        }

        private void ShowLookup(string emplId)
        {
            JS.Write(string.Format("mouseOver('{0}', '{1}');", emplId, Global.UserPhoto));
        }
    }
}