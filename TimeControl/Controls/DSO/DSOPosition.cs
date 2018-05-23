namespace Kesco.App.Web.TimeControl.Controls.DSO
{
    /// <summary>
    /// Класс реализации поиска должностей
    /// </summary>
    public class DSOPosition
    {
        /// <summary>
        /// Ограничения по должности
        /// </summary>
        public string PositionIds;

        /// <summary>
        /// Ограничения по компании
        /// </summary>
        public string CompanyIds;

        /// <summary>
        /// Ограничения по подразделениям
        /// </summary>
        public string SubdivisionIds;

        /// <summary>
        /// Признак выбора реверса компании
        /// </summary>
        public bool IsReversCompany;

        /// <summary>
        /// Признак выбора реверса подразделения
        /// </summary>
        public bool IsReversSubdivision;

        /// <summary>
        /// Условие Where
        /// </summary>
        /// <returns></returns>
        public string SQLGetClause()
        {
            string result = "";
            if (!string.IsNullOrEmpty(PositionIds))
            {
                result = string.Format("t.Должность not in ({0})", PositionIds);
            }
            if (!string.IsNullOrEmpty(CompanyIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                result += string.Format("t.КодЛица {1} in ({0})", CompanyIds, IsReversCompany ? "NOT" : "");
            }
            if (!string.IsNullOrEmpty(SubdivisionIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                result += string.Format("t.Parent {1} in ({0})", SubdivisionIds, IsReversSubdivision ? "NOT" : "");
            }
            return result;
        }
    }
}