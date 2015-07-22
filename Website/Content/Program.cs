using System;
using Starcounter;
using Content.ViewModels;

namespace Content {
    class Program {
        static void Main() {
            Handle.GET("/content/center", () => {
                AdPage page = new AdPage();

                page.Html = "<div>An AD goes here!</div>";

                return page;
            });

            Handle.GET("/content/center/{?}", (string Key) => {
                AdPage page = new AdPage();
                string message = null;

                switch (Key) { 
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

                page.Html = "<div>" + message + "</div>";

                return page;
            });

            PolyjuiceNamespace.Polyjuice.Map("/content/center", "/polyjuice/default/center");
            PolyjuiceNamespace.Polyjuice.Map("/content/center/@w", "/polyjuice/side/center/@w");
        }
    }
}