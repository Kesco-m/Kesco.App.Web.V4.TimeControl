using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using Kesco.App.Web.TimeControl.Common;
using Kesco.Lib.Entities.Persons;
using Kesco.Lib.Localization;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.TimeControl.Forms
{
    /// <summary>
    ///     Список компаний
    /// </summary>
    public partial class CompanyList : BasePage
    {
        private string _id, _lang;
        private ResourceManager _resx;


        /// <summary>
        ///     Метод Load для страницы CompanyList.aspx
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Аргументы события</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _id = Request.QueryString["id"];
            _lang = Request.QueryString["lang"];
            switch (_lang)
            {
                case "ru":
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU", false);
                    break;
                case "en":
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
                    break;
                case "et":
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("et-EE", false);
                    break;
            }

            _resx = Resources.Resx;
        }

        /// <summary>
        ///     Отрисовка списка компаний
        /// </summary>
        /// <param name="w"></param>
        protected void RenderData(TextWriter w)
        {
            if (!string.IsNullOrEmpty(_id))
            {
                List<PersonCustomer> list;
                if (Application["PersonCustomer" + _id] != null)
                {
                    list = (List<PersonCustomer>) Application["PersonCustomer" + _id];
                }
                else
                {
                    list = new ExecQuery().GetCompanyBySubdivision(_id);
                    Application["PersonCustomer" + _id] = list;
                }

                if (list.Count > 0)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (i > 0)
                            w.Write(
                                "<hr style=\"height:1px;border:none;color:lightgray;background-color:lightgray;\" />");
                        else
                            w.Write("<div style=\"width: 200px;\">");
                        w.Write(
                            "<a title=\"{0}\" data-id=\"{2}\" class=\"v4_callerControl\" caller-type=\"3\" href=\"javascript: void(0);\" onclick=\"v4_windowOpen('{1}');\">{0}</a>",
                            _lang == "ru" || string.IsNullOrEmpty(list[i].NameLat) ? list[i].Name : list[i].NameLat,
                            Config.person_form + "?id=" + list[i].Id, list[i].Id);
                    }

                    w.Write("</div>");
                }
                else
                {
                    w.Write("<div style=\"width: 100px;\">{0}.</div>", _resx.GetString("lNoData"));
                }
            }
            else
            {
                w.Write("<div style=\"width: 100px;\">{0}.</div>", _resx.GetString("lNoData"));
            }
        }
    }
}