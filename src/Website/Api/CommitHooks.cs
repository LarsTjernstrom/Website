using System;
using Starcounter;
using Simplified.Ring5;

namespace Website {
    internal class CommitHooks {
        public void Register() {
            Hook<SystemUserSession>.CommitInsert += (s, a) => {
                this.RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitDelete += (s, a) => {
                this.RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitUpdate += (s, a) => {
                this.RefreshSignInState();
            };
        }

        protected void RefreshSignInState()
        {
            var page = Session.Current.Data as StandalonePage;

            if (page == null) {
                return;
            }

            page.RefreshCurrentPage();
        }
    }
}
