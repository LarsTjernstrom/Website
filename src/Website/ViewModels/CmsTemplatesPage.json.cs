using System;
using System.Linq;
using Starcounter;
using Simplified.Ring6;

namespace Website {
    partial class CmsTemplatesPage : Json
    {
        public void RefreshData() {
            this.Templates.Clear();
            this.Templates.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t ORDER BY t.Name");
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
            if (Templates.Any(val => val.Default))
            {
                this.Transaction.Commit();
            }
            else
            {
                this.Error = "At least one template should be marked as Default!";
            }
        }

        void Handle(Input.Create Action) {
            this.Templates.Add().Data = new WebTemplate();
        }

        [CmsTemplatesPage_json.Templates]
        partial class CmsTemplatesItemPage : Json, IBound<WebTemplate> {
            void Handle(Input.Delete Action) {
                this.ParentPage.Templates.Remove(this);
                this.Data.Delete();
            }

            CmsTemplatesPage ParentPage {
                get {
                    return this.Parent.Parent as CmsTemplatesPage;
                }
            }
        }

        [CmsTemplatesPage_json.Trn]
        partial class CmsTemplatesTransactionPage : Json, IBound<Transaction> {
        }
    }
}
