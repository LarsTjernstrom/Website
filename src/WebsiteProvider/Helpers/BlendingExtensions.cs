using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public static class BlendingExtensions
    {
        public static string GetBlendingToken(this WebMap webMap)
        {
            return webMap.Section.GetBlendingToken(webMap.Url);
        }

        public static string GetBlendingToken(this WebSection webSection, WebUrl webUrl)
        {
            return webUrl == null
                ? $"{webSection.Template.Name}%{webSection.Name}"
                : $"{webSection.Template.Name}%{webSection.Name}%{webUrl.Url}";
        }

        public static string GetBlendingToken(this WebSection webSection)
        {
            return $"{webSection.Template.Name}%{webSection.Name}";
        }

        public static string GetBlendingUrl(this WebMap webMap)
        {
            return webMap.Section.GetBlendingUrl(webMap.Url);
        }

        public static string GetBlendingUrl(this WebSection webSection, WebUrl webUrl)
        {
            return webUrl == null
                ? $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}"
                : $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}/uri/{webUrl.Url}";
        }

        public static string GetBlendingUrl(this WebSection webSection)
        {
            return $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}";
        }
    }
}