using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ConfigHandler : IConfigHandler
    {
        private static readonly string configPath = ConfigurationManager.AppSettings["TaskbarConfigName"];

        public void SaveToFile(MainConfig config)
        {
            using (StreamWriter file = File.CreateText(configPath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, config);
            }
        }

        public MainConfig LoadFromFile()
        {
            if (!File.Exists(configPath))
            {
                return CreateFresh();
            }
            return JsonConvert.DeserializeObject<MainConfig>(File.ReadAllText(configPath));
        }

        private MainConfig CreateFresh()
        {
            MainConfig config = new MainConfig
            {
                HueConfig = new HueConfig
                {
                    HueUser = "",
                    BridgeUrl = ConfigurationManager.AppSettings["DefaultBridgeUrl"],
                    LightToAffect = int.TryParse(ConfigurationManager.AppSettings["DefaultLightToAffect"], out int defaultLight)
                        ? defaultLight
                        : 0
                },
                SkypeWorkingDirectory = ConfigurationManager.AppSettings["SkypeWorkingDirectory"],
                SkypeFileName = ConfigurationManager.AppSettings["SkypeFileName"],
                SteamWorkingDirectory = ConfigurationManager.AppSettings["SteamWorkingDirectory"],
                SteamFileName = ConfigurationManager.AppSettings["SteamFileName"],
            };
            SaveToFile(config);
            return config;
        }
    }
}
