using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class ParsedRequest
    {
        public string Url;
        public IDictionary<string, object> Headers;
        public IDictionary<string, object> Cookies;
        public string RequestBody;
    }
}
