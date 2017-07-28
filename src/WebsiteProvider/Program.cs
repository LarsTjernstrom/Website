using WebsiteProvider.Api;

namespace WebsiteProvider
{
    class Program
    {
        static void Main()
        {
            var mapping = new MappingHandlers();
            var content = new ContentHandlers();
            var hooks = new CommitHooks(mapping);

            mapping.Register();
            content.Register();
            hooks.Register();
        }
    }
}