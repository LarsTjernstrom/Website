using System;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Routing;
using WebsiteEditor.Helpers;

namespace WebsiteEditor.ViewModels
{
    [Url("/websiteeditor/surface/{?}/general")]
    partial class GeneralPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }
        public void RefreshData()
        {
            if (string.IsNullOrEmpty(SurfaceKey))
            {
                throw new InvalidOperationException("Surface key is empty.");
            }

            this.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ? ORDER BY t.Name", SurfaceKey).First;
            this.BlendingPoints.Data = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Template = ? ORDER BY s.Template.Name, s.Name", this.Surface.Data);
            this.SurfaceGroups.Data = Db.SQL<WebTemplateGroup>("SELECT g FROM Simplified.Ring6.WebTemplateGroup g ORDER BY g.Name");

            this.CurrentGroupKey = this.Surface.Data.WebTemplateGroup.Key;
            this.DefaultBlendingPointKey = this.BlendingPoints.FirstOrDefault(x => x.Default)?.Key;
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.DefaultBlendingPointKey action)
        {
            var currentDefaultBlendingPoint =
                Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Key = ?", action.OldValue).First;
            if (currentDefaultBlendingPoint != null)
            {
                currentDefaultBlendingPoint.Default = false;
            }

            var newDefaultBlendingPoint = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Key = ?", action.Value).First;
            if (newDefaultBlendingPoint != null)
            {
                newDefaultBlendingPoint.Default = true;
            }
        }

        void Handle(Input.CurrentGroupKey action)
        {
            var newGroup = Db.SQL<WebTemplateGroup>("SELECT g FROM Simplified.Ring6.WebTemplateGroup g WHERE g.Key = ?", action.Value).First;
            this.Surface.Data.WebTemplateGroup = newGroup;
        }

        void Handle(Input.Delete action)
        {
            this.Surface.Data.Delete();
            this.Transaction.Commit();
            this.RedirectUrl = "/WebsiteEditor/surfacegroups";
        }

        void Handle(Input.Save action)
        {
            this.Transaction.Commit();
        }

        [GeneralPage_json.Trn]
        partial class GeneralTransactionPage : Json, IBound<Transaction>
        {
        }

        [GeneralPage_json.Surface]
        partial class GeneralPageSurface : Json, IBound<WebTemplate>
        {
        }
    }
}