using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class MappingHandlers
    {
        public void Initialize()
        {
            var webSections = Db.SQL<WebSection>("SELECT ws FROM Simplified.Ring6.WebSection ws");
            var registeredEmptyHandleUris = new List<string>();

            foreach (WebSection section in webSections)
            {
                var sectionBlendingUrl = section.GetMappingUrl();
                RegisterEmptyHandler(sectionBlendingUrl, registeredEmptyHandleUris);

                foreach (WebMap webMap in section.Maps.OrderBy(x => x.SortNumber))
                {
                    string token = webMap.GetMappingToken();
                    string blendingUrl = webMap.GetMappingUrl();

                    if (webMap.Url != null && !Blender.IsMapped(sectionBlendingUrl, token))
                    {
                        Blender.MapUri(sectionBlendingUrl, token);
                    }

                    if (!Blender.IsMapped(blendingUrl, token))
                    {
                        RegisterEmptyHandler(blendingUrl, registeredEmptyHandleUris);
                        Blender.MapUri(blendingUrl, token);
                    }

                    Blender.MapUri(webMap.ForeignUrl, token);
                }
            }
        }

        private void RegisterEmptyHandler(string url, List<string> registeredEmptyHandleUris)
        {
            if (!registeredEmptyHandleUris.Contains(url))
            {
                Handle.GET(url, () => new Json(), new HandlerOptions { SelfOnly = true });
                registeredEmptyHandleUris.Add(url);
            }
        }
    }
}