using Starcounter;
using Simplified.Ring5;

namespace WebsiteEditor
{
    internal class CommitHooks
    {
        public void Register()
        {
            Hook<SystemUserSession>.CommitInsert += (s, a) =>
            {
                this.RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitDelete += (s, a) =>
            {
                this.RefreshSignInState();
            };
        }

        protected void RefreshSignInState()
        {
            var page = Session.Current.Data as MasterPage;

            page?.RefreshCurrentPage();
        }
    }
}
