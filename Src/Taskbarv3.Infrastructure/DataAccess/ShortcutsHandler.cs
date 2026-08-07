using System.Text.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ShortcutsHandler : DataHandler, IShortcutsHandler
    {
        private static readonly string shortcutPath =
            Path.Join(ApplicationSettingsDirectory, ConfigurationManager.AppSettings["ShortcutsName"]);

        public void SaveToFile(IEnumerable<Shortcut> shortcutsData)
        {
            File.WriteAllText(shortcutPath, JsonSerializer.Serialize(shortcutsData, JsonOptions.Default));
        }

        public IEnumerable<Shortcut> LoadFromFile()
        {
            if (!File.Exists(shortcutPath))
            {
                CreateDirectory();
                return [];
            }

            var shortcuts = JsonSerializer.Deserialize<List<Shortcut>>(File.ReadAllText(shortcutPath), JsonOptions.Default)!;
            var shortcutsModified = ConvertShortcutsIfNeededVersionOne(shortcuts);

            if (shortcutsModified)
            {
                SaveToFile(shortcuts);
            }

            return shortcuts;
        }

        /// <summary>
        /// Converts a ShortCut up to version one if needed to keep backwards compatibility
        /// </summary>
        /// <returns>True if any shortcut was changed</returns>
        private static bool ConvertShortcutsIfNeededVersionOne(List<Shortcut> shortCuts)
        {
            const int targetVersion = 1;

            if (shortCuts.Any(x => x.Version >= targetVersion))
            {
                return false;
            }

            // Version one added indices and version number to shortcuts

            var index = 0;
            foreach (var shortCut in shortCuts)
            {
                shortCut.Index = index++;
                shortCut.Version = targetVersion;
            }

            return true;
        }
    }
}
