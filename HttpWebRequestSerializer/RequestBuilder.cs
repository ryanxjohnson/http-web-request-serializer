using System;
using System.Collections.Generic;
using System.Net;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        public static HttpWebRequest BuildBaseHttpWebRequest(this IDictionary<string, object> requestHeaders, string url = null)
        {
            var uri = !string.IsNullOrEmpty(url) ? url : (string) requestHeaders["Url"];

            var req = (HttpWebRequest)WebRequest.Create(uri);

            foreach (var header in requestHeaders)
            {
                var value = (string)header.Value;
                switch (header.Key)
                {
                    case "Url":
                        // no need to do anything with this
                        break;
                    case "Method":
                        req.Method = value;
                        break;
                    case "Accept":
                        req.Accept = value;
                        break;
                    case "Connection":
                        // throws an exception when setting value, use KeepAlive
                        break;
                    case "ContentType":
                        req.ContentType = value;
                        break;
                    case "Content-Length":
                        req.ContentLength = Convert.ToInt64(value);
                        break;
                    case "Date":
                        req.Date = Convert.ToDateTime(value);
                        break;
                    case "Expect":
                        req.Expect = value;
                        break;
                    case "Host":
                        req.Host = value;
                        break;
                    case "IfModifiedSince":
                    case "If-Modified-Since":
                        req.IfModifiedSince = Convert.ToDateTime(value);
                        break;
                    case "IfNoneMatch":
                        //req.IfNoneMatch
                        break;
                    case "KeepAlive":
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
                    case "HttpVersion":
                        req.ProtocolVersion = Version.Parse("1.1");
                        break;
                    default:
                        req.Headers[header.Key] = value;
                        break;
                }
            }
            
            return req;
        }
    }
}