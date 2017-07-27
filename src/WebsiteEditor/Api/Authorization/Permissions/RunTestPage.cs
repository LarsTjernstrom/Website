using Starcounter.Authorization.Core;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor.Api.Authorization.Permissions
{
    public class RunTestPage : Permission
    {
        public TestObject TestObject { get; private set; }

        public RunTestPage(TestObject testObject)
        {
            this.TestObject = testObject;
        }
    }
}
