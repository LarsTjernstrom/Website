﻿using Starcounter;

namespace WebsiteEditor.Api
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/websiteeditor/app-name", () => new AppName());
        }
    }
}
