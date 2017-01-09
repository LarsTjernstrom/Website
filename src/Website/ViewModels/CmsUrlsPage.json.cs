using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsUrlsPage : Json
    {
        public void RefreshData() {
            this.Urls.Clear();
            this.Templates.Clear();
            this.Templates.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            this.Urls.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u ORDER BY u.Template.Name, u.Url");
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
            this.Urls.Add().Data = new WebUrl();
        }

        [CmsUrlsPage_json.Urls]
        partial class CmsUrlsItemPage : Json, IBound<WebUrl> {
            protected override void OnData() {
                base.OnData();
                this.TemplateKey = (this.Data != null && this.Data.Template != null) ? this.Data.Template.Key : string.Empty;
            }

            void Handle(Input.Delete Action) {
                this.ParentPage.Urls.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.TemplateKey Action) {
                if (string.IsNullOrEmpty(Action.Value)) {
                    this.Data.Template = null;
                    return;
                }

                this.Data.Template = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebTemplate;
            }

            CmsUrlsPage ParentPage {
                get {
                    return this.Parent.Parent as CmsUrlsPage;
                }
            }
        }

        [CmsUrlsPage_json.Trn]
        partial class CmsUrlsTransactionPage : Json, IBound<Transaction> {
        }
    }
}
