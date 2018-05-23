using System;
using System.Web.UI;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    /// Класс реализации поиска сотрудников
    /// </summary>
    [Serializable]
    public class Filter
    {
        /// <summary>
        /// Коллекция условий
        /// </summary>
        public string CompanyItems = "";

        /// <summary>
        /// Коллекция условий
        /// </summary>
        public string SubdivisionItems = "";

        /// <summary>
        /// Коллекция условий
        /// </summary>
        public string PositionItems = "";

        /// <summary>
        /// Коллекция условий
        /// </summary>
        public string PersonItems = "";

        /// <summary>
        /// Признак учета времени Ушедшие до
        /// </summary>
        public bool TimeExitAvaible;

        /// <summary>
        /// Время ухода
        /// </summary>
        public string TimeExit;

        /// <summary>
        /// Признак сотрудников имеющих карточки и/или должности
        /// </summary>
        public bool EmplAvaible;

        /// <summary>
        /// Признак включая подчиненных
        /// </summary>
        public bool IsSubEmployee;

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int RowsPerPage;

        /// <summary>
        /// Количество записей на странице детализации
        /// </summary>
        public int RowsPerPageDetails;

        /// <summary>
        /// Признак не отображать пустые строки в детализации
        /// </summary>
        public bool IsDetailNoEmpty;

        /// <summary>
        /// Признак расчета нарушений режима в пользу сотрудника
        /// </summary>
        public bool IsEmployeePrimary;

        private PeriodInfo _periodInfo = new PeriodInfo();
        /// <summary>
        /// Период
        /// </summary>
        public PeriodInfo PeriodInfo { get { return _periodInfo; } set { _periodInfo = value; } }

        /// <summary>
        /// Пара высота/ширина диалогового окна звонилки
        /// </summary>
        public Pair PairCallerSize;

        /// <summary>
        /// Пара координат x,y диалогового окна звонилки
        /// </summary>
        public Pair PairCallerPosition;

        /// <summary>
        /// Пара высота/ширина диалогового окна сообщения
        /// </summary>
        public Pair PairAlertSize;

        /// <summary>
        /// Пара координат x,y диалогового окна сообщения
        /// </summary>
        public Pair PairAlertPosition;
        
        /// <summary>
        /// Id клиента, используется в таблице Настройки
        /// </summary>
        public string Clid = "";
    }
}