using Starcounter;
using Simplified.Ring6;
using WebsiteEditor.Helpers;

namespace WebsiteEditor
{
    partial class PinningRulesPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }
        public void RefreshData()
        {
            this.BlendingPoints.Clear();
            this.CatchingRules.Clear();
            this.PinningRules.Clear();
            var surface = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", SurfaceKey).First;
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Template = ? ORDER BY u.Template.Name, u.Url", surface);
            this.BlendingPoints.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Template = ? ORDER BY s.Template.Name, s.Name", surface);
            this.PinningRules.Data = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.Section.Template = ?  ORDER BY m.Section.Template.Name, m.Section.Name, m.Url.Url", surface);
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
            this.PinningRules.Add().Data = new WebMap();
        }

        [PinningRulesPage_json.PinningRules]
        partial class CmsPinningRulesItemPage : Json, IBound<WebMap>
        {
            protected override void OnData()
            {
                base.OnData();

                this.SectionKey = (this.Data != null && this.Data.Section != null) ? this.Data.Section.Key : string.Empty;
                this.UrlKey = (this.Data != null && this.Data.Url != null) ? this.Data.Url.Key : string.Empty;
            }

            PinningRulesPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as PinningRulesPage;
                }
            }

            void Handle(Input.Delete Action)
            {
                this.ParentPage.PinningRules.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.SectionKey Action)
            {
                if (string.IsNullOrEmpty(Action.Value))
                {
                    this.Data.Section = null;
                }
                else
                {
                    this.Data.Section = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebSection;
                }
            }

            void Handle(Input.UrlKey Action)
            {
                if (string.IsNullOrEmpty(Action.Value))
                {
                    this.Data.Url = null;
                }
                else
                {
                    this.Data.Url = DbHelper.FromID(DbHelper.Base64DecodeObjectID(Action.Value)) as WebUrl;
                }
            }
        }

        [PinningRulesPage_json.BlendingPoints]
        partial class CmsPinningRulesBlendingPointPage : Json, IBound<WebSection>
        {
            protected override void OnData()
            {
                base.OnData();

                if (this.Data == null || this.Data.Template == null)
                {
                    this.FullName = this.Name;
                }
                else
                {
                    this.FullName = string.Format("{0} - {1}", this.Data.Template.Name, this.Name);
                }
            }

            PinningRulesPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as PinningRulesPage;
                }
            }
        }

        [PinningRulesPage_json.CatchingRules]
        partial class CmsPinningRulesCatchingRulePage : Json, IBound<WebUrl>
        {
        }

        [PinningRulesPage_json.Trn]
        partial class CmsPinningRulesTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
