namespace Taskbarv3.Core.Models
{
    public class ShortcutMetaData(string name, string processPath, string? iconPath, string workingDirectory)
    {
        public string Name { get; set; } = name;
        public string ProcessPath { get; set; } = processPath;
        public string? IconPath { get; set; } = iconPath;
        public string WorkingDirectory { get; set; } = workingDirectory;
    }
}
