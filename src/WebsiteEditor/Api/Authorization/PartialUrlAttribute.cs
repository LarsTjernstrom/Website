using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteEditor
{
    class PartialUrlAttribute : Attribute
    {
        public string UriPartialVersion { get; }
        public string UriApiVersion { get; }

        public PartialUrlAttribute(string uriPartialVersion)
        {
            UriPartialVersion = uriPartialVersion;
            UriApiVersion = uriPartialVersion.Replace(AuthHelper.PartialUriPart, string.Empty);
        }
    }
}
