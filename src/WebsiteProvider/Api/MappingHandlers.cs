using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class MappingHandlers
    {
        private static bool isRegistered;
        private readonly List<WebsiteBlendingInfo> blendingInfos = new List<WebsiteBlendingInfo>();

        public void Register()
        {
            this.ValidateState(isExpectedRegistered: false);
            isRegistered = true;

            var webSections = Db.SQL<WebSection>("SELECT ws FROM Simplified.Ring6.WebSection ws");

            foreach (WebSection section in webSections)
            {
                var sectionUri = section.GetMappingUrl();
                this.RegisterEmptyHandler(sectionUri);

                foreach (WebMap webMap in section.Maps.OrderBy(x => x.SortNumber))
                {
                    this.MapPinningRule(webMap, sectionUri);
                }
            }
        }

        public void MapPinningRule(WebMap webMap, string registeredSectionUri = null)
        {
            this.ValidateState(isExpectedRegistered: true);

            if (webMap.Section == null)
            {
                throw new InvalidOperationException("Section cannot be null!");
            }

            string sectionUri = registeredSectionUri ?? webMap.Section.GetMappingUrl();

            if (registeredSectionUri == null)
            {
                this.RegisterEmptyHandler(sectionUri);
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
                this.RegisterEmptyHandler(mapUri);
                Blender.MapUri(mapUri, token);
            }

            // map URI for WebMap's Pin URI on the same token
            Blender.MapUri(webMap.ForeignUrl, token);

            this.blendingInfos.Add(new WebsiteBlendingInfo
            {
                MapId = webMap.GetObjectNo(),
                SectionId = webMap.Section.GetObjectNo(),
                TemplateId = webMap.Section.Template.GetObjectNo(),
                HasUrl = webMap.Url != null,
                Token = token,
                EmptyHandlerUri = mapUri,
                SectionHandlerUri = sectionUri,
                ForeignUri = webMap.ForeignUrl
            });
        }

        public void UpdatePinningRule(WebMap webMap)
        {
            this.ValidateState(isExpectedRegistered: true);

            this.UnmapPinningRule(webMap.GetObjectNo());
            this.MapPinningRule(webMap);
        }

        public void UnmapPinningRule(ulong mapId)
        {
            this.ValidateState(isExpectedRegistered: true);
            this.UnmapPinningRule(this.blendingInfos.First(x => x.MapId == mapId));
        }

        public void UnmapBlendingPoint(ulong sectionId)
        {
            this.ValidateState(isExpectedRegistered: true);
            this.UnmapBlendingPoint(this.blendingInfos.Where(x => x.SectionId == sectionId).ToList());
        }

        public void UnmapSurface(ulong templateId)
        {
            this.ValidateState(isExpectedRegistered: true);
            var infos = this.blendingInfos.Where(x => x.TemplateId == templateId);
            foreach (var infosBySection in infos.GroupBy(x => x.SectionId))
            {
                this.UnmapBlendingPoint(infosBySection.ToList());
            }
        }

        private void UnmapPinningRule(WebsiteBlendingInfo info)
        {
            if (!IsSectionExists(info.SectionId) || !IsTemplateExists(info.TemplateId))
            {
                // if the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
                return;
            }

            Blender.UnmapUri(info.ForeignUri, info.Token);

            if (info.HasUrl &&
                Blender.ListByTokens()[info.Token].Count == 2) // one URI for empty WebMap's handler and another one for empty WebSection's handler
            {
                Blender.UnmapUri(info.EmptyHandlerUri, info.Token);
                Handle.UnregisterHttpHandler("GET", info.EmptyHandlerUri);
            }
        }

        private void UnmapBlendingPoint(List<WebsiteBlendingInfo> mapInfos)
        {
            var sectionInfo = mapInfos[0];

            if (!IsTemplateExists(sectionInfo.TemplateId))
            {
                // if the Surface (WebTemplate) was deleted earlier
                return;
            }

            foreach (var info in mapInfos)
            {
                UnmapPinningRule(info);
            }
            //Blender.UnmapUri(sectionInfo.SectionHandlerUri, sec);
            if (Handle.IsHandlerRegistered("GET", sectionInfo.SectionHandlerUri))
            {
                Handle.UnregisterHttpHandler("GET", sectionInfo.SectionHandlerUri);
            }
        }

        private bool IsTemplateExists(ulong templateId)
        {
            return Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.ObjectNo = ?", templateId).Any();
        }

        private bool IsSectionExists(ulong sectionId)
        {
            return Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.ObjectNo = ?", sectionId).Any();
        }

        private void RegisterEmptyHandler(string uri)
        {
            if (!Handle.IsHandlerRegistered("GET", uri))
            {
                Handle.GET(uri, () => new Json(), new HandlerOptions { SelfOnly = true });
            }
        }

        private void ValidateState(bool isExpectedRegistered)
        {
            if (isExpectedRegistered && !isRegistered)
            {
                throw new InvalidOperationException("Mappings should be registered first.");
            }
            if (!isExpectedRegistered && isRegistered)
            {
                throw new InvalidOperationException("Mappings is already registered.");
            }
        }

        private class WebsiteBlendingInfo
        {
            public ulong MapId { get; set; }
            public ulong SectionId { get; set; }
            public ulong TemplateId { get; set; }

            public bool HasUrl { get; set; }

            public string EmptyHandlerUri { get; set; }
            public string SectionHandlerUri { get; set; }
            public string ForeignUri { get; set; }
            public string Token { get; set; }
        }
    }
}
