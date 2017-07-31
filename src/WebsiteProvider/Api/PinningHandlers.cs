using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using WebsiteProvider.Helpers;

namespace WebsiteProvider.Api
{
    /// <summary>
    /// Do mapping/unmapping of the Pinning Rules
    /// </summary>
    public class PinningHandlers
    {
        private static PinningHandlers instance;
        private readonly object locker = new object();
        private bool isRegistered;
        private readonly List<PinningRuleMappingInfo> mappingInfos = new List<PinningRuleMappingInfo>();

        private PinningHandlers()
        {
        }

        /// <summary>
        /// Get single instance of <see cref="PinningHandlers"/>
        /// </summary>
        public static PinningHandlers GetInstance()
        {
            return instance ?? (instance = new PinningHandlers());
        }

        /// <summary>
        /// Map all Pinning Rules defined in Database
        /// </summary>
        public void Register()
        {
            this.ValidateState(isExpectedRegistered: false);
            isRegistered = true;

            var webSections = Db.SQL<WebSection>("SELECT ws FROM Simplified.Ring6.WebSection ws");

            foreach (WebSection webSection in webSections)
            {
                // do map only Pinning Rules defined with empty Catching Rules first
                foreach (WebMap webMap in webSection.Maps.Where(x => string.IsNullOrEmpty(x.Url?.Url)).OrderBy(x => x.SortNumber))
                {
                    this.MapPinningRule(webMap);
                }
                // now do map only Pinning Rules defined with non-empty Catching Rules
                foreach (WebMap webMap in webSection.Maps.Where(x => !string.IsNullOrEmpty(x.Url?.Url)).OrderBy(x => x.SortNumber))
                {
                    this.MapPinningRule(webMap);
                }
            }
        }

        /// <summary>
        /// Map Pinning Rule
        /// </summary>
        public void MapPinningRule(WebMap webMap)
        {
            // validate
            this.ValidateState(isExpectedRegistered: true);

            if (webMap.Section == null)
            {
                throw new Exception("Blending point is not defined for the Pinning Rule!");
            }
            if (webMap.Section.Template == null)
            {
                throw new Exception("Surface is not defined for the Pinning Rule!");
            }

            // prepare mapping info
            string token = webMap.GetMappingToken();
            string callerUri = webMap.GetCallerUri();

            var mapInfo = new PinningRuleMappingInfo
            {
                MapId = webMap.GetObjectNo(),
                SectionId = webMap.Section.GetObjectNo(),
                TemplateId = webMap.Section.Template.GetObjectNo(),
                UseCustomCatchingRule = !string.IsNullOrEmpty(webMap.Url?.Url),
                Token = token,
                TokenBase = webMap.Section.GetMappingToken(),
                CallerUri = callerUri,
                ForeignUri = webMap.ForeignUrl
            };

            // register "empty" handler for URI ("caller" URI) which will be called by WebsiteProvider middleware when getting content for a Blending Point
            // and map it if it has not been registered and mapped earlier
            if (!Blender.IsMapped(callerUri, token))
            {
                this.RegisterEmptyHandler(callerUri);
                Blender.MapUri(callerUri, token);
            }

            if (mapInfo.UseCustomCatchingRule)
            {
                // get info of already mapped Pinning Rules defined with empty Catching Rule and for the same Blending Point
                var emptyCatchingRuleMappings = this.GetMappingInfos(info => info.SectionId == mapInfo.SectionId && !info.UseCustomCatchingRule);
                // and map its URIs to the current token, so these Pinning Rules will be blended with the current one when the catching rule is applicable
                foreach (var info in emptyCatchingRuleMappings)
                {
                    if (!Blender.IsMapped(info.ForeignUri, token))
                    {
                        Blender.MapUri(info.ForeignUri, token);
                    }
                }
            }
            else
            {
                // get info of already mapped Pinning Rules with defined Catching Rule and for the same Blending Point
                var customCatchingRuleMappings = this.GetMappingInfos(info => info.SectionId == mapInfo.SectionId && info.UseCustomCatchingRule);
                // and map current URI to its tokens, so current Pinning Rule will be blended with these ones when its catching rules are applicable
                foreach (var otherTokens in customCatchingRuleMappings.Select(x => x.Token).Distinct())
                {
                    if (!Blender.IsMapped(webMap.ForeignUrl, otherTokens))
                    {
                        Blender.MapUri(webMap.ForeignUrl, otherTokens);
                    }
                }
            }

            // now do map current Pinning Rule's URI to the current token and save mapping info
            Blender.MapUri(webMap.ForeignUrl, token);

            this.AddMappingInfo(mapInfo);
        }

        /// <summary>
        /// Update mapping for Pinning Rule
        /// </summary>
        public void UpdatePinningRule(WebMap webMap)
        {
            this.ValidateState(isExpectedRegistered: true);

            this.UnmapPinningRule(webMap.GetObjectNo());
            this.MapPinningRule(webMap);
        }

        /// <summary>
        /// Unmap Pinning Rule
        /// </summary>
        /// <param name="mapId">Pinning Rule ID</param>
        public void UnmapPinningRule(ulong mapId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var info = this.TakeMappingInfo(x => x.MapId == mapId);
            if (info == null)
            {
                return;
            }

            this.UnmapPinningRule(info);
        }

        /// <summary>
        /// Unmap all Blending Point's Pinning Rules
        /// </summary>
        /// <param name="sectionId">Blending Point ID</param>
        public void UnmapBlendingPoint(ulong sectionId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var mapInfos = this.TakeMappingInfos(x => x.SectionId == sectionId);
            foreach (var info in mapInfos)
            {
                UnmapPinningRule(info);
            }
        }

        /// <summary>
        /// Unmap all Surface's Pinning Rules
        /// </summary>
        /// <param name="templateId">Surface ID</param>
        public void UnmapSurface(ulong templateId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var infos = this.TakeMappingInfos(x => x.TemplateId == templateId);
            foreach (var info in infos.GroupBy(x => x.SectionId).SelectMany(x => x))
            {
                UnmapPinningRule(info);
            }
        }

        private void UnmapPinningRule(PinningRuleMappingInfo info)
        {
            // get tokens of the current Pinning Rule's Blending Point which was mapped with current Pinning Rule's URI and unmapped it
            var sectionTokens = Blender.ListByUris()[info.ForeignUri]
                .Where(x => x.Token.StartsWith(info.TokenBase))
                .Select(x => x.Token)
                .ToList();
            foreach (var token in sectionTokens)
            {
                Blender.UnmapUri(info.ForeignUri, token);
            }

            // if it was last Pinning Rule for the current token then unregister and unmap "caller" URI
            if (Blender.ListByTokens()[info.Token].Count == 1)
            {
                Blender.UnmapUri(info.CallerUri, info.Token);
                Handle.UnregisterHttpHandler("GET", info.CallerUri);
            }
        }

        private void AddMappingInfo(PinningRuleMappingInfo info)
        {
            lock (locker)
            {
                this.mappingInfos.Add(info);
            }
        }

        private PinningRuleMappingInfo TakeMappingInfo(Func<PinningRuleMappingInfo, bool> predicate)
        {
            lock (locker)
            {
                var item = this.mappingInfos.FirstOrDefault(predicate);
                this.mappingInfos.Remove(item);
                return item;
            }
        }

        private List<PinningRuleMappingInfo> TakeMappingInfos(Func<PinningRuleMappingInfo, bool> predicate)
        {
            lock (locker)
            {
                var items = this.mappingInfos.Where(predicate).ToList();
                foreach (var info in items)
                {
                    this.mappingInfos.Remove(info);
                }
                return items;
            }
        }

        private List<PinningRuleMappingInfo> GetMappingInfos(Func<PinningRuleMappingInfo, bool> predicate)
        {
            lock (locker)
            {
                var items = this.mappingInfos.Where(predicate).ToList();
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

        /// <summary>
        /// Summary info for Pinning Rule mapping
        /// </summary>
        private class PinningRuleMappingInfo
        {
            public ulong MapId { get; set; }
            public ulong SectionId { get; set; }
            public ulong TemplateId { get; set; }

            public bool UseCustomCatchingRule { get; set; }

            public string CallerUri { get; set; }
            public string ForeignUri { get; set; }
            public string Token { get; set; }
            public string TokenBase { get; set; }
        }
    }
}
