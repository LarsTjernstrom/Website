using Simplified.Ring6;

namespace WebsiteProvider
{
    public static class MappingExtensions
    {
        public static string GetMappingToken(this WebMap webMap)
        {
            return webMap.Section.GetMappingToken(webMap.Url);
        }

        public static string GetMappingToken(this WebSection webSection, WebUrl webUrl = null)
        {
            return webUrl == null
                ? $"website%{webSection.Template.Key}%{webSection.Key}"
                : $"website%{webSection.Template.Key}%{webSection.Key}%{webUrl.Key}";
        }

        public static string GetMappingUrl(this WebMap webMap)
        {
            return webMap.Section.GetMappingUrl(webMap.Url);
        }

        public static string GetMappingUrl(this WebSection webSection, WebUrl webUrl = null)
        {
            return string.IsNullOrWhiteSpace(webUrl?.Url)
                ? $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}"
                : $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}/uri/{webUrl.Key}";
        }
    }
}