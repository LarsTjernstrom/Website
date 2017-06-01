using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Website.Api
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/website/app-name", () => {
                return new AppName();
            });
        }
    }
}
