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
                Url = @"http://www.google.com"
            };

            var req = parsed.CreateWebRequestFromParsedRequest();
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

            var req = parsed.CreateWebRequestFromParsedRequest(CallBack);
            Console.WriteLine("Built Request");
        }

        [Test]
        public void Should_Parse_Build_Request_CallBack()
        {
            
        }

        private static void CallBack(HttpWebRequest req, string postData)
        {
            Console.WriteLine("Adding custom headers...");
            Console.WriteLine($"Adding request body to {req.RequestUri}");
            req.WritePostDataToRequestStream(postData);
        }
    }
}
