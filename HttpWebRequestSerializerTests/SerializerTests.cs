using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using NUnit.Framework;
using HttpWebRequestSerializer;

namespace HttpWebRequestSerializerTests
{
    [TestFixture]
    public class RequestBuilderTests
    {
        private string json = @"{""Url"":""http://www.google.com"",""Headers"":{""Method"":""get"",""Accept"":""text/html"",""ContentType"":""json"",""Host"":""www.google.com"",""Referer"":""http://www.google.com"",""UserAgent"":""chrome"",""KeepAlive"":""true"",""VerificationToken"":""abcdefg"",""Upgrade-Insecure-Requests"":""1""}}";
        private IDictionary<string, object> requestDictionary;
        private IDictionary<string, object> requestHeaders;

        [SetUp]
        public void Setup()
        {
            // for my purpose headers are unknown until runtime, 
            // so ExpandoObject was a good fit to dynamically create key/value relationship
            requestDictionary = new ExpandoObject();
            requestHeaders = new ExpandoObject();

            requestDictionary["Url"] = "http://www.google.com";
            requestHeaders["Method"] = "get";
            requestHeaders["Accept"] = "text/html";
            requestHeaders["ContentType"] = "json";
            requestHeaders["Host"] = "www.google.com";
            requestHeaders["Referer"] = "http://www.google.com";
            requestHeaders["UserAgent"] = "chrome";
            requestHeaders["KeepAlive"] = "true";
            requestHeaders["VerificationToken"] = "abcdefg";
            requestHeaders["Upgrade-Insecure-Requests"] = "1";

            requestDictionary["Headers"] = requestHeaders.MakeDictionary();
        }

        [Test]
        public void Should_Serialize()
        {
            var result = requestDictionary.SerializeRequestProperties();

            Assert.AreEqual(json, result);
        }

        [Test]
        public void Should_Deserialize()
        {
            var result = json.DeserializeRequestProperties();

            Assert.AreEqual(requestDictionary, result);
        }

        [Test]
        public void Should_Build_Web_Request_From_Dictionary()
        {
            var req = requestHeaders.BuildBaseHttpWebRequest("http://www.google.com");

            var expected = MockRequest();
            Assert.AreEqual(expected.GetType(), req.GetType());
        }

        [Test]
        public void Should_Deserialize_And_Build_Valid_Request()
        {
            var request = json.DeserializeRequestProperties();
            var headers = request["Headers"] as IDictionary<string, object>;

            var req = headers.BuildBaseHttpWebRequest("http://www.google.com");

            var expected = MockRequest();
            Assert.AreEqual(expected.GetType(), req.GetType());
        }

        #region Private Methods
        private HttpWebRequest MockRequest()
        {
            var req = (HttpWebRequest)WebRequest.Create("http://www.google.com");
            req.Method = "GET";
            req.Accept = "text/html";
            req.ContentType = "json";
            req.Host = "www.google.com";
            req.Referer = "http://www.google.com";
            req.UserAgent = "chrome";
            req.KeepAlive = true;
            req.Headers.Add("VerificationToken", "abcdefg");
            req.Headers.Add("Upgrade-Insecure-Requests", "1");

            return req;
        }
        #endregion
    }
}