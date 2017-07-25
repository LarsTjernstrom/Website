using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for correct handling of HTTP Headers for Catching Rules
    /// </summary>
    /// <remarks>
    /// Selenium doesn't provide API for HTTP headers, so it is not used here.
    /// These tests rely on the Website data which is different for different fixtures and tests.
    /// One test case can be incompatible with another one so some tests can't be passed.
    /// So we cannot run them in parallel.
    /// </remarks>
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class CatchingRuleHeadersTests
    {
        [OneTimeSetUp]
        public void SetUpFixture()
        {
            SendRequest(Config.AcceptanceHelperOneUrl + "/ResetData");
            SendRequest(Config.AcceptanceHelperOneUrl + "/SetupCatchingRuleHeadersTests");
        }

        [OneTimeTearDown]
        public void ResetData()
        {
            SendRequest(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        /// <summary>
        /// Make a request without additional headers and check received JSON data for Default Surface
        /// </summary>
        [TestCase(null, "/websiteprovider/surfaces/DefaultSurface.html")]
        public void MakeHttpRequest_WoAdditionalHeaders_DefaultPageisLoaded(string testHeaderValue, string surfacePath)
        {
            RequestPageAndCheckSurface(testHeaderValue, surfacePath);
        }

        /// <summary>
        /// Make a request with test header and check received JSON data for HolyGrail Surface
        /// </summary>
        [TestCase("test-header-value", "/websiteprovider/surfaces/HolyGrailSurface.html")]
        public void MakeHttpRequest_WithTestHeader_AnotherPageisLoaded(string testHeaderValue, string surfacePath)
        {
            RequestPageAndCheckSurface(testHeaderValue, surfacePath);
        }

        /// <summary>
        /// Make a request and check received JSON data for Surface
        /// </summary>
        protected void RequestPageAndCheckSurface(string testHeaderValue, string surfacePath)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(testHeaderValue))
            {
                headers.Add("test-header", testHeaderValue);
            }

            var response = SendRequest(Config.AcceptanceHelperOneUrl + "/SimplePage", "application/json", headers);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            using (response)
            {
                using (var stream = response.GetResponseStream())
                {
                    Assert.NotNull(stream);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var jsonString = reader.ReadToEnd();
                        var json = JObject.Parse(jsonString);
                        Assert.NotNull(json);
                        var htmlField = json["Html"];
                        Assert.NotNull(htmlField);
                        Assert.AreEqual(surfacePath.ToLowerInvariant(), htmlField.Value<string>().ToLowerInvariant());
                    }
                }
            }
        }

        public static HttpWebResponse SendRequest(string url, string accept = null, Dictionary<string, string> headers = null)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            if (accept != null)
            {
                request.Accept = accept;
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }
}