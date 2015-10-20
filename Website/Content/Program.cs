using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Simplified.Ring3;
using Simplified.Ring5;

namespace Content {
    class Program {
        static void Main() {
            MainHandlers handlers = new MainHandlers();
            ContentHandlers content = new ContentHandlers();
            CommitHooks hooks = new CommitHooks();
            DataHelper data = new DataHelper();

            data.GenerateData();
            handlers.Register();
            content.Register();
            hooks.Register();
        }
    }
}