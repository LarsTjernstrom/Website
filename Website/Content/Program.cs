using System;
using System.Linq;
using Starcounter;
using Content.ViewModels;

namespace Content {
    class Program {
        static void Main() {
            Handle.GET("/content/partial/cache", () => {
                if (Session.Current != null && Session.Current.Data != null) {
                    return Session.Current.Data;
                }

                JsonCache page = new JsonCache();

                page.Session = new Session(SessionOptions.PatchVersioning);
                
                return page;
            });

            Handle.GET("/content/partial/content/{?}", (string value) => {
                string[] parts = value.Split(new char[] { '-' });
                string template = parts[0];
                string section = parts[1];

                if (template == "DefaultTemplate" || (template == "SideTemplate" && section == "Side")) {
                    JsonCache cache = GetCache();
                    string key = "/content/center/";
                    var item = cache.Items.FirstOrDefault(x => x.Key == key);
                    AdPage page = item != null ? item.Value as AdPage : null;

                    if (page == null) {
                        page = new AdPage();
                        page.Html = "<div>An AD goes here!</div>";
                        item = cache.Items.Add();
                        item.Key = key;
                        item.Value = page;
                    }

                    return page;
                } else if (template == "SideTemplate" && section == "Center") {
                    string name = parts[2];
                    JsonCache cache = GetCache();
                    string key = "/content/center/" + name;
                    var item = cache.Items.FirstOrDefault(x => x.Key == key);
                    AdPage page = item != null ? item.Value as AdPage : null;
                    string message = null;

                    if (item != null) {
                        return page;
                    }

                    page = new AdPage();

                    switch (name) {
                        case "konstantin":
                            message = "Is a new developer of Starcounter team.";
                            break;
                        case "tomek":
                            message = "Is a JavaScript developer of Starcounter team.";
                            break;
                        case "marcin":
                            message = "Is a team lead of Starcounter team!";
                            break;
                        default:
                            message = "Is a member of Starcounter team.";
                            break;
                    }

                    message += " At a time of: " + DateTime.Now.ToString();
                    page.Html = "<div>" + message + "</div>";
                    item = cache.Items.Add();
                    item.Key = key;
                    item.Value = page;

                    return page;
                }

                return null;
            });

            PolyjuiceNamespace.Polyjuice.Map("/content/partial/content/@w", "/polyjuice/website/partial/content/@w");
        }

        static JsonCache GetCache() {
            return Self.GET<JsonCache>("/content/partial/cache");
        }
    }
}