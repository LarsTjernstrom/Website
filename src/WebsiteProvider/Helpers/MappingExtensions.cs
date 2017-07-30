using Simplified.Ring6;

namespace WebsiteProvider.Helpers
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
                ? $"websiteeditor%{webSection.Template.Key}%{webSection.Key}"
                : $"websiteeditor%{webSection.Template.Key}%{webSection.Key}%{webUrl.Key}";
        }

        public static string GetMappingUrl(this WebMap webMap)
        {
            return webMap.Section.GetMappingUrl(webMap.Url);
        }

        public static string GetMappingUrl(this WebSection webSection, WebUrl webUrl = null)
        {
            return string.IsNullOrWhiteSpace(webUrl?.Url)
                ? $"/websiteeditor/blender/surface/{webSection.Template.Key}/point/{webSection.Key}"
                : $"/websiteeditor/blender/surface/{webSection.Template.Key}/point/{webSection.Key}/uri/{webUrl.Key}";
        }
    }
}