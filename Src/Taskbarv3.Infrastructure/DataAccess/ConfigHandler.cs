using Newtonsoft.Json;
using System.IO;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Infrastructure.DataAccess
{
    public class ConfigHandler : IConfigHandler
    {
        private const string configPath = @"taskbarConfig.json";

        public void SaveToFile(MainConfig config)
        {
            using (StreamWriter file = File.CreateText(configPath))
            {
                JsonSerializer serializer = new JsonSerializer();
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
                    BridgeUrl = @"http://192.168.1.178/api/",
                    LightToAffect = 3
                },
                SkypeWorkingDirectory = @"C:\Program Files (x86)\Skype\Phone\",
                SkypeFileName = @"C:\Program Files (x86)\Skype\Phone\Skype.exe",
                SteamWorkingDirectory = @"E:\Saker\ESteam\",
                SteamFileName = @"E:\Saker\ESteam\Steam.exe",
            };
            SaveToFile(config);
            return config;
        }
    }
}
