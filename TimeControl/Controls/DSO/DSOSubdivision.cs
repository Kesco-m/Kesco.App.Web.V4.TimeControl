namespace Kesco.App.Web.TimeControl.Controls.DSO
{
    /// <summary>
    /// Класс реализации поиска подразделений
    /// </summary>
    public class DSOSubdivision
    {
        /// <summary>
        /// Ограничения по подразделению
        /// </summary>
        public string SubdivisionIds;

        /// <summary>
        /// Ограничения по компании
        /// </summary>
        public string CompanyIds;

        /// <summary>
        /// Признак выбора реверса компании
        /// </summary>
        public bool IsReversCompany;

        /// <summary>
        /// Условие Where
        /// </summary>
        /// <returns></returns>
        public string SQLGetClause()
        {
            string result = "";
            if (!string.IsNullOrEmpty(SubdivisionIds))
            {
                //result = string.Format("t.КодДолжности not in ({0})", SubdivisionIds);
                result = string.Format("t.Подразделение not in ({0})", SubdivisionIds);
            }
            if (!string.IsNullOrEmpty(CompanyIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                result += string.Format("t.КодЛица {1} in ({0})", CompanyIds, IsReversCompany ? "NOT" : "");
            }
            return result;
        }
    }
}