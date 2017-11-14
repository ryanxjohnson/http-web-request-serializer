using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture]
    internal class HttpParserTests
    {
        private const string sampleGet =
@"GET https://httpbin.org/get HTTP/1.1
Host: httpbin.org
Connection: keep-alive
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
Upgrade-Insecure-Requests: 1
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9";

        private const string samplePost =
@"POST https://httpbin.org/post HTTP/1.1
Host: httpbin.org
User-Agent: curl/7.54.1
Accept: */*
Content-Length: 10
Content-Type: application/x-www-form-urlencoded
Cookies: ilikecookies:chocchip

helloworld";

        [Test]
        public void Should_Parse_Headers_Get()
        {
            var dictionary = sampleGet.ParseRawRequest();

            Console.WriteLine(dictionary.uri);
            
            foreach (var o in dictionary.headers)
                Console.WriteLine($"{o.Key}: {o.Value}");

            foreach (var o in dictionary.cookies)
                Console.WriteLine($"{o.Key}: {o.Value}");

            Console.WriteLine(dictionary.postData);
        }

        [Test]
        public void Should_Parse_Headers_Post()
        {
            var dictionary = samplePost.ParseRawRequest();

            Console.WriteLine(dictionary.uri);

            foreach (var o in dictionary.headers)
                Console.WriteLine($"{o.Key}: {o.Value}");

            foreach (var o in dictionary.cookies)
                Console.WriteLine($"{o.Key}: {o.Value}");

            Console.WriteLine(dictionary.postData);
        }

        [Test]
        public void Should_Parse_PostData()
        {
            samplePost.TryParsePostDataString(out string postData);
            Assert.AreEqual("helloworld", postData);
        }

        [Test]
        public void Should_Parse_Cookies()
        {
            var expected = new Dictionary<string, string>
            {
                { "ilikecookies", "chocchip" }
            };

            samplePost.TryParseCookies(out Dictionary<string, string> results);

            foreach (var kv in expected)
            {
                foreach (var kv1 in results)
                {
                    Assert.AreEqual(kv.Key, kv1.Key);
                    Assert.AreEqual(kv.Value, kv1.Value);
                }
            }
        }

        [Test]
        public void Should_Handle_No_Cookies()
        {
            var cookies = sampleGet.TryParseCookies(out Dictionary<string, string> cookieDictionary);

            if (!cookies) return;

            foreach (var cookie in cookieDictionary)
                Console.WriteLine($"{cookie.Key}:{cookie.Value}");
        }

        [Test]
        public void Should_Parse_Headers_And_Build_Http_Request()
        {
            //var result = sampleGet.ParseHeaders();
            var result = samplePost.ParseRawRequest();

            var req = (HttpWebRequest) WebRequest.Create(result.uri);

            foreach (var kv in result.headers)
                req.SetHeader(kv.Key, (string)kv.Value);

            if (req.Method == "POST")
                req.WritePostDataToRequestStream(result.postData);

            req.CookieContainer = new CookieContainer();
            foreach (var cookie in result.cookies)
                req.CookieContainer.Add(new Uri(result.uri), new Cookie(cookie.Key, cookie.Value));

            Console.WriteLine(req.GetResponseString());
        }
    }
}
