using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Routing;
using WebsiteEditor.Helpers;

namespace WebsiteEditor.ViewModels
{
    [Url("/websiteeditor/surface/{?}/blendingpoints")]
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
            this.CatchingRules.Data = Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Template = ? ORDER BY u.Template.Name, u.Url", this.Surface.Data);
            this.BlendingPoints.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Template = ? ORDER BY s.Template.Name, s.Name", this.Surface.Data);
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChanges action)
        {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        void Handle(Input.SaveChanges action)
        {
            RemoveEmptyPinningRules();
            this.Transaction.Commit();
        }

        void Handle(Input.Create action)
        {
            this.BlendingPoints.Add().Data = new WebSection { Template = this.Surface.Data as WebTemplate };
        }

        private void RemoveEmptyPinningRules()
        {
            var emptyPinningRules = new List<PinningRulesItemPage>();
            foreach (var blendingPoint in this.BlendingPoints)
            {
                emptyPinningRules.AddRange(blendingPoint.PinningRules.Where(x => string.IsNullOrEmpty(x.ForeignUrl)));
            }

            foreach (var emptyPinningRule in emptyPinningRules)
            {
                emptyPinningRule.DeleteAction?.Invoke();
            }
        }

        [BlendingPointsPage_json.BlendingPoints.PinningRules]
        partial class PinningRulesItemPage : Json, IBound<WebMap>
        {
            public Action DeleteAction { get; set; }
            protected override void OnData()
            {
                base.OnData();

                this.SectionKey = Data?.Section != null ? this.Data.Section.Key : string.Empty;
                this.UrlKey = Data?.Url != null ? this.Data.Url.Key : string.Empty;
            }

            void Handle(Input.Delete action)
            {
                this.DeleteAction?.Invoke();
            }

            void Handle(Input.UrlKey action)
            {
                if (string.IsNullOrEmpty(action.Value))
                {
                    this.Data.Url = null;
                }
                else
                {
                    this.Data.Url = DbHelper.FromID(DbHelper.Base64DecodeObjectID(action.Value)) as WebUrl;
                }
            }
        }

        [BlendingPointsPage_json.BlendingPoints]
        partial class BlendingPointsItemPage : Json, IBound<WebSection>
        {
            private BlendingPointsPage ParentPage => this.Parent.Parent as BlendingPointsPage;

            protected override void OnData()
            {
                base.OnData();
                this.PinningRules.Data = this.Data.Maps.OrderBy(x => x.SortNumber);
                foreach (var pinningRule in PinningRules)
                {
                    pinningRule.DeleteAction = () =>
                    {
                        this.PinningRules.Remove(pinningRule);
                        pinningRule.Data.Delete();
                    };
                }

                this.TemplateKey = Data?.Template != null ? this.Data.Template.Key : string.Empty;
            }

            void Handle(Input.Delete action)
            {
                this.ParentPage.BlendingPoints.Remove(this);
                this.Data.Delete();
            }

            void Handle(Input.AddPinningRule action)
            {
                var newPinningRule = new WebMap { Section = this.Data };
                var pinningRulesItemPage = this.PinningRules.Add();
                pinningRulesItemPage.DeleteAction = () =>
                {
                    this.PinningRules.Remove(pinningRulesItemPage);
                    pinningRulesItemPage.Data.Delete();
                };

                pinningRulesItemPage.Data = newPinningRule;
            }

            void Handle(Input.TemplateKey action)
            {
                if (string.IsNullOrEmpty(action.Value))
                {
                    this.Data.Template = null;
                    return;
                }

                this.Data.Template = DbHelper.FromID(DbHelper.Base64DecodeObjectID(action.Value)) as WebTemplate;
            }
        }

        [BlendingPointsPage_json.Trn]
        partial class BlendingPointsTransactinPage : Json, IBound<Transaction>
        {
        }
    }
}
