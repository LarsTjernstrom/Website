using System;
using Starcounter;

namespace Content {
    public class DataHelper {
        public const string UrlPrefix = "/content/dynamic";

        public void ClearData() {
            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Content.ContentEntry");
                Db.SlowSQL("DELETE FROM Content.ContentItem");
            });
        }

        public void GenerateData() {
            if (Db.SQL("SELECT p FROM Content.ContentEntry p").First != null) {
                return;
            }

            Db.Transact(() => {
                ClearData();

                new ContentEntry() {
                    Url = "/apps"
                };

                new ContentEntry() {
                    Url = "/apps/wanted-apps"
                };

                new ContentEntry() {
                    Url = "/userprofile"
                };

                new ContentItem() {
                    Url = "/navigation",
                    HtmlPath = "/NavigationPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/index/header",
                    HtmlPath = "/index/HeaderPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/index/left",
                    HtmlPath = "/index/LeftPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/index/right",
                    HtmlPath = "/index/RightPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/index/registration",
                    HtmlPath = "/index/RegistrationPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/index/footer",
                    HtmlPath = "/index/FooterPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/apps/header",
                    HtmlPath = "/apps/HeaderPage.html",
                    Protected = true
                };

                new ContentItem() {
                    Url = "/apps/footer",
                    HtmlPath = "/apps/FooterPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/userprofile/header",
                    HtmlPath = "/userprofile/HeaderPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/userprofile/footer",
                    HtmlPath = "/userprofile/FooterPage.html",
                    Protected = false
                };
            });
        }
    }
}
