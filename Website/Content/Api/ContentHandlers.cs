using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Simplified.Ring3;

namespace Content {
    public class ContentHandlers {
        public void Register() {
            Handle.GET("/content/dynamic/{?}", (Request req, string query) => {
                string url = req.Uri.ToLower().Replace(DataHelper.UrlPrefix, string.Empty);
                ContentEntry entry = Db.SQL<ContentEntry>("SELECT e FROM Content.ContentEntry e WHERE e.Url = ?", url).First;

                if (entry != null) {
                    return HandleEntry();
                }

                ContentItem item = Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i WHERE i.Url = ?", url).First;

                if (item != null) {
                    return HandleContentItem(item);
                }

                item = Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i WHERE i.HtmlPath = ?", url).First;

                if (item != null) {
                    return HandleContentItemHtml(req, item);
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
            string html = DataHelper.UrlPrefix + Item.HtmlPath;
            SystemUser user = GetCurrentSystemUserFromCookie();

            if (Item.Protected && user == null) {
                html = "/Content/viewmodels/EmptyPage.html";
            }

            ContentPage page = new ContentPage() {
                Html = html,
                Data = null
            };

            master.Pages.Add(page);

            return page;
        }

        protected Response HandleContentItemHtml(Request req, ContentItem item) {
            
            Response resp = null;

            if (item == null) {
                resp = new Response() {
                    Body = "Not found",
                    StatusCode = 404
                };
            } else if (item.Protected && GetCurrentSystemUserFromCookie() == null) {
                Handle.AddOutgoingHeader("ContentApp", DateTime.Now.ToString());

                resp = new Response();
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

        protected SystemUser GetCurrentSystemUserFromCookie() {
            Cookie cookie = GetSignInCookie();
            string token = cookie != null ? cookie.Value : string.Empty;

            return SystemUser.GetSystemUserByToken(token);
        }
    }
}
