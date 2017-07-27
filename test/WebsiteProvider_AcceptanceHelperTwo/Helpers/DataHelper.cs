using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    public class DataHelper
    {
        public void SetDefaultCatchingRules()
        {
            Db.Transact(() =>
            {
                var sidebarSurface = GenerateSidebarSurface();
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperTwo").First ??
                         new WebUrl
                         {
                             Template = sidebarSurface,
                             Url = "/WebsiteProvider_AcceptanceHelperTwo",
                             IsFinal = true
                         };
            });
        }

        protected WebTemplate GenerateSidebarSurface()
        {
            const string surfaceName = "TestSidebarSurface";
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", surfaceName).First;

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = surfaceName,
                Html = "/websiteprovider/surfaces/SidebarSurface.html"
            };

            new WebSection
            {
                Template = surface,
                Name = "Left",
                Default = false
            };
            new WebSection
            {
                Template = surface,
                Name = "Right",
                Default = true
            };

            return surface;
        }
    }
}