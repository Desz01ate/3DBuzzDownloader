using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace _3DBuzzDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await foreach (var line in HtmlHelper.Source.GetSourceAsync("https://www.3dbuzz.com/"))
            //{
            //    Console.WriteLine(line);
            //}
            //return;
            var htmlSource = HtmlHelper.Source.GetSource("https://www.3dbuzz.com");
            if (htmlSource == null)
            {
                htmlSource = File.ReadAllText("Resources\\source.html");
            }
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlSource);
            var availableFiles = htmlDoc.DocumentNode.Descendants("a").Select(x => x.Attributes["href"].Value);
            Console.WriteLine($"Found {availableFiles.Count()} files.");
            Console.Write($"Please enter download destination : ");
            var baseDir = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(baseDir) && Directory.Exists(baseDir))
            {
                foreach (var file in availableFiles)
                {
                    var fileName = file.Substring(file.LastIndexOf("/") + 1);
                    var savedFile = Path.Combine(baseDir, fileName);
                    if (File.Exists(savedFile))
                    {
                        Console.WriteLine($"--{fileName} is already exists, skip.");
                    }
                    else
                    {
                        var wc = Download(file, savedFile);
                        wc.DownloadProgressChanged += (s, e) =>
                        {
                            Console.Write($"\r--Downloading {fileName}... {e.ProgressPercentage}%");
                        };
                        while (wc.IsBusy)
                        {
                            await Task.Delay(1000);
                        }
                        Console.WriteLine(" Done.");
                    }
                    //Console.Write($@"--Downloading {fileName}... ");
                    //Console.WriteLine("Done");
                }
            }

        }


        public static WebClient Download(string fileUri, string savePath)
        {
            using var client = new WebClient();
            var uri = new Uri(fileUri);
            client.DownloadFileAsync(uri, savePath);
            return client;
        }
    }
}
