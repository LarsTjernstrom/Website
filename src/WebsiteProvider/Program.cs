using System;
using Starcounter;

namespace WebsiteProvider
{
    class Program
    {
        static void Main()
        {
            var mapping = new MappingHandlers();
            var content = new ContentHandlers();

            mapping.Initialize();
            content.Register();
        }
    }
}