using System;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    ///     Бизнес-объект - период
    /// </summary>
    [Serializable]
    public class PeriodInfo
    {
        /// <summary>
        /// </summary>
        public string CompanyHowSearch = "0";

        /// <summary>
        /// </summary>
        public string PersonHowSearch = "0";

        /// <summary>
        /// </summary>
        public string PositionHowSearch = "0";

        /// <summary>
        /// </summary>
        public string SubdivisionHowSearch = "0";

        /// <summary>
        ///     Тип периода
        /// </summary>
        public string PeriodType { get; set; }

        /// <summary>
        ///     Дата начала периода
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        ///     Дата конца периода
        /// </summary>
        public DateTime? DateTo { get; set; }
    }
}