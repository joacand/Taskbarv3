namespace Taskbarv3.Core.Models
{
    public class ShortcutMetaData
    {
        public string Name { get; set; }
        public string ProcessPath { get; set; }
        public string IconPath { get; set; }
        public string WorkingDirectory { get; set; }

        public ShortcutMetaData(string name, string processPath, string iconPath, string workingDirectory)
        {
            Name = name;
            ProcessPath = processPath;
            IconPath = iconPath;
            WorkingDirectory = workingDirectory;
        }
    }
}
