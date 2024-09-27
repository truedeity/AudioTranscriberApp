using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Vosk;
using NAudio.Wave;

class Program
{
    static async Task Main(string[] args)
    {
        ModelDownloader downloader = new ModelDownloader();
        string modelPath = await downloader.EnsureModelIsAvailableAsync();

        DatabaseHelper dbHelper = new DatabaseHelper();
        dbHelper.InitializeDatabase();
        Vosk.Vosk.SetLogLevel(0);
        Model model = new Model(modelPath);

        using (VoskRecognizer recognizer = new VoskRecognizer(model, 16000))
        {
            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = new WaveFormat(16000, 1); 
                waveIn.BufferMilliseconds = 1000;

                waveIn.DataAvailable += (sender, e) =>
                {
                    if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {
                        string jsonResult = recognizer.Result();
                        Console.WriteLine(jsonResult);

                        var parsedResult = JObject.Parse(jsonResult);
                        string transcribedText = parsedResult["text"]?.ToString();

                        if (!string.IsNullOrEmpty(transcribedText))
                        {
                            Console.WriteLine($"Transcribed Text: {transcribedText}");
                            dbHelper.SaveTranscription(transcribedText); 
                        }
                    }
                };

                waveIn.StartRecording();
                Console.WriteLine("Listening... Press any key to stop.");

                Console.ReadKey();
                waveIn.StopRecording();
            }
        }
    }
}
