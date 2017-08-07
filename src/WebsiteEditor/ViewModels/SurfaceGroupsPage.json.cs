using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor.ViewModels
{
    partial class SurfaceGroupsPage : Json
    {
        public void RefreshData()
        {
            this.SurfaceGroups.Clear();
            this.SurfaceGroups.Data = Db.SQL<WebTemplateGroup>("SELECT g FROM Simplified.Ring6.WebTemplateGroup g ORDER BY g.Name");
            this.UnassignedSurfaces.Data = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt  WHERE wt.WebTemplateGroup IS NULL ORDER BY wt.Name");
        }

        void Handle(Input.CreateGroup action)
        {
            Db.Transact(() =>
            {
                this.SurfaceGroups.Add().Data = new WebTemplateGroup { Name = "New group" };
            });

            this.RefreshData();
        }

        void Handle(Input.CreateUnassignedSurface action)
        {
            Db.Transact(() =>
            {
                this.UnassignedSurfaces.Add().Data = new WebTemplate
                {
                    Name = "New surface"
                };
            });

            RefreshData();
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

            void Handle(Input.Name action)
            {
                this.Name = action.Value;
                this.Transaction.Commit();
            }

            void Handle(Input.DeleteGroup action)
            {
                var surfaces =
                    Db.SQL<WebTemplate>(
                        "SELECT w FROM Simplified.Ring6.WebTemplate w WHERE w.WebTemplateGroup = ?", this.Data);

                // Make surfaces unassigned.
                foreach (var surface in surfaces)
                {
                    (surface as WebTemplate).WebTemplateGroup = null;
                }

                this.Data.Delete();
                this.Transaction.Commit();
                this.ParentPage.RefreshData();
            }
        }

        [SurfaceGroupsPage_json.SurfaceGroups.Surfaces]
        partial class SurfacesPage : Json, IBound<WebTemplate>
        {
        }
    }
}