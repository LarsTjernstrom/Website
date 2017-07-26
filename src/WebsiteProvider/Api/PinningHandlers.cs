using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    public class PinningHandlers
    {
        private static PinningHandlers instance;
        private bool isRegistered;
        private readonly object locker = new object();
        private readonly List<WebsiteBlendingInfo> blendingInfos = new List<WebsiteBlendingInfo>();

        private PinningHandlers()
        {
        }

        public static PinningHandlers GetInstance()
        {
            return instance ?? (instance = new PinningHandlers());
        }

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
                throw new Exception("Section cannot be null!");
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

            this.AddBlendingInfo(new WebsiteBlendingInfo
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
            this.UnmapPinningRule(this.TakeBlendingInfo(x => x.MapId == mapId));
        }

        public void UnmapBlendingPoint(ulong sectionId)
        {
            this.ValidateState(isExpectedRegistered: true);
            this.UnmapBlendingPoint(this.TakeBlendingInfos(x => x.SectionId == sectionId));
        }

        public void UnmapSurface(ulong templateId)
        {
            this.ValidateState(isExpectedRegistered: true);
            var infos = this.TakeBlendingInfos(x => x.TemplateId == templateId);
            foreach (var infosBySection in infos.GroupBy(x => x.SectionId))
            {
                this.UnmapBlendingPoint(infosBySection.ToList());
            }
        }

        private void UnmapPinningRule(WebsiteBlendingInfo info)
        {
            if (info == null || !IsSectionExists(info.SectionId) || !IsTemplateExists(info.TemplateId))
            {
                return;     // if the Pinning Rule (WebMap) or the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
            }

            Blender.UnmapUri(info.ForeignUri, info.Token);

            if (info.HasUrl &&
                Blender.ListByTokens()[info.Token].Count == 2)      // one URI for empty WebMap's handler and another one for empty WebSection's handler
            {
                Blender.UnmapUri(info.EmptyHandlerUri, info.Token);
                Handle.UnregisterHttpHandler("GET", info.EmptyHandlerUri);
                this.CompleteUnmapBlendingPoint(info);
            }
        }

        private void UnmapBlendingPoint(List<WebsiteBlendingInfo> mapInfos)
        {
            var sectionInfo = mapInfos.FirstOrDefault();

            if (sectionInfo == null || !IsTemplateExists(sectionInfo.TemplateId))
            {
                return;     // if the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
            }

            foreach (var info in mapInfos)
            {
                UnmapPinningRule(info);
            }

            this.CompleteUnmapBlendingPoint(sectionInfo);
        }

        private void CompleteUnmapBlendingPoint(WebsiteBlendingInfo sectionInfo)
        {
            foreach (var blendingInfo in Blender.ListByUris()[sectionInfo.SectionHandlerUri])
            {
                Blender.UnmapUri(blendingInfo.Uri, blendingInfo.Token);
            }
            if (Handle.IsHandlerRegistered("GET", sectionInfo.SectionHandlerUri))
            {
                Handle.UnregisterHttpHandler("GET", sectionInfo.SectionHandlerUri);
            }
        }

        private void AddBlendingInfo(WebsiteBlendingInfo info)
        {
            lock (locker)
            {
                this.blendingInfos.Add(info);
            }
        }

        private WebsiteBlendingInfo TakeBlendingInfo(Func<WebsiteBlendingInfo, bool> predicate)
        {
            lock (locker)
            {
                var item = this.blendingInfos.FirstOrDefault(predicate);
                this.blendingInfos.Remove(item);
                return item;
            }
        }

        private List<WebsiteBlendingInfo> TakeBlendingInfos(Func<WebsiteBlendingInfo, bool> predicate)
        {
            lock (locker)
            {
                var items = this.blendingInfos.Where(predicate).ToList();
                foreach (var info in items)
                {
                    this.blendingInfos.Remove(info);
                }
                return items;
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
