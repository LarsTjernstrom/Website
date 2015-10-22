using Starcounter;

namespace Website.Models {
    [Database]
    public class WebUrl {
        public string Url;
        public WebPage Page { get; set; }
    }
}
