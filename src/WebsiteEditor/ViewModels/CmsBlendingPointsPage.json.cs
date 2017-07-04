using Starcounter;
using Simplified.Ring6;

namespace WebsiteEditor {
    partial class CmsBlendingPointsPage : Json
    {
        public void RefreshData() {
            this.BlendingPoints.Clear();
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            this.BlendingPoints.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s ORDER BY s.Template.Name, s.Name");
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
            this.BlendingPoints.Add().Data = new WebSection();
        }

        [CmsBlendingPointsPage_json.BlendingPoints]
        partial class CmsBlendingPointsItemPage : Json, IBound<WebSection> {
            protected override void OnData() {
                base.OnData();
                this.TemplateKey = (this.Data != null && this.Data.Template != null) ? this.Data.Template.Key : string.Empty;
            }

            void Handle(Input.Delete Action) {
                this.ParentPage.BlendingPoints.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.TemplateKey Action) {
                if (string.IsNullOrEmpty(Action.Value)) {
                    this.Data.Template = null;
                    return;
                }

                this.Data.Template = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebTemplate;
            }

            CmsBlendingPointsPage ParentPage {
                get {
                    return this.Parent.Parent as CmsBlendingPointsPage;
                }
            }
        }

        [CmsBlendingPointsPage_json.Trn]
        partial class CmsBlendingPointsTransactinPage : Json, IBound<Transaction> {
        }
    }
}
