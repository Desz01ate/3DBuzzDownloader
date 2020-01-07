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
            var availableFiles = htmlDoc.DocumentNode.Descendants("a").Select(x => x.Attributes["href"].Value).ToArray();
            Console.WriteLine($"Found {availableFiles.Count()} files.");
            Console.Write($"Please enter download destination : ");
            var baseDir = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(baseDir) && Directory.Exists(baseDir))
            {
                var di = new DirectoryInfo(baseDir);
                foreach (var file in di.GetFiles())
                {
                    if (file.Length == 0)
                    {
                        Console.WriteLine($"##found corrupted file ${file.FullName}, deleting...");
                        File.Delete(file.FullName);
                    }
                }
                foreach (var file in availableFiles)
                {
                    var fileName = file.Substring(file.LastIndexOf("/") + 1);
                    var savedFile = Path.Combine(baseDir, fileName);
                    try
                    {
                        using var wc = new WebClient();
                        var shouldLoad = true;
                        if (File.Exists(savedFile))
                        {
                            var fi = new FileInfo(savedFile);
                            wc.OpenRead(file);
                            var bytesLength = Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
                            if (fi.Length == bytesLength)
                            {
                                Console.WriteLine($"--{fileName} is already exists, skip.");
                                shouldLoad = false;
                            }
                        }
                        if (shouldLoad)
                        {
                            wc.DownloadFileAsync(new Uri(file), savedFile);
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
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to load {file}, skip.");
                    }
                    //Console.Write($@"--Downloading {fileName}... ");
                    //Console.WriteLine("Done");
                }
            }

        }
    }
}
