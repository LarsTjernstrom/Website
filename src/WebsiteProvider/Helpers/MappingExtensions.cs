using System.Linq;
using Simplified.Ring6;
using Starcounter;

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
                ? $"websiteprovider%{webSection.Template.Key}%{webSection.Key}"
                : $"websiteprovider%{webSection.Template.Key}%{webSection.Key}%{webUrl.Key}";
        }

        public static string GetCallerUri(this WebMap webMap)
        {
            return GetMappingUrl(webMap.Section, webMap.Url, false);
        }

        public static string GetMappingUrl(this WebSection webSection, WebUrl webUrl)
        {
            return GetMappingUrl(webSection, webUrl, true);
        }

        private static string GetMappingUrl(WebSection webSection, WebUrl webUrl, bool checkForPinningRules)
        {
            return string.IsNullOrWhiteSpace(webUrl?.Url) || checkForPinningRules && !HasPinningRules(webSection, webUrl)
                ? $"/websiteprovider/blender/surface/{webSection.Template.Key}/point/{webSection.Key}"
                : $"/websiteprovider/blender/surface/{webSection.Template.Key}/point/{webSection.Key}/uri/{webUrl.Key}";
        }

        private static bool HasPinningRules(WebSection webSection, WebUrl webUrl)
        {
            return Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.Section = ? AND m.Url = ?", webSection, webUrl).Any();
        }
    }
}