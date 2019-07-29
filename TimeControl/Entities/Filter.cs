using System;
using System.Web.UI;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    ///     Класс реализации поиска сотрудников
    /// </summary>
    [Serializable]
    public class Filter
    {
        private PeriodInfo _periodInfo = new PeriodInfo();

        /// <summary>
        ///     Id клиента, используется в таблице Настройки
        /// </summary>
        public string Clid = "";

        /// <summary>
        ///     Коллекция условий
        /// </summary>
        public string CompanyItems = "";

        /// <summary>
        ///     Признак сотрудников имеющих карточки и/или должности
        /// </summary>
        public bool EmplAvaible;

        /// <summary>
        ///     Признак не отображать пустые строки в детализации
        /// </summary>
        public bool IsDetailNoEmpty;

        /// <summary>
        ///     Признак расчета нарушений режима в пользу сотрудника
        /// </summary>
        public bool IsEmployeePrimary;

        /// <summary>
        ///     Признак включая подчиненных
        /// </summary>
        public bool IsSubEmployee;

        /// <summary>
        ///     Пара координат x,y диалогового окна сообщения
        /// </summary>
        public Pair PairAlertPosition;

        /// <summary>
        ///     Пара высота/ширина диалогового окна сообщения
        /// </summary>
        public Pair PairAlertSize;

        /// <summary>
        ///     Пара координат x,y диалогового окна звонилки
        /// </summary>
        public Pair PairCallerPosition;

        /// <summary>
        ///     Пара высота/ширина диалогового окна звонилки
        /// </summary>
        public Pair PairCallerSize;

        /// <summary>
        ///     Коллекция условий
        /// </summary>
        public string PersonItems = "";

        /// <summary>
        ///     Коллекция условий
        /// </summary>
        public string PositionItems = "";

        /// <summary>
        ///     Количество записей на странице
        /// </summary>
        public int RowsPerPage;

        /// <summary>
        ///     Количество записей на странице детализации
        /// </summary>
        public int RowsPerPageDetails;

        /// <summary>
        ///     Коллекция условий
        /// </summary>
        public string SubdivisionItems = "";

        /// <summary>
        ///     Время ухода
        /// </summary>
        public string TimeExit;

        /// <summary>
        ///     Признак учета времени Ушедшие до
        /// </summary>
        public bool TimeExitAvaible;

        /// <summary>
        ///     Период
        /// </summary>
        public PeriodInfo PeriodInfo
        {
            get { return _periodInfo; }
            set { _periodInfo = value; }
        }
    }
}