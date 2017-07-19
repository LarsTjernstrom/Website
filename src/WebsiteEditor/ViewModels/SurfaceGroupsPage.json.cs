using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor
{
    partial class SurfaceGroupsPage : Json
    {
        public void RefreshData()
        {
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
        }

        void Handle(Input.Create action)
        {

            this.Surfaces.Add().Data = new WebTemplate { Name = "New surface" };
            this.Transaction.Commit();
            this.RefreshData();
        }
    }
}