using Starcounter;

namespace Content {
    [Database]
    public class ContentEntry {
        public string Url;

        public string Key {
            get {
                return this.GetObjectID();
            }
        }
    }
}
