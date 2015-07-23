using Starcounter;

namespace Website.Models {
    [Database]
    public class WebSection {
        public string Name;
        public WebTemplate Template { get; set; }

        public QueryResultRows<WebMap> Maps {
            get {
                return Db.SQL<WebMap>("SELECT wm FROM Website.Models.WebMap wm WHERE wm.Section = ?", this);
            }
        }
    }
}
