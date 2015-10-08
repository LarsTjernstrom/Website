using Starcounter;

namespace Content {
    [Database]
    public class ContentItem {
        public string HtmlPath;
        public string JsonData;
        public string Url;

        public string Key {
            get {
                return this.GetObjectID();
            }
        }
    }
}
