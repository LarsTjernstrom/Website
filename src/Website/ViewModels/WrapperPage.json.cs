using Starcounter;

/// <summary>
///  WrapperPage needs to wrap WebTemplatePage, because Starcounter applies middleware
///  from the original request. The default middleware contains <template is="imported-template"></template>
///  instead of <starcounter-include>, which is why we have to wrap it.
/// </summary>

namespace Website {
    partial class WrapperPage : Json
    {
    }
}
