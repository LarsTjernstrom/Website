using Starcounter;
using Simplified.Ring3;

namespace Content {
    partial class StandalonePage : Page {
        protected string allowedSystemUserGroup = "Admin (System Users)";

        public void RefreshCurrentPage() {
            SystemUser user = SystemUser.GetCurrentSystemUser();

            if (SystemUser.IsMemberOfGroup(user, allowedSystemUserGroup)) {
                this.CurrentPage = Self.GET(this.PartialUrl);
            } else {
                this.CurrentPage = Self.GET("/content/partials/deny");
            }
        }
    }
}
