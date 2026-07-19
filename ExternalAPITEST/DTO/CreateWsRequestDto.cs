using Newtonsoft.Json;

namespace ExternalAPI.DTO
{
    internal class CreateWsRequestDto
    {
        [JsonProperty("operatingSystem")] 
        public string OperatingSystem { get; set; } = "WIN_10_64";

        [JsonProperty("powerState")] 
        public string PowerState { get; set; } = "POWER";

        [JsonProperty("type")] 
        public string Type { get; set; } = "DESKTOP";

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("ipAddresses")] 
        public List<string> IpAddresses { get; set; }

        [JsonProperty("proxy")] 
        public bool Proxy { get; set; } = false;

        [JsonProperty("clientVersion")] 
        public string ClientVersion { get; set; } = "0.0.0.0";

        [JsonProperty("networking")] 
        public NetworkRequestDto Networking { get; set; }

        [JsonProperty("active")] 
        public bool Active { get; set; } = false;
    }
}