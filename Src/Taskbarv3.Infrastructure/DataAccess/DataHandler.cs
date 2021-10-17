using System;
using System.IO;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public abstract class DataHandler
    {
        private static string ApplicationName => "Taskbarv3";

        protected static string ApplicationSettingsDirectory { get; } =
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

        protected void CreateDirectory()
        {
            Directory.CreateDirectory(ApplicationSettingsDirectory);
        }
    }
}
