using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor
{
    partial class SurfaceGroupsPage : Json
    {
        public void RefreshData()
        {
            this.SurfaceGroups.Clear();
            this.SurfaceGroups.Data = Db.SQL<WebGroup>("SELECT g FROM Simplified.Ring6.WebGroup g ORDER BY g.Name");
        }

        [SurfaceGroupsPage_json.SurfaceGroups]
        partial class SurfaceGroupPage : Json, IBound<WebGroup>
        {
        }

        [SurfaceGroupsPage_json.SurfaceGroups.Surfaces]
        partial class SurfacesPage : Json, IBound<WebTemplate>
        {

        }

        //void Handle(Input.Create action)
        //{

        //    //this.Surfaces.Add().Data = new WebTemplate { Name = "New surface" };
        //    this.Transaction.Commit();
        //    this.RefreshData();
        //}
    }
}
