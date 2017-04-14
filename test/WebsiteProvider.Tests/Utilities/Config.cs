using System;
using System.Collections.Generic;

namespace WebsiteProvider.Tests.Utilities
{
    public class Config
    {
        public enum Browser
        {
            Chrome,
            Edge,
            Firefox
        }

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);
        public static readonly Uri RemoteWebDriverUri = new Uri("http://localhost:4444/wd/hub");
        public static readonly Uri AcceptanceHelperOneUrl = new Uri("http://localhost:8080/WebsiteProvider_AcceptanceHelperOne");
        public static readonly Uri AcceptanceHelperTwoUrl = new Uri("http://localhost:8080/WebsiteProvider_AcceptanceHelperTwo");

        public static readonly Dictionary<Browser, string> BrowserDictionary = new Dictionary<Browser, string>
        {
            {Browser.Chrome, "Chrome"},
            {Browser.Edge, "Edge"},
            {Browser.Firefox, "Firefox"}
        };
    }
}