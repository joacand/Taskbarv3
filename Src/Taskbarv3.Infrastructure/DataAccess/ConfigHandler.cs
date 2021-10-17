using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ConfigHandler : DataHandler, IConfigHandler
    {
        private static readonly string configPath =
            Path.Join(ApplicationSettingsDirectory, ConfigurationManager.AppSettings["TaskbarConfigName"]);

        public void SaveToFile(MainConfig config)
        {
            using StreamWriter file = File.CreateText(configPath);
            JsonSerializer serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, config);
        }

        public MainConfig LoadFromFile()
        {
            return File.Exists(configPath)
                ? JsonConvert.DeserializeObject<MainConfig>(File.ReadAllText(configPath))
                : CreateFresh();
        }

        private MainConfig CreateFresh()
        {
            CreateDirectory();
            var config = new MainConfig
            {
                HueConfig = new HueConfig
                {
                    HueUser = "",
                    BridgeUrl = ConfigurationManager.AppSettings["DefaultBridgeUrl"],
                    LightToAffect = int.TryParse(ConfigurationManager.AppSettings["DefaultLightToAffect"], out int defaultLight)
                        ? defaultLight
                        : 0
                }
            };
            SaveToFile(config);
            return config;
        }
    }
}
