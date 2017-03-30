using System;

namespace WebsiteProvider.Tests.Utilities
{
    public class Config
    {
        public enum Browser
        {
            Chrome,
            //Edge,
            Firefox
        }

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);
    }
}