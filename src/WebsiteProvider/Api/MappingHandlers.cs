using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class MappingHandlers
    {
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
            if (webMap.Section == null)
            {
                throw new InvalidOperationException("Section cannot be null!");
            }

            string sectionUri = registeredSectionUri ?? webMap.Section.GetMappingUrl();

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
            this.UnmapPinningRule(webMap, GetOldUri(webMap));
            this.MapPinningRule(webMap);
        }

        public void UnmapPinningRule(WebMap webMap, string webMapForeignUrl = null)
        {
            if (webMap.Section?.Template == null)
            {
                // if the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
                return;
            }

            webMapForeignUrl = webMapForeignUrl ?? webMap.ForeignUrl;

            string token = webMap.GetMappingToken();
            string mapUri = webMap.GetMappingUrl();

            if (webMapForeignUrl != null)
            {
                Blender.UnmapUri(webMapForeignUrl, token);
            }

            if (webMap.Url != null &&
                Blender.ListByTokens()[token].Count == 2) // one URI for empty WebMap's handler and another one for empty WebSection's handler
            {
                Blender.UnmapUri(mapUri, token);
                Handle.UnregisterHttpHandler("GET", mapUri);
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
            if (Handle.IsHandlerRegistered("GET", mapUri))
            {
                Handle.UnregisterHttpHandler("GET", mapUri);
            }
        }

        private void RegisterEmptyHandler(string uri)
        {
            if (!Handle.IsHandlerRegistered("GET", uri))
            {
                Handle.GET(uri, () => new Json(), new HandlerOptions { SelfOnly = true });
            }
        }

        private static string GetOldUri(WebMap webMap)
        {
            var token = webMap.GetMappingToken();
            var currentMappingUrls = new List<string> {webMap.Section.GetMappingUrl()};

            foreach (WebMap map in webMap.Section.Maps)
            {
                currentMappingUrls.Add(map.GetMappingUrl());
                currentMappingUrls.Add(map.ForeignUrl);
            }

            return Blender.ListByTokens()[token].FirstOrDefault(x => !currentMappingUrls.Contains(x.Uri))?.Uri;
        }
    }
}
