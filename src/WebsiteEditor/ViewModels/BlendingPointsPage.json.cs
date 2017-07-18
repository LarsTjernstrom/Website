using System;
using Starcounter;
using Simplified.Ring6;

namespace WebsiteEditor
{
    partial class BlendingPointsPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }

        public void RefreshData()
        {
            if (string.IsNullOrEmpty(SurfaceKey))
            {
                throw new InvalidOperationException("Surface key is empty.");
            }

            this.BlendingPoints.Clear();
            this.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ? ORDER BY t.Name", SurfaceKey).First;
            this.BlendingPoints.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Template = ? ORDER BY s.Template.Name, s.Name", this.Surface.Data);
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
            this.BlendingPoints.Add().Data = new WebSection();
        }

        [BlendingPointsPage_json.BlendingPoints]
        partial class CmsBlendingPointsItemPage : Json, IBound<WebSection>
        {
            protected override void OnData()
            {
                base.OnData();
                this.TemplateKey = (this.Data != null && this.Data.Template != null) ? this.Data.Template.Key : string.Empty;
            }

            void Handle(Input.Delete Action)
            {
                this.ParentPage.BlendingPoints.Remove(this);
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

            BlendingPointsPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as BlendingPointsPage;
                }
            }
        }

        [BlendingPointsPage_json.Trn]
        partial class CmsBlendingPointsTransactinPage : Json, IBound<Transaction>
        {
        }
    }
}
