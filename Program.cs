using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWUE4CSVConvert
{
    class Options
    {
        [Option('f', "folder", Required = true,
          HelpText = "Path to UE4 Win64 Binaries.")]
        public String SourceFilesPath { get; set; }

        [Option('m', "merge", Required = true,
          HelpText = "If true, merge all CSVs to one.")]
        public bool MergeAllCSV { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Start Progress Time now
            DateTime startTime = DateTime.Now;

            // Get Command line args
            var result = Parser.Default.ParseArguments<Options>(args);

            // If no errors occured..
            if (!result.Errors.Any())
            {
                // Get files in source directory
                DirectoryInfo sourceDir = new DirectoryInfo(result.Value.SourceFilesPath);
                FileInfo[] sourceFiles = sourceDir.GetFiles("*.csv");

                // Get the file count and create a counter
                int fileMaxCount = sourceFiles.Count();
                int fileCount = 0;

                // Create folders where processed files are being placed into                
                Directory.CreateDirectory(Path.Combine(result.Value.SourceFilesPath, "processed"));

                bool merge = result.Value.MergeAllCSV;
                
                // Create new Progress Bar
                using (var progress = new ProgressBar())
                {
                    List<string> allLines = new List<string>();
                    var allIndex = 1;

                    // For every file in the source folder..
                    foreach (FileInfo importFile in sourceFiles)
                    {
                        // Make new index
                        var lineIndex = 1;

                        // Read all lines
                        var lines = File.ReadAllLines(importFile.FullName);

                        // Loop through them
                        for (int i = 0; i < lines.Length; i++)
                        {
                            // Replace all semicolons with commas
                            lines[i] = lines[i].Replace(';', ',');

                            // If it's the first line
                            if (i == 0)
                            {
                                // Add a comma at the start
                                lines[i] = "," + lines[i];
                            }
                            else
                            {
                                if (merge)
                                {
                                    allLines.Add(allIndex + "," + lines[i]);
                                    allIndex++;
                                }
                                else
                                {
                                    lines[i] = lineIndex + "," + lines[i];
                                    lineIndex++;
                                }
                            }            
                        }

                        if(!merge)
                        {
                            // Write all lines
                            File.WriteAllLines(Path.Combine(importFile.DirectoryName, "processed", importFile.Name), lines);
                        }        
                                   
                        // Increase Progress
                        fileCount++;
                        TimeSpan timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (fileMaxCount - (fileCount + 1)) / (fileCount + 1));
                        progress.timeRemainingText = timeRemaining.ToString("G");
                        progress.Report((double)fileCount / fileMaxCount);                        
                    }

                    if(merge)
                    {
                        File.WriteAllLines(Path.Combine(result.Value.SourceFilesPath, "combined.csv"), allLines);
                    }
                }
            }
        }
    }
}
