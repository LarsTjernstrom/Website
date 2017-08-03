using Simplified.Ring6;
using Starcounter.Authorization.Core;

namespace WebsiteEditor.Api.Authorization.Permissions
{
    public class ShowSurface : Permission
    {
        public WebTemplate Surface { get; set; } // moge ograniczyc WIDOCZNOSC surface'a?
    }
}