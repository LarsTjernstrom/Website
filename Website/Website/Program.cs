using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Website.ViewModels;

namespace Website {
    class Program {
        static List<Person> people = new List<Person>() {
            new Person("Konstantin", "Mi"),
            new Person("Tomek", "Wytrębowicz"),
            new Person("Marcin", "Warpechowski")
        };

        static void Main() {
            Handle.GET("/website", () => {
                LayoutPage master = GetLayoutPage();

                master.TemplateContent = null;

                return master;
            });

            Handle.GET("/website/team", () => {
                LayoutPage master = GetLayoutPage();
                dynamic content = master.TemplateContent;

                if (content == null || master.TemplateName != "Default") {
                    content = new Json();
                    content.Sections = "Center";
                    //content.Center = GetTeamPage();
                    content.Center = (Json)Self.GET("/website/partial/team");

                    //System.Exception: Cannot add the Center property to the template as the type Response is not supported for Json properties
                    //content.Center = Self.GET("/website/partial/team");

                    master.TemplateName = "Default";
                    //master.TemplateHtml = @"<puppet-import model=""{{model.Center.Value}}"" path=""model.Center.Value"" html=""{{model.Center.Html}}""></puppet-import>";
                    master.TemplateHtml = @"<website-section model=""{{model.Center}}"" path=""model.Content.Center""></website-section>";
                    master.TemplateContent = content;
                } else {
                    content.Center.Value = GetTeamValue();
                }
                
                return master;
            });

            Handle.GET("/website/team/{?}", (string Key) => {
                LayoutPage master = GetLayoutPage();
                dynamic content = master.TemplateContent;
                
                if (content == null || master.TemplateName != "Side") {
                    content = new Json();
                    content.Sections = "Side,Center";
                    //content.Side = GetTeamPage();
                    //content.Center = GetMemberPage(Key);
                    content.Side = (Json)Self.GET("/website/partial/team");
                    content.Center = (Json)Self.GET("/website/partial/team/" + Key);

                    master.TemplateName = "Side";
                    //master.TemplateHtml =
                    //    @"<div class=""side""><puppet-import model=""{{model.Side.Value}}"" path=""model.Side.Value"" html=""{{model.Side.Html}}""></puppet-import></div>" +
                    //    @"<div class=""center""><puppet-import model=""{{model.Center.Value}}"" path=""model.Center.Value"" html=""{{model.Center.Html}}""></puppet-import></div>";
                    master.TemplateHtml =
                        @"<div class=""side""><website-section model=""{{model.Side}}"" path=""model.Content.Side""></website-section></div>" +
                        @"<div class=""center""><website-section model=""{{model.Center}}"" path=""model.Content.Center""></website-section></div>";
                    master.TemplateContent = content;
                } else {
                    //content.Center.Value = GetMemberValue(Key);
                    content.Center = (Json)Self.GET("/website/partial/team/" + Key);
                }

                return master;
            });

            Handle.GET("/website/partial/layout", () => {
                LayoutPage page = null;

                if (Session.Current != null) {
                    page = Session.Current.Data as LayoutPage;

                    return Session.Current.Data;
                }

                Session.Current = new Session(SessionOptions.PatchVersioning);

                page = new LayoutPage();
                page.Session = Session.Current;

                return page;
            });

            Handle.GET("/website/partial/team", () => {
                return GetTeamPage();
            });

            Handle.GET("/website/partial/team/{?}", (string Name) => {
                return GetMemberPage(Name);
            });

            PolyjuiceNamespace.Polyjuice.Map("/website/partial/team", "/polyjuice/default/center");
            //PolyjuiceNamespace.Polyjuice.Map("/website/partial/team", "/polyjuice/side/side");
            PolyjuiceNamespace.Polyjuice.Map("/website/partial/team/@w", "/polyjuice/side/center/@w");
        }

        static string GetTeamHtml() {
            return "<ul><template is='dom-repeat' items='{{model.People}}'><li><a href='{{item.Url}}'><span>{{item.FirstName}}</span> <span>{{item.LastName}}</span></a></li></template></ul>" +
                        @"<iframe width=""200"" height=""150"" src=""https://www.youtube.com/embed/Xp7HVA5CPjQ"" frameborder=""0"" allowfullscreen></iframe>";
        }

        static Json GetTeamValue() {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return new Json(serializer.Serialize(new { People = people.Select(x => x.ToJson()) }));
        }

        static ContainerPage GetTeamPage() {
            ContainerPage page = new ContainerPage() {
                Html = GetTeamHtml(),
                Value = GetTeamValue()
            };

            return page;
        }

        static string GetMemberHtml() {
            return "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>";
        }

        static Json GetMemberValue(string Key) {
            Person person = people.FirstOrDefault(x => x.Key == Key);
            dynamic json = new Json();

            json.FirstName = person.FirstName;
            json.LastName = person.LastName;
            //json.Person = person;

            return json;
        }

        static ContainerPage GetMemberPage(string Key) {
            ContainerPage page = new ContainerPage() {
                Html = GetMemberHtml(),
                Value = GetMemberValue(Key)
            };

            return page;
        }

        static LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }
    }
}