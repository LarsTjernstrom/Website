using System;
using System.Linq;
using Starcounter;
using Content.ViewModels;
using Content.Models;

namespace Content {
    class Program {
        static void Main() {
            GenerateData();

            Handle.GET("/content/team", () => {
                TeamPage page = new TeamPage();

                page.People.Data = Db.SQL<Person>("SELECT p FROM Content.Models.Person p");
                page.Html = "<ul><template is='dom-repeat' items='{{model.People}}'><li><a href='{{item.Url}}'><span>{{item.FirstName}}</span> <span>{{item.LastName}}</span></a></li></template></ul>" +
                            @"<iframe width=""200"" height=""150"" src=""https://www.youtube.com/embed/Xp7HVA5CPjQ"" frameborder=""0"" allowfullscreen></iframe>";

                return page;
            });

            Handle.GET("/content/team-ad/{?}", (string name) => {
                Person person = GetPerson(name);
                TeamAdPage page = new TeamAdPage();

                page.Html = "<div>{{model.Text}}</div>";
                page.Text = person.AdText;
                page.Key = person.Key;

                return page;
            });

            Handle.GET("/content/team/{?}", (string name) => {
                Person person = GetPerson(name);
                PersonPage page = new PersonPage();

                page.Html = "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>";
                page.Data = person;

                return page;
            });

            Handle.GET("/content/description/{?}", (string name) => {
                Person person = GetPerson(name);
                DescriptionPage page = new DescriptionPage();

                page.Html = "<div>{{model.Text}}</div>";
                page.Key = person.Key;
                page.Text = person.Description;

                return page;
            });
        }

        static Person GetPerson(string Key) {
            return Db.SQL<Person>("SELECT p FROM Content.Models.Person p WHERE p.Key = ?", Key).First;
        }

        static void GenerateData() {
            Person person = Db.SQL<Person>("SELECT p FROM Content.Models.Person p").First;

            if (person != null) {
                return;
            }

            Db.Transact(() => {
                person = new Person() {
                    Key = "konstantin",
                    FirstName = "Konstantin",
                    LastName = "Mi",
                    Url = "/website/team/konstantin",
                    AdText = "Left side AD for Konstantin",
                    Description = "Is a new developer of Starcounter team."
                };

                person = new Person() {
                    Key = "tomek",
                    FirstName = "Tomek",
                    LastName = "Wytrębowicz",
                    Url = "/website/team/tomek",
                    AdText = "Left side AD for Tomek",
                    Description = "Is a JavaScript developer of Starcounter team."
                };

                person = new Person() {
                    Key = "marcin",
                    FirstName = "Marcin",
                    LastName = "Warpechowski",
                    Url = "/website/team/marcin",
                    AdText = "Left side AD for Marcin",
                    Description = "Is a team leader of Starcounter team."
                };
            });
        }
    }
}