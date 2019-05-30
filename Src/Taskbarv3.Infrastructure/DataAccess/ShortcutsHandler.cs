using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ShortcutsHandler : IShortcutsHandler
    {
        private static readonly string shortcutPath = ConfigurationManager.AppSettings["ShortcutsName"];

        public void SaveToFile(IEnumerable<Shortcut> shortcutsData)
        {
            using (StreamWriter file = File.CreateText(shortcutPath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, shortcutsData);
            }
        }

        public IEnumerable<Shortcut> LoadFromFile()
        {
            if (File.Exists(shortcutPath))
            {
                var shortCuts = JsonConvert.DeserializeObject<List<Shortcut>>(File.ReadAllText(shortcutPath));
                var shortcutsModified = ConvertShortcutsIfNeededVersionOne(shortCuts);

                shortCuts.OrderBy(x => x.Index);

                if (shortcutsModified)
                {
                    SaveToFile(shortCuts);
                }

                return shortCuts;
            }
            return new List<Shortcut>(); ;
        }

        /// <summary>
        /// Converts a ShortCut up to version one if needed to keep backwards compatibility
        /// </summary>
        /// <returns>True if any shortcut was changed</returns>
        private bool ConvertShortcutsIfNeededVersionOne(List<Shortcut> shortCuts)
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
