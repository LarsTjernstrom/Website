using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor
{
    partial class SurfaceGroupsPage : Json
    {
        public void RefreshData()
        {
            this.SurfaceGroups.Clear();
            this.SurfaceGroups.Data = Db.SQL<WebTemplateGroup>("SELECT g FROM Simplified.Ring6.WebTemplateGroup g ORDER BY g.Name");
        }

        void Handle(Input.CreateGroup action)
        {
            Db.Transact(() =>
            {
                this.SurfaceGroups.Add().Data = new WebTemplateGroup { Name = "New group" };
            });
        }

        [SurfaceGroupsPage_json.SurfaceGroups]
        partial class SurfaceGroupPage : Json, IBound<WebTemplateGroup>
        {
            SurfaceGroupsPage ParentPage => this.Parent.Parent as SurfaceGroupsPage;

            protected override void OnData()
            {
                base.OnData();
                this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.WebTemplateGroup.Name = ? ORDER BY wt.Name", this.Name);
            }

            void Handle(Input.CreateSurface action)
            {
                Db.Transact(() =>
                {
                    this.Surfaces.Add().Data = new WebTemplate
                    {
                        Name = "New surface",
                        WebTemplateGroup = this.Data
                    };
                });

                ParentPage.RefreshData();
            }
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
