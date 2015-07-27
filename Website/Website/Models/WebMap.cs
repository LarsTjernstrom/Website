using Starcounter;

namespace Website.Models {
    [Database]
    public class WebMap {
        public WebSection Section { get; set; }
        public string ForeignUrl { get; set; }
        public int SortNumber { get; set; }
    }
}
