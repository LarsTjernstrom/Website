using Starcounter;

/// <summary>
///  WrapperPage needs to wrap WebTemplatePage, because Starcounter applies middleware
///  from the original request. The default middleware contains <template is="imported-template"></template>
///  instead of <starcounter-include>, which is why we have to wrap it.
/// </summary>

namespace WebsiteProvider
{
    partial class WrapperPage : Json
    {
        public Json UnwrappedPublicViewModel;
        public bool IsFinal = false;
        public string RequestUri;

        public void Reset()
        {
            this.Html = "";
            this.WebTemplatePage.Data = null;
        }
    }
}
