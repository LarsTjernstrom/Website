using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsMapsPage : Page {
        public void RefreshData() {
            this.Sections.Clear();
            this.Urls.Clear();
            this.Maps.Clear();
            this.Urls.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u ORDER BY u.Template.Name, u.Url");
            this.Sections.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s ORDER BY s.Template.Name, s.Name");
            this.Maps.Data = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m ORDER BY m.Section.Template.Name, m.Section.Name, m.Url.Url");
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChanges Action) {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        void Handle(Input.SaveChanges Action) {
            this.Transaction.Commit();
        }

        void Handle(Input.Create Action) {
            this.Maps.Add().Data = new WebMap();
        }

        [CmsMapsPage_json.Maps]
        partial class CmsMapsItemPage : Json, IBound<WebMap> {
            protected override void OnData() {
                base.OnData();

                this.SectionKey = (this.Data != null && this.Data.Section != null) ? this.Data.Section.Key : string.Empty;
                this.UrlKey = (this.Data != null && this.Data.Url != null) ? this.Data.Url.Key : string.Empty;
            }

            CmsMapsPage ParentPage {
                get {
                    return this.Parent.Parent as CmsMapsPage;
                }
            }

            void Handle(Input.Delete Action) {
                this.ParentPage.Maps.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.SectionKey Action) {
                if (string.IsNullOrEmpty(Action.Value)) {
                    this.Data.Section = null;
                } else {
                    this.Data.Section = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebSection;
                }
            }

            void Handle(Input.UrlKey Action) {
                if (string.IsNullOrEmpty(Action.Value)) {
                    this.Data.Url = null;
                } else {
                    this.Data.Url = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebUrl;
                }
            }
        }

        [CmsMapsPage_json.Sections]
        partial class CmsMapsSectionPage : Json, IBound<WebSection> {
            protected override void OnData() {
                base.OnData();

                if (this.Data == null || this.Data.Template == null) {
                    this.FullName = this.Name;
                } else {
                    this.FullName = string.Format("{0} - {1}", this.Data.Template.Name, this.Name);
                }
            }

            CmsMapsPage ParentPage {
                get {
                    return this.Parent.Parent as CmsMapsPage;
                }
            }
        }

        [CmsMapsPage_json.Urls]
        partial class CmsMapsUrlPage : Json, IBound<WebUrl> {
        }

        [CmsMapsPage_json.Trn]
        partial class CmsMapsTransactionPage : Json, IBound<Transaction> {
        }
    }
}
