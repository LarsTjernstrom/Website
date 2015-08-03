using Starcounter;

namespace Website.Models {
    [Database]
    public class WebTemplate {
        public bool Default;
        public string Name;

        /// <summary>
        /// Raw html
        /// </summary>
        public string Html;

        /// <summary>
        /// Path to html file
        /// </summary>
        public string Content;

        public QueryResultRows<WebSection> Sections {
            get {
                return Db.SQL<WebSection>("SELECT ws FROM Website.Models.WebSection ws WHERE ws.Template = ?", this);
            }
        }

        public QueryResultRows<WebPage> Pages {
            get {
                return Db.SQL<WebPage>("SELECT wp FROM Website.Models.WebPage wp WHERE wp.Template = ?", this);
            }
        }
    }
}
