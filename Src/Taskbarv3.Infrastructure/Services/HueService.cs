using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.Core.Models.Hue;
using System.Threading.Tasks;

namespace Taskbarv3.Infrastructure.Services
{
    public class HueService : IHueService
    {
        private string user = "";
        private bool HasUser => !string.IsNullOrWhiteSpace(user);
        private readonly RestClient client;
        private readonly MaxRequestsGuard requestGuard = new MaxRequestsGuard();
        private readonly string baseUrl;
        private readonly MainConfig config;
        private readonly IConfigHandler configHandler;

        public HueService(IConfigHandler configHandler)
        {
            this.configHandler = configHandler;
            config = configHandler.LoadFromFile();
            baseUrl = config.HueConfig.BridgeUrl;
            client = new RestClient(baseUrl);

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

        public Task<bool> PowerLight(bool on)
        {
            var light = config.HueConfig.LightToAffect;
            if (!HasUser)
            {
                return Task.FromResult(false);
            }
            requestGuard.WaitUntilFreeSlots();
            var request = new RestRequest(user + $"/lights/{light}/state", Method.PUT);
            var body = new { on };
            request.AddJsonBody(body);

            var response = client.ExecuteTaskAsync(request);

            return Task.FromResult(true);
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

            int brightnessValue = (int)(255 * (dimValue / 100.0));

            var request = new RestRequest(user + $"/lights/{light}/state", Method.PUT);
            var body = new { bri = brightnessValue };
            request.AddJsonBody(body);

            var response = await client.ExecuteTaskAsync(request);

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
            int brightness = 0;

            var request = new RestRequest(user + $"/lights/{light}", Method.GET);
            var response = await client.ExecuteTaskAsync<RootObject>(request);

            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(response.Content);
            if (rootObject == null || rootObject?.Error != null)
            {
                return 0;
            }

            brightness = (int)Math.Round(((double)rootObject.State.Bri / 255) * 100);

            return brightness;
        }

        public async Task<bool> RegisterAccount()
        {
            var request = new RestRequest(Method.POST);
            var body = new { devicetype = "taskbarv3" };
            request.AddJsonBody(body);

            var response = await client.ExecuteTaskAsync<RootObject>(request);

            LinkRoot linkRoot = JsonConvert.DeserializeObject<List<LinkRoot>>(response.Content).First();

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
