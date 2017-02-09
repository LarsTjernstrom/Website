using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Website
{
    public class MapHandlers
    {
        public void Register()
        {
            UriMapping.Map("/Website/user", UriMapping.MappingUriPrefix + "/user");
        }
    }
}
