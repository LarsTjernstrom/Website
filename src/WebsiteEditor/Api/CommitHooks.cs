using Simplified.Ring5;
using Starcounter;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor.Api
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
            if (Session.Current == null)
            {
                return;
            }

            Session.ScheduleTask(Session.Current.SessionId, (session, id) =>
            {
                if (session == null) // Session have timed out after the task was scheduled 
                {
                    return;
                }

                var page = session.Store[nameof(MasterPage)] as MasterPage;

                page?.Transaction.Scope(() => page.RefreshCurrentPage());
                session.CalculatePatchAndPushOnWebSocket();
            });
        }
    }
}
