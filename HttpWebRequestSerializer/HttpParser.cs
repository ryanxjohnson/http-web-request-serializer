using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class HttpParser
    {
        public static string GetRawRequestAsJson(string request, SerializationOptions so = null)
        {
            try
            {
                return GetRawRequestAsDictionary(request).SerializeRequestProperties(so);
            }
            catch (Exception)
            {
                // TODO: Make this a custom exception
                throw new Exception("Invalid Request");
            }
        }

        public static IDictionary<string, object> GetRawRequestAsDictionary(string request, SerializationOptions so = null)
        {
            var parsed = request.ParseRawRequest();

            var dict =  new Dictionary<string, object>
            {
                { "Uri", parsed.uri },
                { "Headers", parsed.headers },
                { "Cookie", parsed.cookies },
                { "Data", parsed.data }
            };

            if (so?.DoNotSerialize == null)
                return dict;

            foreach (var s in so.DoNotSerialize)
                dict.Remove(s);

            return dict;
        }

        public static (string uri, IDictionary<string, object> headers, IDictionary<string, object> cookies, string data) ParseRawRequest(this string request)
        {
            var parsedRequest = request.Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);

            var requestLine = parsedRequest[0].ParseRequestLine();
            var headers = new Dictionary<string, object>
            {
                ["Method"] = requestLine.method,
                ["HttpVersion"] = requestLine.httpVersion
            };

            var blankIndex = Array.IndexOf(parsedRequest, "");
            var indexToUse = blankIndex == -1 ? parsedRequest.Length : blankIndex - 1;

            for (var i = 1; i < indexToUse; i++)
            {
                var header = parsedRequest[i].Split(':');
                headers[header[0].CleanHeader()] = header[1].CleanHeader();
            }

            headers.Remove("Cookie"); // should be serialized on its own

            IDictionary<string, object> cookies;
            var cookieIndex = Array.FindLastIndex(parsedRequest, s => s.StartsWith("Cookie"));
            if (cookieIndex > 0)
            {
                parsedRequest[cookieIndex].TryParseCookies(out IDictionary<string, object> c);
                cookies = c;
            }
            else
            {
                request.TryParseCookies(out IDictionary<string, object> c);
                cookies = c;
            }

            string data = null;
            if ((string)headers["Method"] == "POST")
            {
                var postDataIndex = Array.FindIndex(parsedRequest, s => s == "");
                if (postDataIndex > 0)
                    request.TryParsePostDataString(out data);
            }
            else
            {
                var queryString = request.Contains('?') ? requestLine.url.Split('?')[1] : null;
                data = queryString;
            }
            
            return ((string uri, IDictionary<string, object> headers, IDictionary<string, object> cookies, string data)) (requestLine.url, headers, cookies, data);
        }

        public static bool TryParseCookies(this string cookieString, out IDictionary<string, object> cookieDictionary)
        {
            var matches = new Regex(@"Cookie:(?<Cookie>(.+))", RegexOptions.Singleline).Match(cookieString);
            var cookies = matches.Groups["Cookie"].ToString().Trim().Split(';');

            if (cookies.Length < 1 || cookies.Contains(""))
            {
                cookieDictionary = new Dictionary<string, object>();
                return false;
            }

            cookieDictionary = new Dictionary<string, object>();

            foreach (var cookie in cookies)
            {
                var key = cookie.Split('=')[0].Trim();
                var value = cookie.Split('=')[1].Trim();
                cookieDictionary[key] = value;
            }

            //cookieDictionary = cookies.ToDictionary(c => c.Split('=')[0].Trim(), c => c.Split('=')[1].Trim());
            return true;
        }

        public static bool TryParsePostDataString(this string request, out string postData)
        {
            var index = request.Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);
            var postDataIndex = index.Length;

            if (postDataIndex == -1)
            {
                postData = "";
                return false;
            }

            postData = index[postDataIndex - 1];
            return true;
        }

        private static (string method, string url, string httpVersion) ParseRequestLine(this string request)
        {
            var firstLine = request.Split(' ');
            return (firstLine[0].CleanHeader(), firstLine[1].CleanHeader(), firstLine[2].CleanHeader());
        }
    }
}
