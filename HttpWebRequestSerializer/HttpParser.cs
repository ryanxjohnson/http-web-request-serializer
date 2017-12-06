﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class HttpParser
    {
        // API Method
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
            var parsedRequest = request.Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);

            var requestLine = parsedRequest[0].ParseRequestLine();
            var headers = new Dictionary<string, string>
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

            Dictionary<string, string> cookies;
            var cookieIndex = Array.FindLastIndex(parsedRequest, s => s.StartsWith("Cookie"));
            if (cookieIndex > 0)
            {
                parsedRequest[cookieIndex].TryParseCookies(out Dictionary<string, string> c);
                cookies = c;
            }
            else
            {
                request.TryParseCookies(out Dictionary<string, string> c);
                cookies = c;
            }

            string postData = null;
            if (headers["Method"] == "POST")
            {
                var postDataIndex = Array.FindIndex(parsedRequest, s => s == "");
                if (postDataIndex > 0)
                    request.TryParsePostDataString(out postData);
            }
            else
            {
                var qs = request.Contains('?') ? requestLine.url.Split('?')[1] : null;
                postData = qs;
            }
            
            return (requestLine.url, headers, cookies, postData);
        }

        public static bool TryParseCookies(this string cookieString, out Dictionary<string, string> cookieDictionary)
        {
            var matches = new Regex(@"Cookie:(?<Cookie>(.+))", RegexOptions.Singleline).Match(cookieString);
            var cookies = matches.Groups["Cookie"].ToString().Trim().Split(';');

            if (cookies.Length < 1 || cookies.Contains(""))
            {
                cookieDictionary = new Dictionary<string, string>();
                return false;
            }

            cookieDictionary = cookies.ToDictionary(c => c.Split('=')[0].Trim(), c => c.Split('=')[1].Trim());
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
