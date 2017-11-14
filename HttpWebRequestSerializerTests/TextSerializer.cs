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
Cookie: ilikecookies:chocchip

helloworld";

        private const string serializedPost = @"{""Uri"":""https://httpbin.org/post"",""Headers"":{""Method"":""POST"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""User-Agent"":""curl/7.54.1"",""Accept"":""*/*"",""Content-Length"":""10"",""Content-Type"":""application/x-www-form-urlencoded""},""Cookie"":{""ilikecookies"":""chocchip""},""Data"":""helloworld""}";

        [Test]
        public void Should_Serlialize_Raw_Request_Get()
        {
            var json = HttpParser.GetRawRequestAsJson(sampleGet);
            Assert.AreEqual(serializedGet, json);
        }

        [Test]
        public void Should_Serlialize_Raw_Request_Post()
        {
            var json = HttpParser.GetRawRequestAsJson(samplePost);
            Assert.AreEqual(serializedPost, json);
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Get()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedGet);
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Post()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedPost);

            var resp = req.GetResponse();

            resp.CreateObjRef(Type.GetType("req"));
        }
    }
}