using Simplified.Ring6;
using Starcounter;

namespace WebsiteEditor
{
    partial class CatchingRulePage : Json, IBound<WebUrl>
    {
        protected override void OnData()
        {
            base.OnData();
            this.Trn.Data = this.Transaction as Transaction;
        }

        void Handle(Input.CancelChangesTrigger action)
        {
            this.Transaction.Rollback();
            this.SetBackRedirect();
        }

        void Handle(Input.SaveChangesTrigger action)
        {
            this.Transaction.Commit();
            this.SetBackRedirect();
        }

        private void SetBackRedirect()
        {
            this.RedirectUrl = $"/WebsiteEditor/surface/{this.Data.Template.Key}/catchingrules";
        }
    }
}
