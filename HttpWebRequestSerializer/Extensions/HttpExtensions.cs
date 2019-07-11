using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HttpWebRequestSerializer.Models;
using ICSharpCode.SharpZipLib.GZip;

namespace HttpWebRequestSerializer.Extensions
{
    public static class HttpExtensions
    {
        public static string GetResponseString(this HttpWebRequest req)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                return ResponseString(resp);
            }
        }

        public static (string response, int statusCode, string statusDescription) GetResponseStringAndStatus(this HttpWebRequest req)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                return (ResponseString(resp), (int)resp.StatusCode, resp.StatusDescription);
            }
        }

        public static (string responseText, int statusCode, string statusDescription, string cookies) GetResponseStringAndStatusAndCookies(this HttpWebRequest req)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                return (ResponseString(resp), (int)resp.StatusCode, resp.StatusDescription, resp.Headers["Set-Cookie"]);
            }
        }

        public static (int statusCode, string statusDescription) ExecuteRequestGetStatus(this HttpWebRequest req)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                return ((int)resp.StatusCode, resp.StatusDescription);
            }
        }

        public static ParsedWebResponse ExecuteRequestGetParsedWebResponse(this HttpWebRequest req)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                return new ParsedWebResponse(resp);
            }
        }

        public static string ResponseString(this HttpWebResponse resp)
        {
            if (resp.Headers["Content-Encoding"] == "gzip")
            {
                using (var stream = resp.GetResponseStream())
                    if (stream != null)
                        using (var streamReader = new StreamReader(GetGzipStream(resp)))
                            return streamReader.ReadToEnd();
            }

            using (var stream = resp.GetResponseStream())
                if (stream != null)
                    using (var streamReader = new StreamReader(stream))
                        return streamReader.ReadToEnd();

            return null;
        }

        public static void WritePostDataToRequestStream(this WebRequest req, string value)
        {
            var data = System.Text.Encoding.ASCII.GetBytes(value);
            req.ContentLength = data.Length;

            using (var streamWriter = req.GetRequestStream())
                streamWriter.Write(data, 0, data.Length);
        }

        public static string GetResponseFromGzip(this HttpWebRequest req)
        {
            using (var resp = req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                    if (stream != null)
                        using (var streamReader = new StreamReader(GetGzipStream((HttpWebResponse)resp)))
                            return streamReader.ReadToEnd();
            }

            return null;
        }

        public static Stream GetGzipStream(HttpWebResponse response)
        {
            Stream compressedStream = null;
            if (response.ContentEncoding == "gzip")
                compressedStream = new GZipInputStream(response.GetResponseStream());

            if (compressedStream == null)
                return response.GetResponseStream();

            var decompressedStream = new MemoryStream();
            var size = 2048;
            var writeData = new byte[size];

            while (true)
            {
                size = compressedStream.Read(writeData, 0, size);
                if (size > 0)
                    decompressedStream.Write(writeData, 0, size);
                else
                    break;
            }

            decompressedStream.Seek(0, SeekOrigin.Begin);
            return decompressedStream;
        }

        public static Dictionary<string, string[]> ConvertWebHeadersToDictionary(this WebHeaderCollection headers)
        {
            return Enumerable.Range(0, headers.Count).ToDictionary(i => headers.Keys[i], headers.GetValues);
        }
    }
}
