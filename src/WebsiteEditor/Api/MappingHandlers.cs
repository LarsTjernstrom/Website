﻿using Starcounter;

namespace WebsiteEditor
{
    internal class MappingHandlers
    {
        public void Register()
        {
            Handle.GET("/WebsiteEditor/app-name", () => new AppName());
        }
    }
}