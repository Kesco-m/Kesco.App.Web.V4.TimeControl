using System;
using System.Data;
using Kesco.App.Web.TimeControl.DataSets;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    ///     Класс для работы со списком сотрудников
    /// </summary>
    public class EmplTableCash
    {
        private readonly BasePage _sourcePage;
        private int _currentPage;
        private DateTime _endDate;
        private bool _filterActiveOnly;
        private string _filterCompany;
        private string _filterEmployee;
        private string _filterPost;
        private string _filterSubdiv;
        private string _isReversCompany;
        private string _isReversPerson;
        private string _isReversPosition;
        private string _isReversSubdivision;
        private int _maxIndex;
        private bool _needCheckId;
        private int _rowsPerPage;
        private readonly DataTable _sourceTable;
        private DateTime _startDate;
        private bool _subEmpl;

        /// <summary>
        ///     Количество сотрудников по заданным параметрам
        /// </summary>
        public int CountEmployee;

        /// <summary>
        ///     Признак наличия нарушений режима
        /// </summary>
        public bool IsError;

        private EmployeePeriodsInfoDs periodsDs;

        /// <summary>
        ///     Признак расчета нарушений режима в пользу сотрудника
        /// </summary>
        public bool PrimaryEmployeeCalc;

        /// <summary>
        ///     Инициализация и Очистка кеша
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="primaryEmployeeCalc"></param>
        /// <param name="sourcePage"></param>
        public EmplTableCash(DataTable sourceTable, bool primaryEmployeeCalc, BasePage sourcePage)
        {
            _sourceTable = sourceTable;
            _sourcePage = sourcePage;
            PrimaryEmployeeCalc = primaryEmployeeCalc;
            ClearCash();
        }

        /// <summary>
        ///     Очистка кеша
        /// </summary>
        public void ClearCash()
        {
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MinValue;
            _maxIndex = -1;
            _needCheckId = false;
            _filterEmployee = string.Empty;
            _filterCompany = string.Empty;
            _filterPost = string.Empty;
            _filterSubdiv = string.Empty;
            _filterActiveOnly = true;
            _isReversCompany = "0";
            _isReversSubdivision = "0";
            _isReversPosition = "0";
            _isReversPerson = "0";
            _currentPage = 1;
            _rowsPerPage = 35;
            _subEmpl = false;
            //_sourceTable.Rows.Clear();

            try
            {
                _sourceTable.Rows.Clear();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Расчет проходов сотрудников
        /// </summary>
        /// <param name="startPeriod"></param>
        /// <param name="endPeriod"></param>
        /// <param name="filterEmployee"></param>
        /// <param name="filterCompany"></param>
        /// <param name="filterPost"></param>
        /// <param name="filterSubdiv"></param>
        /// <param name="filterTimeExitActive"></param>
        /// <param name="tm"></param>
        /// <param name="filterActiveOnly"></param>
        /// <param name="stz"></param>
        /// <param name="isReversCompany"></param>
        /// <param name="isReversSubdivision"></param>
        /// <param name="isReversPosition"></param>
        /// <param name="isReversPerson"></param>
        /// <param name="currentPage"> </param>
        /// <param name="rowsPerPage"> </param>
        /// <param name="subEmpl"> </param>
        public void CompleteEmplTable(DateTime? startPeriod, DateTime? endPeriod, string filterEmployee,
            string filterCompany, string filterPost, string filterSubdiv,
            bool filterTimeExitActive, DateTime tm, bool filterActiveOnly, string stz, string isReversCompany,
            string isReversSubdivision, string isReversPosition, string isReversPerson,
            int currentPage, int rowsPerPage, bool subEmpl)
        {
            int tz;

            try
            {
                tz = Convert.ToInt32(stz);
            }
            catch
            {
                tz = 0;
            }

            var hasNewIds = _needCheckId && EmployeeTimePeriodsInfo.GetMaxId(_startDate, _endDate, -tz) != _maxIndex;
            var isChanged = _startDate == DateTime.MinValue || _endDate == DateTime.MinValue || hasNewIds;
            var _startPeriod = startPeriod ?? new DateTime(1753, 1, 2);
            var _endPeriod = endPeriod ?? new DateTime(1753, 1, 2);

            if (startPeriod != _startDate || endPeriod != _endDate || !filterEmployee.Equals(_filterEmployee) ||
                !filterCompany.Equals(_filterCompany) || !filterPost.Equals(_filterPost) ||
                !filterSubdiv.Equals(_filterSubdiv) || isChanged || _filterActiveOnly != filterActiveOnly ||
                _isReversCompany != isReversCompany || _isReversSubdivision != isReversSubdivision ||
                _isReversPosition != isReversPosition || _isReversPerson != isReversPerson ||
                _subEmpl != subEmpl)
            {
                ClearCash();
                IsError = false;
                periodsDs = new ExecQuery().CompleteEmployeePeriodsInfoDs(filterEmployee, filterCompany, filterPost,
                    filterSubdiv, _startPeriod, _endPeriod,
                    _sourcePage.IsRusLocal, -tz, filterTimeExitActive, tm, filterActiveOnly, isReversCompany,
                    isReversSubdivision, isReversPosition, isReversPerson, subEmpl);

                FillPage(periodsDs, _startPeriod, _endPeriod, currentPage, rowsPerPage);

                _startDate = _startPeriod;
                _endDate = _endPeriod;
                _filterEmployee = filterEmployee;
                _filterCompany = filterCompany;
                _filterPost = filterPost;
                _filterActiveOnly = filterActiveOnly;
                _filterSubdiv = filterSubdiv;
                _isReversCompany = isReversCompany;
                _isReversSubdivision = isReversSubdivision;
                _isReversPosition = isReversPosition;
                _isReversPerson = isReversPerson;
                _maxIndex = EmployeeTimePeriodsInfo.GetMaxId(_startDate, _endDate, -tz);
                _needCheckId = _maxIndex == EmployeeTimePeriodsInfo.GetMaxId() || _maxIndex == -1;
                _currentPage = currentPage;
                _rowsPerPage = rowsPerPage;
                _subEmpl = subEmpl;
            }
            else if (currentPage != _currentPage || rowsPerPage != _rowsPerPage)
            {
                FillPage(periodsDs, _startPeriod, _endPeriod, currentPage, rowsPerPage);
                _currentPage = currentPage;
                _rowsPerPage = rowsPerPage;
            }
        }

        public void FillPage(EmployeePeriodsInfoDs periodsDs, DateTime _startPeriod, DateTime _endPeriod,
            int currentPage, int rowsPerPage)
        {
            //foreach (EmployeePeriodsInfoDs.СотрудникиRow row in periodsDs.Сотрудники.Rows)
            _sourceTable.Rows.Clear();
            CountEmployee = periodsDs.Сотрудники.Rows.Count;
            var startPosition = currentPage * rowsPerPage - rowsPerPage;
            var endPosition = currentPage * rowsPerPage;
            if (endPosition >= CountEmployee)
                endPosition = CountEmployee;
            for (var i = startPosition; i < endPosition; i++)
            {
                var row = (EmployeePeriodsInfoDs.СотрудникиRow) periodsDs.Сотрудники.Rows[i];
                var periodsInfo = new EmployeeTimePeriodsInfo(_startPeriod, _endPeriod, row.КодСотрудника,
                    PrimaryEmployeeCalc, periodsDs.ПроходыСотрудников);
                var empName = _sourcePage.IsRusLocal ? row.Сотрудник : row.Employee;

                var strEntranceTime = "";
                var strExitTime = "";
                var isEnterAfterExit = "";
                var isEnterAfterExit2 = "";

                if (periodsInfo.ExitTime != null) strExitTime = ((DateTime) periodsInfo.ExitTime).ToString("HH:mm:ss");


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

                object absentTime;

                var periodsInfoTotalAbsentTime = periodsInfo.TotalAbsentTime;
                if ((int) periodsInfoTotalAbsentTime.TotalSeconds != 0)
                    absentTime = string.Format("{0}:{1}:{2}",
                        periodsInfoTotalAbsentTime.Hours + periodsInfoTotalAbsentTime.Days * 24,
                        periodsInfoTotalAbsentTime.Minutes.ToString("D2"),
                        periodsInfoTotalAbsentTime.Seconds.ToString("D2"));
                else
                    absentTime = "";

                object workTime;
                var periodsInfoTotalWorkTime = periodsInfo.TotalWorkTime;
                if ((int) periodsInfoTotalWorkTime.TotalSeconds != 0)
                    workTime = string.Format("{0}:{1}:{2}",
                        periodsInfoTotalWorkTime.Hours + periodsInfoTotalWorkTime.Days * 24,
                        periodsInfoTotalWorkTime.Minutes.ToString("D2"),
                        periodsInfoTotalWorkTime.Seconds.ToString("D2"));
                else
                    workTime = "";

                if (periodsInfo.HasErrors) workTime = workTime + " *";

                _sourceTable.Rows.Add(row.КодСотрудника, row.КодЛица, null, null, strEntranceTime, strExitTime,
                    absentTime, workTime, periodsInfoTotalAbsentTime, periodsInfoTotalWorkTime, empName,
                    periodsInfo.HasErrors ? "errorRow" : "singleRow", "", "", isEnterAfterExit, isEnterAfterExit2);
                if (periodsInfo.HasErrors && !IsError) IsError = true;
            }
        }

        /// <summary>
        ///     Получение источника данных
        /// </summary>
        /// <returns></returns>
        public DataTable GetSourceTable()
        {
            return _sourceTable;
        }

        /// <summary>
        ///     Получение полного источника данных
        /// </summary>
        /// <returns></returns>
        public EmployeePeriodsInfoDs GetDS()
        {
            return periodsDs;
        }
    }
}