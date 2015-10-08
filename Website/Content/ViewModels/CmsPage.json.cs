using Starcounter;

namespace Content {
    partial class CmsPage : Page {
        protected override void OnData() {
            base.OnData();
        }

        public void RefreshData() {
            this.Entries.Data = Db.SQL<ContentEntry>("SELECT e FROM Content.ContentEntry e ORDER BY e.Url");
            this.Items.Data = Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i ORDER BY i.Url");
        }

        [CmsPage_json.Entries]
        partial class CmsEntryPage : Json, IBound<ContentEntry> { 
        }

        [CmsPage_json.Items]
        partial class CmsItemPage : Json, IBound<ContentItem> {
            protected override void OnData() {
                base.OnData();
                this.Href = string.Format("/content/cms/item/{0}", this.Key);
            }
        }
    }
}
