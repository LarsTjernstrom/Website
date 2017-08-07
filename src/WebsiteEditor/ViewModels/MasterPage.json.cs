using Simplified.Ring3;
using Starcounter;
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

            this.CurrentPage = Self.GET(this.PartialUrl);

            if (this.CurrentPage is IKnowSurfacePage page)
            {
                page.SurfaceKey = Surface.Key;
                page.RefreshData();
            }
        }
    }
}
