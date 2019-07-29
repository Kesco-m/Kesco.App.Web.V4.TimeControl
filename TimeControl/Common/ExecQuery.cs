using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Xml.Serialization;
using Kesco.App.Web.TimeControl.DataSets;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Log;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    ///     Класс с запросами к БД
    /// </summary>
    public class ExecQuery
    {
        /// <summary>
        ///     Получение сотрудника по ID
        /// </summary>
        /// <param name="id">Код сотрудника</param>
        /// <returns>CardPerson</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public CardPerson GetCardPersonById(string id)
        {
            var sql = string.Format(
                @"SELECT * FROM [Инвентаризация].[dbo].[Сотрудники] (nolock) WHERE КодСотрудника={0}", id);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            return dt.AsEnumerable().Select(dr => new CardPerson
            {
                CodeEmpl = dr.Field<int>("КодСотрудника"),
                CodePerson = dr.Field<int?>("КодЛица"),
                FIO = dr.Field<string>("Сотрудник"),
                FIOshort = dr.Field<string>("ФИО"),
                FIOshortLat = dr.Field<string>("FIO"),
                Employee = dr.Field<string>("Employee"),
                ChangeBy = Convert.ToInt32(dr.Field<int>("Изменил")),
                ChangeDate = Convert.ToDateTime(dr.Field<DateTime>("Изменено"))
            }).FirstOrDefault();
        }

        /// <summary>
        ///     Проверка наличия должности у компании
        /// </summary>
        /// <param name="id">Код компании</param>
        /// <param name="position">Должность </param>
        /// <param name="isRevers">Признак реверса для компаний </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckPositionByCompanyId(string id, string position, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("{1} IN ({0}) ", id, isRevers ? "NOT" : "");
            if (isNull)
                clause = "IS NULL";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[vwДолжности] (nolock) 
            WHERE КодЛица {0} AND Должность='{1}'", clause, position);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Проверка наличия должности у подразделения
        /// </summary>
        /// <param name="id">Код подразделения</param>
        /// <param name="position">Должность </param>
        /// <param name="isRevers">Признак реверса для подразделений </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckPositionBySubdivisionId(string id, string position, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("P0.Подразделение {1} IN ({0}) ", id, isRevers ? "NOT" : "");
            if (isNull)
                clause =
                    "EXISTS (SELECT P0.КодДолжности FROM vwДолжности AS P0 WHERE (T0.L >= P0.L AND T0.R <= P0.R) AND (P0.Подразделение IS NULL OR P0.Подразделение = ''))";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[vwДолжности] t (nolock) 
            WHERE EXISTS (SELECT P0.КодДолжности FROM vwДолжности AS P0 (nolock) WHERE (t.L >= P0.L AND t.R <= P0.R) AND {0}) AND t.Должность='{1}'",
                clause, position);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Проверка наличия подразделения у компании
        /// </summary>
        /// <param name="id">Код компании</param>
        /// <param name="subdivision">Подразделение </param>
        /// <param name="isRevers">Признак реверса для компаний </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckSubdivisionByCompanyId(string id, string subdivision, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("{1} IN ({0}) ", id, isRevers ? "NOT" : "");
            if (isNull)
                clause = "IS NULL";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[vwДолжности] (nolock) 
            WHERE КодЛица {0} AND Подразделение='{1}'", clause, subdivision);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Получение списка компаний по подразделению
        /// </summary>
        /// <param name="subdivision">Подразделение</param>
        /// <returns>Список компаний</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public List<PersonCustomer> GetCompanyBySubdivision(string subdivision)
        {
            var sql = string.Format(@"SELECT t1.КодЛица, t2.Кличка, t2.КраткоеНазваниеЛат
FROM [Инвентаризация].[dbo].[vwДолжности] t1 (nolock) 
JOIN [Инвентаризация].[dbo].[ЛицаЗаказчики] t2 ON t1.КодЛица = t2.КодЛица
WHERE t1.Подразделение='{0}'
ORDER BY t2.Кличка", subdivision);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            var result = dt.AsEnumerable().Select(dr => new PersonCustomer
            {
                Id = dr.Field<int>("КодЛица").ToString(CultureInfo.InvariantCulture),
                Name = dr.Field<string>("Кличка"),
                NameLat = dr.Field<string>("КраткоеНазваниеЛат")
            }).ToList();
            return result;
        }

        /// <summary>
        ///     Проверка наличия сотрудника у подразделения
        /// </summary>
        /// <param name="id">Код подразделения</param>
        /// <param name="idEmloyee">сотрудник </param>
        /// <param name="isRevers">Признак реверса для компаний </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckPersonBySubdivisionId(string id, string idEmloyee, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("{1} IN ({0})", id, isRevers ? "NOT" : "");
            if (isNull)
                clause = "IS NULL OR T1.Подразделение = ''";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[Сотрудники] T0 (nolock)
            LEFT JOIN [Инвентаризация].[dbo].[vwДолжности] T1 ON T0.КодСотрудника = T1.КодСотрудника
            WHERE T1.Подразделение {0} AND T0.КодСотрудника={1}", clause, idEmloyee);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Проверка наличия сотрудника у должности
        /// </summary>
        /// <param name="id">Код должности</param>
        /// <param name="idEmloyee">сотрудник </param>
        /// <param name="isRevers">Признак реверса для должностей </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckPersonByPositionId(string id, string idEmloyee, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("{1} IN ({0})", id, isRevers ? "NOT" : "");
            if (isNull)
                clause = "IS NULL OR T1.Должность = ''";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[Сотрудники] T0 (nolock)
            LEFT JOIN [Инвентаризация].[dbo].[vwДолжности] T1 ON T0.КодСотрудника = T1.КодСотрудника
            WHERE T1.Должность {0} AND T0.КодСотрудника={1}", clause, idEmloyee);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Проверка наличия сотрудника у компании
        /// </summary>
        /// <param name="id">Код компании</param>
        /// <param name="fio">сотрудник </param>
        /// <param name="isRevers">Признак реверса для компаний </param>
        /// <param name="isNull">Признак наименования компании - NULL </param>
        /// <returns>да/нет</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public bool CheckPersonByCompanyId(string id, string fio, bool isRevers, bool isNull)
        {
            int count;
            var clause = string.Format("{1} IN ({0}) ", id, isRevers ? "NOT" : "");
            if (isNull)
                clause = "IS NULL";
            var sql = string.Format(@"SELECT count(*) FROM [Инвентаризация].[dbo].[Сотрудники] (nolock) 
            WHERE КодЛицаЗаказчика {0} AND Сотрудник='{1}'", clause, fio);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            try
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                count = 0;
            }

            return count > 0;
        }

        /// <summary>
        ///     Получение проходов сотрудника
        /// </summary>
        /// <param name="emplName"></param>
        /// <param name="companyInfo"></param>
        /// <param name="postInfo"></param>
        /// <param name="divisionInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isRusLocal"></param>
        /// <param name="tz"></param>
        /// <param name="activeTimeExit"></param>
        /// <param name="tm"></param>
        /// <param name="active"></param>
        /// <param name="isReversCompany"></param>
        /// <param name="isReversSubdivision"></param>
        /// <param name="isReversPosition"></param>
        /// <param name="isReversPerson"></param>
        /// <param name="subEmpl"></param>
        /// <returns></returns>
        public EmployeePeriodsInfoDs CompleteEmployeePeriodsInfoDs(string emplName, string companyInfo, string postInfo,
            string divisionInfo, DateTime startDate, DateTime endDate,
            bool isRusLocal, int tz, bool activeTimeExit, DateTime tm, bool active, string isReversCompany,
            string isReversSubdivision, string isReversPosition, string isReversPerson,
            bool subEmpl)
        {
            var activeTimeExitVerify = startDate.Equals(endDate);
            var sStartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            var sEndDate = endDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            var timeExitRestriction = "";
            var timeExitLocation = "";
            var clauseCompany = 0;
            var clauseSubdivision = 0;
            var clausePosition = 0;
            var clausePerson = 0;
            if (isReversCompany == "1")
                clauseCompany = 1;
            else if (isReversCompany == "3")
                clauseCompany = 2;
            else if (isReversCompany == "2")
                clauseCompany = 3;
            if (isReversSubdivision == "1")
                clauseSubdivision = 1;
            else if (isReversSubdivision == "3")
                clauseSubdivision = 2;
            else if (isReversSubdivision == "2")
                clauseSubdivision = 3;
            if (isReversPosition == "1")
                clausePosition = 1;
            else if (isReversPosition == "3")
                clausePosition = 2;
            else if (isReversPosition == "2")
                clausePosition = 3;
            if (isReversPerson == "1")
                clausePerson = 1;
            else if (isReversPerson == "3")
                clausePerson = 2;
            else if (isReversPerson == "2")
                clausePerson = 3;
            if (activeTimeExitVerify && activeTimeExit)
            {
                timeExitLocation = @" 
 AND TE0.КодРасположения IS NULL";
                var sTimeExit = tm.ToString("yyyy-MM-dd HH:mm:ss");
                timeExitRestriction = string.Format(@"
 AND NOT EXISTS(SELECT * FROM vwПроходыСотрудников TE1 (nolock) 
								WHERE TE0.КодСотрудника = TE1.КодСотрудника 
										AND Dateadd(MINUTE,{1},TE1.[Когда]) > Dateadd(MINUTE,{1},TE0.[Когда])
										AND Dateadd(MINUTE,{1},TE1.[Когда]) < CONVERT(datetime, '{0}', 120))",
                    sEndDate, tz);
                sEndDate = sTimeExit;
            }

            var result = new EmployeePeriodsInfoDs();
            var additionalInnerJoin = string.Empty;
            var additionalRestrictions = string.Empty;
            var additionalRestrictionsCr = string.Empty;

            if (!emplName.Equals(string.Empty) && clausePerson != 2)
                additionalRestrictions = string.Format(" AND {2} T0.КодСотрудника {1} IN ({0})", emplName,
                    clausePerson == 1 ? "NOT" : "", string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clausePerson == 2)
                additionalRestrictions = string.Format(" AND {0} T0.КодСотрудника IS NULL",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");

            if (!postInfo.Equals(string.Empty) || !divisionInfo.Equals(string.Empty) ||
                !companyInfo.Equals(string.Empty) || clausePosition == 2 || clauseSubdivision == 2 || clauseCompany == 2
                || clausePosition == 3 || clauseSubdivision == 3 || clauseCompany == 3)
                additionalInnerJoin = " LEFT JOIN vwДолжности AS T1 ON (T0.КодСотрудника = T1.КодСотрудника)";

            if (!postInfo.Equals(string.Empty) && clausePosition != 2 && clausePosition != 3)
                additionalRestrictions += string.Format(" AND {2} T1.Должность {1} IN ({0})", postInfo,
                    clausePosition == 1 ? "NOT" : "", string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clausePosition == 2)
                additionalRestrictions += string.Format(" AND {0} (T1.Должность IS NULL OR T1.Должность = '')",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clausePosition == 3)
                additionalRestrictions += string.Format(" AND {0} (T1.Должность IS NOT NULL AND T1.Должность <> '')",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");

            if (!divisionInfo.Equals(string.Empty) && clauseSubdivision != 2 && clauseSubdivision != 3)
                additionalRestrictions += string.Format(
                    " AND {2} EXISTS (SELECT P0.КодДолжности FROM vwДолжности AS P0 WHERE (T1.L >= P0.L AND T1.R <= P0.R) AND P0.Подразделение {1} IN ({0}))",
                    divisionInfo, clauseSubdivision == 1 ? "NOT" : "",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clauseSubdivision == 2)
                additionalRestrictions += string.Format(
                    " AND {0} EXISTS (SELECT P0.КодДолжности FROM vwДолжности AS P0 WHERE (T1.L >= P0.L AND T1.R <= P0.R) AND (P0.Подразделение IS NULL OR P0.Подразделение = ''))",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clauseSubdivision == 3)
                additionalRestrictions += string.Format(
                    " AND {0} EXISTS (SELECT P0.КодДолжности FROM vwДолжности AS P0 WHERE (T1.L >= P0.L AND T1.R <= P0.R) AND (P0.Подразделение IS NOT NULL AND P0.Подразделение <> ''))",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");

            if (!companyInfo.Equals(string.Empty) && clauseCompany != 2)
                additionalRestrictions += string.Format(" AND {2} T0.КодЛицаЗаказчика {1} IN ({0})", companyInfo,
                    clauseCompany == 1 ? "NOT" : "", string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");
            if (clauseCompany == 2)
                additionalRestrictions += string.Format(" AND {0} T0.КодЛицаЗаказчика IS NULL",
                    string.IsNullOrEmpty(additionalRestrictions) ? "(" : " ");

            var dateRestriction = string.Format(
                " Dateadd(MINUTE,{2},TE0.[Когда]) > CONVERT(datetime, '{0}', 120) AND Dateadd(MINUTE,{2},TE0.[Когда]) < CONVERT(datetime, '{1}', 120)",
                sStartDate, sEndDate, tz);

            const string queryEmplTempl = @"
SELECT T0.КодСотрудника{0}
FROM Сотрудники AS T0 {1}
WHERE T0.КодСотрудника IN (SELECT TE0.КодСотрудника FROM vwПроходыСотрудников TE0 (nolock) WHERE {5} {6} {4}){2}{3}";

            var personWhere = string.Format(@"
WHERE (T0.КодСотрудника IN (SELECT TE0.КодСотрудника FROM vwПроходыСотрудников TE0 (nolock)
WHERE DATEADD(minute, {0}, TE0.[Когда]) >= CONVERT(datetime, '{1}', 120) AND DATEADD(minute, {0}, TE0.[Когда]) <= CONVERT(datetime, '{2}', 120)",
                tz, sStartDate, sEndDate);

            //string personActive = active ? "" : " OR T0.КодСотрудника IN (SELECT КодСотрудника FROM vwСотрудникиИмеющиеДолжностиИлиКарточки)";

            var personActive = active
                ? ""
                : @" OR T0.КодСотрудника IN (SELECT КодСотрудника FROM(SELECT КодСотрудника FROM vwДолжности WHERE КодСотрудника IS NOT NULL 
		UNION SELECT КодСотрудника FROM КарточкиСотрудников) X
WHERE КодСотрудника IN (SELECT КодСотрудника FROM vwПодчинённые) 
	OR EXISTS (SELECT * FROM dbo.fn_ТекущиеРоли() Y 
			WHERE КодРоли IN(31,32,33) AND (КодЛица = 0 OR КодЛица IN(SELECT КодЛица FROM vwДолжности UNION SELECT КодЛицаЗаказчика FROM Сотрудники))))";

            //string personSubEmpl = !subEmpl ? "" : @" OR T0.КодСотрудника IN (SELECT КодСотрудника FROM vwПодчинённые)";
            var startAddFilter = "";
            var endAddFilter = "";
            if (subEmpl && (!string.IsNullOrEmpty(divisionInfo) || !string.IsNullOrEmpty(postInfo) ||
                            !string.IsNullOrEmpty(emplName)))
            {
                startAddFilter = " OR (";
                endAddFilter = ")";
            }

            var divisionInfoSubEmpl = !subEmpl || string.IsNullOrEmpty(divisionInfo)
                ? ""
                : string.Format(@"T0.КодСотрудника IN (SELECT DISTINCT Должности.КодСотрудника
FROM	dbo.vwДолжности Chief 
INNER JOIN dbo.vwДолжности Должности ON Chief.L <= Должности.L AND Chief.R >= Должности.R
WHERE	Должности.КодСотрудника IS NOT NULL AND
	Chief.КодДолжности IN
	(
		SELECT	КодДолжности
		FROM	dbo.vwДолжности
		WHERE	Подразделение IN ({0})
		UNION
		SELECT	Slave.КодДолжности
		FROM	dbo.ПодчинениеАдминистративное Chief INNER JOIN
			dbo.ПодчинениеАдминистративное Slave ON Chief.L < Slave.L AND Chief.R > Slave.R INNER JOIN
			dbo.vwДолжности ON Chief.КодДолжности = dbo.vwДолжности.КодДолжности
		WHERE	Подразделение IN ({0})		
	))", divisionInfo);
            var startdivisionFilter = "";
            if (subEmpl && !string.IsNullOrEmpty(divisionInfo)) startdivisionFilter = " AND ";
            var postInfoSubEmpl = !subEmpl || string.IsNullOrEmpty(postInfo)
                ? ""
                : string.Format(@"{1}T0.КодСотрудника IN (SELECT DISTINCT Должности.КодСотрудника
FROM	dbo.vwДолжности Chief 
INNER JOIN dbo.vwДолжности Должности ON Chief.L <= Должности.L AND Chief.R >= Должности.R
WHERE	Должности.КодСотрудника IS NOT NULL AND
	Chief.КодДолжности IN
	(
		SELECT	КодДолжности
		FROM	dbo.vwДолжности
		WHERE	Должность IN ({0})
		UNION
		SELECT	Slave.КодДолжности
		FROM	dbo.ПодчинениеАдминистративное Chief INNER JOIN
			dbo.ПодчинениеАдминистративное Slave ON Chief.L < Slave.L AND Chief.R > Slave.R INNER JOIN
			dbo.vwДолжности ON Chief.КодДолжности = dbo.vwДолжности.КодДолжности
		WHERE	Должность IN ({0})
		
	))", postInfo, startdivisionFilter);
            var startpostFilter = "";
            if (subEmpl && (!string.IsNullOrEmpty(divisionInfo) || !string.IsNullOrEmpty(postInfo)))
                startpostFilter = " AND ";
            var personSubEmpl = !subEmpl || string.IsNullOrEmpty(emplName)
                ? ""
                : string.Format(@"{1}T0.КодСотрудника IN (SELECT DISTINCT Должности.КодСотрудника
FROM	dbo.vwДолжности Chief 
INNER JOIN dbo.vwДолжности Должности ON Chief.L <= Должности.L AND Chief.R >= Должности.R
WHERE	Должности.КодСотрудника IS NOT NULL AND
	Chief.КодДолжности IN
	(
		SELECT	КодДолжности
		FROM	dbo.vwДолжности
		WHERE	КодСотрудника IN ({0})
		UNION
		SELECT	Slave.КодДолжности
		FROM	dbo.ПодчинениеАдминистративное Chief INNER JOIN
			dbo.ПодчинениеАдминистративное Slave ON Chief.L < Slave.L AND Chief.R > Slave.R INNER JOIN
			dbo.vwДолжности ON Chief.КодДолжности = dbo.vwДолжности.КодДолжности
		WHERE	КодСотрудника IN ({0})
		
	))", emplName, startpostFilter);

            var endRestrictions = "";
            if (!additionalRestrictions.Equals(string.Empty)) endRestrictions = " ) ";

            var personSearch = string.Format(
                @"SELECT DISTINCT T0.КодСотрудника, CONVERT(varchar, COALESCE(T0.КодЛица, -1)) КодЛица, T0.Сотрудник, T0.Employee FROM Сотрудники T0
{0}{1}{2}{3}){4}){5}{9}{6}{7}{8}{10}{11}
ORDER BY T0.Сотрудник", additionalInnerJoin, personWhere, timeExitLocation, timeExitRestriction, personActive,
                additionalRestrictions,
                divisionInfoSubEmpl, postInfoSubEmpl, personSubEmpl, startAddFilter, endAddFilter, endRestrictions);

//            string entranceQuery = @"
//SELECT TE0.КодПроходаСотрудника, TE0.КодСотрудника, Dateadd(MINUTE," + tz + @",TE0.[Когда]) Когда, TE0.КодРасположения 
//FROM vwПроходыСотрудников AS TE0 (nolock) WHERE КодСотрудника IN ("
//                + string.Format(queryEmplTempl, "", additionalInnerJoin, additionalRestrictionsCr, additionalRestrictions, timeExitRestriction, dateRestriction, timeExitLocation) + ")";

            var entranceQuery = @"
SELECT TE0.КодПроходаСотрудника, TE0.КодСотрудника, Dateadd(MINUTE," + tz + @",TE0.[Когда]) Когда, TE0.КодРасположения 
FROM vwПроходыСотрудников AS TE0 (nolock) WHERE КодСотрудника IN ("
                                + string.Format(queryEmplTempl, "", additionalInnerJoin, additionalRestrictionsCr,
                                    additionalRestrictions, timeExitRestriction, dateRestriction, timeExitLocation) +
                                string.Format("{0}{1}{2}{3}{4}{5})", startAddFilter, divisionInfoSubEmpl,
                                    postInfoSubEmpl, personSubEmpl, endAddFilter, endRestrictions);

            //var args = new Dictionary<string, object>();
            //args.Add("@ОтображатьВсех", active ? 0 : 1);
            //args.Add("@УшедшиеДо", !(activeTimeExitVerify && activeTimeExit) ? 0 : 1);
            //args.Add("@ДатаНачала", startDate);
            //args.Add("@ДатаКонца", Convert.ToDateTime(sEndDate));//endDate.AddDays(1));
            //args.Add("@ТаймЗона", tz);
            //args.Add("@ФИО", emplName);
            //args.Add("@Должность", postInfo);
            //args.Add("@Подразделение", divisionInfo);
            //args.Add("@КодЛица", companyInfo);
            //args.Add("@РеверсКомпаний", clauseCompany);
            //args.Add("@РеверсПодразделений", clauseSubdivision);
            //args.Add("@РеверсДолжностей", clausePosition);
            //args.Add("@РеверсСотрудников", clausePerson);
            //DataTable dt1 = DBManager.GetData("sp_УчетРабочегоВремени_Поиск", Global.ConnectionString, CommandType.StoredProcedure, args);

            var dt1 = DBManager.GetData(personSearch, Global.ConnectionString);
            result.Сотрудники.Clear();
            result.Сотрудники.Merge(dt1);

            var dt2 = DBManager.GetData(entranceQuery, Global.ConnectionString);
            result.ПроходыСотрудников.Clear();
            result.ПроходыСотрудников.Merge(dt2);

            return result;
        }

        /// <summary>
        ///     Получение проходов сотрудника
        /// </summary>
        /// <param name="emplId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public EmployeePeriodsInfoDs CompleteEmployeePeriodsInfoDs(int emplId, DateTime startDate, DateTime endDate,
            int tz)
        {
            var sStartDate = startDate.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            var sEndDate = endDate.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");

            var result = new EmployeePeriodsInfoDs();

            var sql = string.Format(
                "SELECT КодСотрудника, Сотрудник, Employee FROM Сотрудники WHERE КодСотрудника = {0}", emplId);
            var dt1 = DBManager.GetData(sql, Global.ConnectionString);
            result.Сотрудники.Clear();
            result.Сотрудники.Merge(dt1);

            sql = string.Format(
                @"SELECT КодПроходаСотрудника, КодСотрудника, Dateadd(MINUTE,{2},[Когда]) Когда, КодРасположения FROM vwПроходыСотрудников (nolock) 
WHERE (КодСотрудника = {3}) AND (Dateadd(MINUTE,{2},[Когда]) >= convert(datetime, '{0}', 120)) AND (Dateadd(MINUTE,{2},[Когда]) <= convert(datetime, '{1}', 120))",
                sStartDate, sEndDate, tz, emplId);
            var dt2 = DBManager.GetData(sql, Global.ConnectionString);
            result.ПроходыСотрудников.Clear();
            result.ПроходыСотрудников.Merge(dt2);

            return result;
        }

        /// <summary>
        ///     Получение проходов сотрудника
        /// </summary>
        /// <param name="emplId"></param>
        /// <param name="startDateP1"></param>
        /// <param name="endDateP1"></param>
        /// <param name="startDateP2"></param>
        /// <param name="endDateP2"></param>
        /// <returns></returns>
        public EmployeePeriodsInfoDs CompleteEmployeePeriodsInfoDs(int emplId, DateTime startDateP1, DateTime endDateP1,
            DateTime startDateP2, DateTime endDateP2)
        {
            var sStartDateP1 = startDateP1.ToString("yyyy-MM-dd HH:mm:ss");
            var sEndDateP1 = endDateP1.ToString("yyyy-MM-dd HH:mm:ss");

            var sStartDateP2 = startDateP2.ToString("yyyy-MM-dd HH:mm:ss");
            var sEndDateP2 = endDateP2.ToString("yyyy-MM-dd HH:mm:ss");

            var result = new EmployeePeriodsInfoDs();

            var sql = string.Format(
                "SELECT КодСотрудника, Сотрудник, Employee FROM Сотрудники WHERE КодСотрудника = {0}", emplId);
            var dt1 = DBManager.GetData(sql, Global.ConnectionString);
            result.Сотрудники.Clear();
            result.Сотрудники.Merge(dt1);

            sql = string.Format(@"
SELECT КодПроходаСотрудника, КодСотрудника, Когда, КодРасположения
FROM vwПроходыСотрудников (nolock)
WHERE (КодСотрудника = {4}) AND ((Когда >= convert(datetime, '{0}', 120) AND Когда <= convert(datetime, '{1}', 120))
	OR (Когда >= convert(datetime, '{2}', 120) AND Когда <= convert(datetime, '{3}', 120)))", sStartDateP1, sEndDateP1,
                sStartDateP2, sEndDateP2, emplId);
            var dt2 = DBManager.GetData(sql, Global.ConnectionString);
            result.ПроходыСотрудников.Clear();
            result.ПроходыСотрудников.Merge(dt2);

            return result;
        }

        /// <summary>
        ///     Сохранение настроек пользователя
        /// </summary>
        /// <param name="filter">Настройки </param>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public void SaveSettings(Filter filter)
        {
            if (string.IsNullOrEmpty(filter.Clid))
                filter.Clid = "0";
            //string item = Serialize(filter, typeof(List<Item>), "CardPerson");
            var sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filteremployee", filter.PersonItems);
            DBManager.GetData(sql, Global.ConnectionString);

            //item = Serialize(filter, typeof(List<Item>), "Company");
            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filtercompany", filter.CompanyItems);
            DBManager.GetData(sql, Global.ConnectionString);

            //item = Serialize(filter, typeof(List<Item>), "Position");
            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filterpost", filter.PositionItems.Replace("'", ""));
            DBManager.GetData(sql, Global.ConnectionString);

            //item = Serialize(filter, typeof(List<Item>), "Subdivision");
            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filtersubdivision", filter.SubdivisionItems.Replace("'", ""));
            DBManager.GetData(sql, Global.ConnectionString);

            var item = Serialize(filter, typeof(PeriodInfo), "PeriodInfo");
            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_emplperiodinfo", item);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', {2})", filter.Clid, "tc_emplrowsperpage", filter.RowsPerPage);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', {2})", filter.Clid, "tc_detailrows", filter.RowsPerPageDetails);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_detailnoempty", filter.IsDetailNoEmpty);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_employeeprimary", filter.IsEmployeePrimary);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filterteavaible", filter.TimeExitAvaible);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filterte", filter.TimeExit);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filteremplavaible", filter.EmplAvaible);
            DBManager.GetData(sql, Global.ConnectionString);

            sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_filtersubempl", filter.IsSubEmployee);
            DBManager.GetData(sql, Global.ConnectionString);

            if (filter.PairCallerPosition != null &&
                !string.IsNullOrEmpty(filter.PairCallerPosition.First.ToString()) &&
                !string.IsNullOrEmpty(filter.PairCallerPosition.Second.ToString()))
            {
                sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_callerPosition",
                    filter.PairCallerPosition.First + "," + filter.PairCallerPosition.Second);
                DBManager.GetData(sql, Global.ConnectionString);
            }

            if (filter.PairCallerSize != null && !string.IsNullOrEmpty(filter.PairCallerSize.First.ToString()) &&
                !string.IsNullOrEmpty(filter.PairCallerSize.Second.ToString()))
            {
                sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_callerSize",
                    filter.PairCallerSize.First + "," + filter.PairCallerSize.Second);
                DBManager.GetData(sql, Global.ConnectionString);
            }

            if (filter.PairAlertPosition != null && !string.IsNullOrEmpty(filter.PairAlertPosition.First.ToString()) &&
                !string.IsNullOrEmpty(filter.PairAlertPosition.Second.ToString()))
            {
                sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_alertPosition",
                    filter.PairAlertPosition.First + "," + filter.PairAlertPosition.Second);
                DBManager.GetData(sql, Global.ConnectionString);
            }

            if (filter.PairAlertSize != null && !string.IsNullOrEmpty(filter.PairAlertSize.First.ToString()) &&
                !string.IsNullOrEmpty(filter.PairAlertSize.Second.ToString()))
            {
                sql = string.Format(@"INSERT INTO vwНастройки(КодНастройкиКлиента, Параметр, Значение)
VALUES({0}, '{1}', '{2}')", filter.Clid, "tc_alertSize",
                    filter.PairAlertSize.First + "," + filter.PairAlertSize.Second);
                DBManager.GetData(sql, Global.ConnectionString);
            }
        }

        /// <summary>
        ///     Получение настроек пользователя
        /// </summary>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public Filter GetSettings(Filter filter)
        {
            var sql = string.Format(
                "Select * from vwНастройки Where [КодНастройкиКлиента] = {0} and [Параметр] Like 'TC_%'", filter.Clid);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            if (dt == null || dt.Rows.Count == 0) return filter;
            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row.ItemArray[2].ToString())) continue;
                if (row.ItemArray[1].ToString() == "tc_emplrowsperpage")
                    filter.RowsPerPage = Convert.ToInt32(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_detailrows")
                    filter.RowsPerPageDetails = Convert.ToInt32(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_detailnoempty")
                    filter.IsDetailNoEmpty = Convert.ToBoolean(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_employeeprimary")
                    filter.IsEmployeePrimary = Convert.ToBoolean(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_filterteavaible")
                    filter.TimeExitAvaible = Convert.ToBoolean(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_filterte")
                    filter.TimeExit = row.ItemArray[2].ToString();
                if (row.ItemArray[1].ToString() == "tc_filteremplavaible")
                    filter.EmplAvaible = Convert.ToBoolean(row.ItemArray[2]);
                if (row.ItemArray[1].ToString() == "tc_filtersubempl")
                    filter.IsSubEmployee = Convert.ToBoolean(row.ItemArray[2]);

                if (row.ItemArray[1].ToString() == "tc_callerPosition")
                {
                    var arr = row.ItemArray[2].ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2) filter.PairCallerPosition = new Pair(arr[0], arr[1]);
                }

                if (row.ItemArray[1].ToString() == "tc_callerSize")
                {
                    var arr = row.ItemArray[2].ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2) filter.PairCallerSize = new Pair(arr[0], arr[1]);
                }

                if (row.ItemArray[1].ToString() == "tc_alertPosition")
                {
                    var arr = row.ItemArray[2].ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2) filter.PairAlertPosition = new Pair(arr[0], arr[1]);
                }

                if (row.ItemArray[1].ToString() == "tc_alertSize")
                {
                    var arr = row.ItemArray[2].ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2) filter.PairAlertSize = new Pair(arr[0], arr[1]);
                }

                if (row.ItemArray[1].ToString() == "tc_filteremployee" &&
                    !row.ItemArray[2].ToString().Contains("<?xml version=\"1.0\"?>"))
                    filter.PersonItems = row.ItemArray[2].ToString();
                if (row.ItemArray[1].ToString() == "tc_filtercompany" &&
                    !row.ItemArray[2].ToString().Contains("<?xml version=\"1.0\"?>"))
                    filter.CompanyItems = row.ItemArray[2].ToString();
                if (row.ItemArray[1].ToString() == "tc_filterpost" &&
                    !row.ItemArray[2].ToString().Contains("<?xml version=\"1.0\"?>"))
                    filter.PositionItems = row.ItemArray[2].ToString();
                if (row.ItemArray[1].ToString() == "tc_filtersubdivision" &&
                    !row.ItemArray[2].ToString().Contains("<?xml version=\"1.0\"?>"))
                    filter.SubdivisionItems = row.ItemArray[2].ToString();

                if (!row.ItemArray[2].ToString().Contains("<?xml version=\"1.0\"?>")) continue;
                switch (row.ItemArray[1].ToString())
                {
                    case "tc_filteremployee"
                        : //filter.PersonItems = (List<Item>)Deserialize(row.ItemArray[2].ToString(), typeof(List<Item>));
                        break;
                    case "tc_filtercompany"
                        : //filter.CompanyItems = (List<Item>)Deserialize(row.ItemArray[2].ToString(), typeof(List<Item>));
                        break;
                    case "tc_filterpost"
                        : //filter.PositionItems = (List<Item>)Deserialize(row.ItemArray[2].ToString(), typeof(List<Item>));
                        break;
                    case "tc_filtersubdivision"
                        : //filter.SubdivisionItems = (List<Item>)Deserialize(row.ItemArray[2].ToString(), typeof(List<Item>));
                        break;
                    case "tc_emplperiodinfo":
                        filter.PeriodInfo = (PeriodInfo) Deserialize(row.ItemArray[2].ToString(), typeof(PeriodInfo));
                        break;
                }
            }

            return filter;
        }

        /// <summary>
        ///     Получение кода сотрудника по коду лица
        /// </summary>
        /// <param name="id">Код лица</param>
        /// <returns>код сотрудника</returns>
        /// <exception cref="DetailedException">SQL ошибка</exception>
        public string GetPersonIdByEmployeeId(string id)
        {
            var sql = string.Format(@"SELECT КодЛица FROM [Инвентаризация].[dbo].[Сотрудники] (nolock) 
            WHERE КодСотрудника = {0}", id);
            var dt = DBManager.GetData(sql, Global.ConnectionString);
            if (dt == null || dt.Rows.Count == 0) return "";
            return dt.Rows[0][0].ToString();
        }

        private string Serialize(Filter filter, Type t, string type)
        {
            string product;
            using (var stream = new MemoryStream())
            {
                var bf = new XmlSerializer(t);
                switch (type)
                {
                    case "CardPerson":
                        bf.Serialize(stream, filter.PersonItems);
                        break;
                    case "Company":
                        bf.Serialize(stream, filter.CompanyItems);
                        break;
                    case "Position":
                        bf.Serialize(stream, filter.PositionItems);
                        break;
                    case "Subdivision":
                        bf.Serialize(stream, filter.SubdivisionItems);
                        break;
                    case "PeriodInfo":
                        bf.Serialize(stream, filter.PeriodInfo);
                        break;
                }

                stream.Position = 0;
                var result = stream.ToArray();
                product = Encoding.UTF8.GetString(result);
            }

            return product;
        }

        private object Deserialize(string source, Type t)
        {
            object result;
            var xs = new XmlSerializer(t);
            using (TextReader reader = new StringReader(source))
            {
                result = xs.Deserialize(reader);
            }

            return result;
        }
    }
}