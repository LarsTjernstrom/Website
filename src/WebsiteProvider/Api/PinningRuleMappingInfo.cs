using System.Collections.Generic;

namespace WebsiteProvider.Api
{
    public partial class PinningHandlers
    {
        /// <summary>
        /// Summary info for Pinning Rule mapping
        /// </summary>
        private class PinningRuleMappingInfo
        {
            public List<ulong> MapIds { get; set; }
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
