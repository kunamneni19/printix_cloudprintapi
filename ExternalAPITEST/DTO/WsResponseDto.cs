using System.Text.Json.Serialization;

namespace ExternalAPI.DTO
{
    internal class WsResponseDto{
        // Main DTO for the Workstation response, restricted to specified fields
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("version")]
            public int Version { get; set; }

            [JsonPropertyName("operatingSystem")]
            public string OperatingSystem { get; set; }

            [JsonPropertyName("userIds")]
            public List<string> UserIds { get; set; }

            [JsonPropertyName("networking")]
            public Networking Networking { get; set; }

            [JsonPropertyName("powerState")]
            public string PowerState { get; set; }

            [JsonPropertyName("ipAddresses")]
            public List<string> IpAddresses { get; set; }

            [JsonPropertyName("networkConfigurationLocked")]
            public bool NetworkConfigurationLocked { get; set; }

            [JsonPropertyName("lastConnectTime")]
            public DateTimeOffset? LastConnectTime { get; set; }

            [JsonPropertyName("lastDisconnectTime")]
            public DateTimeOffset? LastDisconnectTime { get; set; }

            [JsonPropertyName("scanGateway")]
            public bool ScanGateway { get; set; }

            [JsonPropertyName("printGateway")]
            public bool PrintGateway { get; set; }

            [JsonPropertyName("hasStaticIP")]
            public bool HasStaticIP { get; set; }

            [JsonPropertyName("clientVersion")]
            public string ClientVersion { get; set; }

            [JsonPropertyName("mostRecentDotNetFrameworkVersion")]
            public string MostRecentDotNetFrameworkVersion { get; set; }

            [JsonPropertyName("kiosk")]
            public bool Kiosk { get; set; }

            [JsonPropertyName("onBehalfOf")]
            public bool OnBehalfOf { get; set; }

            [JsonPropertyName("active")]
            public bool Active { get; set; }

            [JsonPropertyName("fullyQualifiedDomainName")]
            public string FullyQualifiedDomainName { get; set; }
        }

        public class Networking
        {
            [JsonPropertyName("external")]
            public External External { get; set; }

            [JsonPropertyName("ssid")]
            public string Ssid { get; set; }
        }

        public class External
        {
            [JsonPropertyName("ip")]
            public string Ip { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

}