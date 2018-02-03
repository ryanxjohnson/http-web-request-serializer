using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture]
    public class RequestBuilderTests
    {
        [Test]
        public void Should_Build_Http_Request()
        {
            var parsed = new ParsedRequest
            {
                Url = @"http://www.google.com",
                Headers = new Dictionary<string, object> { { "Method", "POST" } },
                Cookies = null,
                RequestBody = "request data"
            };

            var req = parsed.CreateWebRequestFromParsedRequest();

            Assert.AreEqual(4, req.Headers.Count);
        }

        [Test]
        public void Should_Build_Http_Request_With_Callback()
        {
            var parsed = new ParsedRequest
            {
                Url = @"http://www.google.com",
                Headers = new Dictionary<string, object>{{"Method", "POST"}},
                Cookies = null,
                RequestBody = "request data"
            };

            var req = parsed.CreateWebRequestFromParsedRequest(AddDynamicHeaders);

            Assert.AreEqual(5, req.Headers.Count);
        }

        private static void AddDynamicHeaders(HttpWebRequest req)
        {
            Console.WriteLine("Adding custom headers...");
            req.Headers.Add("Key", "Value");
        }
    }
}
