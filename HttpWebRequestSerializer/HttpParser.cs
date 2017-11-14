using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class HttpParser
    {
        // API Method
        public static string GetRawRequestAsJson(string request)
        {
            return GetRawRequestAsDictionary(request).SerializeRequestProperties();
        }

        public static IDictionary<string, object> GetRawRequestAsDictionary(string request)
        {
            var parsed = request.ParseRawRequest();

            return  new Dictionary<string, object>
            {
                { "Uri", parsed.uri },
                { "Headers", parsed.headers },
                { "Cookie", parsed.cookies },
                { "Data", parsed.postData }
            };
        }

        public static (string uri, Dictionary<string, string> headers, Dictionary<string, string> cookies, string postData) ParseRawRequest(this string request)
        {
            var parsedRequest = request.Split('\n');

            var requestLine = parsedRequest[0].ParseRequestLine();
            var headers = new Dictionary<string, string>
            {
                ["Method"] = requestLine.method,
                ["HttpVersion"] = requestLine.httpVersion
            };

            var lastHeaderIndex = Array.IndexOf(parsedRequest, "\r");
            for (var i = 1; i < (lastHeaderIndex > -1 ? lastHeaderIndex : parsedRequest.Length); i++)
            {
                var header = parsedRequest[i].Split(':');
                headers[header[0].CleanHeader()] = header[1].CleanHeader();
            }

            headers.Remove("Cookie"); // not really a header, should be set in req.CookieContainer
            request.TryParseCookies(out Dictionary<string, string> cookies);
            request.TryParsePostDataString(out string postData);

            return (requestLine.url, headers, cookies, postData);
        }

        public static bool TryParseCookies(this string request, out Dictionary<string, string> cookieDictionary)
        {
            var matches = new Regex(@"Cookie:(?<Cookie>(.+))").Match(request);
            var cookies = matches.Groups["Cookie"].ToString().Trim().Split(';');

            if (cookies.Length < 1 || cookies.Contains(""))
            {
                cookieDictionary = new Dictionary<string, string>();
                return false;
            }

            cookieDictionary = cookies.ToDictionary(c => c.Split(':')[0], c => c.Split(':')[1]);
            return true;
        }

        public static bool TryParsePostDataString(this string request, out string postData)
        {
            var index = request.IndexOf("\r\n\r\n", StringComparison.Ordinal);

            if (index == -1)
            {
                postData = null;
                return false;
            }

            postData = request.Substring(index, request.Length - index).CleanHeader();
            return true;
        }

        private static (string method, string url, string httpVersion) ParseRequestLine(this string request)
        {
            var firstLine = request.Split(' ');
            return (firstLine[0].CleanHeader(), firstLine[1].CleanHeader(), firstLine[2].CleanHeader());
        }
    }
}
