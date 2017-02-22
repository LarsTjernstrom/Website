using Starcounter;
using Simplified.Ring6;

namespace Website
{
    partial class CmsCatchingRulesPage : Json
    {
        public void RefreshData()
        {
            this.CatchingRules.Clear();
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u ORDER BY u.Template.Name, u.Url");
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

        [CmsCatchingRulesPage_json.CatchingRules]
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

            CmsCatchingRulesPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as CmsCatchingRulesPage;
                }
            }
        }

        [CmsCatchingRulesPage_json.Trn]
        partial class CmsCatchingRulesTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
