namespace WebsiteEditor
{
    public interface IKnowSurfacePage
    {
        string SurfaceKey { get; set; }
        void RefreshData();
    }
}
