using Starcounter;
using Simplified.Ring3;

namespace Content {
    partial class ContentPage : Page {
        protected override void OnData() {
            base.OnData();
            this.User.Data = SystemUser.GetCurrentSystemUser();
        }

        [ContentPage_json.User]
        partial class ContentPageUser : Json, IBound<SystemUser> { 
        }
    }
}
