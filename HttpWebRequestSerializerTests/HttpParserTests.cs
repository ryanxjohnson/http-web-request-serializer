using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;
using ICSharpCode.SharpZipLib.GZip;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture]
    internal class HttpParserTests
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
//If-None-Match: ""359670651+gzip""
//If-Modified-Since: Fri, 09 Aug 2013 23:54:35 GMT";

        [Test]
        public void Should_Parse_Headers()
        {
            var d = headers1.ParseHeaders();

            foreach (var o in d)
                Console.WriteLine($"{o.Key}: {o.Value}");
        }

        [Test]
        public void Should_Parse_Headers_And_Build_Http_Request()
        {
            var result = headers1.ParseHeaders().BuildBaseHttpWebRequest().GetResponseFromGzip();
            Console.WriteLine(result);
        }
    }
}
