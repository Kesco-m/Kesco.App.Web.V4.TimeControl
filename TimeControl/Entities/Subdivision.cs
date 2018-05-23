using System;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    /// Бизнес-объект - подразделение
    /// </summary>
    [Serializable]
    public class Subdivision
    {
        /// <summary>
        /// КодПодразделения
        /// </summary>
        public int CodeSubdivision { get; set; }

        /// <summary>
        /// КодЛица
        /// </summary>
        public int CodePerson { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string NameSubdivision { get; set; }

        /// <summary>
        /// Наименование подразделения и компании
        /// </summary>
        public string NameSubdivisionRus { get; set; }

        /// <summary>
        /// Наименование подразделения и компании (Lat)
        /// </summary>
        public string NameSubdivisionLat { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        public string NameCompany { get; set; }

        /// <summary>
        /// Наименование компании (Lat)
        /// </summary>
        public string NameCompanyLat { get; set; }

        /// <summary>
        /// КодПодразделения
        /// </summary>
        public string SubdivisionIdString { get; set; }
    }
}