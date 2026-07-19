using Newtonsoft.Json;

namespace ExternalAPI.DTO
{
    internal class NetworkRequestDto
    {
        [JsonProperty("adapters")] 
        public List<AdapterRequestDto> Adapters { get; set; } // List of Adapter DTOs
    }

    public class AdapterRequestDto
    {
        [JsonProperty("gateway")]
        public GatewayRequestDto Gateway { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; } = "adapter-1";

        [JsonProperty("ipv4Address")]
        public string Ipv4Address { get; set; } = null;

        [JsonProperty("ipv6Address")]
        public string Ipv6Address { get; set; } = null;

        [JsonProperty("name")]
        public string Name { get; set; } = null;

        [JsonProperty("ssid")]
        public string Ssid { get; set; } = null;
    }

    public class GatewayRequestDto
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }
    }
}