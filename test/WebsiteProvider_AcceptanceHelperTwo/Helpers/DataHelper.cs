using System;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    public class DataHelper
    {
        public void GenerateData()
        {
            var launcherTemplate = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", "LauncherTemplate").First;
            if (launcherTemplate == null)
            {
                throw new Exception("Website surfaces is not found.");
            }

            Db.Transact(() =>
            {
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperTwo").First ??
                         new WebUrl
                         {
                             Template = launcherTemplate,
                             Url = "/WebsiteProvider_AcceptanceHelperTwo",
                             IsFinal = true
                         };
            });
        }
    }
}