using System;
using Starcounter;
using WebsiteEditor.Helpers;

namespace WebsiteEditor.ViewModels
{
    partial class CmsGeneralPage : Json, IKnowSurfacePage
    {
        public string SurfaceKey { get; set; }
        public void RefreshData()
        {
        }
    }
}
