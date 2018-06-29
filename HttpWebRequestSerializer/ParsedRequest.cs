using System;
using System.Collections.Generic;
using System.Net;

namespace HttpWebRequestSerializer
{
    public class ParsedRequest
    {
        public string Url;
        public IDictionary<string, object> Headers;
        public IDictionary<string, object> Cookies;
        public string RequestBody;
        public Uri Uri;
        public CookieContainer CookieContainer;
    }
}
