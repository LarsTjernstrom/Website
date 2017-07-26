using System;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    // To use it should be called "/WebsiteProvider_AcceptanceHelperOne/SetupPinningRulesMappingTests" first
    partial class PinningPage : Json
    {
        public static PinningPage Create()
        {
            var page = new PinningPage();
            page.RefreshData();
            return page;
        }

        private void RefreshData()
        {
            this.Trn.Data = this.Transaction as Transaction;
            this.EditPinningRuleTrigger = 0;
            this.DeletePinningRuleTrigger = 0;
            this.DeleteBlendingPointTrigger = 0;
        }

        private void Handle(Input.SaveChangesTrigger action)
        {
            this.Transaction.Commit();
        }

        private void Handle(Input.CancelChangesTrigger action)
        {
            this.Transaction.Rollback();
            this.RefreshData();
        }

        private void Handle(Input.DeletePinningRuleTrigger action)
        {
            if (action.Value == 1)
            {
                var rule = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.ForeignUrl = ?", "/WebsiteProvider_AcceptanceHelperTwo/pin1").FirstOrDefault();
                if (rule == null)
                {
                    throw new Exception("The pinning rule not found");
                }
                rule.Delete();
            }
        }

        private void Handle(Input.EditPinningRuleTrigger action)
        {
            if (action.Value == 1)
            {
                var rule = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.ForeignUrl = ?", "/WebsiteProvider_AcceptanceHelperTwo/pin2").FirstOrDefault();
                if (rule == null)
                {
                    throw new Exception("The pinning rule not found");
                }
                rule.ForeignUrl = "/WebsiteProvider_AcceptanceHelperTwo/pin4";
            }
        }

        private void Handle(Input.DeleteBlendingPointTrigger action)
        {
            if (action.Value == 1)
            {
                var point = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Name = ?", "TopBar").FirstOrDefault();
                if (point == null)
                {
                    throw new Exception("The blending point not found");
                }
                point.Delete();
            }
        }

        [PinningPage_json.Trn]
        partial class PinningTransactionPage : Json, IBound<Transaction>
        {
        }
    }
}
