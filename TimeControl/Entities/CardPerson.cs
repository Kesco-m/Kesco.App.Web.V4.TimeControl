using System;

namespace Kesco.App.Web.TimeControl.Entities
{
    /// <summary>
    ///     Бизнес-объект - сотрудник
    /// </summary>
    [Serializable]
    public class CardPerson
    {
        /// <summary>
        ///     КодСотрудника
        /// </summary>
        public int CodeEmpl { get; set; }

        /// <summary>
        ///     КодЛица
        /// </summary>
        public int? CodePerson { get; set; }

        /// <summary>
        ///     ФамилияРус
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        ///     ИмяРус
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     ОтчествоРус
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        public string FIOshort { get; set; }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        public string FIOshortLat { get; set; }

        /// <summary>
        ///     Employee
        /// </summary>
        public string Employee { get; set; }

        /// <summary>
        ///     ФамилияЛат
        /// </summary>
        public string SecondNameLat { get; set; }

        /// <summary>
        ///     ИмяЛат
        /// </summary>
        public string FirstNameLat { get; set; }

        /// <summary>
        ///     ОтчествоЛат
        /// </summary>
        public string PatronymicLat { get; set; }

        /// <summary>
        ///     Изменил
        /// </summary>
        public int ChangeBy { get; set; }

        /// <summary>
        ///     Изменено
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        ///     Признак физическое лицо
        /// </summary>
        public bool IsIndividual { get; set; }

        /// <summary>
        ///     ФИОЛат
        /// </summary>
        public string FIOLat => SecondNameLat + " " + FirstNameLat + " " + PatronymicLat;
    }
}