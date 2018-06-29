using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer
{
    public static class RequestBuilder
    {
        public static HttpWebRequest CreateWebRequestFromParsedRequest(this ParsedRequest parsedRequest, Action<HttpWebRequest> callback = null)
        {
            return BuildRequest(parsedRequest, callback);
        }

        public static HttpWebRequest CreateWebRequestFromJson(string json, Action<HttpWebRequest> callback = null)
        {
            return BuildRequest(json.DeserializeRequestProperties(), callback);
        }

        public static HttpWebRequest CreateWebRequestFromDictionary(IDictionary<string, object> dict, Action<HttpWebRequest> callback = null)
        {
            return BuildRequest(dict, callback);
        }

        public static HttpWebRequest SetHeader(this HttpWebRequest req, string key, string value)
        {
            switch (key.ToLower())
            {
                case "method":
                    req.Method = value;
                    break;
                case "accept":
                    req.Accept = value;
                    break;
                case "connection":
                    req.KeepAlive = value.ToLower() == "keep-alive";
                    //req.Connection = value;
                    //System.ArgumentException : Keep-Alive and Close may not be set using this property.
                    break;
                case "contenttype":
                case "content-type":
                    req.ContentType = value;
                    break;
                case "content-length":
                case "contentlength":
                    req.ContentLength = Convert.ToInt64(value);
                    break;
                case "date":
                    req.Date = Convert.ToDateTime(value);
                    break;
                case "expect":
                    if (value == "100-continue")
                        break;
                    req.Expect = value;
                    break;
                case "host":
                    req.Host = value;
                    break;
                case "httpversion":
                    var version = Convert.ToString(value).Split('/')[1];
                    req.ProtocolVersion = Version.Parse(version);
                    break;
                case "ifmodifiedsince":
                case "if-modified-since":
                    req.IfModifiedSince = Convert.ToDateTime(value);
                    break;
                case "keepalive":
                case "keep-alive":
                    req.KeepAlive = Convert.ToBoolean(value);
                    break;
                case "proxy-connection":
                    break;
                case "referer":
                    req.Referer = value;
                    break;
                case "transferEncoding":
                    req.TransferEncoding = value;
                    break;
                case "useragent":
                case "user-agent":
                    req.UserAgent = value;
                    break;
                default:
                    req.Headers[key] = value;
                    break;
            }

            return req;
        }

        private static HttpWebRequest BuildRequest(ParsedRequest parsedRequest, Action<HttpWebRequest> callback = null)
        {
            var uri = parsedRequest.Url;

            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.SetHeaders(parsedRequest.Headers);

            if (parsedRequest.CookieContainer != null)
            {
                req.CookieContainer = parsedRequest.CookieContainer;
            }
            else
            {
                if (parsedRequest.Cookies != null) req.SetCookies(parsedRequest.Cookies, parsedRequest.Uri);
            }

            callback?.Invoke(req);

            // Calling GetRequestStream closes the request for adding headers, 
            // so don't do this unless you are positive you don't need to add
            // any new headers after appending the request body
            // Use the call back to defer this to your client
            if (!string.IsNullOrEmpty(parsedRequest.RequestBody)) req.SetPostData(parsedRequest.RequestBody);

            return req;
        }

        private static HttpWebRequest BuildRequest(IDictionary<string, object> dict, Action<HttpWebRequest> callback = null)
        {
            var uri = (string)dict["Uri"];

            var req = (HttpWebRequest)WebRequest.Create(uri);
            if (dict.ContainsKey("Headers")) req.SetHeaders((IDictionary<string, object>)dict["Headers"]);
            if (dict.ContainsKey("Cookie")) req.SetCookies((IDictionary<string, object>) dict["Cookie"], new Uri(uri));

            callback?.Invoke(req);

            if (dict.ContainsKey("Data")) req.SetPostData((string)dict["Data"]);

            return req;
        }

        private static void SetHeaders(this HttpWebRequest req, IDictionary<string, object> headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
                req.SetHeader(header.Key, (string) header.Value);
        }

        private static void SetCookies(this HttpWebRequest req, IDictionary<string, object> cookies, Uri uri)
        {
            if (cookies == null) return;

            req.CookieContainer = new CookieContainer();
            foreach (var cookie in cookies)
                req.CookieContainer.Add(uri, new Cookie(cookie.Key, (string)cookie.Value));
        }

        private static void SetPostData(this HttpWebRequest req, string postData)
        {
            if (string.IsNullOrEmpty(postData)) return;
            
            if (req.Method == "POST")
                req.WritePostDataToRequestStream(postData);
        }
    }
}