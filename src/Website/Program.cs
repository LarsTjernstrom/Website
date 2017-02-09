using System;
using Starcounter;

namespace Website {
    class Program {

        static void Main() {
            DataHelper data = new DataHelper();
            MainHandlers main = new MainHandlers();
            ContentHandlers content = new ContentHandlers();
            CommitHooks hooks = new CommitHooks();
            MapHandlers map = new MapHandlers();

            data.GenerateData();
            main.Register();
            content.Register();
            hooks.Register();
            map.Register();
        }
    }
}