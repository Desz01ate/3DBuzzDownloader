using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace _3DBuzzDownloader.HtmlHelper
{
    public static class Source
    {
        public static async IAsyncEnumerable<string> GetSourceAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                StreamReader readStream;
                if (string.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                while (!readStream.EndOfStream)
                {
                    var currentLine = await readStream.ReadLineAsync();
                    yield return currentLine;
                }

                response.Close();
                readStream.Close();
            }
        }
        public static string GetSource(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                StreamReader readStream;
                if (string.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                var data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return data;
            }
            return null;
        }
    }
}
