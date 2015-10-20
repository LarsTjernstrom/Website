using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Simplified.Ring3;

namespace Content {
    public class ContentHandlers {
        public void Register() {
            Handle.GET("/content/{?}", (Request req, string query) => {
                ContentEntry entry = Db.SQL<ContentEntry>("SELECT e FROM Content.ContentEntry e WHERE e.Url = ?", req.Uri).First;

                if (entry != null) {
                    return HandleEntry();
                }

                ContentItem item = Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i WHERE i.Url = ?", req.Uri).First;

                if (item != null) {
                    return HandleContentItem(item);
                }

                item = Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i WHERE i.HtmlPath = ?", req.Uri).First;

                if (item != null) {
                    return HandleContentItemHtml(req);
                }

                return 404;
            });
        }


        protected ContentCachePage GetCachePage() {
            if (Session.Current.Data is ContentCachePage) {
                return Session.Current.Data as ContentCachePage;
            }

            ContentCachePage page = new ContentCachePage();
            Session.Current.Data = page;

            return page;
        }

        protected Json HandleEntry() {
            return new Page();
        }

        protected ContentPage HandleContentItem(ContentItem Item) {
            ContentCachePage master = GetCachePage();
            ContentPage page = new ContentPage() {
                Html = Item.HtmlPath,
                Data = null
            };

            master.Pages.Add(page);

            return page;
        }

        protected Response HandleContentItemHtml(Request req) {
            Cookie cookie = GetSignInCookie();
            string token = cookie != null ? cookie.Value : string.Empty;
            Response resp = null;
            ContentItem item = Db.SQL<ContentItem>("SELECT ci FROM Content.ContentItem ci WHERE ci.HtmlPath = ?", req.Uri).First;

            if (item == null) {
                resp = new Response() {
                    Body = "Not found",
                    StatusCode = 404
                };
            } else if (item.Protected && Simplified.Ring3.SystemUser.GetSystemUserByToken(token) == null) {
                Handle.AddOutgoingHeader("ContentApp", DateTime.Now.ToString());

                resp = Handle.ResolveStaticResource("/Content/viewmodels/DenyPage.html", req);
            } else {
                resp = Handle.ResolveStaticResource(req.Uri, req);
            }

            return resp;
        }

        protected Cookie GetSignInCookie() {
            List<Cookie> cookies = Handle.IncomingRequest.Cookies.Select(x => new Cookie(x)).ToList();
            Cookie cookie = cookies.FirstOrDefault(x => x.Name == "soauthtoken");

            return cookie;
        }
    }
}
