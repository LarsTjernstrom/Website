using System;
using Starcounter;
using Simplified.Ring6;

namespace Website
{
    partial class CmsSurfacesPage : Json
    {
        public void RefreshData()
        {
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            //this.CanSave = !this.Transaction.IsDirty;
            //this.Trn.Data = Session.Current.Data.Transaction as Transaction;
            var a = this.Transaction == null;
            var b = Session.Current.Data.Transaction as Transaction;
            var c = this.Transaction as Transaction;
            this.CanSave = !a;
        }

        void Handle(Input.Restore Action)
        {
            this.Transaction.Rollback();

            DataHelper helper = new DataHelper();
            helper.ClearData();
            helper.GenerateData();

            this.RefreshData();
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
            this.Surfaces.Add().Data = new WebTemplate();
        }

        [CmsSurfacesPage_json.Surfaces]
        partial class CmsSurfacesItemPage : Json, IBound<WebTemplate>
        {
            void Handle(Input.Delete Action)
            {
                this.ParentPage.Surfaces.Remove(this);
                this.Data.Delete();
            }

            CmsSurfacesPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as CmsSurfacesPage;
                }
            }
        }
    }
}
