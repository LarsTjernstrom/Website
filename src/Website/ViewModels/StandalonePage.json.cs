using Starcounter;
using Simplified.Ring3;

namespace Website {
    partial class StandalonePage : Page {
        protected string allowedSystemUserGroup = "Admin (System Users)";

        public void RefreshCurrentPage() {
            if (string.IsNullOrEmpty(this.PartialUrl)) {
                this.CurrentPage = null;
                return;
            }

            SystemUser user = SystemUser.GetCurrentSystemUser();

            if (SystemUser.IsMemberOfGroup(user, allowedSystemUserGroup)) {
                this.CurrentPage = Self.GET(this.PartialUrl);
            } else {
                this.CurrentPage = Self.GET("/website/partials/deny");
            }
        }
    }
}
