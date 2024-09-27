using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;

public class ModelDownloader
{
    private readonly string modelUrl = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip";
    private readonly string modelZipFileName = "vosk-model-small-en-us-0.15.zip";
    private readonly string modelDirName = "vosk-model-small-en-us-0.15";

    public async Task<string> EnsureModelIsAvailableAsync()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string modelDirPath = Path.Combine(currentDir, modelDirName);
        string modelZipPath = Path.Combine(currentDir, modelZipFileName);

        if (!Directory.Exists(modelDirPath))
        {
            Console.WriteLine("Model not found, downloading...");

            if (!File.Exists(modelZipPath))
            {
                await DownloadModelAsync(modelZipPath);
            }

            Console.WriteLine("Extracting model...");
            ZipFile.ExtractToDirectory(modelZipPath, currentDir);
            Console.WriteLine("Model extracted.");
        }
        else
        {
            Console.WriteLine("Model is already available.");
        }

        return GetModelFolder(currentDir);
    }

    private string GetModelFolder(string currentDir)
    {
        var directories = Directory.GetDirectories(currentDir, "*", SearchOption.AllDirectories);

        foreach (var directory in directories)
        {
            if (Directory.Exists(Path.Combine(directory, "am")) && File.Exists(Path.Combine(directory, "README")))
            {
                Console.WriteLine($"Model found at: {directory}");
                return directory;
            }
        }

        throw new Exception("Model directory not found or incorrect model structure.");
    }

    private async Task DownloadModelAsync(string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(modelUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            using (var downloadStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = new byte[8192]; // 8KB buffer
                int bytesRead;
                long totalBytesRead = 0;

                while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    totalBytesRead += bytesRead;
                    fileStream.Write(buffer, 0, bytesRead);

                    if (totalBytes.HasValue)
                    {
                        double progress = (double)totalBytesRead / totalBytes.Value * 100;
                        Console.Write($"\rDownloading... {progress:F2}%");
                    }
                }

                Console.WriteLine("\nModel downloaded successfully.");
            }
        }
    }
}
