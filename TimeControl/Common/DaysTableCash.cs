using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Resources;
using Kesco.App.Web.TimeControl.DataSets;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.DALC;
using Kesco.Lib.Localization;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    ///     Класс для работы с периодом
    /// </summary>
    [Serializable]
    public class DaysTableCash
    {
        //private bool _needCheckIndex;
        //private Int32 _maxIndex;
        private int _emplId;
        private string _empNameLat;
        private string _empNameRus;
        private DateTime _endDate;
        private HolidaysInfoDs _holidays;

        /// <summary>
        ///     Локализация
        /// </summary>
        private readonly ResourceManager _resx = Resources.Resx;

        private DataTable _sourceTable;
        private DateTime _startDate;

        /// <summary>
        ///     Признак наличия нарушений режима
        /// </summary>
        public bool IsError;

        /// <summary>
        ///     Признак расчета нарушений режима в пользу сотрудника
        /// </summary>
        public bool PrimaryEmployeeCalc;

        /// <summary>
        ///     Конструктор класса
        /// </summary>
        public DaysTableCash()
        {
        }

        /// <summary>
        ///     Конструктор класса с параметрами
        /// </summary>
        /// <param name="sourceTable">Источник данных</param>
        /// <param name="primaryEmployeeCalc">Признак расчета нарушений режима в пользу сотрудника</param>
        public DaysTableCash(DataTable sourceTable, bool primaryEmployeeCalc)
        {
            _sourceTable = sourceTable;
            PrimaryEmployeeCalc = primaryEmployeeCalc;
            ClearCash();
        }

        /// <summary>
        ///     Очистка кеша
        /// </summary>
        public void ClearCash()
        {
            //_maxIndex = -1;
            _emplId = -1;
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MinValue;
            _sourceTable.Rows.Clear();
            //_needCheckIndex = false;
        }

        /// <summary>
        ///     Получение строки с празником
        /// </summary>
        /// <param name="sourceDay">Строка с празником</param>
        /// <returns></returns>
        public HolidaysInfoDs.ПраздникиRow GetHolidayInfo(DateTime sourceDay)
        {
            HolidaysInfoDs.ПраздникиRow result = null;
            var rows = (HolidaysInfoDs.ПраздникиRow[]) _holidays.Праздники.Select(
                "Дата = #" + sourceDay.ToString("yyyy-MM-dd") + "#");

            if (rows.Length > 0) result = rows[0];
            return result;
        }

        /// <summary>
        ///     Получение списка праздников за выбранный период
        /// </summary>
        /// <param name="startDate">Дата начала</param>
        /// <param name="endDate">Дата конца</param>
        /// <returns>Датасет с праздниками за выбранный период</returns>
        public HolidaysInfoDs CompleteHolidaysInfoDs(DateTime startDate, DateTime endDate)
        {
            var nStartDate = startDate.ToSqlDateNormalized();
            var nEndDate = endDate.ToSqlDateNormalized();

            var sStartDate = nStartDate.ToString("yyyy-MM-dd HH:mm:ss");
            var sEndDate = nEndDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            var result = new HolidaysInfoDs();

            var sql = string.Format(
                "SELECT Дата, Название, РабочийВыходной, Праздник FROM Праздники WHERE (Дата >= convert(datetime, '{0}', 120)) AND (Дата <= convert(datetime, '{1}', 120)) AND (КодТерритории = 188)",
                sStartDate, sEndDate);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            result.Праздники.Clear();
            result.Праздники.Merge(dt);
            return result;
        }

        /// <summary>
        ///     Получение списка дней за выбранный период
        /// </summary>
        /// <param name="startPeriodParam">Дата начала</param>
        /// <param name="endPeriodParam">Дата конца</param>
        /// <param name="semplId">ID сотрудника</param>
        /// <param name="stz">Таймзона</param>
        public void CompleteDaysTable(DateTime? startPeriodParam, DateTime? endPeriodParam, string semplId, string stz)
        {
            int tz, emplId;
            try
            {
                tz = Convert.ToInt32(stz);
            }
            catch
            {
                tz = 0;
            }

            try
            {
                emplId = Convert.ToInt32(semplId);
            }
            catch
            {
                emplId = 0;
            }

            //bool hasNewIds = _needCheckIndex && (EmployeeTimePeriodsInfo.GetMaxId(_startDate, _endDate, _emplId, -tz) != _maxIndex);
            //bool sourceChanged = (_startDate == DateTime.MinValue) || (_endDate == DateTime.MinValue) || hasNewIds;
            //bool sourceChanged = true;
            var startPeriod = startPeriodParam ?? DateTime.MinValue;
            var endPeriod = endPeriodParam ?? DateTime.MinValue;

            //if (emplId != _emplId || sourceChanged)
            //{
            //    ClearCash();
            //}
            ClearCash();
            if (startPeriod != _startDate || endPeriod != _endDate)
            {
                IsError = false;
                _holidays = CompleteHolidaysInfoDs(startPeriod, endPeriod);
                var minDate = DateTime.MaxValue;
                var maxDate = DateTime.MinValue;

                var temp = _sourceTable.Clone();

                foreach (DataRow row in _sourceTable.Rows)
                    if (((DateTime) row["DAY_VALUE"]).Date <= endPeriod.Date &&
                        ((DateTime) row["DAY_VALUE"]).Date >= startPeriod.Date)
                    {
                        if (minDate > ((DateTime) row["DAY_VALUE"]).Date) minDate = ((DateTime) row["DAY_VALUE"]).Date;

                        if (maxDate < ((DateTime) row["DAY_VALUE"]).Date) maxDate = ((DateTime) row["DAY_VALUE"]).Date;

                        temp.ImportRow(row);
                    }

                _sourceTable.Rows.Clear();
                _sourceTable = temp.Copy();

                EmployeePeriodsInfoDs periodsDs;

                if (maxDate < minDate)
                {
                    _startDate = DateTime.MinValue;
                    _endDate = DateTime.MinValue;
                    periodsDs = new ExecQuery().CompleteEmployeePeriodsInfoDs(emplId, startPeriod.Date, endPeriod.Date,
                        -tz);
                }
                else
                {
                    if (minDate.Date != startPeriod.Date && maxDate.Date != endPeriod.Date)
                        periodsDs = new ExecQuery().CompleteEmployeePeriodsInfoDs(emplId, startPeriod.Date,
                            minDate.Date, maxDate.Date, endPeriod.Date.AddDays(1));
                    else if (minDate.Date != startPeriod.Date)
                        periodsDs = new ExecQuery().CompleteEmployeePeriodsInfoDs(emplId, startPeriod.Date,
                            minDate.Date, -tz);
                    else if (maxDate.Date != endPeriod.Date)
                        periodsDs = new ExecQuery().CompleteEmployeePeriodsInfoDs(emplId, maxDate.Date, endPeriod.Date,
                            -tz);
                    else
                        periodsDs = new EmployeePeriodsInfoDs();

                    _startDate = minDate;
                    _endDate = maxDate;
                }

                if (periodsDs.Сотрудники.Rows.Count > 0)
                {
                    _empNameRus = ((EmployeePeriodsInfoDs.СотрудникиRow) periodsDs.Сотрудники.Rows[0]).Сотрудник;
                    _empNameLat = ((EmployeePeriodsInfoDs.СотрудникиRow) periodsDs.Сотрудники.Rows[0]).Employee;
                }

                var dayIntervals = new EmployeeTimeInervals();

                for (var curentDay = startPeriod; curentDay <= endPeriod; curentDay = curentDay.AddDays(1))
                    dayIntervals.Add(new TimeInterval(curentDay.Date, curentDay.Date.AddDays(1), -1));
                var isEnterPrevDayByDay = false;
                foreach (TimeInterval curInterval in dayIntervals)
                {
                    if (curInterval.StartTime.Date >= _startDate.Date &&
                        curInterval.StartTime.Date <= _endDate.Date) continue;

                    var periodsInfo = new EmployeeTimePeriodsInfo(curInterval.StartTime, curInterval.EndTime, emplId,
                        PrimaryEmployeeCalc, periodsDs.ПроходыСотрудников, true, isEnterPrevDayByDay);
                    isEnterPrevDayByDay = periodsInfo.IsEnterPrevDayByDay;
                    var internetAccessInfo =
                        new EmployeeInternetAccessInfo(curInterval.StartTime, curInterval.EndTime, emplId);

                    object linkCell = null;

                    if (periodsInfo.EntranceTime != null || periodsInfo.ExitTime != null ||
                        periodsInfo.ExitTimePrevDay != null && periodsInfo.EnterTimeNextDay != null)
                        linkCell = string.Format(
                            "<A href=\"#\" onclick=\"cmd('cmd', 'openDetails', 'id', '{0}', 'from', '{1}', 'to', '{2}');\"><IMG src=\"{3}detail.GIF\" border=\"0\" title=\"{4}\"></A>",
                            emplId, curInterval.StartTime, curInterval.EndTime, Global.Styles,
                            _resx.GetString("hPageSubTitleDetails2"));

                    var totalWorkTime = periodsInfo.TotalWorkTime;
                    var totalAbsentTime = periodsInfo.TotalAbsentTime;

                    var strEntranceTime = "";
                    var strExitTime = "";
                    var isEnterAfterExit = "";
                    var isEnterAfterExit2 = "";

                    if (periodsInfo.ExitTime != null)
                        strExitTime = ((DateTime) periodsInfo.ExitTime).ToString("HH:mm:ss");

                    if (periodsInfo.EntranceTime != null)
                        strEntranceTime = ((DateTime) periodsInfo.EntranceTime).ToString("HH:mm:ss");

                    if (periodsInfo.ExitTime != null && periodsInfo.EntranceTime != null &&
                        (DateTime) periodsInfo.EntranceTime > (DateTime) periodsInfo.ExitTime)
                    {
                        isEnterAfterExit = "<div>00:00:00</div><hr />";
                        isEnterAfterExit2 = "<hr /><div>24:00:00</div>";
                    }

                    if (periodsInfo.ExitTime == null && periodsInfo.EntranceTime == null &&
                        periodsInfo.ExitTimePrevDay != null && periodsInfo.EnterTimeNextDay != null)
                    {
                        //strExitTime = String.Format("{0}-{1}-{2} {3}:{4}:{5}", ((DateTime)periodsInfo.ExitTimePrevDay).Year.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.ExitTimePrevDay).Month.ToString(CultureInfo.InvariantCulture), ((DateTime)periodsInfo.ExitTimePrevDay).Day.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.ExitTimePrevDay).Hour.ToString(CultureInfo.InvariantCulture), ((DateTime)periodsInfo.ExitTimePrevDay).Minute.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.ExitTimePrevDay).Second.ToString(CultureInfo.InvariantCulture));
                        strExitTime = ((DateTime) periodsInfo.ExitTimePrevDay).ToString("HH:mm:ss");
                        //strEntranceTime = String.Format("{0}-{1}-{2} {3}:{4}:{5}", ((DateTime)periodsInfo.EnterTimeNextDay).Year.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.EnterTimeNextDay).Month.ToString(CultureInfo.InvariantCulture), ((DateTime)periodsInfo.EnterTimeNextDay).Day.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.EnterTimeNextDay).Hour.ToString(CultureInfo.InvariantCulture), ((DateTime)periodsInfo.EnterTimeNextDay).Minute.ToString(CultureInfo.InvariantCulture), 
                        //    ((DateTime)periodsInfo.EnterTimeNextDay).Second.ToString(CultureInfo.InvariantCulture));
                        strEntranceTime = ((DateTime) periodsInfo.EnterTimeNextDay).ToString("HH:mm:ss");
                        isEnterAfterExit = "<div>00:00:00</div><hr />";
                        isEnterAfterExit2 = "<hr /><div>24:00:00</div>";
                    }

                    if (periodsInfo.ExitTime == null && periodsInfo.EntranceTime != null &&
                        periodsInfo.EnterTimeNextDay != null) strExitTime = "24:00:00";
                    if (periodsInfo.ExitTime != null && periodsInfo.EntranceTime == null &&
                        periodsInfo.ExitTimePrevDay != null) strEntranceTime = "00:00:00";

                    object absentTime = null;
                    object workTime = null;

                    if (totalAbsentTime.TotalSeconds > 0)
                        absentTime = string.Format("{0}:{1}:{2}", totalAbsentTime.Hours + totalAbsentTime.Days * 24,
                            totalAbsentTime.Minutes.ToString("D2"), totalAbsentTime.Seconds.ToString("D2"));

                    if (totalWorkTime.TotalSeconds > 0)
                    {
                        //workTime = String.Format("<P align=\"center\">{0}:{1}:{2}</P>", totalWorkTime.Hours + (totalWorkTime.Days * 24), totalWorkTime.Minutes.ToString("D2"), 
                        //    totalWorkTime.Seconds.ToString("D2"));
                        workTime = string.Format("{0}:{1}:{2}", totalWorkTime.Hours + totalWorkTime.Days * 24,
                            totalWorkTime.Minutes.ToString("D2"), totalWorkTime.Seconds.ToString("D2"));
                        if (periodsInfo.HasErrors) workTime = workTime + " *";
                    }

                    var internetAccessInfoCount = internetAccessInfo.Count;
                    var internetAccessCount = "";
                    if (internetAccessInfoCount != 0)
                        internetAccessCount = internetAccessInfoCount.ToString(CultureInfo.InvariantCulture);

                    string internetAccessTotal;
                    var internetAccessInfoTotal = internetAccessInfo.Total;
                    if (internetAccessInfoTotal == 0)
                        internetAccessTotal = "";
                    else
                        internetAccessTotal = new TimeSpan(0, 0, internetAccessInfoTotal).ToString();

                    _sourceTable.Rows.Add(linkCell, curInterval.StartTime, strEntranceTime, strExitTime, absentTime,
                        workTime, totalAbsentTime, totalWorkTime, periodsInfo.HasErrors ? "errorRow" : "singleRow",
                        internetAccessCount, internetAccessTotal, isEnterAfterExit, isEnterAfterExit2);
                    if (periodsInfo.HasErrors && !IsError) IsError = true;
                }

                _emplId = emplId;
                _startDate = startPeriod;
                _endDate = endPeriod;
                //_maxIndex = EmployeeTimePeriodsInfo.GetMaxId(_startDate, _endDate, _emplId, -tz);
                //_needCheckIndex = (_maxIndex == EmployeeTimePeriodsInfo.GetMaxId(_emplId)) || (_maxIndex == -1);
            }
        }

        /// <summary>
        ///     Сумма рабочего времени
        /// </summary>
        /// <returns>Сумма рабочего времени</returns>
        public TimeSpan GetSummaryWorkTime()
        {
            var result = new TimeSpan(0);
            return _sourceTable.Rows.Cast<DataRow>().Where(row => row["INTERVAL_SORT"] != DBNull.Value)
                .Aggregate(result, (current, row) => current + (TimeSpan) row["INTERVAL_SORT"]);
        }

        /// <summary>
        ///     Сумма перерывов
        /// </summary>
        /// <returns>Сумма перерывов</returns>
        public TimeSpan GetSummaryAbsentTime()
        {
            var result = new TimeSpan(0);
            return _sourceTable.Rows.Cast<DataRow>().Where(row => row["ABSENT_TIME_SORT"] != DBNull.Value)
                .Aggregate(result, (current, row) => current + (TimeSpan) row["ABSENT_TIME_SORT"]);
        }

        /// <summary>
        ///     Получение источника данных
        /// </summary>
        /// <param name="needEmptyRecords">Признак отображения пустых строк (без проходов)</param>
        /// <returns>Источник данных</returns>
        public DataTable GetSourceTable(bool needEmptyRecords)
        {
            if (needEmptyRecords) return _sourceTable;
            var result = _sourceTable.Clone();
            result.Rows.Clear();
            foreach (DataRow row in _sourceTable.Rows)
                if (row["START_TIME"] != DBNull.Value && !row["START_TIME"].Equals("") ||
                    row["END_TIME"] != DBNull.Value && !row["END_TIME"].Equals(""))
                    result.ImportRow(row);

            return result;
        }

        /// <summary>
        ///     Получение имени сотрудника в зависимости от текущей культуры
        /// </summary>
        /// <param name="isRusLocal">Признак русской культуры</param>
        /// <returns>Имя сотрудника</returns>
        public string GetEmployeeName(bool isRusLocal)
        {
            return isRusLocal ? _empNameRus : _empNameLat;
        }
    }
}