namespace WebsiteEditor.Helpers
{
    public interface IKnowSurfacePage
    {
        string SurfaceKey { get; set; }
        void RefreshData();
    }
}
