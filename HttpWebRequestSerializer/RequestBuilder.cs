using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        // API Method
        public static HttpWebRequest CreateWebRequestFromJson(string json)
        {
            var dict = json.DeserializeRequestProperties();
            var uri = (string)dict["Uri"];

            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.CookieContainer = new CookieContainer();

            foreach (var header in (Dictionary<string, object>)dict["Headers"])
                req.SetHeader(header.Key, (string)header.Value);

            if (dict.ContainsKey("Cookie"))
                foreach (var cookie in (IDictionary<string, object>)dict["Cookie"])
                    req.CookieContainer.Add(new Uri(uri), new Cookie(cookie.Key, (string)cookie.Value));

            if (dict.ContainsKey("Data"))
                if (req.Method == "POST")
                    req.WritePostDataToRequestStream((string)dict["Data"]);

            return req;
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
    }
}