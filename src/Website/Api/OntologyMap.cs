using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Website.Api
{
    internal class OntologyMap
    {
        public void Register()
        {
            UriMapping.Map("/website/app-name", UriMapping.MappingUriPrefix + "/app-name");
        }
    }
}
