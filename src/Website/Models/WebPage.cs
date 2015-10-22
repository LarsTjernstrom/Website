using Starcounter;

namespace Website.Models {
    [Database]
    public class WebPage {
        public string Name;
        public WebTemplate Template { get; set; }

        public QueryResultRows<WebUrl> Urls {
            get {
                return Db.SQL<WebUrl>("SELECT wu FROM Websites.Models.WebUrl wu WHERE wu.WebPage = ?", this);
            }
        }
    }
}
