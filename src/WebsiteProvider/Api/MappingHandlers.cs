using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class MappingHandlers
    {
        private readonly HandlerOptions selfOnlyOptions = new HandlerOptions {SelfOnly = true};

        public void Register()
        {
            var webSections = Db.SQL<WebSection>("SELECT ws FROM Simplified.Ring6.WebSection ws");

            foreach (WebSection section in webSections)
            {
                var sectionUrl = section.GetMappingUrl();
                RegisterEmptyHandler(sectionUrl);

                foreach (WebMap webMap in section.Maps.OrderBy(x => x.SortNumber))
                {
                    MapPinningRule(webMap, sectionUrl);
                }
            }
        }

        public void MapPinningRule(WebMap webMap, string registeredSectionUrl = null)
        {
            var sectionUrl = registeredSectionUrl ?? webMap.Section.GetMappingUrl();

            if (registeredSectionUrl == null)
            {
                RegisterEmptyHandler(sectionUrl);
            }

            string token = webMap.GetMappingToken();
            string mapUrl = webMap.GetMappingUrl();

            // map URI for empty Catch URI on the blending token
            if (webMap.Url != null && !Blender.IsMapped(sectionUrl, token))
            {
                Blender.MapUri(sectionUrl, token);
            }

            // map URI for WebMap's Catch URI on the same blending token;
            // this will be calling by WebsiteProvider
            if (!Blender.IsMapped(mapUrl, token))
            {
                RegisterEmptyHandler(mapUrl);
                Blender.MapUri(mapUrl, token);
            }

            // map URI for WebMap's Pin URI on the same token
            Blender.MapUri(webMap.ForeignUrl, token);
        }

        public void UnmapPinningRule(WebMap webMap)
        {
            string token = webMap.GetMappingToken();
            string mapUrl = webMap.GetMappingUrl();
            Blender.UnmapUri(webMap.ForeignUrl, token);
            Blender.UnmapUri(mapUrl, token);
            if (Handle.IsHandlerRegistered(mapUrl, selfOnlyOptions))
            {
                Handle.UnregisterHttpHandler("GET", mapUrl);
            }
        }

        private void RegisterEmptyHandler(string url)
        {
            if (!Handle.IsHandlerRegistered(url, selfOnlyOptions))
            {
                Handle.GET(url, () => new Json(), selfOnlyOptions);
            }
        }
    }
}