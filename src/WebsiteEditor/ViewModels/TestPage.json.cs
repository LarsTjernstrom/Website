using Simplified.Ring1;
using Starcounter;

namespace WebsiteEditor.ViewModels
{
    [Database]
    public class TestObject : Something
    {
        public bool IsThisWorking { get; set; }
    }

    [PartialUrl("/websiteeditor/partials/test")]
    partial class TestPage : Json
    {
    }
}
