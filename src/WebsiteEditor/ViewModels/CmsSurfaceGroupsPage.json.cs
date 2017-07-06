using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor.ViewModels
{
    partial class CmsSurfaceGroupsPage : Json
    {
        public void RefreshData()
        {
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            // this.Trn.Data = this.Transaction as Transaction;
        }

    }
}
