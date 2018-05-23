using System;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    /// Бизнес-объект - должность
    /// </summary>
    [Serializable]
    public class Position
    {
        /// <summary>
        /// КодДолжности
        /// </summary>
        public int CodePosition { get; set; }

        /// <summary>
        /// КодЛица
        /// </summary>
        public int CodePerson { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        public string NamePosition { get; set; }

        /// <summary>
        /// Наименование должности и компании
        /// </summary>
        public string NamePositionRus { get; set; }

        /// <summary>
        /// Наименование должности и компании (Lat)
        /// </summary>
        public string NamePositionLat { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        public string NameCompany { get; set; }

        /// <summary>
        /// Наименование компании (Lat)
        /// </summary>
        public string NameCompanyLat { get; set; }

        /// <summary>
        /// КодДолжности
        /// </summary>
        public string PositionIdString { get; set; }
    }
}