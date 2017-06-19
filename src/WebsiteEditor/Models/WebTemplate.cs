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

        public QueryResultRows<WebUrl> Pages {
            get {
                return Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Template = ?", this);
            }
        }
    }
}
