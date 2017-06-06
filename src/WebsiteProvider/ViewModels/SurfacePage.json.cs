using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    partial class SurfacePage : Json, IBound<WebTemplate>
    {
        public bool IsFinal = false;
        public string RequestUri;
        public SectionPage DefaultSection;
        public Json LastJson;
    }
}