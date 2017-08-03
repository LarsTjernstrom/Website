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
    public partial class PinningHandlers
    {
        private static PinningHandlers instance;
        private bool isRegistered;
        private readonly PinningRulesMappingCollection mappings = new PinningRulesMappingCollection();

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

            if (Blender.IsMapped(webMap.ForeignUrl, token))
            {
                // if current URI is already mapped with current token we just increment Count property.
                // actually current case is valid only on transaction committing state when
                // some pinning rules was changed for the same blending point with the same catching rule.
                // such state is temporary because WebsiteEditor should not allow to save pinning rules,
                // i.e. next changed/added pinning rule's commit hook will execute unmapping of this URI-token pair.
                this.mappings.Process(
                    info => info.ForeignUri == webMap.ForeignUrl && info.Token == token,
                    list =>
                    {
                        if (Blender.IsMapped(webMap.ForeignUrl, token))
                        {
                            list.First().MapIds.Add(webMap.GetObjectNo());
                        }
                    });
                return;
            }

            string callerUri = webMap.GetCallerUri();

            var mapInfo = new PinningRuleMappingInfo
            {
                MapIds = new List<ulong> { webMap.GetObjectNo() },
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
                Blender.MapUri(callerUri, token, true, false);
            }

            if (mapInfo.UseCustomCatchingRule)
            {
                // get info of already mapped Pinning Rules defined with empty Catching Rule and for the same Blending Point
                this.mappings.Process(
                    info => info.SectionId == mapInfo.SectionId && !info.UseCustomCatchingRule,
                    emptyCatchingRuleMappings =>
                    {
                        // and map its URIs to the current token, so these Pinning Rules will be blended with the current one when the catching rule is applicable
                        foreach (var info in emptyCatchingRuleMappings)
                        {
                            if (!Blender.IsMapped(info.ForeignUri, token))
                            {
                                Blender.MapUri(info.ForeignUri, token, false, true);
                            }
                        }
                    });
            }
            else
            {
                // get info of already mapped Pinning Rules with defined Catching Rule and for the same Blending Point
                this.mappings.Process(
                    info => info.SectionId == mapInfo.SectionId && info.UseCustomCatchingRule,
                    customCatchingRuleMappings =>
                    {
                        // and map current URI to its tokens, so current Pinning Rule will be blended with these ones when its catching rules are applicable
                        foreach (var otherTokens in customCatchingRuleMappings.Select(x => x.Token).Distinct())
                        {
                            if (!Blender.IsMapped(webMap.ForeignUrl, otherTokens))
                            {
                                Blender.MapUri(webMap.ForeignUrl, otherTokens, false, true);
                            }
                        }
                    });
            }

            // now do map current Pinning Rule's URI to the current token and save mapping info.
            Blender.MapUri(webMap.ForeignUrl, token, false, true);

            this.mappings.Add(mapInfo);
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

            var info = this.mappings.Take(x => x.MapIds.Contains(mapId)).FirstOrDefault();
            if (info == null)
            {
                return;
            }

            this.UnmapPinningRule(info, mapId);
        }

        /// <summary>
        /// Unmap all Blending Point's Pinning Rules
        /// </summary>
        /// <param name="sectionId">Blending Point ID</param>
        public void UnmapBlendingPoint(ulong sectionId)
        {
            this.ValidateState(isExpectedRegistered: true);

            var mapInfos = this.mappings.Take(x => x.SectionId == sectionId);
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

            var infos = this.mappings.Take(x => x.TemplateId == templateId);
            foreach (var info in infos.GroupBy(x => x.SectionId).SelectMany(x => x))
            {
                UnmapPinningRule(info);
            }
        }

        private void UnmapPinningRule(PinningRuleMappingInfo info, ulong? certainMapId = null)
        {
            if (certainMapId.HasValue)
            {
                info.MapIds.Remove(certainMapId.Value);
                if (info.MapIds.Any())
                {
                    // it means that at least one other pinning rule with the same properies will still be mapped
                    return;
                }
            }
            info.MapIds.Clear();    // now it is invalid mapping info

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
    }
}
