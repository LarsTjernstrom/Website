using System;
using System.Collections.Generic;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Routing;
using WebsiteEditor.Helpers;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor.Api.Authorization
{
    public class MasterPageMiddleware : IPageMiddleware
    {
        private readonly Dictionary<Type, string> currentPageMapper = new Dictionary<Type, string>
        {
            {typeof(SurfaceGroupsPage), "/websiteeditor/partials/surfacegroups"},
            {typeof(GeneralPage), "/websiteeditor/partials/general"},
            {typeof(BlendingPointsPage), "/websiteeditor/partials/blendingpoints"},
            {typeof(CatchingRulesPage), "/websiteeditor/partials/catchingrules"}
        };

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            MasterPage master = SessionHelper.GetMasterPage();

            master.CurrentPage = next();

            var isSurfaceGroupsPage = master.CurrentPage is SurfaceGroupsPage;
            if (!isSurfaceGroupsPage)
            {
                if (routingInfo.Arguments.Any())
                {
                    var surfaceKey = routingInfo.Arguments?[0];
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", surfaceKey).FirstOrDefault();
                }
            }

            master.ShowNavigation = !isSurfaceGroupsPage;
            master.RefreshCurrentPage(currentPageMapper[master.CurrentPage.GetType()]);

            return master;
        }
    }
}
