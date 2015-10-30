using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsPage : Page {
        public void RefreshData() {
            this.Templates.Clear();
            this.Sections.Clear();

            this.Templates.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            this.Sections.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s ORDER BY s.Template.Name, s.Name");
        }

        [CmsPage_json.Sections]
        partial class CmsSectionPage : Json, IBound<WebSection> {
            protected override void OnData() {
                base.OnData();

                this.TemplateKey = this.Data != null && this.Data.Template != null 
                    ? this.Data.Template.Key : string.Empty;
            }
        }
    }
}
