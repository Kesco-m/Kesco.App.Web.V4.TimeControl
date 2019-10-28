using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.UI;
using Kesco.App.Web.TimeControl.Entities;
using Kesco.Lib.Web.Controls.V4.Globals;
using Page = Kesco.Lib.Web.Controls.V4.Common.Page;

namespace Kesco.App.Web.TimeControl.Common
{
    /// <summary>
    ///     Базовая страница
    ///     Наследуется от V4.Page
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        ///     Фильтр
        /// </summary>
        public Filter Filter = new Filter();

        /// <summary>
        ///     Путь к странице
        /// </summary>
        public string Path;

        /// <summary>
        ///     Признак в чью пользу считать наружения режима прохода сотрудников
        /// </summary>
        public bool PrimaryEmployeeCalc;

        /// <summary>
        ///     Таймзона клиента
        /// </summary>
        public string Tz;

        protected string WrResponse;

        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Принадлежность к русской культуре
        /// </summary>
        public bool IsRusLocal => CurrentUser.Language.ToLower() == "ru";


        protected void DoMakeCall(string number, string inter, string phone)
        {
            JS.Write("ConfirmPhone.render('{0}','{1}', '{2}', '{3}');", number, inter, phone, GlobalBase.Caller);
        }

        /// <summary>
        ///     Инициализирует объект <see cref="T:System.Web.UI.HtmlTextWriter" /> и вызывает дочерние элементы управления
        ///     страницы <see cref="T:System.Web.UI.Page" /> для отображения.
        /// </summary>
        /// <param name="w"><see cref="T:System.Web.UI.HtmlTextWriter" />, получающий содержимое страницы.</param>
        protected override void Render(HtmlTextWriter w)
        {
            if (!string.IsNullOrEmpty(WrResponse))
            {
                w.Write(WrResponse);
                return;
            }

            base.Render(w);
        }

        /// <summary>
        ///     Загрузка базового класса страницы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //HelpUrl = "Forms/hlp/help.htm";
            //HelpUrl = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath + "/Forms/hlp/help.htm";
            if (!V4IsPostBack)
            {
                Path = V4Request == null ? Request.Path : V4Request.Path;
                var path = Request.Url.Scheme + "://" + Request.Url.Host + Request.ApplicationPath + "/Common/";
                RegisterScript("TimeControl",
                    string.Format("<script src='{0}Kesco.TimeControl.js' type='text/javascript'></script>", path));
                RegisterCss(string.Format("{0}Kesco.TimeControl.css", path));
            }

            base.OnLoad(e);
        }

        
        /// <summary>
        ///     Парсинг строки в формат DateTime
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public DateTime Str2DateTime(string val)
        {
            if (val.IndexOf(":", StringComparison.Ordinal) > 0 && val.Length == 8)
                return DateTime.Parse(DateTime.MinValue.ToString("yyyy.MM.dd") + " " + val);
            return DateTime.ParseExact(val.PadRight(14, '0'), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        protected string GetTimeDivID()
        {
            return "utctime" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        protected string GetTimeDivID2()
        {
            return "utctimeForce";
        }
    }
}