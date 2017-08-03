using System;
using Starcounter;
using Starcounter.Authorization.Authentication;
using Starcounter.Authorization.Core;
using Starcounter.Authorization.Core.Rules;
using Starcounter.Authorization.PageSecurity;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;
using WebsiteEditor.Api.Authorization.Permissions;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor
{
    public class AuthorizedHandlers
    {
        public void Register()
        {
            var rules = GetAuthorizationRules();
            var enforcement = new AuthorizationEnforcement(rules, new SystemUserAuthentication());
            // var canI = enforcement.CheckPermission(new RunTestPage(new TestObject()));

            var router = Router.CreateDefault();
            router.AddMiddleware(new SecurityMiddleware(enforcement, info => Response.FromStatusCode(403), PageSecurity.CreateThrowingDeniedHandler<Exception>()));
            router.AddMiddleware(new DbScopeMiddleware(true));
        }

        protected Router CreateApiRouter()
        {
            var router = new Router(info =>
            {
                var partialUri = GetPartialUri(info.Request.Uri);
                return Self.GET(partialUri);
            });

            //router.AddMiddleware(new MasterPageMiddleware());
            //AddSecurityMiddleware(router);

            //router.AddMiddleware(new SecurityMiddleware());

            return router;
        }

        private static AuthorizationRules GetAuthorizationRules()
        {
            var rules = new AuthorizationRules();
            rules.AddRule(new ClaimRule<RunTestPage, SystemUserClaim>((claim, permission) => claim.SystemUser.Name == "admin"));
            return rules;
        }

        private static string GetPartialUri(string apiUri)
        {
            return apiUri.Insert(AuthHelper.AppName.Length + 1, AuthHelper.PartialUriPart);
        }
    }
}
