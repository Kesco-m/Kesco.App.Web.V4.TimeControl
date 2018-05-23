using System;
using System.Data;
using Kesco.App.Web.TimeControl.DataSets;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.DALC;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    /// Класс для работы с периодами сотрудников
    /// </summary>
    public class EmployeeTimePeriodsInfo
    {
        private struct EnterEvent
        {
            public DateTime when;
            public int? code;
            public string place;
        }

        /// <summary>
        /// Признак прохода в предыдущий день
        /// </summary>
        public bool IsEnterPrevDayByDay;
        private readonly bool _employeePrimary;
        private readonly bool _resolvePlaces;
        private DateTime _startTime;
        private DateTime _endTime;
        //private DataRow[] _sourceRows;
        //private DataRow[] _sourceRowsByDay;
        private EnterEvent[] _sourceEvents;
        private EnterEvent[] _sourceEventsByDay; 
        
        private bool _hasErrors;
        private readonly EmployeeTimeInervals _workPeriods = new EmployeeTimeInervals();
        private readonly EmployeeTimeInervals _absentPeriods = new EmployeeTimeInervals();

        /// <summary>
        /// Признак наличия ошибок
        /// </summary>
        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        /// <summary>
        /// Интервалы нахождения на рабочем месте
        /// </summary>
        public EmployeeTimeInervals EmployeeWorkTimeInervals
        {
            get { return _workPeriods; }
        }

        /// <summary>
        /// Интервалы отсутствия на рабочем месте
        /// </summary>
        public EmployeeTimeInervals EmployeeAbsentTimeInervals
        {
            get { return _absentPeriods; }
        }

        /// <summary>
        /// Время вхождения в офис
        /// </summary>
        public object EntranceTime
        {
            get
            {
                object res = null;
                DateTime temp;
                if (_startTime == _endTime)
                {
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                }
                else
                {
                    temp = _endTime.AddSeconds(-1);
                }

                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (EnterEvent e in _sourceEvents)
                    {
                        if (e.when.Day == _startTime.Day)
                        {
                            if (e.code.HasValue)
                            {
                                return e.when;
                            }
                            return null;
                        }
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
                    {
                        res = _sourceEvents[0].when;
                    }
                    else if (IsEnterPrevDayByDay && _sourceEvents.Length > 1)
                    {
                        if (_sourceEvents[1].code.HasValue)
                        {
                            res = _sourceEvents[1].when;
                        }
                    }
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
        /// Время выхода из офиса
        /// </summary>
        public object ExitTime
        {
            get
            {
                object res = null;
                DateTime temp;
                if (_startTime == _endTime)
                {
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                }
                else
                {
                    temp = _endTime.AddSeconds(-1);
                }

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

                    for (int i = _sourceEvents.Length - 1; i > -1; i--)
                    {
                        if (_sourceEvents[i].when.Day == _startTime.Day)
                        {
                            if (!_sourceEvents[i].code.HasValue)
                            {
                                return _sourceEvents[i].when;
                            }

                            return null;
                        }
                    }

                    return null;
                }

                if (_sourceEvents != null && _sourceEvents.Length > 0)
                {
                    if (!_sourceEvents[_sourceEvents.Length - 1].code.HasValue)
                    {
                        res = _sourceEvents[_sourceEvents.Length - 1].when;
                    }
                    else if (IsEnterPrevDayByDay)
                    {
                        if (!_sourceEvents[0].code.HasValue)
                        {
                            res = _sourceEvents[0].when;
                        }
                    }
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
        /// Время выхода из офиса в предыдущий день
        /// </summary>
        public object ExitTimePrevDay
        {
            get
            {
                DateTime temp;
                if (_startTime == _endTime)
                {
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                }
                else
                {
                    temp = _endTime.AddSeconds(-1);
                }
                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (TimeInterval interval in _absentPeriods)
                    {
                        if (interval.PeriodDuration.Hours > 7)
                            return interval.StartTime;
                    }

                    for (int i = 0; i < _sourceEvents.Length; i++)
                    {
                        if (_sourceEvents[i].when.Day == _startTime.Day)
                        {
                            if (!_sourceEvents[i].code.HasValue)
                            {
                                return _sourceEvents[i].when;
                            }
                            return null;
                        }
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
        /// Время входа в офис в следующий день
        /// </summary>
        public object EnterTimeNextDay
        {
            get
            {
                DateTime temp;
                if (_startTime == _endTime)
                {
                    temp = _endTime.AddDays(1).AddSeconds(-1);
                }
                else
                {
                    temp = _endTime.AddSeconds(-1);
                }
                if (_startTime.Day == temp.Day && _startTime.Month == temp.Month && _startTime.Year == temp.Year)
                {
                    foreach (TimeInterval interval in _absentPeriods)
                    {
                        if (interval.PeriodDuration.Hours > 7)
                            return interval.EndTime;
                    }

                    for (int i = _sourceEvents.Length - 1; i > -1; i--)
                    {
                        if (_sourceEvents[i].when.Day == _startTime.Day && _startTime.Day != DateTime.Now.Day)
                        {
                            if (_sourceEvents[i].code.HasValue)
                            {
                                return _sourceEvents[i].when;
                            }
                            return null;
                        }
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
        /// Общее время работы
        /// </summary>
        public TimeSpan TotalWorkTime
        {
            get
            {
                var res = new TimeSpan(0, 0, 0);
                foreach (TimeInterval interval in _workPeriods)
                {
                    res += interval.PeriodDuration;
                }
                return res;
            }
        }

        /// <summary>
        /// Общее время перерывов
        /// </summary>
        public TimeSpan TotalAbsentTime
        {
            get
            {
                var res = new TimeSpan(0, 0, 0);
                foreach (TimeInterval interval in _absentPeriods)
                {
                    if (interval.PeriodDuration.Hours < 8)
                        res += interval.PeriodDuration;
                }
                return res;
            }
        }

        /// <summary>
        /// Получение кода прохода сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="emplCode">Код сотрудника</param>
        /// <param name="tz">Таймзона</param>
        /// <returns>Код прохода сотрудника</returns>
        public static Int32 GetMaxId(DateTime startPeriod, DateTime endPeriod, int emplCode, int tz)
        {
            string sql = String.Format(@"SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) 
AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) AND (КодСотрудника = {2}) ORDER BY КодПроходаСотрудника DESC", startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    (endPeriod.Date.AddDays(1)).ToString("yyyy-MM-dd HH:mm:ss"), emplCode, tz);
            DataTable resultTable = DBManager.GetData(sql, Global.ConnectionString);

            Int32 result = -1;

            if (resultTable.Rows.Count > 0)
            {
                result = (Int32)resultTable.Rows[0]["КодПроходаСотрудника"];
            }
            return result;
        }

        /// <summary>
        /// Получение кода прохода сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="tz">Таймзона</param>
        /// <returns>Код прохода сотрудника</returns>
        public static Int32 GetMaxId(DateTime startPeriod, DateTime endPeriod, int tz)
        {
            string sql = String.Format(@"SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (Dateadd(MINUTE,{2},[Когда]) >= convert(datetime, '{0}', 120)) AND 
(Dateadd(MINUTE,{2},[Когда]) <= convert(datetime, '{1}', 120)) ORDER BY КодПроходаСотрудника DESC",
            startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"), (endPeriod.Date.AddDays(1)).ToString("yyyy-MM-dd HH:mm:ss"), tz);
            DataTable resultTable = DBManager.GetData(sql, Global.ConnectionString);

            Int32 result = -1;

            if (resultTable.Rows.Count > 0)
            {
                result = (Int32)resultTable.Rows[0]["КодПроходаСотрудника"];
            }
            return result;
        }

        /// <summary>
        /// Получение кода прохода сотрудника
        /// </summary>
        /// <returns>Код прохода сотрудника</returns>
        public static Int32 GetMaxId()
        {
            string sql = "SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) ORDER BY КодПроходаСотрудника DESC";
            DataTable resultTable = DBManager.GetData(sql, Global.ConnectionString);
            Int32 result = -1;

            if (resultTable.Rows.Count > 0)
            {
                result = (Int32)resultTable.Rows[0]["КодПроходаСотрудника"];
            }

            return result;
        }

        /// <summary>
        /// Получение кода прохода сотрудника
        /// </summary>
        /// <param name="emplCode">Код сотрудника</param>
        /// <returns>Код прохода сотрудника</returns>
        public static Int32 GetMaxId(int emplCode)
        {
            string sql = String.Format("SELECT TOP 1 КодПроходаСотрудника FROM vwПроходыСотрудников (nolock) WHERE (КодСотрудника = {0}) ORDER BY КодПроходаСотрудника DESC", emplCode);
            DataTable resultTable = DBManager.GetData(sql, Global.ConnectionString);
            Int32 result = -1;

            if (resultTable.Rows.Count > 0)
            {
                result = (Int32)resultTable.Rows[0]["КодПроходаСотрудника"];
            }

            return result;
        }

        /// <summary>
        /// Метод получения проходов сотрудника
        /// </summary>
        /// <param name="startPeriod">Начало периода</param>
        /// <param name="endPeriod">окончание периода</param>
        /// <param name="emplCode">Код сотрудника</param>
        /// <param name="pEmployeePrimary"></param>
        /// <param name="sourceTable"></param>
        /// <param name="isByDay"></param>
        /// <param name="isEnterPrevDayByDay"></param>
        public EmployeeTimePeriodsInfo(DateTime startPeriod, DateTime endPeriod, int emplCode, bool pEmployeePrimary, EmployeePeriodsInfoDs.ПроходыСотрудниковDataTable sourceTable,
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
        /// Метод получения проходов сотрудника
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
            ComlpeteEventsTableFromDB(startPeriod.AddDays(-1), startPeriod.AddDays(1), emplCode, _resolvePlaces, tz, true);
            CompletePeriodsByDay();
        }

        private void ComlpeteEventsTableFromDB(DateTime startPeriod, DateTime endPeriod, int emplCode, bool rp, int tz, bool isByDay = false)
        {
            DataTable tEvents;
            string sql;
            if (rp)
            {
                sql = String.Format(@"SELECT Dateadd(MINUTE,{3},[Когда]) as Когда, КодРасположения, CASE WHEN КодРасположения IS NULL THEN '' ELSE Считыватель END Расположение FROM vwПроходыСотрудников (nolock) 
WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) AND (КодСотрудника = {2}) ORDER BY Когда",
                        startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                        (endPeriod.Date.AddDays(1)).ToString("yyyy-MM-dd HH:mm:ss"),
                        emplCode, tz);
                tEvents = DBManager.GetData(sql, Global.ConnectionString);
            }
            else
            {
                sql = String.Format(@"SELECT Dateadd(MINUTE,{3},[Когда]) as Когда, КодРасположения FROM vwПроходыСотрудников (nolock) 
WHERE (Dateadd(MINUTE,{3},[Когда]) >= convert(datetime, '{0}', 120)) AND (Dateadd(MINUTE,{3},[Когда]) <= convert(datetime, '{1}', 120)) 
AND (КодСотрудника = {2}) ORDER BY Когда",
                        startPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                        (endPeriod.Date.AddDays(1)).ToString("yyyy-MM-dd HH:mm:ss"),
                        emplCode, tz);
                tEvents = DBManager.GetData(sql, Global.ConnectionString);
            }

            EnterEvent[] sourceEvents = new EnterEvent[tEvents.Rows.Count];

            for (int i = 0; i < tEvents.Rows.Count; i++)
            {
                object objExitTime = tEvents.Rows[i]["Когда"];
                if (DBNull.Value == objExitTime) continue;

                DateTime rowWhen = (DateTime)tEvents.Rows[i]["Когда"];

                sourceEvents[i].when = rowWhen;
                sourceEvents[i].code = DBNull.Value == tEvents.Rows[i]["КодРасположения"] ? null : (int?)tEvents.Rows[i]["КодРасположения"];
                sourceEvents[i].place = _resolvePlaces ? tEvents.Rows[i]["Расположение"].ToString() : String.Empty;
            }

            if (isByDay)
            {
                String filterString = String.Format("Когда >= '{0}' AND Когда <= '{1}'", startPeriod.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), endPeriod.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                DataRow[] source_rows = tEvents.Select(filterString, "Когда");

                _sourceEvents = new EnterEvent[sourceEvents.Length];

                for (int i = 0; i < source_rows.Length; i++)
                {
                    object objExitTime = source_rows[i]["Когда"];
                    if (DBNull.Value == objExitTime) continue;

                    DateTime rowWhen = (DateTime)source_rows[i]["Когда"];

                    _sourceEvents[i].when = rowWhen;
                    _sourceEvents[i].code = DBNull.Value == source_rows[i]["КодРасположения"] ? null : (int?)source_rows[i]["КодРасположения"];
                    _sourceEvents[i].place = _resolvePlaces ? source_rows[i]["Расположение"].ToString() : String.Empty;
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

        private void ComlpeteEventsTableFromDataTable(DateTime startPeriod, DateTime endPeriod, int emplCode, EmployeePeriodsInfoDs.ПроходыСотрудниковDataTable sourceTable, bool isByDay = false)
        {
            String filterString = String.Format("Когда >= '{0}' AND Когда <= '{1}' AND КодСотрудника = {2}", startPeriod.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                                                endPeriod.Date.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"), emplCode);
            //_sourceRows = sourceTable.Select(filterString, "Когда");

            DataRow[] source_rows = sourceTable.Select(filterString, "Когда");

            _sourceEvents = new EnterEvent[source_rows.Length];
            for (int i = 0; i < source_rows.Length; i++)
            {
                _sourceEvents[i].when = (DateTime)source_rows[i]["Когда"];
                _sourceEvents[i].code = DBNull.Value == source_rows[i]["КодРасположения"] ? null : (int?)source_rows[i]["КодРасположения"];
                _sourceEvents[i].place = _resolvePlaces ? source_rows[i]["Расположение"].ToString() : String.Empty;
            }


            if (isByDay)
            {
                filterString = String.Format("Когда >= '{0}' AND Когда <= '{1}' AND КодСотрудника = {2}", startPeriod.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                                                endPeriod.Date.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"), emplCode);
                //_sourceRowsByDay = sourceTable.Select(filterString, "Когда");

                source_rows = sourceTable.Select(filterString, "Когда");

                _sourceEventsByDay = new EnterEvent[source_rows.Length];
                for (int i = 0; i < source_rows.Length; i++)
                {
                    _sourceEventsByDay[i].when = (DateTime)source_rows[i]["Когда"];
                    _sourceEventsByDay[i].code = DBNull.Value == source_rows[i]["КодРасположения"] ? null : (int?)source_rows[i]["КодРасположения"];
                    _sourceEventsByDay[i].place = _resolvePlaces ? source_rows[i]["Расположение"].ToString() : String.Empty;
                }
            }
        }

        private void CompletePeriods()
        {
            TimeSpan ts = _endTime.AddDays(1) - _startTime;
            double totalDays = ts.TotalDays; // число дней в диапазоне дат
            TimeSpan t = _startTime - _startTime;
            _hasErrors = false;

            //if (_sourceRows.Length > 0)
            if (_sourceEvents.Length < 1) return;

            _workPeriods.Clear();
            _absentPeriods.Clear();

            for (double dayNum = 0; dayNum < totalDays; dayNum++)
            { // нарезка диапазона по суткам
                bool expectingEntrance = true;
                DateTime enterTime = DateTime.MinValue;
                bool exitTimeEmpty = true;
                DateTime exitTime = DateTime.MinValue;
                int enterCode = -1;
                string enterPlace = String.Empty;
                bool isExitNextDay = false;
                bool isEnterTomorow = false;
                int firstEnterTomorow = -1;

                DateTime nextDay = _startTime.AddDays(dayNum + 1);
                DateTime currentDay = _startTime.AddDays(dayNum);

                for (int i = 0; i < _sourceEvents.Length; i++)
                {
                    if (_sourceEvents[i].when > nextDay.AddDays(1)) break;

                    DateTime rowExitTime = _sourceEvents[i].when;
                    
                    bool fEnterCodeIsNull = !_sourceEvents[i].code.HasValue;
                    bool fGreaterThanNext = rowExitTime >= nextDay;
                    bool fLessThanCurrent = rowExitTime < currentDay;
                                        

                    if (fLessThanCurrent || fGreaterThanNext) /* если запись не попадает в диапазон обрабатываемого дня */
                    {
                        if (fGreaterThanNext && fEnterCodeIsNull)
                        {
                            isExitNextDay = true;
                            if (!isEnterTomorow)
                            {
                                firstEnterTomorow = i;
                            }
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
                            enterCode = (int)_sourceEvents[i].code;
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
                                _workPeriods.Add(new TimeInterval(currentDay.AddMinutes((int)t.TotalMinutes), exitTime, enterCode, false, false, enterPlace));
                                exitTimeEmpty = false;
                                IsEnterPrevDayByDay = false;
                                isExitNextDay = false;
                                continue;
                            }
                            _hasErrors = true;
                            // исключительная ситуация: если не был зафиксирован вход (идут 2 выхода подряд или 1ая позиция - выход)
                            if (exitTimeEmpty)
                            {
                                // 1ая позиция - выход
                                if (_employeePrimary)
                                {
                                    _workPeriods.Add(new TimeInterval(currentDay.AddMinutes((int)t.TotalMinutes), rowExitTime, -1, true, false));
                                }
                                else
                                {
                                    _workPeriods.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true, false));
                                }
                            }
                            else
                            {
                                // идут 2 выхода подряд
                                if (_employeePrimary)
                                {
                                    _workPeriods.Add(new TimeInterval(exitTime, rowExitTime, -1, true, false));
                                    _absentPeriods.Add(new TimeInterval(exitTime, exitTime, -1, false, true));
                                }
                                else
                                {
                                    _workPeriods.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true, false));
                                    _absentPeriods.Add(new TimeInterval(exitTime, rowExitTime, -1, false, true));
                                }
                            }

                            exitTimeEmpty = false;
                            exitTime = rowExitTime;
                            continue;
                        }

                        enterCode = (int)_sourceEvents[i].code;
                        enterTime = rowExitTime;
                        enterPlace = _sourceEvents[i].place;
                        expectingEntrance = false;

                        if (!exitTimeEmpty)
                        {
                            _absentPeriods.Add(new TimeInterval(exitTime, enterTime, -1));
                        }
                    }
                    else
                    {
                        if (!fEnterCodeIsNull)
                        {
                            // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                            _hasErrors = true;

                            if (_employeePrimary)
                            {
                                _workPeriods.Add(new TimeInterval(enterTime, rowExitTime, enterCode, false, true, enterPlace));
                                _absentPeriods.Add(new TimeInterval(rowExitTime, rowExitTime, -1, true, false));
                            }
                            else
                            {
                                _workPeriods.Add(new TimeInterval(enterTime, enterTime, enterCode, false, true, enterPlace));
                                _absentPeriods.Add(new TimeInterval(enterTime, rowExitTime, -1, true, false));
                            }

                            enterTime = rowExitTime;
                            enterCode = (int)_sourceEvents[i].code;
                            enterPlace = _sourceEvents[i].place;

                            continue;
                        }
                        _workPeriods.Add(new TimeInterval(enterTime, rowExitTime, enterCode, enterPlace));
                        expectingEntrance = true;
                        exitTimeEmpty = false;
                        exitTime = rowExitTime;
                    }
                }

                if (!expectingEntrance)
                {
                    if (enterTime.Date == DateTime.Today)
                    {
                        _workPeriods.Add(new TimeInterval(enterTime, DateTime.Now, enterCode, false, false, enterPlace));
                    }
                    else
                    {
                        if (!isExitNextDay)
                        {
                            // исключительная ситуация: за день не был зафиксирован окончательный выход из офиса
                            _hasErrors = true;
                            if (_employeePrimary)
                            {
                                _workPeriods.Add(new TimeInterval(enterTime, nextDay.AddMinutes((int)t.TotalMinutes), enterCode, false, true, enterPlace));
                            }
                            else
                            {
                                _workPeriods.Add(new TimeInterval(enterTime, enterTime, enterCode, false, true, enterPlace));
                            }
                        }
                        else
                        {
                            _workPeriods.Add(new TimeInterval(enterTime, nextDay.AddMinutes((int)t.TotalMinutes), enterCode, false, false, enterPlace));
                        }

                        if (isEnterTomorow)
                        {
                            // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                            _hasErrors = true;
                        }
                    }
                }
            }
        }

        private void CompletePeriodsByDay()
        {
            //if (_sourceRowsByDay == null) return;
            if (_sourceEventsByDay == null) return;
            TimeSpan ts = _endTime - _startTime;
            double totalDays = ts.TotalDays; // число дней в диапазоне дат
            TimeSpan t = _startTime - _startTime;
            _hasErrors = false;

            if (_sourceEventsByDay.Length > 0)
            {
                _workPeriods.Clear();
                _absentPeriods.Clear();

                for (double dayNum = 0; dayNum < totalDays; dayNum++)
                { // нарезка диапазона по суткам
                    bool expectingEntrance = true;
                    DateTime enterTime = DateTime.MinValue;

                    bool exitTimeEmpty = true;
                    DateTime exitTime = DateTime.MinValue;

                    int enterCode = -1;
                    string enterPlace = String.Empty;

                    bool isExitNextDay = false;
                    bool isEnterTomorow = false;
                    int firstEnterTomorow = -1;

                    DateTime nextDay = _startTime.AddDays(dayNum + 1);
                    DateTime currentDay = _startTime.AddDays(dayNum);

                    for (int i = 0; i < _sourceEventsByDay.Length; i++)
                    {
                        DateTime rowExitTime = _sourceEventsByDay[i].when;
                        bool fEnterCodeIsNull = !_sourceEventsByDay[i].code.HasValue;
                        bool fGreaterThanNext = rowExitTime >= nextDay;
                        bool fLessThanCurrent = rowExitTime < currentDay;

                        if (fLessThanCurrent || fGreaterThanNext) /* если запись не попадает в диапазон обрабатываемого дня */
                        {
                            if (fGreaterThanNext && fEnterCodeIsNull)
                            {
                                isExitNextDay = true;
                                if (!isEnterTomorow)
                                {
                                    firstEnterTomorow = i;
                                }
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
                                enterCode = (int)_sourceEventsByDay[i].code;
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
                                    _workPeriods.Add(new TimeInterval(currentDay.AddMinutes((int)t.TotalMinutes), exitTime, enterCode, false, false, enterPlace));
                                    exitTimeEmpty = false;
                                    IsEnterPrevDayByDay = false;
                                    isExitNextDay = false;
                                    continue;
                                }
                                _hasErrors = true;
                                // исключительная ситуация: если не был зафиксирован вход (идут 2 выхода подряд или 1ая позиция - выход)
                                if (exitTimeEmpty)
                                {
                                    // 1ая позиция - выход
                                    if (_employeePrimary)
                                    {
                                        _workPeriods.Add(new TimeInterval(currentDay.AddMinutes((int)t.TotalMinutes), _sourceEventsByDay[i].when, -1, true, false));
                                    }
                                    else
                                    {
                                        _workPeriods.Add(new TimeInterval(_sourceEventsByDay[i].when, _sourceEventsByDay[i].when, -1, true, false));
                                    }
                                }
                                else
                                {
                                    // идут 2 выхода подряд
                                    if (_employeePrimary)
                                    {
                                        _workPeriods.Add(new TimeInterval(exitTime, _sourceEventsByDay[i].when, -1, true, false));
                                        _absentPeriods.Add(new TimeInterval(exitTime, exitTime, -1, false, true));
                                    }
                                    else
                                    {
                                        _workPeriods.Add(new TimeInterval(_sourceEventsByDay[i].when, _sourceEventsByDay[i].when, -1, true, false));
                                        _absentPeriods.Add(new TimeInterval(exitTime, _sourceEventsByDay[i].when, -1, false, true));
                                    }
                                }
                                exitTimeEmpty = false;
                                exitTime = _sourceEventsByDay[i].when;
                                continue;
                            }
                            enterCode = (int)_sourceEventsByDay[i].code;
                            enterTime = _sourceEventsByDay[i].when;
                            enterPlace = _sourceEventsByDay[i].place;
                            expectingEntrance = false;

                            if (!exitTimeEmpty)
                            {
                                _absentPeriods.Add(new TimeInterval(exitTime, enterTime, -1));
                            }
                        }
                        else
                        {
                            if (!fEnterCodeIsNull)
                            {
                                // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                                _hasErrors = true;

                                if (_employeePrimary)
                                {
                                    _workPeriods.Add(new TimeInterval(enterTime, _sourceEventsByDay[i].when, enterCode, false, true, enterPlace));
                                    _absentPeriods.Add(new TimeInterval(_sourceEventsByDay[i].when, _sourceEventsByDay[i].when, -1, true, false));
                                }
                                else
                                {
                                    _workPeriods.Add(new TimeInterval(enterTime, enterTime, enterCode, false, true, enterPlace));
                                    _absentPeriods.Add(new TimeInterval(enterTime, _sourceEventsByDay[i].when, -1, true, false));
                                }

                                enterTime = _sourceEventsByDay[i].when;
                                enterCode = (int)_sourceEventsByDay[i].code;
                                enterPlace = _sourceEventsByDay[i].place;

                                continue;
                            }
                            _workPeriods.Add(new TimeInterval(enterTime, _sourceEventsByDay[i].when, enterCode, enterPlace));
                            expectingEntrance = true;
                            exitTimeEmpty = false;
                            exitTime = _sourceEventsByDay[i].when;
                        }
                    }

                    if (!expectingEntrance)
                    {
                        if (enterTime.Date == DateTime.Today)
                        {
                            _workPeriods.Add(new TimeInterval(enterTime, DateTime.Now, enterCode, false, false, enterPlace));
                        }
                        else
                        {
                            if (!isExitNextDay)
                            {
                                // исключительная ситуация: за день не был зафиксирован окончательный выход из офиса
                                _hasErrors = true;
                                if (_employeePrimary)
                                {
                                    //workPeriods.Add(new TimeInterval(enterTime, startTime.AddDays(dayNum + 1).AddSeconds(-1).AddMinutes((int)t.TotalMinutes), enterCode, false, true, enterPlace));
                                    _workPeriods.Add(new TimeInterval(enterTime, nextDay.AddMinutes((int)t.TotalMinutes), enterCode, false, true, enterPlace));
                                }
                                else
                                {
                                    _workPeriods.Add(new TimeInterval(enterTime, enterTime, enterCode, false, true, enterPlace));
                                }
                            }
                            else
                            {
                                //workPeriods.Add(new TimeInterval(enterTime, startTime.AddDays(dayNum + 1).AddSeconds(-1).AddMinutes((int)t.TotalMinutes), enterCode, false, false, enterPlace));
                                _workPeriods.Add(new TimeInterval(enterTime, nextDay.AddMinutes((int)t.TotalMinutes), enterCode, false, false, enterPlace));
                            }
                            if (isEnterTomorow)
                            {
                                // исключительная ситуация: если не был зафиксирован выход (идут 2 входа подряд)
                                _hasErrors = true;
                            }
                        }
                    }
                }
            }
            else
            {
                IsEnterPrevDayByDay = false;
            }
        }
    }

    /// <summary>
    /// Класс выхода сотрудника в Интернет
    /// </summary>
    public class EmployeeInternetAccessInfo
    {
        private readonly DataRow[] _sourceRows;
        private readonly int _count;
        private readonly int _total;

        /// <summary>
        /// Суммарное время доступа в интернет за период в секундах
        /// </summary>
        public EmployeeInternetAccessInfo(DateTime startPeriod, DateTime endPeriod, int emplCode) { }

        private void ComlpeteTableFromDB(DateTime startPeriod, DateTime endPeriod, int emplCode) { }

        /// <summary>
        /// Количество
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Сумма
        /// </summary>
        public int Total
        {
            get { return _total; }
        }

        /// <summary>
        /// Источник данных
        /// </summary>
        public DataRow[] SourceRows
        {
            get { return _sourceRows; }
        }
    }
}