using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ShortcutsHandler : IShortcutsHandler
    {
        private const string shortcutPath = @"shortcuts.json";

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
                return JsonConvert.DeserializeObject<List<Shortcut>>(File.ReadAllText(shortcutPath));
            }
            return new List<Shortcut>(); ;
        }
    }
}
