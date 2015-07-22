using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website {
    class Person {
        public Person(string FirstName, string LastName) {
            this.Key = FirstName.ToLower();
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Url = "/website/team/" + this.Key;
        }

        public string Key { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Url { get; set; }
    }
}
