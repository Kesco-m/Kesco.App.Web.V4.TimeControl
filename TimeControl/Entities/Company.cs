using System;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    ///     Бизнес-объект - компания
    /// </summary>
    [Serializable]
    public class Company
    {
        /// <summary>
        ///     КодКомпании
        /// </summary>
        public int CodeCompany { get; set; }

        /// <summary>
        ///     Наименование компании
        /// </summary>
        public string NameCompany { get; set; }

        /// <summary>
        ///     Наименование компании (Lat)
        /// </summary>
        public string NameCompanyLat { get; set; }
    }
}