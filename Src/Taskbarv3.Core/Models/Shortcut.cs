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
        [JsonIgnore]
        public Action OpenProcess { get; set; }

        public Shortcut(string name, string processPath, string iconPath, string workingDirectory)
        {
            Name = File.Exists(iconPath)
                ? ""
                : name;

            IconPath = string.IsNullOrWhiteSpace(iconPath) || !File.Exists(iconPath)
                ? null
                : iconPath;

            WorkingDirectory = workingDirectory;
            ProcessPath = processPath;

            OpenProcess = new Action(OnOpenProcess);
        }

        private void OnOpenProcess()
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = ProcessPath;
            process.Start();
        }
    }
}
