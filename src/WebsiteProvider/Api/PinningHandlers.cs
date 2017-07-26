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
        private readonly List<PinningBlendingInfo> pinningInfos = new List<PinningBlendingInfo>();

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

            // TODO : optimize
            foreach (WebMap webMap in webSections.SelectMany(x => x.Maps.OrderBy(m => m.SortNumber)))
            {
                this.MapPinningRule(webMap);
            }
        }

        public void MapPinningRule(WebMap webMap)
        {
            this.ValidateState(isExpectedRegistered: true);

            if (webMap.Section == null)
            {
                throw new Exception("Section cannot be null!");
            }

            string token = webMap.GetMappingToken();
            string callerUri = webMap.GetMappingUrl();

            var mapInfo = new PinningBlendingInfo
            {
                MapId = webMap.GetObjectNo(),
                SectionId = webMap.Section.GetObjectNo(),
                TemplateId = webMap.Section.Template.GetObjectNo(),
                HasUrl = !string.IsNullOrEmpty(webMap.Url?.Url),
                Token = token,
                CallerUri = callerUri,
                ForeignUri = webMap.ForeignUrl
            };

            if (!Blender.IsMapped(callerUri, token))
            {
                this.RegisterEmptyHandler(callerUri);
                Blender.MapUri(callerUri, token);
            }

            if (mapInfo.HasUrl)
            {
                var emptyUrlMaps = this.GetPinningInfos(info => info.SectionId == mapInfo.SectionId && !info.HasUrl);
                foreach (var info in emptyUrlMaps)
                {
                    if (!Blender.IsMapped(info.ForeignUri, token))
                    {
                        Blender.MapUri(info.ForeignUri, token);
                    }
                }
            }
            else
            {
                var notEmptyUrlMaps = this.GetPinningInfos(info => info.SectionId == mapInfo.SectionId && info.HasUrl);
                foreach (var otherTokens in notEmptyUrlMaps.Select(x => x.Token).Distinct())
                {
                    if (!Blender.IsMapped(webMap.ForeignUrl, otherTokens))
                    {
                        Blender.MapUri(webMap.ForeignUrl, otherTokens);
                    }
                }
            }

            // map URI for WebMap's Pin URI on the same token
            Blender.MapUri(webMap.ForeignUrl, token);

            this.AddPinningInfo(mapInfo);
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

            var info = this.TakePinningInfo(x => x.MapId == mapId);
            if (info == null)
            {
                return;     // if the Pinning Rule (WebMap) or the Blending Point (WebSection) or the Surface (WebTemplate) was deleted earlier
            }

            this.UnmapPinningRule(info);
        }

        public void UnmapBlendingPoint(ulong sectionId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var mapInfos = this.TakePinningInfos(x => x.SectionId == sectionId);
            foreach (var info in mapInfos)
            {
                UnmapPinningRule(info);
            }
        }

        public void UnmapSurface(ulong templateId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var infos = this.TakePinningInfos(x => x.TemplateId == templateId);
            foreach (var info in infos.GroupBy(x => x.SectionId).SelectMany(x => x))
            {
                UnmapPinningRule(info);
            }
        }

        private void UnmapPinningRule(PinningBlendingInfo info)
        {
            var urlMappings = Blender.ListByUris()[info.ForeignUri];
            foreach (var mapping in urlMappings)
            {
                Blender.UnmapUri(mapping.Uri, mapping.Token);
            }

            urlMappings = Blender.ListByTokens()[info.Token];
            if (urlMappings.Count == 1)
            {
                Blender.UnmapUri(info.CallerUri, info.Token);
                Handle.UnregisterHttpHandler("GET", info.CallerUri);
            }
        }

        private void AddPinningInfo(PinningBlendingInfo info)
        {
            lock (locker)
            {
                this.pinningInfos.Add(info);
            }
        }

        private PinningBlendingInfo TakePinningInfo(Func<PinningBlendingInfo, bool> predicate)
        {
            lock (locker)
            {
                var item = this.pinningInfos.FirstOrDefault(predicate);
                this.pinningInfos.Remove(item);
                return item;
            }
        }

        private List<PinningBlendingInfo> TakePinningInfos(Func<PinningBlendingInfo, bool> predicate)
        {
            lock (locker)
            {
                var items = this.pinningInfos.Where(predicate).ToList();
                foreach (var info in items)
                {
                    this.pinningInfos.Remove(info);
                }
                return items;
            }
        }

        private List<PinningBlendingInfo> GetPinningInfos(Func<PinningBlendingInfo, bool> predicate)
        {
            lock (locker)
            {
                var items = this.pinningInfos.Where(predicate).ToList();
                return items;
            }
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

        private class PinningBlendingInfo
        {
            public ulong MapId { get; set; }
            public ulong SectionId { get; set; }
            public ulong TemplateId { get; set; }

            public bool HasUrl { get; set; }

            public string CallerUri { get; set; }
            public string ForeignUri { get; set; }
            public string Token { get; set; }
        }
    }
}
