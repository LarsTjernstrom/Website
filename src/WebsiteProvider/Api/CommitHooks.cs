using System;
using System.Collections.Generic;
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

            Hook<WebMap>.CommitDelete += (s, id) =>
            {
                MappingHandlers.UnmapPinningRule(id);
            };

            Hook<WebMap>.CommitUpdate += (s, webMap) =>
            {
                MappingHandlers.UpdatePinningRule(webMap);
            };

            Hook<WebSection>.CommitDelete += (s, id) =>
            {
                MappingHandlers.UnmapBlendingPoint(id);
            };

            Hook<WebTemplate>.CommitDelete += (s, id) =>
            {
                MappingHandlers.UnmapSurface(id);
            };
        }
    }
}