using Starcounter;
using Simplified.Ring6;

namespace WebsiteEditor
{
    partial class CatchingRulesPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }

        public void RefreshData()
        {
            this.CatchingRules.Clear();
            this.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ? ORDER BY t.Name", SurfaceKey).First;
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Template = ? ORDER BY u.Template.Name, u.Url", this.Surface.Data);
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChanges Action)
        {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        void Handle(Input.SaveChanges Action)
        {
            this.Transaction.Commit();
        }

        void Handle(Input.Create Action)
        {
            this.CatchingRules.Add().Data = new WebUrl();
        }

        [CatchingRulesPage_json.CatchingRules]
        partial class CmsCatchingRulesItemPage : Json, IBound<WebUrl>
        {
            protected override void OnData()
            {
                base.OnData();
                this.TemplateKey = (this.Data != null && this.Data.Template != null) ? this.Data.Template.Key : string.Empty;
            }

            void Handle(Input.Delete Action)
            {
                this.ParentPage.CatchingRules.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.TemplateKey Action)
            {
                if (string.IsNullOrEmpty(Action.Value))
                {
                    this.Data.Template = null;
                    return;
                }

                this.Data.Template = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebTemplate;
            }

            CatchingRulesPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as CatchingRulesPage;
                }
            }
        }

        [CatchingRulesPage_json.Trn]
        partial class CmsCatchingRulesTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
