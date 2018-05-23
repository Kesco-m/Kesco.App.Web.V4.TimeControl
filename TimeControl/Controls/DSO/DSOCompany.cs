namespace Kesco.App.Web.TimeControl.Controls.DSO
{
    /// <summary>
    /// Класс реализации поиска компаний
    /// </summary>
    public class DSOCompany
    {
        /// <summary>
        /// Ограничения по компании
        /// </summary>
        public string CompanyIds;

        /// <summary>
        /// Условие Where
        /// </summary>
        /// <returns></returns>
        public string SQLGetClause()
        {
            if (!string.IsNullOrEmpty(CompanyIds))
                return string.Format("(t.КодЛица not in ({0}))", CompanyIds);
            return "";
        }
    }
}