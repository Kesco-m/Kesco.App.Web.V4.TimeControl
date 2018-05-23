using System;
using System.Text.RegularExpressions;

namespace Kesco.App.Web.TimeControl.Common
{
    public class SearchTextOption
    {
        private int _checkID;
        private int _wordsCount;
        private const int MaxWordsCount = 3;
        private string _text = "";

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string RenderSqlPrepClause()
        {
            string result = "";
            var namePattern = new Regex("((?<=\")[^\"]+(?=\"))|[-'0-9А-ЯA-Z_ЁSZOAOUEAEUAEIOUCEIY?????Nn]+", RegexOptions.IgnoreCase);
            MatchCollection m = namePattern.Matches(Text);

            if (m.Count > 0 && m.Count <= MaxWordsCount)
            {
                result = @"DECLARE @S1 varchar(50), @S2 varchar(50), @S3 varchar(50)";

                for (_wordsCount = 0; _wordsCount < m.Count; _wordsCount++)
                {
                    result += String.Format(@" SET @S{0} = {1}", _wordsCount + 1, ReplaceRusLat(m[_wordsCount].Value + "%"));
                }

                if (_wordsCount == 1 && Regex.IsMatch(m[0].Value, "^\\d{1,6}$"))
                {
                    _checkID = Int32.Parse(m[0].Value);

                    result += @" DECLARE @TblTel TABLE(КодТелефонногоНомера int, КодТипаТелефонныхНомеров int, Абонент nvarchar(100), ПолныйНомер nvarchar(50), КодСотрудника int, Login varchar(50), Email varchar(50))";
                    result += @" INSERT @TblTel EXEC sp_ПоискТелефонногоНомера @НомерВнутренний = " + _checkID;
                }
            }
            return result;
        }

        public string RenderSqlWhereClause()
        {
            /* в зависимости от количества слов в поиске определяем поля для поиска.
             * 1 слово - фамилия | имя
            * 2 слова - фамилия + имя | имя + фамилия | имя + отчество
            * 3 слова - фамилия + имя + отчество | имя + отчество + фамилия
            * 4+ слов - не накладываем ограничений
            * к указанным полям добавляются соответствующие английские поля ("Иван" + " " + "Ivan") - в полученной строке ищем */
            string result = "";
            switch (_wordsCount)
            {
                case 1:
                    result = "(";
                    if (_checkID > 0)
                    {
                        result += String.Format("(T0.КодСотрудника = {0} OR T0.КодСотрудника IN (SELECT КодСотрудника FROM @TblTel X)) OR ", _checkID);
                    }
                    result += "T0.ФамилияRL LIKE @S1 OR T0.LastName LIKE @S1 OR T0.ИмяRL LIKE @S1 OR T0.FirstName LIKE @S1";
                    result += ")";
                    break;
                case 2:
                    result += @"(
 ( (T0.ФамилияRL LIKE @S1 OR T0.LastName LIKE @S1) AND (T0.ИмяRL LIKE @S2 OR T0.FirstName LIKE @S2) ) OR
 ( (T0.ИмяRL LIKE @S1 OR T0.FirstName LIKE @S1) AND (T0.ФамилияRL LIKE @S2 OR T0.LastName LIKE @S2) ) OR
 ( (T0.ИмяRL LIKE @S1 OR T0.FirstName LIKE @S1) AND (T0.ОтчествоRL LIKE @S2 OR T0.MiddleName LIKE @S2) ) )";
                    break;
                case 3:
                    result += @"(
 ( (T0.ФамилияRL LIKE @S1 OR T0.LastName LIKE @S1) AND (T0.ИмяRL LIKE @S2 OR T0.FirstName LIKE @S2) AND (T0.ОтчествоRL LIKE @S3 OR T0.MiddleName LIKE @S3) ) OR
 ( (T0.ИмяRL LIKE @S1 OR T0.FirstName LIKE @S1) AND (T0.ОтчествоRL LIKE @S2 OR T0.MiddleName LIKE @S2) AND (T0.ФамилияRL LIKE @S3 OR T0.LastName LIKE @S3) ) )";
                    break;
            }
            return result;
        }

        public string ReplaceRusLat(string s)
        {
            return String.Format("Инвентаризация.dbo.fn_ReplaceRusLat(N'{0}')", s);
        }
    }
}