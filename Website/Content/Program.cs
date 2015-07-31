using System;
using System.Linq;
using Starcounter;
using Content.ViewModels;

namespace Content {
    class Program {
        static void Main() {
            Handle.GET("/content/team", () => {
                AdPage page = new AdPage();

                page.Html = "<ul><template is='dom-repeat' items='{{model.People}}'><li><a href='{{item.Url}}'><span>{{item.FirstName}}</span> <span>{{item.LastName}}</span></a></li></template></ul>" +
                            @"<iframe width=""200"" height=""150"" src=""https://www.youtube.com/embed/Xp7HVA5CPjQ"" frameborder=""0"" allowfullscreen></iframe>";
                page.Value = new Json("{\"People\":[{\"Key\":\"konstantin\",\"FirstName\":\"Konstantin\",\"LastName\":\"Mi\",\"Url\":\"/website/team/konstantin\"},{\"Key\":\"tomek\",\"FirstName\":\"Tomek\",\"LastName\":\"Wytrębowicz\",\"Url\":\"/website/team/tomek\"},{\"Key\":\"marcin\",\"FirstName\":\"Marcin\",\"LastName\":\"Warpechowski\",\"Url\":\"/website/team/marcin\"}]}");

                return page;
            });

            Handle.GET("/content/team-ad/{?}", (string name) => {
                AdPage page = new AdPage();

                page.Html = "<div>{{model.Text}}</div>";

                switch (name) {
                    case "konstantin":
                        page.Value = new Json("{\"Key\":\"konstantin\",\"Text\":\"Left side AD for Konstantin\"}");
                        break;
                    case "tomek":
                        page.Value = new Json("{\"Key\":\"tomek\",\"Text\":\"Left side AD for Tomek\"}");
                        break;
                    case "marcin":
                        page.Value = new Json("{\"Key\":\"marcin\",\"Text\":\"Left side AD for Marcin\"}");
                        break;
                }

                return page;
            });

            Handle.GET("/content/team/{?}", (string name) => {
                AdPage page = new AdPage();

                page.Html = "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>";
                
                switch (name) {
                    case "konstantin":
                        page.Value = new Json("{\"Key\":\"konstantin\",\"FirstName\":\"Konstantin\",\"LastName\":\"Mi\",\"Url\":\"/website/team/konstantin\"}");
                        break;
                    case "tomek":
                        page.Value = new Json("{\"Key\":\"tomek\",\"FirstName\":\"Tomek\",\"LastName\":\"Wytrębowicz\",\"Url\":\"/website/team/tomek\"}");
                        break;
                    case "marcin":
                        page.Value = new Json("{\"Key\":\"marcin\",\"FirstName\":\"Marcin\",\"LastName\":\"Warpechowski\",\"Url\":\"/website/team/marcin\"}");
                        break;
                }

                return page;
            });

            Handle.GET("/content/description/{?}", (string name) => {
                AdPage page = new AdPage();

                page.Html = "<div>{{model.Text}}</div>";

                switch (name) {
                    case "konstantin":
                        page.Value = new Json("{\"Key\":\"konstantin\",\"Text\":\"Is a new developer of Starcounter team.\"}");
                        break;
                    case "tomek":
                        page.Value = new Json("{\"Key\":\"tomek\",\"Text\":\"Is a JavaScript developer of Starcounter team.\"}");
                        break;
                    case "marcin":
                        page.Value = new Json("{\"Key\":\"marcin\",\"Text\":\"Is a team leader of Starcounter team.\"}");
                        break;
                }

                return page;
            });
        }
    }
}