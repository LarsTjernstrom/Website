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
            if (mappingHandlers == null) throw new ArgumentNullException(nameof(mappingHandlers));

            MappingHandlers = mappingHandlers;
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

            //Hook<WebMap>.CommitUpdate += (s, webMap) =>
            //{
            //};
        }
    }
}