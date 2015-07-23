using Starcounter;

namespace Website.Models {
    [Database]
    public class WebContent {
        public string Name;
        public string Html;
        public string Value;

        public QueryResultRows<WebMap> Maps {
            get {
                return Db.SQL<WebMap>("SELECT wm FROM Website.Models.WebMap wm WHERE wm.Section = ?", this);
            }
        }
    }
}
