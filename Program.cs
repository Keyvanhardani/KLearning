using System;
using Newtonsoft.Json;
using System.IO;
using ScottPlot;
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        KLerning kLerning = new KLerning(30, 4);
        var (converged, episodes) = kLerning.Train();
        Console.WriteLine($"Converged: {converged}, Episodes: {episodes}");

        // Pfad für die JSON- und PNG-Datei
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "rewardsPerEpisode.json");
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Q-Learning.png");

        // Speichern der Belohnungen pro Episode in eine JSON-Datei
        try
        {
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(kLerning.RewardsPerEpisode));
            Console.WriteLine("Belohnungen wurden in " + jsonPath + " gespeichert.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Schreiben der JSON-Datei: " + ex.Message);
        }

        // Erstellen und Speichern des Plots
        try
        {
            var plt = new ScottPlot.Plot(600, 400);
            plt.AddScatter(DataGen.Consecutive(kLerning.RewardsPerEpisode.Count), kLerning.RewardsPerEpisode.ToArray());
            plt.Title("Q-Learning Fortschritt");
            plt.XLabel("Episoden");
            plt.YLabel("Belohnung");
            plt.SaveFig(imagePath);
            Console.WriteLine("Plot gespeichert als: " + imagePath);
        }
         catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Erstellen des Plots: " + ex.Message);
        }
        try
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // oder "python3", je nach Systemkonfiguration
            start.Arguments = "plot_data.py"; // Pfad zum Python-Skript
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using(Process process = Process.Start(start))
            {
                using(StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Starten des Python-Skripts: " + ex.Message);
        }
    }
}
