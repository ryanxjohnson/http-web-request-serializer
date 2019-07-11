using System;
using System.Collections.Generic;
using System.Net;
using HttpWebRequestSerializer.Extensions;

namespace HttpWebRequestSerializer.Models
{
    public class ParsedWebResponse
    {
        public string ResponseText { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Cookies { get; set; }
        public Uri ResponseUri { get; set; }
        public Dictionary<string, string[]> ResponseHeaders { get; set; }

        public ParsedWebResponse(HttpWebResponse response)
        {
            ResponseText = response.ResponseString();
            StatusCode = (int) response.StatusCode;
            StatusDescription = response.StatusDescription;
            ResponseHeaders = response.Headers.ConvertWebHeadersToDictionary();
            Cookies = response.Headers["Set-Cookie"];
            ResponseUri = response.ResponseUri;
        }
    }
}