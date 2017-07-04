using System;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    internal class CommitHooks
    {
        public MappingHandlers MappingHandlers { get; }

        public CommitHooks(MappingHandlers mappingHandlers)
        {
            MappingHandlers = mappingHandlers ?? throw new ArgumentNullException(nameof(mappingHandlers));
        }

        public void Register()
        {
            Hook<WebMap>.CommitInsert += (s, webMap) =>
            {
                MappingHandlers.MapPinningRule(webMap);
            };

            Hook<WebMap>.BeforeDelete += (s, webMap) =>
            {
                MappingHandlers.UnmapPinningRule(webMap);
            };

            Hook<WebMap>.CommitUpdate += (s, webMap) =>
            {
                MappingHandlers.UpdatePinningRule(webMap);
            };

            Hook<WebSection>.BeforeDelete += (s, webSection) =>
            {
                MappingHandlers.UnmapBlendingPoint(webSection);
            };

            Hook<WebTemplate>.BeforeDelete += (s, webTemplate) =>
            {
                foreach (WebSection webSection in webTemplate.Sections)
                {
                    MappingHandlers.UnmapBlendingPoint(webSection);
                }
            };
        }
    }
}