using System;
using Starcounter;

namespace WebsiteProvider
{
    class Program
    {
        static void Main()
        {
            ContentHandlers content = new ContentHandlers();

            content.Register();
        }
    }
}