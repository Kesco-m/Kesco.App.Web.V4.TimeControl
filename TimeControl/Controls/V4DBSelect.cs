using Kesco.Lib.Web.Controls.V4;
using System.Collections;
using Kesco.App.Web.TimeControl.Common;
using System;
using System.Linq;
using Kesco.App.Web.TimeControl.Controls.DSO;
using System.Configuration;
using System.Web;

namespace Kesco.App.Web.TimeControl.Controls
{
    /// <summary>
    /// Тип запроса
    /// </summary>
    public enum TypeQuery
    {
        /// <summary>
        /// Сотрудник
        /// </summary>
        EmplName = 0,
        /// <summary>
        /// Компания
        /// </summary>
        Company = 1,
        /// <summary>
        /// Должность
        /// </summary>
        Position = 2,
        /// <summary>
        /// Подразделение
        /// </summary>
        Subdivision = 3
    }

    /// <summary>
    /// Имплементация контрола SelectAlt
    /// </summary>
    public class V4DBSelect : Select
    {
        /// <summary>
        /// Код запроса
        /// </summary>
        public int IdQuery { get; set; }
        /// <summary>
        /// Тип запроса
        /// </summary>
        public TypeQuery TypeQuery
        {
            get { return (TypeQuery)IdQuery; }
            set { IdQuery = (int)value; }
        }
        /// <summary>
        /// Параметры поиска
        /// </summary>
        public string ParamSearch { get; set; }
        /// <summary>
        /// Ограничения поиска
        /// </summary>
        public object DSO { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        public V4DBSelect()
        {
            //V3FillPopup = FillSelect;
            //V3GetObjectByID = GetObjectById;
            //V3AdvancedSearch = AdvancedSearch;
        }

        /// <summary>
        /// Заполнение списка
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        //public IEnumerable FillSelect(string searchParam)
        //{
        //    switch (TypeQuery)
        //    {
        //        case TypeQuery.EmplName:
        //            return new ExecQuery().GetCardPerson(searchParam, MaxItemsInQuery, ((DSOEmplName)DSO).SQLGetClause());
        //        case TypeQuery.Company:
        //            return new ExecQuery().GetCompany(searchParam, MaxItemsInQuery, ((DSOCompany)DSO).SQLGetClause());
        //        case TypeQuery.Position:
        //            return new ExecQuery().GetPosition(searchParam, MaxItemsInQuery, ((DSOPosition)DSO).SQLGetClause());
        //        case TypeQuery.Subdivision:
        //            return new ExecQuery().GetSubdivision(searchParam, MaxItemsInQuery, ((DSOSubdivision)DSO).SQLGetClause());
        //    }
        //    return null;
        //}   

        /// <summary>
        /// Получение элемента списка по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public object GetObjectById(string id)
        //{
        //    switch (TypeQuery)
        //    {
        //        case TypeQuery.EmplName:
        //            return new ExecQuery().GetCardPersonById(id);
        //        case TypeQuery.Company:
        //            return new ExecQuery().GetCompanyById(id);
        //        case TypeQuery.Position:
        //            return new ExecQuery().GetPositionById(id);
        //        case TypeQuery.Subdivision:
        //            return new ExecQuery().GetSubdivisionById(id);
        //    }
        //    return null;
        //}

        /// <summary>
        /// Расширенный поиск
        /// </summary>
        public virtual void AdvancedSearch()
        {
            switch (TypeQuery)
            {
                case TypeQuery.EmplName:
                    AdvancedSearchPerson(ValueText);break;
                case TypeQuery.Company:
                    AdvancedSearchCompany(ValueText);break;
            }
        }

        private void AdvancedSearchPerson(string search)
        {
            string callbackUrl = GetCallbackUrl();
            //var personSearchUrl = ConfigurationManager.AppSettings["alf32"] + "mvc/persons/search.aspx?return=1&clid=0&mvc=1&HideOldVer=true&search=" + search + "&control=c&callbackKey=c1";
            var personSearchUrl = ConfigurationManager.AppSettings["URI_user_search"] + "?return=2&mvc=1&search=" + HttpUtility.UrlEncode(search);
            var url = personSearchUrl + "&callbackUrl=" + HttpUtility.UrlEncode(callbackUrl);
            JS.Write("v3s_openForm('{0}','Поиск лиц');", url);
        }

        private void AdvancedSearchCompany(string search)
        {
            string callbackUrl = GetCallbackUrl();
            var personSearchUrl = ConfigurationManager.AppSettings["alf32"] + "mvc/persons/search.aspx?return=1&clid=0&mvc=1&HideOldVer=true&PersonType=1&search=" + HttpUtility.UrlEncode(search) + 
                "&control=c&callbackKey=c1";
            var url = personSearchUrl + "&callbackUrl=" + HttpUtility.UrlEncode(callbackUrl);
            JS.Write("v3s_openForm('{0}','Поиск лиц');", url);
        }

        private string GetCallbackUrl()
        {
            string callbackUrl = "";
            if (V4Page.V4Request.UrlReferrer != null)
            {
                string[] path = V4Page.V4Request.UrlReferrer.OriginalString.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries);
                string query = "";
                if (path.Length > 1)
                {
                    string keys = "";
                    string[] qs = path[1].Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in qs)
                    {
                        string[] arrKeys = s.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrKeys.Length > 0)
                            keys += arrKeys[0] + "&";
                    }
                    qs = keys.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    query = qs.Where(s => s != "type" && s != "currentperson" && s != "docDir").Aggregate("?", (current, s) => current + (s + "=" + V4Page.V4Request.QueryString[s] + "&"));
                }
                else
                {
                    query = "?";
                }
                callbackUrl = path[0] + query + "idp=" + V4Page.IDPage + "&ctrl=" + ID + "&cmd=callback";
            }
            return callbackUrl;
        }
    }
}