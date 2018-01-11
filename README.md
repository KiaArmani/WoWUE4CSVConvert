# WoWUE4CSVConvert
Command-line tool to convert WoW CSV Placement Information files to be readable by Unreal Engine 4.

# Usage
```
class Options
{
    [Option('f', "folder", Required = true,
      HelpText = "Folder with CSV files.")]
    public String SourceFilesPath { get; set; }

    [Option('m', "merge", Required = true,
      HelpText = "If true, merge all CSVs to one.")]
    public bool MergeAllCSV { get; set; }
}
```

# Example

Run from Powershell

```
.\WoWUE4CSVConvert.exe -f "C:\CSV" -m true
```
