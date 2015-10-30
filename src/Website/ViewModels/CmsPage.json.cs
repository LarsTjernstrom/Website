using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsPage : Page {
        public void RefreshData() {
            this.Templates.Clear();
            this.Templates.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
        }
    }
}
