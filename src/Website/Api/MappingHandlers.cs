using Starcounter;

namespace Website.Api
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/website/app-name", () => new AppName());
        }
    }
}
