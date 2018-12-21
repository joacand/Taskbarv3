namespace Taskbarv3.Core.Models
{
    public sealed class HueConfig
    {
        public string HueUser { get; set; }
        public string BridgeUrl { get; set; }
        public int LightToAffect { get; set; }
    }
}
