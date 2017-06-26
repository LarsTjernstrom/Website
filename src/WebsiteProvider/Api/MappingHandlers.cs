using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class MappingHandlers
    {
        private readonly HandlerOptions selfOnlyOptions = new HandlerOptions { SelfOnly = true };

        public void Register()
        {
            var webSections = Db.SQL<WebSection>("SELECT ws FROM Simplified.Ring6.WebSection ws");

            foreach (WebSection section in webSections)
            {
                var sectionUri = section.GetMappingUrl();
                RegisterEmptyHandler(sectionUri);

                foreach (WebMap webMap in section.Maps.OrderBy(x => x.SortNumber))
                {
                    MapPinningRule(webMap, sectionUri);
                }
            }
        }

        public void MapPinningRule(WebMap webMap, string registeredSectionUri = null)
        {
            var sectionUri = registeredSectionUri ?? webMap.Section.GetMappingUrl();

            if (registeredSectionUri == null)
            {
                RegisterEmptyHandler(sectionUri);
            }

            string token = webMap.GetMappingToken();
            string mapUri = webMap.GetMappingUrl();

            // map URI for empty Catch URI on the blending token
            if (webMap.Url != null && !Blender.IsMapped(sectionUri, token))
            {
                Blender.MapUri(sectionUri, token);
            }

            // map URI for WebMap's Catch URI on the same blending token;
            // this will be calling by WebsiteProvider
            if (!Blender.IsMapped(mapUri, token))
            {
                RegisterEmptyHandler(mapUri);
                Blender.MapUri(mapUri, token);
            }

            // map URI for WebMap's Pin URI on the same token
            Blender.MapUri(webMap.ForeignUrl, token);
        }

        public void UpdatePinningRule(WebMap webMap)
        {
            var token = webMap.GetMappingToken();
            var newUri = webMap.ForeignUrl;
            var oldMapping = Blender.ListAll().FirstOrDefault(x => x.Key == token);
            webMap.ForeignUrl = oldMapping.Value.FirstOrDefault(x => !x.Contains(oldMapping.Key.Substring(oldMapping.Key.Length - 3))); // old URI
            this.UnmapPinningRule(webMap);
            webMap.ForeignUrl = newUri;
            this.MapPinningRule(webMap);
        }

        public void UnmapPinningRule(WebMap webMap)
        {
            if (webMap.Section?.Template == null)
            {
                // if the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
                return;
            }

            string token = webMap.GetMappingToken();
            string mapUri = webMap.GetMappingUrl();

            Blender.UnmapUri(webMap.ForeignUrl, token);

            if (webMap.Url != null)
            {
                var uriByTokenCount = Blender.ListAll().Where(x => x.Key == token).SelectMany(x => x.Value).Count();
                if (uriByTokenCount == 2) // one URI for empty WebMap's handler and another one for empty WebSection's handler
                {
                    Blender.UnmapUri(mapUri, token);
                    Handle.UnregisterHttpHandler("GET", mapUri);
                }
            }
        }

        public void UnmapBlendingPoint(WebSection webSection)
        {
            if (webSection.Template == null)
            {
                // if the Surface (WebTemplate) was deleted earlier
                return;
            }
            string token = webSection.GetMappingToken();
            string mapUri = webSection.GetMappingUrl();

            foreach (WebMap webMap in webSection.Maps)
            {
                UnmapPinningRule(webMap);
            }
            Blender.UnmapUri(mapUri, token);
            if (Handle.IsHandlerRegistered("GET " + mapUri, selfOnlyOptions))
            {
                Handle.UnregisterHttpHandler("GET", mapUri);
            }
        }

        private void RegisterEmptyHandler(string uri)
        {
            if (!Handle.IsHandlerRegistered("GET " + uri, selfOnlyOptions))
            {
                Handle.GET(uri, () => new Json(), selfOnlyOptions);
            }
        }
    }
}