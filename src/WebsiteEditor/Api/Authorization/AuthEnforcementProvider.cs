using Starcounter.Authorization.Authentication;
using Starcounter.Authorization.Core;
using Starcounter.Authorization.Core.Rules;
using WebsiteEditor.Api.Authorization.Permissions;

namespace WebsiteEditor.Api.Authorization
{
    public static class AuthEnforcementProvider
    {
        private static AuthorizationEnforcement _instance;
        private static readonly object ThreadSync = new object();

        public static AuthorizationEnforcement Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (ThreadSync)
                    {
                        if (_instance == null)
                        {
                            _instance = InitializeAuthEnforcement();
                        }
                    }
                }

                return _instance;
            }
        }

        private static AuthorizationEnforcement InitializeAuthEnforcement()
        {
            var rules = GetAuthorizationRules();
            var enforcement = new AuthorizationEnforcement(rules, new SystemUserAuthentication());

            return enforcement;
        }

        private static AuthorizationRules GetAuthorizationRules()
        {
            var rules = new AuthorizationRules();
            rules.AddRule(new ClaimRule<ShowSurfaceGroups, SystemUserClaim>((claim, permission) => claim.SystemUser.Name == "admin"));
            return rules;
        }
    }
}
