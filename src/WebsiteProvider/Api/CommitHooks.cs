using System;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider.Api
{
    internal class CommitHooks
    {
        public PinningHandlers PinningHandlers { get; }

        public CommitHooks(PinningHandlers pinningHandlers)
        {
            PinningHandlers = pinningHandlers ?? throw new ArgumentNullException(nameof(pinningHandlers));
        }

        public void Register()
        {
            Hook<WebMap>.CommitInsert += (s, webMap) =>
            {
                PinningHandlers.MapPinningRule(webMap);
            };

            Hook<WebMap>.CommitDelete += (s, id) =>
            {
                PinningHandlers.UnmapPinningRule(id);
            };

            Hook<WebMap>.CommitUpdate += (s, webMap) =>
            {
                PinningHandlers.UpdatePinningRule(webMap);
            };

            Hook<WebSection>.CommitDelete += (s, id) =>
            {
                PinningHandlers.UnmapBlendingPoint(id);
            };

            Hook<WebTemplate>.CommitDelete += (s, id) =>
            {
                PinningHandlers.UnmapSurface(id);
            };
        }
    }
}