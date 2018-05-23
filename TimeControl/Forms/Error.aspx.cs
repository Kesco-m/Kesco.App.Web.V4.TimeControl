using System;
using System.Data.SqlClient;
using System.IO;
using System.Resources;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Comet;

namespace Kesco.App.Web.TimeControl.Forms
{
    /// <summary>
    /// Отображение ошибок
    /// </summary>
    public partial class Error : System.Web.UI.Page
    {
        /// <summary>
        /// Локализация
        /// </summary>
        public ResourceManager Resx = Lib.Localization.Resources.Resx;

        /// <summary>
        /// Рендеринг ошибки
        /// </summary>
        /// <param name="w"></param>
        protected virtual void RenderError(TextWriter w)
        {
            if (Application["Error"] != null)
            {
                if (Application["Error"] is Exception)
                {
                    var ex = (Exception) Application["Error"];
                    w.Write(ex.Message + "<br /><br /><br />" + ex.StackTrace.Replace("\r\n", "<br />"));
                }
                else if (Application["Error"] is SqlException || Application["Error"] is LogicalException)
                {
                    var ex = (SqlException)Application["Error"];
                    w.Write(ex.Message + "<br /><br /><br />" + ex.StackTrace.Replace("\r\n", "<br />"));
                }
                Application["Error"] = null;
            }
        }
    }
}