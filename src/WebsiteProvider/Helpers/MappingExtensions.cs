using Simplified.Ring6;

namespace WebsiteProvider
{
    public static class MappingExtensions
    {
        public static string GetMappingToken(this WebMap webMap)
        {
            return webMap.Section.GetMappingToken(webMap.Url);
        }

        public static string GetMappingToken(this WebSection webSection, WebUrl webUrl)
        {
            return webUrl == null
                ? $"{webSection.Template.Name}%{webSection.Name}"
                : $"{webSection.Template.Name}%{webSection.Name}%{webUrl.Url}";
        }

        public static string GetMappingToken(this WebSection webSection)
        {
            return $"{webSection.Template.Name}%{webSection.Name}";
        }

        public static string GetMappingUrl(this WebMap webMap)
        {
            return webMap.Section.GetMappingUrl(webMap.Url);
        }

        public static string GetMappingUrl(this WebSection webSection, WebUrl webUrl)
        {
            return webUrl == null || string.IsNullOrWhiteSpace(webUrl.Url)
                ? $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}"
                : $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}/uri/{webUrl.Url}";
        }

        public static string GetMappingUrl(this WebSection webSection)
        {
            return $"/website/blender/surface/{webSection.Template.Key}/point/{webSection.Key}";
        }
    }
}