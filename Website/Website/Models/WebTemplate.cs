using Starcounter;

namespace Website.Models {
    [Database]
    public class WebTemplate {
        public string Name;
        public string Html;

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
