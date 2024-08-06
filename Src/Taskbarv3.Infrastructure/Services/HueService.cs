using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.Core.Models.Hue;

namespace Taskbarv3.Infrastructure.Services
{
    public class HueService : IHueService
    {
        private string user = string.Empty;
        private bool HasUser => !string.IsNullOrWhiteSpace(user);
        private static HttpClient client;
        private readonly MaxRequestsGuard requestGuard = new();
        private readonly string baseUrl;
        private readonly MainConfig config;
        private readonly IConfigHandler configHandler;

        public HueService(IConfigHandler configHandler)
        {
            this.configHandler = configHandler;
            config = configHandler.LoadFromFile();
            baseUrl = config.HueConfig.BridgeUrl;
            client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            TryLoadUser();
        }

        private bool TryLoadUser()
        {
            if (!string.IsNullOrWhiteSpace(config.HueConfig.HueUser))
            {
                user = config.HueConfig.HueUser;
                return true;
            }
            return false;
        }

        public async Task<bool> PowerLight(bool on)
        {
            var light = config.HueConfig.LightToAffect;
            if (!HasUser)
            {
                return false;
            }
            requestGuard.WaitUntilFreeSlots();

            var url = $"{user}/lights/{light}/state";
            var jsonBody = JsonConvert.SerializeObject(new { on });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _ = await client.PutAsync(url, content);

            return true;
        }

        /// <param name="dimValue">Procentage 0-100</param>
        public async Task<bool> DimLight(int dimValue)
        {
            var light = config.HueConfig.LightToAffect;
            if (dimValue < 0 || dimValue > 100 || !HasUser)
            {
                return false;
            }
            requestGuard.WaitUntilFreeSlots();

            var brightnessValue = (int)(255 * (dimValue / 100.0));

            var url = $"{user}/lights/{light}/state";
            var jsonBody = JsonConvert.SerializeObject(new { bri = brightnessValue });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _ = await client.PutAsync(url, content);

            return true;
        }

        public async Task<int> GetBrightnessProcentage()
        {
            var light = config.HueConfig.LightToAffect;
            if (!HasUser)
            {
                return 0;
            }
            requestGuard.WaitUntilFreeSlots();

            var url = $"{user}/lights/{light}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) { return 0; }

            var rootObject = content.StartsWith('[')
               ? JsonConvert.DeserializeObject<List<RootObject>>(content)?.FirstOrDefault()
               : JsonConvert.DeserializeObject<RootObject>(content);

            if (rootObject == null || rootObject?.Error != null)
            {
                return 0;
            }

            var brightness = (int)Math.Round((double)rootObject.State.Bri / 255 * 100);

            return brightness;
        }

        public async Task<bool> RegisterAccount()
        {
            var jsonBody = JsonConvert.SerializeObject(new { devicetype = "taskbarv3" });
            var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("", requestContent);
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) { return false; }

            var linkRoot = content.StartsWith('[')
               ? JsonConvert.DeserializeObject<List<LinkRoot>>(content)?.FirstOrDefault()
               : JsonConvert.DeserializeObject<LinkRoot>(content);

            if (linkRoot?.Error == null)
            {
                user = linkRoot.Success.Username;
                SaveUserToConfig(linkRoot.Success.Username);
                return true;
            }
            return false;
        }

        private void SaveUserToConfig(string user)
        {
            config.HueConfig.HueUser = user;
            configHandler.SaveToFile(config);
        }
    }
}
