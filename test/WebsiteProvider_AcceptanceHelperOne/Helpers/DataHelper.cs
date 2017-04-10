using System;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class DataHelper
    {
        public void SetDefaultCatchingRules()
        {
            var defaultTemplate = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", "DefaultTemplate").First;
            var sidebarTemplate = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", "SidebarTemplate").First;
            if (defaultTemplate == null || sidebarTemplate == null)
            {
                throw new Exception("Website surfaces is not found.");
            }

            Db.Transact(() =>
            {
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne").First ??
                         new WebUrl
                         {
                             Template = defaultTemplate,
                             Url = "/WebsiteProvider_AcceptanceHelperOne",
                             IsFinal = true
                         };
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne/EmptyPage").First ??
                         new WebUrl
                         {
                             Template = sidebarTemplate,
                             Url = "/WebsiteProvider_AcceptanceHelperOne/EmptyPage",
                             IsFinal = true
                         };
            });
        }

        public void SetNoFinalCatchAllRules()
        {
            Db.Transact(() =>
            {
                var catchAllRules = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true);
                foreach (WebUrl rule in catchAllRules)
                {
                    rule.IsFinal = false;
                }
            });
        }

        public void DeleteCatchAllRules()
        {
            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?)", string.Empty);
            });
        }
    }
}
