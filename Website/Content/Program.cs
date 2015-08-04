using System;
using System.Linq;
using Starcounter;
using Content.ViewModels;
using Content.Models;

namespace Content {
    class Program {
        static void Main() {
            GenerateData();

            Handle.GET("/content", () => {
                return new Page();
            });

            Handle.GET("/content/navigation", () => {
                NavigationPage page = new NavigationPage();

                page.Html = "/Content/viewmodels/NavigationPage.html";

                return page;
            });

            Handle.GET("/content/team", () => {
                TeamPage page = new TeamPage();

                page.People.Data = Db.SQL<Person>("SELECT p FROM Content.Models.Person p");
                page.Html = "/Content/viewmodels/TeamPage.html";

                return page;
            });

            Handle.GET("/content/team-ad/{?}", (string name) => {
                var people = Db.SQL<Person>("SELECT p FROM Content.Models.Person p");
                TeamAdPage page = new TeamAdPage();

                page.Html = "/Content/viewmodels/TeamAdPage.html";

                foreach (Person p in people) {
                    var pj = page.People.Add();

                    pj.Text = p.AdText;
                    pj.Key = p.Key;
                    pj.Url = string.Format("/content/team-ad/{0}", p.Key);
                    pj.Selected = p.Key == name;
                }

                return page;
            });

            Handle.GET("/content/team/{?}", (string name) => {
                Person person = GetPerson(name);
                PersonPage page = new PersonPage();

                page.Html = "/Content/viewmodels/PersonPage.html";
                page.Data = person;

                return page;
            });

            Handle.GET("/content/description/{?}", (string name) => {
                Person person = GetPerson(name);
                DescriptionPage page = new DescriptionPage();

                page.Html = "/Content/viewmodels/DescriptionPage.html";
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
                //return;
            }

            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Content.Models.Person");

                person = new Person() {
                    Key = "konstantin",
                    FirstName = "Konstantin",
                    LastName = "Mi",
                    Url = "/content/team/konstantin",
                    AdText = "Left side AD for Konstantin",
                    Description = "Is a new developer of Starcounter team."
                };

                person = new Person() {
                    Key = "tomek",
                    FirstName = "Tomek",
                    LastName = "Wytrębowicz",
                    Url = "/content/team/tomek",
                    AdText = "Left side AD for Tomek",
                    Description = "Is a JavaScript developer of Starcounter team."
                };

                person = new Person() {
                    Key = "marcin",
                    FirstName = "Marcin",
                    LastName = "Warpechowski",
                    Url = "/content/team/marcin",
                    AdText = "Left side AD for Marcin",
                    Description = "Is a team leader of Starcounter team."
                };
            });
        }
    }
}