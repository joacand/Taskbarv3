using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Taskbarv3.Core.Models
{
    public class Shortcut
    {
        public string Name { get; set; }
        public string ProcessPath { get; set; }
        public string IconPath { get; set; }
        public string WorkingDirectory { get; set; }
        public int Index { get; set; }
        public int Version { get; set; }

        [JsonIgnore]
        public Action OpenProcess { get; set; }
        [JsonIgnore]
        public static int CurrentVersion = 1;

        public Shortcut(string name, string processPath, string iconPath, string workingDirectory, int index, int version)
        {
            Name = File.Exists(iconPath)
                ? string.Empty
                : name ?? string.Empty;

            IconPath = string.IsNullOrWhiteSpace(iconPath) || !File.Exists(iconPath)
                ? null
                : iconPath;

            WorkingDirectory = workingDirectory;
            ProcessPath = processPath;
            Index = index;
            Version = version;

            OpenProcess = new Action(OnOpenProcess);
        }

        private void OnOpenProcess()
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = ProcessPath;
            process.StartInfo.UseShellExecute = true;

            process.Start();
        }
    }
}
