using System;
using System.Globalization;
using System.IO;
using EmotionQuest.Models;
using Newtonsoft.Json;

namespace EmotionQuest.Services
{
    public class StorageService
    {
        private const string ProgressFileName = "progress.json";
        private const string MetricsFileName = "metrics.csv";

        public void SaveProgress(GameState state)
        {
            var json = JsonConvert.SerializeObject(state, Formatting.Indented);
            File.WriteAllText(ProgressFileName, json);
        }

        public void AppendMetrics(Stimulus stimulus, string userChoice, bool correct, int rtMs, string level)
        {
            bool fileExists = File.Exists(MetricsFileName);

            using (var writer = new StreamWriter(MetricsFileName, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("timestamp,stimulusId,choice,correct,rt_ms,level");
                }

                string line = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3},{4},{5}",
                    DateTime.Now.ToString("s"),
                    stimulus.Id,
                    userChoice,
                    correct ? 1 : 0,
                    rtMs,
                    level);

                writer.WriteLine(line);
            }
        }
    }
}

