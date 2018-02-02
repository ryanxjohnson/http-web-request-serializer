using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        public static HttpWebRequest CreateWebRequestFromParsedRequest(this ParsedRequest parsedRequest)
        {
            return BuildRequest(parsedRequest);
        }

        public static HttpWebRequest CreateWebRequestFromJson(string json)
        {
            return BuildRequest(json.DeserializeRequestProperties());
        }

        public static HttpWebRequest CreateWebRequestFromDictionary(IDictionary<string, object> dict)
        {
            return BuildRequest(dict);
        }

        public static HttpWebRequest SetHeader(this HttpWebRequest req, string key, string value)
        {
            switch (key)
            {
                case "Method":
                    req.Method = value;
                    break;
                case "Accept":
                    req.Accept = value;
                    break;
                case "Connection":
                    //req.Connection = value;
                    //System.ArgumentException : Keep-Alive and Close may not be set using this property.
                    break;
                case "ContentType":
                case "Content-Type":
                    req.ContentType = value;
                    break;
                case "Content-Length":
                    req.ContentLength = Convert.ToInt64(value);
                    break;
                case "Date":
                    req.Date = Convert.ToDateTime(value);
                    break;
                case "Expect":
                    if (value == "100-continue")
                        break;
                    req.Expect = value;
                    break;
                case "Host":
                    req.Host = value;
                    break;
                case "HttpVersion":
                    var version = Convert.ToString(value).Split('/')[1];
                    req.ProtocolVersion = Version.Parse(version);
                    break;
                case "IfModifiedSince":
                case "If-Modified-Since":
                    req.IfModifiedSince = Convert.ToDateTime(value);
                    break;
                case "KeepAlive":
                case "Keep-Alive":
                    req.KeepAlive = Convert.ToBoolean(value);
                    break;
                case "Proxy-Connection":
                    break;
                case "Referer":
                    req.Referer = value;
                    break;
                case "TransferEncoding":
                    req.TransferEncoding = value;
                    break;
                case "UserAgent":
                case "User-Agent":
                    req.UserAgent = value;
                    break;
                default:
                    req.Headers[key] = value;
                    break;
            }

            return req;
        }

        private static HttpWebRequest BuildRequest(ParsedRequest parsedRequest)
        {
            var uri = parsedRequest.Url;

            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.SetHeaders(parsedRequest.Headers);
            if (parsedRequest.Cookies != null) req.SetCookies(parsedRequest.Cookies, uri);
            if (!string.IsNullOrEmpty(parsedRequest.RequestBody)) req.SetPostData(parsedRequest.RequestBody);

            return req;
        }

        private static HttpWebRequest BuildRequest(IDictionary<string, object> dict)
        {
            var uri = (string)dict["Uri"];

            var req = (HttpWebRequest)WebRequest.Create(uri);
            if (dict.ContainsKey("Headers")) req.SetHeaders((IDictionary<string, object>)dict["Headers"]);
            if (dict.ContainsKey("Cookie")) req.SetCookies((IDictionary<string, object>) dict["Cookie"], uri);
            if (dict.ContainsKey("Data")) req.SetPostData((string)dict["Data"]);

            return req;
        }

        private static void SetHeaders(this HttpWebRequest req, IDictionary<string, object> headers)
        {
            foreach (var header in headers)
                req.SetHeader(header.Key, (string) header.Value);
        }

        private static void SetCookies(this HttpWebRequest req, IDictionary<string, object> cookies, string uri)
        {
            req.CookieContainer = new CookieContainer();
            foreach (var cookie in cookies)
                req.CookieContainer.Add(new Uri(uri), new Cookie(cookie.Key, (string)cookie.Value));
        }

        private static void SetPostData(this HttpWebRequest req, string postData)
        {
            if (string.IsNullOrEmpty(postData)) return;
            
            if (req.Method == "POST")
                req.WritePostDataToRequestStream(postData);
        }
    }
}