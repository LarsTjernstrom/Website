using System;
using Starcounter;
using Website.Api;

namespace Website {
    class Program {

        static void Main() {
            DataHelper data = new DataHelper();
            MappingHandlers mapping = new MappingHandlers();
            MainHandlers main = new MainHandlers();
            OntologyMap ontology = new OntologyMap();
            CommitHooks hooks = new CommitHooks();

            data.GenerateData();
            mapping.Register();
            main.Register();
            ontology.Register();
            hooks.Register();
        }
    }
}