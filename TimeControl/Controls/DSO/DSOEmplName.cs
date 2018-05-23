namespace Kesco.App.Web.TimeControl.Controls.DSO
{
    /// <summary>
    /// Класс реализации поиска сотрудников
    /// </summary>
    public class DSOEmplName
    {
        /// <summary>
        /// Ограничения по сотруднику
        /// </summary>
        public string EmplIds;

        /// <summary>
        /// Ограничения по компании
        /// </summary>
        public string CompanyIds;

        /// <summary>
        /// Ограничения по подразделениям
        /// </summary>
        public string SubdivisionIds;

        /// <summary>
        /// Ограничения по должности
        /// </summary>
        public string PositionIds;

        /// <summary>
        /// Признак выбора реверса компании
        /// </summary>
        public bool IsReversCompany;

        /// <summary>
        /// Признак выбора реверса подразделения
        /// </summary>
        public bool IsReversSubdivision;

        /// <summary>
        /// Признак выбора реверса должностей
        /// </summary>
        public bool IsReversPosition;

        /// <summary>
        /// Признак выбора реверса сотрудников
        /// </summary>
        public bool IsReversPerson;

        /// <summary>
        /// Условие Where
        /// </summary>
        /// <returns></returns>
        public string SQLGetClause()
        {
            string result = "";
            if (!string.IsNullOrEmpty(EmplIds))
            {
                result = string.Format("T0.КодСотрудника not in ({0})", EmplIds);
            }
            if (!string.IsNullOrEmpty(CompanyIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                result += string.Format("T0.КодЛицаЗаказчика {1} in ({0})", CompanyIds, IsReversCompany ? "NOT" : "");
            }
            if (!string.IsNullOrEmpty(SubdivisionIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                //result += string.Format("(T1.КодДолжности {1} in ({0}) {2} T1.Parent {1} in ({0}))", SubdivisionIds, IsReversSubdivision ? "NOT" : "", IsReversSubdivision ? "and" : "or");
                result += string.Format("{1} EXISTS (SELECT * FROM [Инвентаризация].[dbo].[vwДолжности] P0 WHERE (T1.L >= P0.L AND T1.R <= P0.R) AND P0.Подразделение IN ({0}))", 
                    SubdivisionIds, IsReversSubdivision ? "NOT" : "");
            }
            if (!string.IsNullOrEmpty(PositionIds))
            {
                if (!string.IsNullOrEmpty(result))
                    result += " and ";
                result += string.Format("T1.КодДолжности {1} in ({0})", PositionIds, IsReversPosition ? "NOT" : "");
            }
            return result;
        }
    }
}