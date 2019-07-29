using System;
using System.Data;
using Kesco.App.Web.TimeControl.DataSets;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.DALC;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    ///     Класс для работы с периодами сотрудников
    /// </summary>
    public class EmployeeTimePeriodsInfo
    {
        private readonly bool _employeePrimary;
        private readonly bool _resolvePlaces;
        private readonly DateTime _endTime;

        //private DataRow[] _sourceRows;
        //private DataRow[] _sourceRowsByDay;
        private EnterEvent[] _sourceEvents;
        private EnterEvent[] _sourceEventsByDay;
        private readonly DateTime _startTime;

        /// <summary>
        ///     Признак прохода в предыдущий день
        /// </summary>
        public bool IsEnterPrevDayByDay;

        /// <summary>
        ///     Метод получения проходов сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="emplCode">Код сотрудника</param>
        /// <param name="pEmployeePrimary"></param>
        /// <param name="sourceTable"></param>
        /// <param name="isByDay"></param>
        /// <param name="isEnterPrevDayByDay"></param>
        public EmployeeTimePeriodsInfo(DateTime startPeriod, DateTime endPeriod, int emplCode, bool pEmployeePrimary,
            EmployeePeriodsInfoDs.ПроходыСотрудниковDataTable sourceTable,
            bool isByDay = false, bool isEnterPrevDayByDay = false)
        {
            IsEnterPrevDayByDay = isEnterPrevDayByDay;
            _employeePrimary = pEmployeePrimary;
            _startTime = startPeriod;
            _endTime = endPeriod;

            ComlpeteEventsTableFromDataTable(startPeriod, endPeriod, emplCode, sourceTable, isByDay);
            if (isByDay)
                CompletePeriodsByDay();
            else
                CompletePeriods();
        }

        /// <summary>
        ///     Метод получения проходов сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="emplCode">Код сотрудника</param>
        /// <param name="pEmployeePrimary"></param>
        /// <param name="tz"></param>
        public EmployeeTimePeriodsInfo(DateTime startPeriod, int emplCode, bool pEmployeePrimary, int tz)
        {
            _employeePrimary = pEmployeePrimary;
            _startTime = startPeriod;
            _endTime = startPeriod.Date.AddDays(1);
            _resolvePlaces = true;
            ComlpeteEventsTableFromDB(startPeriod.AddDays(-1), startPeriod.AddDays(1), emplCode, _resolvePlaces, tz,
                true);
            CompletePeriodsByDay();
        }

        /// <summary>
        ///     Признак наличия ошибок
        /// </summary>
        public bool HasErrors { get; private set; }

        /// <summary>
        ///     Интервалы нахождения на рабочем месте
        /// </summary>
        public EmployeeTimeInervals EmployeeWorkTimeInervals { get; } = new EmployeeTimeInervals();

        /// <summary>
        ///     Интервалы отсутствия на рабочем месте
        /// </summary>
        public EmployeeTimeInervals EmployeeAbsentTimeInervals { get; } = new EmployeeTimeInervals();

        /// <summary>
        ///     Время вхождения в офис
        /// </summary>
        public object EntranceTime
        {
            get
            {
                object res = null;
                DateTime temp;
                if (_startTime == _endTime)
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                else
                    temp = _endTime.AddSeconds(-1);

                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (var e in _sourceEvents)
                        if (e.when.Day == _startTime.Day)
                        {
                            if (e.code.HasValue) return e.when;
                            return null;
                        }

                    /*
                    foreach (var item in _sourceRows)
                    {
                        if (((DateTime)item["Когда"]).Day == _startTime.Day)
                        {
                            if (item["КодРасположения"] != DBNull.Value)
                            {
                                return item["Когда"];
                            }
                            return null;
                        }
                    }
                     * */
                    return null;
                }

                if (_sourceEvents != null && _sourceEvents.Length > 0)
                {
                    if (_sourceEvents[0].code.HasValue)
                        res = _sourceEvents[0].when;
                    else if (IsEnterPrevDayByDay && _sourceEvents.Length > 1)
                        if (_sourceEvents[1].code.HasValue)
                            res = _sourceEvents[1].when;
                }

                /*
                if (_sourceRows != null && _sourceRows.Length > 0)
                {
                    if (_sourceRows[0]["КодРасположения"] != DBNull.Value)
                    {
                        res = _sourceRows[0]["Когда"];
                    }
                    else if (IsEnterPrevDayByDay && _sourceRows.Length > 1)
                    {
                        if (_sourceRows[1]["КодРасположения"] != DBNull.Value)
                        {
                            res = _sourceRows[1]["Когда"];
                        }
                    }
                }
                 * */
                return res;
            }
        }

        /// <summary>
        ///     Время выхода из офиса
        /// </summary>
        public object ExitTime
        {
            get
            {
                object res = null;
                DateTime temp;
                if (_startTime == _endTime)
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                else
                    temp = _endTime.AddSeconds(-1);

                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    /*
                    for (int i = _sourceRows.Length - 1; i > -1; i--)
                    {
                        if (((DateTime)_sourceRows[i]["Когда"]).Day == _startTime.Day)
                        {
                            if (_sourceRows[i]["КодРасположения"] == DBNull.Value)
                            {
                                return _sourceRows[i]["Когда"];
                            }
                            return null;
                        }
                    }
                     * */

                    for (var i = _sourceEvents.Length - 1; i > -1; i--)
                        if (_sourceEvents[i].when.Day == _startTime.Day)
                        {
                            if (!_sourceEvents[i].code.HasValue) return _sourceEvents[i].when;

                            return null;
                        }

                    return null;
                }

                if (_sourceEvents != null && _sourceEvents.Length > 0)
                {
                    if (!_sourceEvents[_sourceEvents.Length - 1].code.HasValue)
                        res = _sourceEvents[_sourceEvents.Length - 1].when;
                    else if (IsEnterPrevDayByDay)
                        if (!_sourceEvents[0].code.HasValue)
                            res = _sourceEvents[0].when;
                }

                /*
                if (_sourceRows != null && _sourceRows.Length > 0)
                {
                    if (_sourceRows[_sourceRows.Length - 1]["КодРасположения"] == DBNull.Value)
                    {
                        res = _sourceRows[_sourceRows.Length - 1]["Когда"];
                    }
                    else if (IsEnterPrevDayByDay)
                    {
                        if (_sourceRows[0]["КодРасположения"] == DBNull.Value)
                        {
                            res = _sourceRows[0]["Когда"];
                        }
                    }
                }
                */

                return res;
            }
        }

        /// <summary>
        ///     Время выхода из офиса в предыдущий день
        /// </summary>
        public object ExitTimePrevDay
        {
            get
            {
                DateTime temp;
                if (_startTime == _endTime)
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                else
                    temp = _endTime.AddSeconds(-1);
                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (TimeInterval interval in EmployeeAbsentTimeInervals)
                        if (interval.PeriodDuration.Hours > 7)
                            return interval.StartTime;

                    for (var i = 0; i < _sourceEvents.Length; i++)
                        if (_sourceEvents[i].when.Day == _startTime.Day)
                        {
                            if (!_sourceEvents[i].code.HasValue) return _sourceEvents[i].when;
                            return null;
                        }

                    /*
                    for (int i = 0; i < _sourceRows.Length; i++)
                    {
                        if (((DateTime)_sourceRows[i]["Когда"]).Day == _startTime.Day)
                        {
                            if (_sourceRows[i]["КодРасположения"] == DBNull.Value)
                            {
                                return _sourceRows[i]["Когда"];
                            }
                            return null;
                        }
                    }
                    */
                }

                return null;
            }
        }

        /// <summary>
        ///     Время входа в офис в следующий день
        /// </summary>
        public object EnterTimeNextDay
        {
            get
            {
                DateTime temp;
                if (_startTime == _endTime)
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                else
                    temp = _endTime.AddSeconds(-1);
                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (TimeInterval interval in EmployeeAbsentTimeInervals)
                        if (interval.PeriodDuration.Hours > 7)
                            return interval.EndTime;

                    for (var i = _sourceEvents.Length - 1; i > -1; i--)
                        if (_sourceEvents[i].when.Day == _startTime.Day && _startTime.Day != DateTime.Now.Day)
                        {
                            if (_sourceEvents[i].code.HasValue) return _sourceEvents[i].when;
                            return null;
                        }

                    /*
                    for (int i = _sourceRows.Length - 1; i > -1; i--)
                    {
                        if (((DateTime)_sourceRows[i]["Когда"]).Day == _startTime.Day && _startTime.Day != DateTime.Now.Day)
                        {
                            if (_sourceRows[i]["КодРасположения"] != DBNull.Value)
                            {
                                return _sourceRows[i]["Когда"];
                            }
                            return null;
                        }
                    }
                     * */
                }

                return null;
            }
        }

        /// <summary>
        ///     Общее время работы
        /// </summary>
        public TimeSpan TotalWorkTime
        {
            get
            {
                var res = new TimeSpan(0, 0, 0);
                foreach (TimeInterval interval in EmployeeWorkTimeInervals) res += interval.PeriodDuration;
                return res;
            }
        }

        /// <summary>
        ///     Общее время перерывов
        /// </summary>
        public TimeSpan TotalAbsentTime
        {
            get
            {
                var res = new TimeSpan(0, 0, 0);
                foreach (TimeInterval interval in EmployeeAbsentTimeInervals)
                    if (interval.PeriodDuration.Hours < 8)
                        res += interval.PeriodDuration;
                return res;
            }
        }

        /// <summary>
        ///     Получение кода прохода сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="emplCode">Код сотрудника</param>
        /// <param name="tz">Таймзона</param>
        /// <returns>Код прохода сотрудника</returns>
        public static int GetMaxId(DateTime startPeriod, DateTime endPeriod, int emplCode, int tz)
        {
            var sql = string.Format(
                @"SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) 
AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) AND (КодСотрудника = {2}) ORDER BY КодПроходаСотрудника DESC",
                startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                endPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), emplCode, tz);
            var resultTable = DBManager.GetData(sql, Global.ConnectionString);

            var result = -1;

            if (resultTable.Rows.Count > 0) result = (int) resultTable.Rows[0]["КодПроходаСотрудника"];
            return result;
        }

        /// <summary>
        ///     Получение кода прохода сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="tz">Таймзона</param>
        /// <returns>Код прохода сотрудника</returns>
        public static int GetMaxId(DateTime startPeriod, DateTime endPeriod, int tz)
        {
            var sql = string.Format(
                @"SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (Dateadd(MINUTE,{2},[Когда]) >= convert(datetime, '{0}', 120)) AND 
(Dateadd(MINUTE,{2},[Когда]) <= convert(datetime, '{1}', 120)) ORDER BY КодПроходаСотрудника DESC",
                startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                endPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), tz);
            var resultTable = DBManager.GetData(sql, Global.ConnectionString);

            var result = -1;

            if (resultTable.Rows.Count > 0) result = (int) resultTable.Rows[0]["КодПроходаСотрудника"];
            return result;
        }

        /// <summary>
        ///     Получение кода прохода сотрудника
        /// </summary>
        /// <returns>Код прохода сотрудника</returns>
        public static int GetMaxId()
        {
            var sql =
                "SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) ORDER BY КодПроходаСотрудника DESC";
            var resultTable = DBManager.GetData(sql, Global.ConnectionString);
            var result = -1;

            if (resultTable.Rows.Count > 0) result = (int) resultTable.Rows[0]["КодПроходаСотрудника"];

            return result;
        }

        /// <summary>
        ///     Получение кода прохода сотрудника
        /// </summary>
        /// <param name="emplCode">Код сотрудника</param>
        /// <returns>Код прохода сотрудника</returns>
        public static int GetMaxId(int emplCode)
        {
            var sql = string.Format(
                "SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (КодСотрудника = {0}) ORDER BY КодПроходаСотрудника DESC",
                emplCode);
            var resultTable = DBManager.GetData(sql, Global.ConnectionString);
            var result = -1;

            if (resultTable.Rows.Count > 0) result = (int) resultTable.Rows[0]["КодПроходаСотрудника"];

            return result;
        }

        private void ComlpeteEventsTableFromDB(DateTime startPeriod, DateTime endPeriod, int emplCode, bool rp, int tz,
            bool isByDay = false)
        {
            DataTable tEvents;
            string sql;
            if (rp)
            {
                sql = string.Format(
                    @"SELECT Dateadd(MINUTE,{3},[Когда]) as Когда, КодРасположения, CASE WHEN КодРасположения IS NULL THEN '' ELSE Считыватель END Расположение FROM vwПроходыСотрудников (nolock) 
WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) AND (КодСотрудника = {2}) ORDER BY Когда",
                    startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    endPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    emplCode, tz);
                tEvents = DBManager.GetData(sql, Global.ConnectionString);
            }
            else
            {
                sql = string.Format(
                    @"SELECT Dateadd(MINUTE,{3},[Когда]) as Когда, КодРасположения FROM vwПроходыСотрудников (nolock) 
WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) 
AND (КодСотрудника = {2}) ORDER BY Когда",
                    startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    endPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    emplCode, tz);
                tEvents = DBManager.GetData(sql, Global.ConnectionString);
            }

            var sourceEvents = new EnterEvent[tEvents.Rows.Count];

            for (var i = 0; i < tEvents.Rows.Count; i++)
            {
                var objExitTime = tEvents.Rows[i]["Когда"];
                if (DBNull.Value == objExitTime) continue;

                var rowWhen = (DateTime) tEvents.Rows[i]["Когда"];

                sourceEvents[i].when = rowWhen;
                sourceEvents[i].code = DBNull.Value == tEvents.Rows[i]["КодРасположения"]
                    ? null
                    : (int?) tEvents.Rows[i]["КодРасположения"];
                sourceEvents[i].place = _resolvePlaces ? tEvents.Rows[i]["Расположение"].ToString() : string.Empty;
            }

            if (isByDay)
            {
                var filterString = string.Format("Когда >= '{0}' AND Когда <= '{1}'",
                    startPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    endPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                var source_rows = tEvents.Select(filterString, "Когда");

                _sourceEvents = new EnterEvent[sourceEvents.Length];

                for (var i = 0; i < source_rows.Length; i++)
                {
                    var objExitTime = source_rows[i]["Когда"];
                    if (DBNull.Value == objExitTime) continue;

                    var rowWhen = (DateTime) source_rows[i]["Когда"];

                    _sourceEvents[i].when = rowWhen;
                    _sourceEvents[i].code = DBNull.Value == source_rows[i]["КодРасположения"]
                        ? null
                        : (int?) source_rows[i]["КодРасположения"];
                    _sourceEvents[i].place = _resolvePlaces ? source_rows[i]["Расположение"].ToString() : string.Empty;
                }

                //_sourceRowsByDay = tEvents.Select();
                _sourceEventsByDay = sourceEvents;
            }
            else
            {
                //_sourceRows = tEvents.Select();
                _sourceEvents = sourceEvents;
            }
        }

        private void ComlpeteEventsTableFromDataTable(DateTime startPeriod, DateTime endPeriod, int emplCode,
            EmployeePeriodsInfoDs.ПроходыСотрудниковDataTable sourceTable, bool isByDay = false)
        {
            var filterString = string.Format("Когда >= '{0}' AND Когда <= '{1}' AND КодСотрудника = {2}",
                startPeriod.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                endPeriod.Date.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"), emplCode);
            //_sourceRows = sourceTable.Select(filterString, "Когда");

            var source_rows = sourceTable.Select(filterString, "Когда");

            _sourceEvents = new EnterEvent[source_rows.Length];
            for (var i = 0; i < source_rows.Length; i++)
            {
                _sourceEvents[i].when = (DateTime) source_rows[i]["Когда"];
                _sourceEvents[i].code = DBNull.Value == source_rows[i]["КодРасположения"]
                    ? null
                    : (int?) source_rows[i]["КодРасположения"];
                _sourceEvents[i].place = _resolvePlaces ? source_rows[i]["Расположение"].ToString() : string.Empty;
            }


            if (isByDay)
            {
                filterString = string.Format("Когда >= '{0}' AND Когда <= '{1}' AND КодСотрудника = {2}",
                    startPeriod.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                    endPeriod.Date.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"), emplCode);
                //_sourceRowsByDay = sourceTable.Select(filterString, "Когда");

                source_rows = sourceTable.Select(filterString, "Когда");

                _sourceEventsByDay = new EnterEvent[source_rows.Length];
                for (var i = 0; i < source_rows.Length; i++)
                {
                    _sourceEventsByDay[i].when = (DateTime) source_rows[i]["Когда"];
                    _sourceEventsByDay[i].code = DBNull.Value == source_rows[i]["КодРасположения"]
                        ? null
                        : (int?) source_rows[i]["КодРасположения"];
                    _sourceEventsByDay[i].place =
                        _resolvePlaces ? source_rows[i]["Расположение"].ToString() : string.Empty;
                }
            }
        }

        private void CompletePeriods()
        {
            var ts = _endTime.AddDays(1) - _startTime;
            var totalDays = ts.TotalDays; // число дней в диапазоне дат
            var t = _startTime - _startTime;
            HasErrors = false;

            //if (_sourceRows.Length > 0)
            if (_sourceEvents.Length < 1) return;

            EmployeeWorkTimeInervals.Clear();
            EmployeeAbsentTimeInervals.Clear();

            for (double dayNum = 0; dayNum < totalDays; dayNum++)
            {
                // нарезка диапазона по суткам
                var expectingEntrance = true;
                var enterTime = DateTime.MinValue;
                var exitTimeEmpty = true;
                var exitTime = DateTime.MinValue;
                var enterCode = -1;
                var enterPlace = string.Empty;
                var isExitNextDay = false;
                var isEnterTomorow = false;
                var firstEnterTomorow = -1;

                var nextDay = _startTime.AddDays(dayNum + 1);
                var currentDay = _startTime.AddDays(dayNum);

                for (var i = 0; i < _sourceEvents.Length; i++)
                {
                    if (_sourceEvents[i].when > nextDay.AddDays(1)) break;

                    var rowExitTime = _sourceEvents[i].when;

                    var fEnterCodeIsNull = !_sourceEvents[i].code.HasValue;
                    var fGreaterThanNext = rowExitTime >= nextDay;
                    var fLessThanCurrent = rowExitTime < currentDay;


                    if (fLessThanCurrent || fGreaterThanNext
                    ) /* если запись не попадает в диапазон обрабатываемого дня */
                    {
                        if (fGreaterThanNext && fEnterCodeIsNull)
                        {
                            isExitNextDay = true;
                            if (!isEnterTomorow) firstEnterTomorow = i;
                        }
                        else if (fGreaterThanNext && !fEnterCodeIsNull)
                        {
                            if (firstEnterTomorow == -1)
                            {
                                isEnterTomorow = true;
                                firstEnterTomorow = i;
                            }
                        }
                        else if (fLessThanCurrent && !fEnterCodeIsNull)
                        {
                            IsEnterPrevDayByDay = true;
                            enterCode = (int) _sourceEvents[i].code;
                            enterPlace = _sourceEvents[i].place;
                        }
                        else if (fLessThanCurrent && fEnterCodeIsNull)
                        {
                            IsEnterPrevDayByDay = false;
                        }

                        continue;
                    }

                    if (expectingEntrance)
                    {
                        if (fEnterCodeIsNull)
                        {
                            if (IsEnterPrevDayByDay)
                            {
                                exitTime = rowExitTime;
                                EmployeeWorkTimeInervals.Add(new TimeInterval(
                                    currentDay.AddMinutes((int) t.TotalMinutes), exitTime, enterCode, false, false,
                                    enterPlace));
                                exitTimeEmpty = false;
                                IsEnterPrevDayByDay = false;
                                isExitNextDay = false;
                                continue;
                            }

                            HasErrors = true;
                            // исключительная ситуация: если не был зафиксирован вход (идут 2 выхода подряд или 1ая позиция - выход)
                            if (exitTimeEmpty)
                            {
                                // 1ая позиция - выход
                                if (_employeePrimary)
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(
                                        currentDay.AddMinutes((int) t.TotalMinutes), rowExitTime, -1, true, false));
                                else
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true,
                                        false));
                            }
                            else
                            {
                                // идут 2 выхода подряд
                                if (_employeePrimary)
                                {
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(exitTime, rowExitTime, -1, true,
                                        false));
                                    EmployeeAbsentTimeInervals.Add(
                                        new TimeInterval(exitTime, exitTime, -1, false, true));
                                }
                                else
                                {
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true,
                                        false));
                                    EmployeeAbsentTimeInervals.Add(new TimeInterval(exitTime, rowExitTime, -1, false,
                                        true));
                                }
                            }

                            exitTimeEmpty = false;
                            exitTime = rowExitTime;
                            continue;
                        }

                        enterCode = (int) _sourceEvents[i].code;
                        enterTime = rowExitTime;
                        enterPlace = _sourceEvents[i].place;
                        expectingEntrance = false;

                        if (!exitTimeEmpty) EmployeeAbsentTimeInervals.Add(new TimeInterval(exitTime, enterTime, -1));
                    }
                    else
                    {
                        if (!fEnterCodeIsNull)
                        {
                            // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                            HasErrors = true;

                            if (_employeePrimary)
                            {
                                EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, rowExitTime, enterCode, false,
                                    true, enterPlace));
                                EmployeeAbsentTimeInervals.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true,
                                    false));
                            }
                            else
                            {
                                EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, enterTime, enterCode, false,
                                    true, enterPlace));
                                EmployeeAbsentTimeInervals.Add(
                                    new TimeInterval(enterTime, rowExitTime, -1, true, false));
                            }

                            enterTime = rowExitTime;
                            enterCode = (int) _sourceEvents[i].code;
                            enterPlace = _sourceEvents[i].place;

                            continue;
                        }

                        EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, rowExitTime, enterCode, enterPlace));
                        expectingEntrance = true;
                        exitTimeEmpty = false;
                        exitTime = rowExitTime;
                    }
                }

                if (!expectingEntrance)
                {
                    if (enterTime.Date == DateTime.Today)
                    {
                        EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, DateTime.Now, enterCode, false, false,
                            enterPlace));
                    }
                    else
                    {
                        if (!isExitNextDay)
                        {
                            // исключительная ситуация: за день не был зафиксирован окончательный выход из офиса
                            HasErrors = true;
                            if (_employeePrimary)
                                EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime,
                                    nextDay.AddMinutes((int) t.TotalMinutes), enterCode, false, true, enterPlace));
                            else
                                EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, enterTime, enterCode, false,
                                    true, enterPlace));
                        }
                        else
                        {
                            EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime,
                                nextDay.AddMinutes((int) t.TotalMinutes), enterCode, false, false, enterPlace));
                        }

                        if (isEnterTomorow) HasErrors = true;
                    }
                }
            }
        }

        private void CompletePeriodsByDay()
        {
            //if (_sourceRowsByDay == null) return;
            if (_sourceEventsByDay == null) return;
            var ts = _endTime - _startTime;
            var totalDays = ts.TotalDays; // число дней в диапазоне дат
            var t = _startTime - _startTime;
            HasErrors = false;

            if (_sourceEventsByDay.Length > 0)
            {
                EmployeeWorkTimeInervals.Clear();
                EmployeeAbsentTimeInervals.Clear();

                for (double dayNum = 0; dayNum < totalDays; dayNum++)
                {
                    // нарезка диапазона по суткам
                    var expectingEntrance = true;
                    var enterTime = DateTime.MinValue;

                    var exitTimeEmpty = true;
                    var exitTime = DateTime.MinValue;

                    var enterCode = -1;
                    var enterPlace = string.Empty;

                    var isExitNextDay = false;
                    var isEnterTomorow = false;
                    var firstEnterTomorow = -1;

                    var nextDay = _startTime.AddDays(dayNum + 1);
                    var currentDay = _startTime.AddDays(dayNum);

                    for (var i = 0; i < _sourceEventsByDay.Length; i++)
                    {
                        var rowExitTime = _sourceEventsByDay[i].when;
                        var fEnterCodeIsNull = !_sourceEventsByDay[i].code.HasValue;
                        var fGreaterThanNext = rowExitTime >= nextDay;
                        var fLessThanCurrent = rowExitTime < currentDay;

                        if (fLessThanCurrent || fGreaterThanNext
                        ) /* если запись не попадает в диапазон обрабатываемого дня */
                        {
                            if (fGreaterThanNext && fEnterCodeIsNull)
                            {
                                isExitNextDay = true;
                                if (!isEnterTomorow) firstEnterTomorow = i;
                            }
                            else if (fGreaterThanNext && !fEnterCodeIsNull)
                            {
                                if (firstEnterTomorow == -1)
                                {
                                    isEnterTomorow = true;
                                    firstEnterTomorow = i;
                                }
                            }
                            else if (fLessThanCurrent && !fEnterCodeIsNull)
                            {
                                IsEnterPrevDayByDay = true;
                                enterCode = (int) _sourceEventsByDay[i].code;
                                enterPlace = _sourceEventsByDay[i].place;
                            }
                            else if (fLessThanCurrent && fEnterCodeIsNull)
                            {
                                IsEnterPrevDayByDay = false;
                            }

                            continue;
                        }

                        if (expectingEntrance)
                        {
                            if (fEnterCodeIsNull)
                            {
                                if (IsEnterPrevDayByDay)
                                {
                                    exitTime = _sourceEventsByDay[i].when;
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(
                                        currentDay.AddMinutes((int) t.TotalMinutes), exitTime, enterCode, false, false,
                                        enterPlace));
                                    exitTimeEmpty = false;
                                    IsEnterPrevDayByDay = false;
                                    isExitNextDay = false;
                                    continue;
                                }

                                HasErrors = true;
                                // исключительная ситуация: если не был зафиксирован вход (идут 2 выхода подряд или 1ая позиция - выход)
                                if (exitTimeEmpty)
                                {
                                    // 1ая позиция - выход
                                    if (_employeePrimary)
                                        EmployeeWorkTimeInervals.Add(new TimeInterval(
                                            currentDay.AddMinutes((int) t.TotalMinutes), _sourceEventsByDay[i].when, -1,
                                            true, false));
                                    else
                                        EmployeeWorkTimeInervals.Add(new TimeInterval(_sourceEventsByDay[i].when,
                                            _sourceEventsByDay[i].when, -1, true, false));
                                }
                                else
                                {
                                    // идут 2 выхода подряд
                                    if (_employeePrimary)
                                    {
                                        EmployeeWorkTimeInervals.Add(new TimeInterval(exitTime,
                                            _sourceEventsByDay[i].when, -1, true, false));
                                        EmployeeAbsentTimeInervals.Add(new TimeInterval(exitTime, exitTime, -1, false,
                                            true));
                                    }
                                    else
                                    {
                                        EmployeeWorkTimeInervals.Add(new TimeInterval(_sourceEventsByDay[i].when,
                                            _sourceEventsByDay[i].when, -1, true, false));
                                        EmployeeAbsentTimeInervals.Add(new TimeInterval(exitTime,
                                            _sourceEventsByDay[i].when, -1, false, true));
                                    }
                                }

                                exitTimeEmpty = false;
                                exitTime = _sourceEventsByDay[i].when;
                                continue;
                            }

                            enterCode = (int) _sourceEventsByDay[i].code;
                            enterTime = _sourceEventsByDay[i].when;
                            enterPlace = _sourceEventsByDay[i].place;
                            expectingEntrance = false;

                            if (!exitTimeEmpty)
                                EmployeeAbsentTimeInervals.Add(new TimeInterval(exitTime, enterTime, -1));
                        }
                        else
                        {
                            if (!fEnterCodeIsNull)
                            {
                                // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                                HasErrors = true;

                                if (_employeePrimary)
                                {
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, _sourceEventsByDay[i].when,
                                        enterCode, false, true, enterPlace));
                                    EmployeeAbsentTimeInervals.Add(new TimeInterval(_sourceEventsByDay[i].when,
                                        _sourceEventsByDay[i].when, -1, true, false));
                                }
                                else
                                {
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, enterTime, enterCode,
                                        false, true, enterPlace));
                                    EmployeeAbsentTimeInervals.Add(new TimeInterval(enterTime,
                                        _sourceEventsByDay[i].when, -1, true, false));
                                }

                                enterTime = _sourceEventsByDay[i].when;
                                enterCode = (int) _sourceEventsByDay[i].code;
                                enterPlace = _sourceEventsByDay[i].place;

                                continue;
                            }

                            EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, _sourceEventsByDay[i].when,
                                enterCode, enterPlace));
                            expectingEntrance = true;
                            exitTimeEmpty = false;
                            exitTime = _sourceEventsByDay[i].when;
                        }
                    }

                    if (!expectingEntrance)
                    {
                        if (enterTime.Date == DateTime.Today)
                        {
                            EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, DateTime.Now, enterCode, false,
                                false, enterPlace));
                        }
                        else
                        {
                            if (!isExitNextDay)
                            {
                                // исключительная ситуация: за день не был зафиксирован окончательный выход из офиса
                                HasErrors = true;
                                if (_employeePrimary)
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime,
                                        nextDay.AddMinutes((int) t.TotalMinutes), enterCode, false, true, enterPlace));
                                else
                                    EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime, enterTime, enterCode,
                                        false, true, enterPlace));
                            }
                            else
                            {
                                //workPeriods.Add(new TimeInterval(enterTime, startTime.AddDays(dayNum + 1).AddSeconds(-1).AddMinutes((int)t.TotalMinutes), enterCode, false, false, enterPlace));
                                EmployeeWorkTimeInervals.Add(new TimeInterval(enterTime,
                                    nextDay.AddMinutes((int) t.TotalMinutes), enterCode, false, false, enterPlace));
                            }

                            if (isEnterTomorow) HasErrors = true;
                        }
                    }
                }
            }
            else
            {
                IsEnterPrevDayByDay = false;
            }
        }

        private struct EnterEvent
        {
            public DateTime when;
            public int? code;
            public string place;
        }
    }

    /// <summary>
    ///     Класс выхода сотрудника в Интернет
    /// </summary>
    public class EmployeeInternetAccessInfo
    {
        /// <summary>
        ///     Суммарное время доступа в интернет за период в секундах
        /// </summary>
        public EmployeeInternetAccessInfo(DateTime startPeriod, DateTime endPeriod, int emplCode)
        {
        }

        /// <summary>
        ///     Количество
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Сумма
        /// </summary>
        public int Total { get; }

        /// <summary>
        ///     Источник данных
        /// </summary>
        public DataRow[] SourceRows { get; }

        private void ComlpeteTableFromDB(DateTime startPeriod, DateTime endPeriod, int emplCode)
        {
        }
    }
}