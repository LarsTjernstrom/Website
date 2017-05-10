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
            var registeredEmptyHandleUris = new List<string>();

            foreach (WebSection section in webSections)
            {
                var sectionMappingUrl = section.GetMappingUrl();
                RegisterEmptyHandler(sectionMappingUrl, registeredEmptyHandleUris);

                foreach (WebMap webMap in section.Maps.OrderBy(x => x.SortNumber))
                {
                    string token = webMap.GetMappingToken();
                    string mappingUrl = webMap.GetMappingUrl();

                    if (webMap.Url != null && !Blender.IsMapped(sectionMappingUrl, token))
                    {
                        Blender.MapUri(sectionMappingUrl, token);
                    }

                    if (!Blender.IsMapped(mappingUrl, token))
                    {
                        RegisterEmptyHandler(mappingUrl, registeredEmptyHandleUris);
                        Blender.MapUri(mappingUrl, token);
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