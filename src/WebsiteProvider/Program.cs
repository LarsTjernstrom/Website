using WebsiteProvider.Api;

namespace WebsiteProvider
{
    class Program
    {
        static void Main()
        {
            var pinningHandlers = PinningHandlers.GetInstance();
            var contentHandlers = new ContentHandlers();
            var commitHooks = new CommitHooks(pinningHandlers);

            pinningHandlers.Register();
            contentHandlers.Register();
            commitHooks.Register();
        }
    }
}