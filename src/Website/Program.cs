namespace Website
{
    class Program
    {
        static void Main()
        {
            var hooks = new CommitHooks();
            var data = new DataHelper();
            var main = new MainHandlers();

            hooks.Register();
            data.GenerateData();
            main.Register();
        }
    }
}