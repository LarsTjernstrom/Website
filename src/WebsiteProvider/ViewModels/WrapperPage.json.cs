using Starcounter;

namespace WebsiteProvider
{
    /// <summary>
    ///  WrapperPage needs to wrap WebTemplatePage, because Starcounter applies middleware
    ///  from the original request. The default middleware contains &lt;template is="imported-template"&gt;&lt;/template&gt;
    ///  instead of &lt;starcounter-include&gt;, which is why we have to wrap it.
    /// </summary>
    partial class WrapperPage : Json
    {
        public bool IsFinal = false;
        public string RequestUri;

        public void Reset()
        {
            this.Html = "";
            this.WebTemplatePage.Data = null;
        }
    }
}
