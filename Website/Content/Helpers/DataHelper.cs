using System;
using Starcounter;

namespace Content {
    public class DataHelper {
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
                    Url = "/content/apps"
                };

                new ContentEntry() {
                    Url = "/content/apps/wanted-apps"
                };

                new ContentEntry() {
                    Url = "/content/userprofile"
                };

                new ContentItem() {
                    Url = "/content/navigation",
                    HtmlPath = "/Content/cms/NavigationPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/index/header",
                    HtmlPath = "/Content/cms/index/HeaderPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/index/left",
                    HtmlPath = "/Content/cms/index/LeftPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/index/right",
                    HtmlPath = "/Content/cms/index/RightPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/index/registration",
                    HtmlPath = "/Content/cms/index/RegistrationPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/index/footer",
                    HtmlPath = "/Content/cms/index/FooterPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/apps/header",
                    HtmlPath = "/Content/cms/apps/HeaderPage.html",
                    Protected = true
                };

                new ContentItem() {
                    Url = "/content/userprofile/header",
                    HtmlPath = "/Content/cms/userprofile/HeaderPage.html",
                    Protected = false
                };

                new ContentItem() {
                    Url = "/content/userprofile/footer",
                    HtmlPath = "/Content/cms/userprofile/FooterPage.html",
                    Protected = false
                };
            });
        }
    }
}
