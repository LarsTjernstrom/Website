using Starcounter;

namespace WebsiteEditor.Api
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/WebsiteEditor/app-name", () => new AppName());
        }
    }
}
