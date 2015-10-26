using Starcounter;

namespace Website.Models {
    [Database]
    public class WebUrl {
        public string Url;
        public WebTemplate Template { get; set; }
    }
}
