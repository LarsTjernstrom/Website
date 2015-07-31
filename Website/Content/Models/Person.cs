using Starcounter;

namespace Content.Models {
    [Database]
    public class Person {
        public string Key;
        public string FirstName;
        public string LastName;
        public string Url;
        public string AdText;
        public string Description;
    }
}
