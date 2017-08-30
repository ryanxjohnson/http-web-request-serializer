using System;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.GZip;

namespace HttpWebRequestSerializer.Extensions
{
    public static class HttpExtensions
    {
        public static string GetResponseString(this HttpWebRequest req)
        {
            using (var resp = req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                    if (stream != null)
                        using (var streamReader = new StreamReader(stream))
                            return streamReader.ReadToEnd();
            }

            return null;
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
            int size = 2048;
            var writeData = new byte[2048];
            while (true)
            {
                size = compressedStream.Read(writeData, 0, size);
                if (size > 0)
                {
                    decompressedStream.Write(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            decompressedStream.Seek(0, SeekOrigin.Begin);
            return decompressedStream;
        }
    }
}
