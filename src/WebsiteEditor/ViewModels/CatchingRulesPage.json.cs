using System;
using System.Linq;
using Starcounter;
using Simplified.Ring6;

namespace WebsiteEditor
{
    partial class CatchingRulesPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }

        public void RefreshData()
        {
            if (string.IsNullOrEmpty(SurfaceKey))
            {
                throw new InvalidOperationException("Surface key is empty.");
            }

            this.CatchingRules.Clear();
            this.SurfaceName = this.GetCurrentSurface().Name;
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Template.Key = ? ORDER BY u.Template.Name, u.Url", this.SurfaceKey);
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChangesTrigger action)
        {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        void Handle(Input.SaveChangesTrigger action)
        {
            this.Transaction.Commit();
        }

        void Handle(Input.CreateTrigger action)
        {
            Db.Transact(() =>
            {
                var surface = this.GetCurrentSurface();
                this.CatchingRules.Add().Data = new WebUrl
                {
                    Template = surface,
                    Url = string.Empty,
                };
            });
        }

        private WebTemplate GetCurrentSurface()
        {
            return Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", this.SurfaceKey).FirstOrDefault()
                   ?? throw new Exception("The surface with specified key is not found.");
        }

        [CatchingRulesPage_json.CatchingRules]
        partial class CmsCatchingRulesItemPage : Json, IBound<WebUrl>
        {
            CatchingRulesPage ParentPage => this.Parent.Parent as CatchingRulesPage;

            void Handle(Input.DeleteTrigger action)
            {
                this.ParentPage.CatchingRules.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.EditTrigger action)
            {
                this.ParentPage.RedirectUrl = "/WebsiteEditor/catchingrule/" + this.Key;
            }
        }

        [CatchingRulesPage_json.Trn]
        partial class CmsCatchingRulesTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
