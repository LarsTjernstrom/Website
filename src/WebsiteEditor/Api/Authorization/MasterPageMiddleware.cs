using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using Starcounter.Authorization.Routing;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor.Api.Authorization
{
    public class MasterPageMiddleware : IPageMiddleware
    {
        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            MasterPage master = Session.Ensure().Store[nameof(MasterPage)] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Store[nameof(MasterPage)] = master;
            }

            master.CurrentPage = next();

            return master;
        }
    }
}
