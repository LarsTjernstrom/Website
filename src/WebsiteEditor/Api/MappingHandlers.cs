using Starcounter;

namespace WebsiteEditor
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/websiteeditor/app-name", () => new AppName());
        }
    }
}
