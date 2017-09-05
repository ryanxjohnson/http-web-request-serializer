using System.Collections.Generic;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class HttpParser
    {
        public static IDictionary<string, object> ParseHeaders(this string request)
        {
            var dict = new Dictionary<string, object>();
            var parsedHeaders = request.Split('\n');

            var requestLine = ParseRequestLine(parsedHeaders[0]);

            dict["Method"] = requestLine.method;
            dict["RequestUri"] = requestLine.url;
            dict["HttpVersion"] = requestLine.httpVersion;

            for (var i = 1; i < parsedHeaders.Length; i++)
            {
                var header = parsedHeaders[i].Split(':');
                dict[header[0].CleanHeader()] = header[1].CleanHeader();
            }

            return dict;
        }

        private static (string method, string url, string httpVersion) ParseRequestLine(string request)
        {
            var firstLine = request.Split(' ');
            return (firstLine[0].CleanHeader(), firstLine[1].CleanHeader(), firstLine[2].CleanHeader());
        }
    }
}
