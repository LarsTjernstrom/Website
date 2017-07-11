using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor
{
    partial class GeneralPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }
        public void RefreshData()
        {
            this.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ? ORDER BY t.Name", SurfaceKey).First;
            this.Trn.Data = this.Transaction as Transaction;
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
    }
}