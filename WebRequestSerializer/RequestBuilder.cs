using System;
using System.Collections.Generic;
using System.Net;

namespace WebRequestSerializer
{
    public static class RequestBuilder
    {
        public static HttpWebRequest BuildBaseHttpWebRequest(this IDictionary<string, object> requestHeaders, string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);

            foreach (var header in requestHeaders)
            {
                var value = (string) header.Value;
                switch (header.Key)
                {
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
                        req.IfModifiedSince = Convert.ToDateTime(value);
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
                        req.UserAgent = value;
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
