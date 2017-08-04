using System;
using Starcounter;
using Starcounter.Authorization.PageSecurity;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;

namespace WebsiteEditor.Api.Authorization
{
    public class AuthorizedHandlers
    {
        public void Register()
        {
            var router = Router.CreateDefault();
            router.AddMiddleware(new ContextMiddleware());
            router.AddMiddleware(new MasterPageMiddleware());
            router.AddMiddleware(new SecurityMiddleware(AuthEnforcementProvider.Instance, info => Response.FromStatusCode(403), PageSecurity.CreateThrowingDeniedHandler<Exception>()));
            router.AddMiddleware(new DbScopeMiddleware(true));
            router.RegisterAllFromCurrentAssembly();
        }
    }
}
