using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        public static HttpWebRequest BuildBaseHttpWebRequest(this IDictionary<string, object> requestHeaders, string url = null)
        {
            var uri = !string.IsNullOrEmpty(url) ? url : (string) requestHeaders["RequestUri"];

            var req = (HttpWebRequest)WebRequest.Create(uri);

            foreach (var header in requestHeaders)
            {
                var value = header.Value;
                switch (header.Key)
                {
                    case "Url":
                    case "Body":
                        // no need to do anything with this
                        break;
                    case "Method":
                        req.Method = (string) value;
                        break;
                    case "Accept":
                        req.Accept = (string)value;
                        break;
                    case "Connection":
                        // throws an exception when setting value, use KeepAlive
                        break;
                    case "ContentType":
                        req.ContentType = (string)value;
                        break;
                    case "Content-Length":
                        req.ContentLength = Convert.ToInt64(value);
                        break;
                    case "Date":
                        req.Date = Convert.ToDateTime(value);
                        break;
                    case "Expect":
                        req.Expect = (string)value;
                        break;
                    case "Host":
                        req.Host = (string)value; 
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
                        req.Referer = (string)value;
                        break;
                    case "TransferEncoding":
                        req.TransferEncoding = (string)value;
                        break;
                    case "UserAgent":
                    case "User-Agent":
                        req.UserAgent = (string)value;
                        break;
                    case "HttpVersion":
                        var version = Convert.ToString(value).Split('/')[1];
                        req.ProtocolVersion = Version.Parse(version);
                        break;
                    default:
                        req.Headers[header.Key] = Convert.ToString(value);
                        break;
                }
            }

            if (req.Method == "POST")
                req.WritePostDataToRequestStream((string)requestHeaders["Body"]);   
            
            return req;
        }
    }
}