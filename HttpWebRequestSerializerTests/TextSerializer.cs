using System;
using HttpWebRequestSerializer;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture()]
    public class TextSerializer
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

        private const string serializedGet = @"{""Uri"":""https://httpbin.org/get"",""Headers"":{""Method"":""GET"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""Connection"":""keep-alive"",""User-Agent"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36"",""Upgrade-Insecure-Requests"":""1"",""Accept"":""text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"",""Accept-Encoding"":""gzip, deflate, br"",""Accept-Language"":""en-US,en;q=0.9""},""Cookie"":{},""Data"":null}";

        private const string samplePost =
            @"POST https://httpbin.org/post HTTP/1.1
Host: httpbin.org
User-Agent: curl/7.54.1
Accept: */*
Content-Length: 10
Content-Type: application/x-www-form-urlencoded
Cookie: ilikecookies=chocchip

helloworld";

        private const string sampleRequestWithExpect =
            @"POST http://www.example.com/ HTTP/1.1
User-Agent: curl/7.54.1
Accept: */*
Content-Type: application/x-www-form-urlencoded
Host: www.example.com
Content-Length: 10
Expect: 100-continue
Connection: Keep-Alive

helloworld";

        private const string samplePostNoCarriageReturn = @"POST https://httpbin.org/post HTTP/1.1\nHost: httpbin.org\nUser-Agent: curl/7.54.1\nAccept: */*\nContent-Length: 10\nContent-Type: application/x-www-form-urlencoded\nCookie: ilikecookies=chocchip\n\nhelloworld";

        private const string serializedPost = @"{""Uri"":""https://httpbin.org/post"",""Headers"":{""Method"":""POST"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""User-Agent"":""curl/7.54.1"",""Accept"":""*/*"",""Content-Length"":""10"",""Content-Type"":""application/x-www-form-urlencoded""},""Cookie"":{""ilikecookies"":""chocchip""},""Data"":""helloworld""}";
        private const string serializedPostNoCookieNoData = "{\"Uri\":\"https://httpbin.org/post\",\"Headers\":{\"Method\":\"POST\",\"HttpVersion\":\"HTTP/1.1\",\"Host\":\"httpbin.org\",\"User-Agent\":\"curl/7.54.1\",\"Accept\":\"*/*\",\"Content-Length\":\"10\",\"Content-Type\":\"application/x-www-form-urlencoded\"}}";

        [Test]
        public void Should_Serlialize_Raw_Request_Get()
        {
            var json = HttpParser.GetRawRequestAsJson(sampleGet);
            Assert.AreEqual(serializedGet, json);
        }

        [TestCase(samplePost, serializedPost)]
        [TestCase(samplePostNoCarriageReturn, serializedPost)]
        public void Should_Serlialize_Raw_Request_Post(string sample, string expected)
        {
            var json = HttpParser.GetRawRequestAsJson(sample);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Should_Not_Serialize_Data_Or_Cookies()
        {
            var so = new SerializationOptions();
            so.IgnoreKey("Cookie");
            so.IgnoreKey("Data");
            var json = HttpParser.GetRawRequestAsJson(samplePost, so);

            Assert.AreEqual(serializedPostNoCookieNoData, json);
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Get()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedGet);
            Assert.AreEqual("System.Net.HttpWebRequest", req.GetType().ToString());
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Post()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedPost);
            Assert.AreEqual("System.Net.HttpWebRequest", req.GetType().ToString());
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("gibberish \n falksfjl;")]
        public void Should_Handle_Invalid_Request(string input)
        {
            var ex = Assert.Throws<Exception>(() => HttpParser.GetRawRequestAsJson(input));
            Assert.AreEqual("Invalid Request", ex.Message);
        }
    }
}