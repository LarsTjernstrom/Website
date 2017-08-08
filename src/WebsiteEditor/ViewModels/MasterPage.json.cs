using Simplified.Ring3;
using Starcounter;
using WebsiteEditor.Api.Authorization;
using WebsiteEditor.Api.Authorization.Permissions;
using WebsiteEditor.Helpers;

namespace WebsiteEditor.ViewModels
{
    partial class MasterPage : Json
    {
        protected string AllowedSystemUserGroup = "Admin (System Users)";

        public void RefreshCurrentPage(string partialPageUrl)
        {
            this.PartialUrl = partialPageUrl;
            this.RefreshCurrentPage();
        }

        public void RefreshCurrentPage()
        {
            if (string.IsNullOrEmpty(this.PartialUrl))
            {
                this.CurrentPage = null;
                return;
            }

            if (AuthEnforcementProvider.Instance.CheckPermission(new ShowSurfaceGroups()))
            {
                this.CurrentPage = Self.GET(this.PartialUrl);

                if (this.CurrentPage is IKnowSurfacePage page)
                {
                    page.SurfaceKey = Surface.Key;
                    page.RefreshData();
                }
            }
            else
            {
                this.CurrentPage = Self.GET("/websiteeditor/partials/deny");
            }
        }
    }
}
