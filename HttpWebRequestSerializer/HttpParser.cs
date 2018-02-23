using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class HttpParser
    {
        public static ParsedRequest GetParsedRequest(string request, IgnoreSerializationOptions so = null)
        {
            var (uri, headers, cookies, data) = request.ParseRawRequest();

            if (so != null)
            {
                foreach (var option in so.DoNotSerialize)
                {
                    switch (option)
                    {
                        case "Uri":
                            uri = null;
                            break;
                        case "Headers":
                            headers = null;
                            break;
                        case "Data":
                            data = null;
                            break;
                        case "Cookie":
                            cookies = null;
                            break;
                    }
                }
            }

            return new ParsedRequest
            {
                Url = uri,
                Headers = headers,
                Cookies = cookies,
                RequestBody = data
            };
        }

        public static string GetRawRequestAsJson(string request, IgnoreSerializationOptions so = null)
        {
            try
            {
                return GetRawRequestAsDictionary(request).SerializeRequestProperties(so);
            }
            catch (Exception e)
            {
                // TODO: Make this a custom exception
                throw new Exception($"Invalid Request: {e.Message}");
            }
        }

        public static IDictionary<string, object> GetRawRequestAsDictionary(string request, IgnoreSerializationOptions so = null)
        {
            var parsed = request.ParseRawRequest();

            var dict = new Dictionary<string, object>
            {
                { IgnoreSerializationOptions.GetKeyName(IgnoreSerializationOptionKey.Uri), parsed.uri },
                { IgnoreSerializationOptions.GetKeyName(IgnoreSerializationOptionKey.Headers), parsed.headers },
                { IgnoreSerializationOptions.GetKeyName(IgnoreSerializationOptionKey.Cookie), parsed.cookies },
                { IgnoreSerializationOptions.GetKeyName(IgnoreSerializationOptionKey.RequestData), parsed.data }
            };

            if (so?.DoNotSerialize == null)
                return dict;

            foreach (var s in so.DoNotSerialize)
                dict.Remove(s);

            return dict;
        }

        public static (string uri, IDictionary<string, object> headers, IDictionary<string, object> cookies, string data) ParseRawRequest(this string rawRequest)
        {
            var parsedRequest = rawRequest.TrimEnd(Environment.NewLine.ToCharArray()).Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);

            var requestLine = parsedRequest[0].ParseRequestLine();

            var headers = InitializeHeadersDictionary(requestLine);
            PopulateHeadersDictionary(parsedRequest, headers);

            var cookies = GetCookiesFromParsedRequest(rawRequest, parsedRequest);
            var data = GetRequestData(rawRequest, headers, parsedRequest, requestLine);

            return (requestLine.url, headers, cookies, data);
        }

        private static void PopulateHeadersDictionary(string[] parsedRequest, IDictionary<string, object> headers)
        {
            for (var i = 1; i < DetectSplitIndex(parsedRequest); i++)
            {
                var header = parsedRequest[i].Split(new[] { ':' }, 2);
                headers[header[0].Trim()] = header[1].Trim();
            }

            // Cookie gets serialized separate from headers, so ignore it
            headers.Remove(IgnoreSerializationOptions.GetKeyName(IgnoreSerializationOptionKey.Cookie));
        }

        private static string GetRequestData(string rawRequest, IReadOnlyDictionary<string, object> headers, string[] parsedRequest, ValueTuple<string, string, string> requestLine)
        {
            string data = null;
            if ((string)headers["Method"] == "POST")
            {
                var postDataIndex = Array.FindIndex(parsedRequest, s => s == "");
                if (postDataIndex > 0)
                    rawRequest.TryParsePostDataString(out data);
            }
            else
            {
                var queryString = rawRequest.Contains('?') ? requestLine.Item2.Split('?')[1] : null;
                data = queryString;
            }
            return data;
        }

        private static int DetectSplitIndex(string[] parsedRequest)
        {
            var blankIndex = Array.IndexOf(parsedRequest, "");
            return blankIndex == -1 ? parsedRequest.Length : blankIndex - 1;
        }

        private static Dictionary<string, object> InitializeHeadersDictionary(ValueTuple<string, string, string> requestLine)
        {
            return new Dictionary<string, object>
            {
                ["Method"] = requestLine.Item1,
                ["HttpVersion"] = requestLine.Item3
            };
        }

        private static IDictionary<string, object> GetCookiesFromParsedRequest(string request, string[] parsedRequest)
        {
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
            return cookies;
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
            return (firstLine[0].CleanHeader(), firstLine[1].Trim(), firstLine[2].CleanHeader());
        }
    }
}
