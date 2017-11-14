using System.IO;
using System.Net;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture]
    class RequestDisassemblerTests
    {
        private const string headers1 =
            @"GET http://www.example.com/ HTTP/1.1
Host: www.example.com
Connection: keep-alive
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8";

        [Test, Ignore("Scratchpad")]
        public void Should_Get_Dictionary_Given_From_Web_Request()
        {
            // what we see in Fiddler
            var req = (HttpWebRequest) WebRequest.Create("http://www.example.com/");
            req.Method = "GET";
            req.Host = "www.example.com";
            req.KeepAlive = true;
            req.Headers.Add("Upgrade-Insecure-Requests", "1");
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            req.Headers.Add("Accept-Encoding", "gzip, deflate");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.8");

            // take the http web request apart
            var result = req.DisassembleWebRequest();

            // turn it into a json string
            var serializedRequest = result.SerializeRequestProperties();

            // turn the json string into a .NET dictionary
            var d = serializedRequest.DeserializeRequestProperties();

            // build the request from the dictionary
            var req2 = (HttpWebRequest) WebRequest.Create((string)d["url"]);
            foreach (var kv in d)
            {
                req2.SetHeader(kv.Key, (string)kv.Value);
            }

            var html1 = req.GetResponseFromGzip();
            var html2 = req2.GetResponseFromGzip();

            Assert.AreEqual(html1, html2);
        }

        // the whole shebang
        [Test, Ignore("Scratchpad")]
        public void Should_Parse_Headers_Build_Request_Serialize_Deserialize_And_Perform_Request()
        {
            var headersDictionary = headers1.ParseRawRequest();
            var req = (HttpWebRequest) WebRequest.Create(headersDictionary.uri);
            foreach (var kv in headersDictionary.headers)
            {
                req.SetHeader(kv.Key, (string)kv.Value);
            }

            // take the http web request apart
            var result = req.DisassembleWebRequest();

            // turn it into a json string
            var serializedRequest = result.SerializeRequestProperties();

            // turn the json string into a .NET dictionary
            var d = serializedRequest.DeserializeRequestProperties();

            // build the request from the dictionary
            var req2 = (HttpWebRequest)WebRequest.Create((string)d["url"]);
            foreach (var kv in d)
            {
                req2.SetHeader(kv.Key, (string)kv.Value);
            }

            var html = req2.GetResponseFromGzip();

            File.WriteAllText(@"c:\users\rjohnson\desktop\example2.html", html);
        }
    }
}
