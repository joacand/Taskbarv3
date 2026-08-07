using System.Text.Json;
using System;
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

        private MainConfig? cachedConfig;
        private DateTime cachedConfigLastWriteUtc;

        public void SaveToFile(MainConfig config)
        {
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, JsonOptions.Default));

            cachedConfig = config;
            cachedConfigLastWriteUtc = File.GetLastWriteTimeUtc(configPath);
        }

        public MainConfig LoadFromFile()
        {
            if (File.Exists(configPath))
            {
                var lastWrite = File.GetLastWriteTimeUtc(configPath);
                if (cachedConfig != null && cachedConfigLastWriteUtc == lastWrite)
                {
                    return cachedConfig;
                }

                var loaded = JsonSerializer.Deserialize<MainConfig>(File.ReadAllText(configPath), JsonOptions.Default);
                if (loaded != null)
                {
                    cachedConfig = loaded;
                    cachedConfigLastWriteUtc = lastWrite;
                    return loaded;
                }
            }

            return CreateFresh();
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
                },
                CpuMonitorDisabled = false
            };
            SaveToFile(config);
            return config;
        }
    }
}
