using Starcounter;

namespace Website.Models {
    [Database]
    public class WebPage {
        public string Url;
        public WebTemplate Template { get; set; }
    }
}
