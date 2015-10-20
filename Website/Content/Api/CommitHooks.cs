using System;
using Starcounter;
using Simplified.Ring5;

namespace Content {
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

        protected void RefreshSignInState() {
            this.RefreshStandaloneSignInState();
            this.RefreshContentCacheSignInState();
        }

        protected void RefreshStandaloneSignInState() {
            StandalonePage page = GetStandalonePage();

            if (page == null) {
                return;
            }

            page.RefreshCurrentPage();
        }

        protected void RefreshContentCacheSignInState() {
            ContentCachePage master = Session.Current.Data as ContentCachePage;

            if (master == null) {
                return;
            }

            foreach (ContentPage page in master.Pages) {
                //Trigger OnData method
                page.Data = null;
            }
        }

        protected StandalonePage GetStandalonePage() {
            if (Session.Current != null && Session.Current.Data is StandalonePage) {
                return Session.Current.Data as StandalonePage;
            }

            return null;
        }
    }
}
