using Starcounter;

namespace WebsiteEditor.Api
{
    internal class OntologyMap
    {
        public void Register()
        {
            Blender.MapUri("/websiteeditor/app-name", "app-name");

        }
    }
}
