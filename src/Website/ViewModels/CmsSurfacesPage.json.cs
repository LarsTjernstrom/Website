using System;
using System.Linq;
using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsSurfacesPage : Json
    {
        public void RefreshData() {
            this.Surfaces.Clear();
            this.Surfaces.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.Restore Action)
        {
            this.Transaction.Rollback();

            DataHelper helper = new DataHelper();
            helper.ClearData();
            helper.GenerateData();

            this.RefreshData();
            this.Error = "";
        }

        void Handle(Input.CancelChanges Action) {
            this.Transaction.Rollback();
            this.RefreshData();
            this.Error = null;
        }

        void Handle(Input.SaveChanges Action) {
            this.Error = null;
            if (Surfaces.Any(val => val.Default))
            {
                this.Transaction.Commit();
            }
            else
            {
                this.Error = "At least one surface should be marked as Default!";
            }
        }

        void Handle(Input.Create Action) {
            this.Surfaces.Add().Data = new WebTemplate();
        }

        [CmsSurfacesPage_json.Surfaces]
        partial class CmsSurfacesItemPage : Json, IBound<WebTemplate> {
            void Handle(Input.Delete Action) {
                this.ParentPage.Surfaces.Remove(this);
                this.Data.Delete();
            }

            CmsSurfacesPage ParentPage {
                get {
                    return this.Parent.Parent as CmsSurfacesPage;
                }
            }
        }

        [CmsSurfacesPage_json.Trn]
        partial class CmsSurfacesTransactionPage : Json, IBound<Transaction> {
        }
    }
}
