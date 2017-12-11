using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        private const string pattern = @"#{{(?<QueryString>\w+)}}";
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

        private static HttpWebRequest BuildRequest(IDictionary<string, object> dict)
        {
            var uri = (string)dict["Uri"];

            var req = (HttpWebRequest)WebRequest.Create(uri);
            SetHeaders(dict, req);
            SetCookies(dict, req, uri);
            SetPostData(dict, req);

            return req;
        }

        private static void SetHeaders(IDictionary<string, object> dict, HttpWebRequest req)
        {
            foreach (var header in (Dictionary<string, object>)dict["Headers"])
                req.SetHeader(header.Key, (string)header.Value);
        }

        private static void SetCookies(IDictionary<string, object> dict, HttpWebRequest req, string uri)
        {
            if (!dict.ContainsKey("Cookie")) return;
            req.CookieContainer = new CookieContainer();
            foreach (var cookie in (IDictionary<string, object>)dict["Cookie"])
                req.CookieContainer.Add(new Uri(uri), new Cookie(cookie.Key, (string)cookie.Value));
        }

        private static void SetPostData(IDictionary<string, object> dict, HttpWebRequest req)
        {
            if (!dict.ContainsKey("Data")) return;
            if (req.Method == "POST")
                req.WritePostDataToRequestStream((string) dict["Data"]);
        }
    }
}