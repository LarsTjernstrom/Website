using Starcounter;

namespace Website.Models {
    [Database]
    public class WebMap {
        public WebContent Content { get; set; }
        public WebSection Section { get; set; }
    }
}
