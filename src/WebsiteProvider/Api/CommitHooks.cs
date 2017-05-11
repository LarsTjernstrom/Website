using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider
{
    internal class CommitHooks
    {
        public void Register()
        {
            Hook<WebMap>.CommitInsert += (s, webMap) =>
            {
                string token = webMap.GetMappingToken();
                Blender.MapUri(webMap.ForeignUrl, token);
            };

            Hook<WebMap>.BeforeDelete += (s, webMap) =>
            {
                string token = webMap.GetMappingToken();
                Blender.UnmapUri(webMap.ForeignUrl, token);
            };

            //Hook<WebMap>.CommitUpdate += (s, webMap) =>
            //{
            //};
        }
    }
}