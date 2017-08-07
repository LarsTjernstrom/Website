using Starcounter;
using WebsiteEditor.ViewModels;

namespace WebsiteEditor.Helpers
{
    public class SessionHelper
    {
        public static MasterPage GetMasterPage(Json partial = null)
        {
            MasterPage master = Session.Ensure().Store[nameof(MasterPage)] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Store[nameof(MasterPage)] = master;
            }

            if (partial != null)
            {
                master.CurrentPage = partial;
            }

            return master;
        }
    }
}
