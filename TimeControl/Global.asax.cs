using System;
using System.Configuration;
using Kesco.Lib.Web.Controls.V4.Globals;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.TimeControl
{
    /// <summary>
    /// Глобальный класс приложения
    /// </summary>
    public class Global : GlobalBase
    {
        /// <summary>
        /// Путь к картинкам
        /// </summary>
        public static string PathImg = "/Styles/Kesco.V4/IMG/";

        /// <summary>
        /// Путь к картинкам (папка STYLES) 
        /// </summary>
        public static string PathPic = "/Styles/";

        /// <summary>
        /// Путь к скриптам 
        /// </summary>
        public static string PathJs = "/Styles/Kesco.V4/JS/";

        /// <summary>
        /// Путь к стилям 
        /// </summary>
        public static string PathCss = "/Styles/Kesco.V4/CSS/";

        /// <summary>
        /// ConnectionString из web.config
        /// </summary>
        public static string ConnectionString =Config.DS_user;

        /// <summary>
        /// Путь к фото из web.config
        /// </summary>
        public static string UserPhoto = Config.user_photo;

        /// <summary>
        /// Путь к Styles из web.config
        /// </summary>
        public static string Styles = Config.styles;

      
    }
}